using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Navigation;
using Starstorm2.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Chirr.Components
{
    //I went overboard with the amount of nullchecks in this class, there's a lot of redundant ones.
    //Don't like how it's ambiguous whether targetMaster/targetBody/trackingTarget/_trackingTargetMasterNetID is used to identify valid targets.
    //Should have a clear-cut hierarchy instead.
    [RequireComponent(typeof(CharacterBody))]
    public class ChirrFriendController : NetworkBehaviour
    {
        public static float befriendHealthFraction = 0.5f;
        public static float befriendChampionHealthFraction = 0.3f;

        public static GameObject teleportHelperPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/DirectorSpawnProbeHelperPrefab.prefab").WaitForCompletion();
        public static GameObject indicatorCannotBefriendPrefab;
        public static GameObject indicatorReadyToBefriendPrefab;
        public static GameObject indicatorFriendPrefab;

        public static Dictionary<BodyIndex, float> bodyDamageValueOverrides = new Dictionary<BodyIndex, float>();

        public static void AddBodyDamageValueOverride(BodyIndex bodyIndex, float value)
        {
            if (bodyIndex != BodyIndex.None)
            {
                bodyDamageValueOverrides.Remove(bodyIndex);
                bodyDamageValueOverrides.Add(bodyIndex, value);
            }
        }

        public static List<string> itemCopyBlacklist = new List<string>
        {
            //"Thorns",
            "FreeChest",
            "TreasureCache",
            //"ShockNearby",
            //"Icicle",
            //"SiphonOnLowHealth"
        };

        private void RemoveBlacklistedItems(Inventory inventory)
        {
            foreach (string str in itemCopyBlacklist)
            {
                ItemIndex blacklistItem = ItemCatalog.FindItemIndex(str);
                if (blacklistItem != ItemIndex.None)
                {
                    inventory.RemoveItem(blacklistItem, inventory.GetItemCount(blacklistItem));
                }
            }
        }

        private bool IsBlacklistedItem(ItemIndex item)
        {
            foreach (string str in itemCopyBlacklist)
            {
                ItemIndex blacklistItem = ItemCatalog.FindItemIndex(str);
                if (blacklistItem == item) return true;
            }
            return false;
        }

        private bool hadBeads = false;

        public static HashSet<BodyIndex> blacklistedBodies = new HashSet<BodyIndex>();

        private Indicator indicatorCannotBefriend;
        private Indicator indicatorReadyToBefriend;
        private Indicator indicatorFriend;

        private HurtBox trackingTarget;
        private float trackerUpdateStopwatch;
        private readonly BullseyeSearch search = new BullseyeSearch();

        private InputBankTest inputBank;
        private CharacterBody ownerBody;
        private CharacterMaster ownerMaster;
        private TeamComponent teamComponent;
        private MasterFriendController masterFriendController;

        //[SyncVar]//Uncomment this if min leash distance is going to be a thing again.
        private bool _canLeash = false;

        [SyncVar]
        private bool _canBefriendTarget = false;

        [SyncVar]
        private bool _hasFriend = false;

        [SyncVar]
        private uint _trackingTargetMasterNetID = NetworkInstanceId.Invalid.Value;

        private CharacterBody targetBody;
        private CharacterMaster targetMaster;

        public float trackerUpdateFrequency = 4f;
        public float maxTrackingDistance = 90f;
        public float maxTrackingAngle = 90f;
        public bool canBefriendChampion = false;

        public static void BlacklistBody(BodyIndex bodyIndex)
        {
            if (bodyIndex != BodyIndex.None) blacklistedBodies.Add(bodyIndex);
        }

        private bool HasLunarTrinket()
        {
            if (!hadBeads) hadBeads = ownerMaster && ownerMaster.inventory && ownerMaster.inventory.GetItemCount(RoR2Content.Items.LunarTrinket) > 0;
            return hadBeads;
        }

        public bool HasFriend() { return _hasFriend; }
        public bool CanBefriend()
        {
            return _canBefriendTarget && targetBody && !targetBody.HasBuff(BuffCore.chirrFriendBuff) && !targetBody.HasBuff(BuffCore.chirrSelfBuff);
        }

        private void Awake()
        {
            ownerBody = base.GetComponent<CharacterBody>();
            teamComponent = base.GetComponent<TeamComponent>();
            inputBank = base.GetComponent<InputBankTest>();

            trackerUpdateStopwatch = 0f;
            RoR2.Inventory.onServerItemGiven += UpdateMinionInventory;//Is there a better way with onInventoryChangedGlobal?
            On.RoR2.PingerController.SetCurrentPing += MinionPingRetarget;

            this.indicatorCannotBefriend = new Indicator(base.gameObject, indicatorCannotBefriendPrefab);
            this.indicatorReadyToBefriend = new Indicator(base.gameObject, indicatorReadyToBefriendPrefab);
            this.indicatorFriend = new Indicator(base.gameObject, indicatorFriendPrefab);
        }
        private void Start()
        {
            if (ownerBody) ownerMaster = ownerBody.master;
        }

        [Server]
        public void TryGetSavedMaster()
        {
            if (!NetworkServer.active) return;
            if (ownerMaster)
            {
                masterFriendController = ownerMaster.GetComponent<MasterFriendController>();
                if (!masterFriendController) masterFriendController = ownerMaster.AddComponent<MasterFriendController>();
                if (masterFriendController.masterNetID != NetworkInstanceId.Invalid.Value)
                {
                    _trackingTargetMasterNetID = masterFriendController.masterNetID;
                    bool hasMaster = ResolveTargetMaster();
                    if (hasMaster)
                    {
                        _hasFriend = true;
                        _canBefriendTarget = false;
                        if (!ownerBody.HasBuff(BuffCore.chirrSelfBuff))
                        {
                            ownerBody.AddBuff(BuffCore.chirrSelfBuff);
                        }
                        if (targetBody)
                        {
                            trackingTarget = targetBody.mainHurtBox;
                            targetBody.AddBuff(BuffCore.chirrFriendBuff);

                            if (targetBody.bodyIndex == EnemyCore.brotherHurtIndex || targetBody.bodyIndex == EnemyCore.brotherIndex)
                            {
                                targetBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                            }
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            RoR2.Inventory.onServerItemGiven -= UpdateMinionInventory;
            On.RoR2.PingerController.SetCurrentPing -= MinionPingRetarget;
        }

        private void UpdateMinionInventory(Inventory inventory, ItemIndex itemIndex, int count)
        {
            if (ownerMaster && this._hasFriend && this.targetMaster && this.targetMaster.inventory && !IsBlacklistedItem(itemIndex))
            {
                CharacterMaster cm = inventory.GetComponent<CharacterMaster>();
                if (cm == ownerMaster)
                {
                    ItemDef item = ItemCatalog.GetItemDef(itemIndex);
                    if (!item.tags.Contains(ItemTag.CannotCopy))
                    {
                        targetMaster.inventory.GiveItem(itemIndex, count);
                    }
                }
            }
        }

        public void MinionPingRetarget(On.RoR2.PingerController.orig_SetCurrentPing orig, PingerController self, PingerController.PingInfo ping)
        {
            orig(self, ping);
            if (NetworkServer.active)
            {
                if (ping.targetGameObject && _hasFriend && targetMaster && targetMaster.aiComponents != null)
                {
                    TeamIndex ownerTeam = TeamIndex.None;
                    if (ownerBody && ownerBody.teamComponent) ownerTeam = ownerBody.teamComponent.teamIndex;
                    CharacterBody pingBody = ping.targetGameObject.GetComponent<CharacterBody>();
                    if (pingBody && pingBody.teamComponent && pingBody.teamComponent.teamIndex != ownerTeam)
                    {
                        for (int i = 0; i < targetMaster.aiComponents.Length; i++)
                        {
                            BaseAI ai = targetMaster.aiComponents[i];
                            ai.currentEnemy.gameObject = pingBody.gameObject;
                            ai.currentEnemy.bestHurtBox = pingBody.mainHurtBox;
                            ai.enemyAttention = ai.enemyAttentionDuration;
                            ai.targetRefreshTimer = 5f;
                            ai.BeginSkillDriver(ai.EvaluateSkillDrivers());
                        }
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                //Server updates what you're supposed to be targeting
                FixedUpdateServer();
            }

            //Client tries to figure out what the server wants it to target.
            ResolveTargetAndUpdateIndicators();
        }
        
        //Returns true if master is found
        //Does not guarantee body is found
        private bool ResolveTargetMaster()
        {
            bool hasValidTarget = false;

            if (_trackingTargetMasterNetID != NetworkInstanceId.Invalid.Value)
            {
                GameObject networkMasterObject = Util.FindNetworkObject(new NetworkInstanceId(_trackingTargetMasterNetID));
                if (networkMasterObject)
                {
                    targetMaster = networkMasterObject.GetComponent<CharacterMaster>();
                    if (targetMaster)
                    {
                        hasValidTarget = true;
                        targetBody = targetMaster.GetBody();
                    }
                }
            }
            return hasValidTarget;
        }

        private void ResolveTargetAndUpdateIndicators()
        {
            bool hasValidTarget = ResolveTargetMaster();

            if (hasValidTarget && targetBody)
            {
                Transform targetTransform = targetBody.mainHurtBox ? targetBody.mainHurtBox.transform : targetBody.transform;
                if (targetTransform)
                {
                    this.indicatorCannotBefriend.targetTransform = targetTransform;
                    this.indicatorReadyToBefriend.targetTransform = targetTransform;
                    this.indicatorFriend.targetTransform = targetTransform;

                    if (this._hasFriend)
                    {
                        this.indicatorFriend.active = true;
                        this.indicatorCannotBefriend.active = false;
                        this.indicatorReadyToBefriend.active = false;
                    }
                    else
                    {
                        if (this._canBefriendTarget)
                        {
                            this.indicatorReadyToBefriend.active = true;
                            this.indicatorFriend.active = false;
                            this.indicatorCannotBefriend.active = false;
                        }
                        else
                        {
                            this.indicatorCannotBefriend.active = true;
                            this.indicatorFriend.active = false;
                            this.indicatorReadyToBefriend.active = false;
                        }
                    }
                }
            }
            else
            {
                /*targetMaster = null;
                targetBody = null;*/

                this.indicatorCannotBefriend.active = false;
                this.indicatorFriend.active = false;
                this.indicatorReadyToBefriend.active = false;
            }
        }

        [Server]
        private void FixedUpdateServer()
        {
            CheckTargetAliveServer();
            if (!_hasFriend)
            {
                if (ownerBody.HasBuff(BuffCore.chirrSelfBuff))
                {
                    ownerBody.RemoveBuff(BuffCore.chirrSelfBuff);
                }

                trackerUpdateStopwatch += Time.fixedDeltaTime;
                if (trackerUpdateStopwatch >= 1f / trackerUpdateFrequency)
                {
                    trackerUpdateStopwatch = 0f;
                    HurtBox newHurtbox = SearchForTarget(inputBank.GetAimRay());
                    if (newHurtbox != trackingTarget)
                    {
                        ChangeTrackingTargetServer(newHurtbox);
                    }
                }
                UpdateCanBefriendServer();
            }
            else
            {
                bool newCanLeash = CanLeashServer();
                if (newCanLeash != _canLeash) _canLeash = newCanLeash;
            }
        }

        [Server]
        private void CheckTargetAliveServer()
        {
            if (targetMaster && !targetMaster.IsDeadAndOutOfLivesServer())
            {
                UpdateCanBefriendServer();
            }
            else
            {
                trackingTarget = null;
                if (_hasFriend)
                {
                    _hasFriend = false;
                }
                _trackingTargetMasterNetID = NetworkInstanceId.Invalid.Value;
            }
        }

        [Server]
        private void ChangeTrackingTargetServer(HurtBox newHurtbox)
        {
            if (newHurtbox && newHurtbox.healthComponent && newHurtbox.healthComponent.body && newHurtbox.healthComponent.body.master)
            {
                uint newID = newHurtbox.healthComponent.body.master.netId.Value;
                if (newID != NetworkInstanceId.Invalid.Value && newID != _trackingTargetMasterNetID)
                {
                    _trackingTargetMasterNetID = newID;
                    trackingTarget = newHurtbox;
                    UpdateCanBefriendServer();
                }
            }
        }

        [Server]
        private void UpdateCanBefriendServer()
        {
            if (trackingTarget && trackingTarget.healthComponent)
            {
                bool befriendStatus = trackingTarget.healthComponent.combinedHealthFraction <= ((trackingTarget.healthComponent.body && trackingTarget.healthComponent.body.isChampion) ? ChirrFriendController.befriendChampionHealthFraction : ChirrFriendController.befriendHealthFraction);
                if (befriendStatus != _canBefriendTarget) _canBefriendTarget = befriendStatus;
            }
        }

        private HurtBox SearchForTarget(Ray aimRay)
        {
            this.search.teamMaskFilter = TeamMask.GetEnemyTeams(teamComponent.teamIndex);
            this.search.filterByLoS = true;
            this.search.searchOrigin = aimRay.origin;
            this.search.searchDirection = aimRay.direction;
            this.search.sortMode = BullseyeSearch.SortMode.Angle;
            this.search.maxDistanceFilter = this.maxTrackingDistance;
            this.search.maxAngleFilter = this.maxTrackingAngle;
            this.search.RefreshCandidates();
            this.search.FilterOutGameObject(base.gameObject);
            IEnumerable<HurtBox> targets = this.search.GetResults();
            List<HurtBox> validTargets = new List<HurtBox>();
            foreach (HurtBox hb in targets)
            {
                if (hb.healthComponent)
                {
                    CharacterBody hbBody = hb.healthComponent.body;
                    if (hbBody)
                    {
                        bool isPlayerControlled = hbBody.isPlayerControlled;
                        bool isBoss = hbBody.isBoss;
                        bool isChampion = hbBody.isChampion;
                        bool isBlacklisted = blacklistedBodies.Contains(hbBody.bodyIndex);
                        bool isDecay = hbBody.inventory && hbBody.inventory.GetItemCount(RoR2Content.Items.HealthDecay) > 0;
                        bool isDestroy = (hbBody.GetComponent<DestroyOnTimer>() != null) || (hbBody.master && hbBody.master.GetComponent<DestroyOnTimer> () != null);
                        bool isAlreadyFriended = hbBody.HasBuff(BuffCore.chirrFriendBuff) || hbBody.HasBuff(BuffCore.chirrSelfBuff);

                        if (!isPlayerControlled && !isDecay && !isDestroy && !isAlreadyFriended && (((!isChampion && !isBoss) || canBefriendChampion) || (hbBody.bodyIndex == EnemyCore.brotherHurtIndex && (canBefriendChampion || HasLunarTrinket()))) && !isBlacklisted)
                        {
                            validTargets.Add(hb);
                        }
                    }
                }
            }

            return validTargets.FirstOrDefault<HurtBox>();
        }

        [Server]
        public void BefriendServer(TeamIndex teamIndex)
        {
            if (!NetworkServer.active) return;
            if (CanBefriend() && targetMaster && targetBody)
            {
                _canBefriendTarget = false;
                _hasFriend = true;

                //Make sure Mithrix returns your items
                ItemStealController isc = targetBody.GetComponent<ItemStealController>();
                if (isc)
                {
                    isc.ForceReclaimAllItemsImmediately();
                }

                targetMaster.teamIndex = teamIndex;
                if (targetBody.teamComponent) targetBody.teamComponent.teamIndex = teamIndex;

                Util.CleanseBody(targetBody, true, false, true, true, true, false);
                targetBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 3f);

                targetBody.AddBuff(BuffCore.chirrFriendBuff);
                if (!ownerBody.HasBuff(BuffCore.chirrSelfBuff))
                {
                    ownerBody.AddBuff(BuffCore.chirrSelfBuff);
                }

                if (targetBody.healthComponent)
                {
                    targetBody.healthComponent.health = targetBody.healthComponent.fullHealth;
                    targetBody.healthComponent.shield = targetBody.healthComponent.fullShield;

                    targetMaster.inventory.CopyItemsFrom(ownerBody.inventory, Inventory.defaultItemCopyFilterDelegate);
                    RemoveBlacklistedItems(targetMaster.inventory);
                }

                if (targetMaster.inventory)
                {
                    targetMaster.inventory.RemoveItem(RoR2Content.Items.UseAmbientLevel, targetMaster.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel));
                }

                //Reset AI targeting
                if (targetMaster.aiComponents != null)
                {
                    for (int i = 0; i < targetMaster.aiComponents.Length; i++)
                    {
                        targetMaster.aiComponents[i].currentEnemy.Reset();
                    }
                }

                //Prevent Chirr from softlocking the teleporter and other kill objectives.
                List<CombatSquad> combatSquads = InstanceTracker.GetInstancesList<CombatSquad>();
                foreach (CombatSquad cs in combatSquads)
                {
                    if (cs.membersList.Contains(targetMaster))
                    {
                        //This doesn't call OnMemberDeathServer. Where is that used?
                        cs.RemoveMember(targetMaster);
                        if (!cs.defeatedServer && cs.membersList.Count == 0)
                        {
                            cs.TriggerDefeat();
                        }
                    }
                }

                //Soul
                if (targetBody.bodyIndex == EnemyCore.brotherHurtIndex || targetBody.bodyIndex == EnemyCore.brotherIndex)
                {
                    targetBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
                    if (EnemyCore.IsMoon())
                    {
                        //He still uses Phase 4 AI if you do this.
                        //targetMaster.bodyPrefab = BodyCatalog.FindBodyPrefab("BrotherBody");
                        //targetMaster.Respawn(targetBody.transform.position, targetBody.transform.rotation);
                        RpcSetMithrixConverted();

                        EnemyCore.FakeMithrixChatMessageServer("BROTHERHURT_CHIRR_BEFRIEND_1");
                    }
                }

                //Ally persists between stages
                RpcDontDestroyOnLoad(_trackingTargetMasterNetID);

                //Save ally netID so it can be remembered next stage.
                if (masterFriendController)
                {
                    masterFriendController.masterNetID = targetMaster.netId.Value;
                }
            }
            else
            {
                Debug.LogError("ChirrFriendController: Befriend called without valid target.");
            }
        }

        [Client]
        public void RemoveFriendClient()
        {
            CmdRemoveFriend();
        }

        [Command]
        private void CmdRemoveFriend()
        {
            if (NetworkServer.active)
            {
                if (HasFriend() && !(targetBody && targetBody.bodyIndex == EnemyCore.brotherHurtIndex))
                {
                    RpcSetTargetBodyGlass();
                    targetMaster.TrueKill();
                }
            }
        }

        [ClientRpc]
        private void RpcSetTargetBodyGlass()
        {
            if (targetBody) targetBody.isGlass = true;
        }

        //Re-evaluate the netID again here to be double sure that it's going to be the right thing getting set.
        [ClientRpc]
        private void RpcDontDestroyOnLoad(uint netID)
        {
            GameObject networkMasterObject = Util.FindNetworkObject(new NetworkInstanceId(netID));
            if (networkMasterObject)
            {
                DontDestroyOnLoad(networkMasterObject);
            }
        }

        [ClientRpc]
        private void RpcSetMithrixConverted()
        {
            ChirrCore.survivorDef.outroFlavorToken = "CHIRR_OUTRO_BROTHER_EASTEREGG";
        }

        [Server]
        public void HurtFriend(DamageInfo damageInfo)
        {
            if (targetBody)
            {
                targetBody.healthComponent.TakeDamage(damageInfo);
            }
        }

        public bool CanLeash()
        {
            return _canLeash;
        }

        private bool CanLeashServer()
        {
            return _hasFriend && targetBody && (targetBody.corePosition - ownerBody.corePosition).sqrMagnitude > 5f * 5f;
        }

        [Client]
        public void LeashFriendClient(Vector3 newPos)
        {
            CmdLeashFriend(newPos);
        }

        [Command]
        private void CmdLeashFriend(Vector3 newPos)
        {
            //if (CanLeash())
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.hullSize = targetBody.hullClassification;
            spawnCard.nodeGraphType = (targetBody.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
            spawnCard.prefab = ChirrFriendController.teleportHelperPrefab;

            GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                position = newPos,
                minDistance = 5f,
                maxDistance = 45f
            }, RoR2Application.rng));

            if (gameObject)
            {
                Vector3 position = gameObject.transform.position;
                TeleportHelper.TeleportBody(targetBody, position);
                GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(targetBody.gameObject);
                if (teleportEffectPrefab)
                {
                    EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
                }
                UnityEngine.Object.Destroy(gameObject);
            }
        }
    }
}
