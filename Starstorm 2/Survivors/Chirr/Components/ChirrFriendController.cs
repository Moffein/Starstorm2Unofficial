using RoR2;
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
    public class ChirrFriendController : NetworkBehaviour
    {
        public static float befriendHealthFraction = 0.5f;

        public static GameObject teleportHelperPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/DirectorSpawnProbeHelperPrefab.prefab").WaitForCompletion();
        public static GameObject indicatorCannotBefriendPrefab;
        public static GameObject indicatorReadyToBefriendPrefab;
        public static GameObject indicatorFriendPrefab;

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

        [SyncVar]
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
        public float maxTrackingAngle = 60f;
        public bool canBefriendChampion = false;

        public static void BlacklistBody(BodyIndex bodyIndex)
        {
            if (bodyIndex != BodyIndex.None) blacklistedBodies.Add(bodyIndex);
        }

        public bool HasFriend() { return _hasFriend; }
        public bool CanBefriend() { return _canBefriendTarget; }


        private void Awake()
        {
            ownerBody = base.GetComponent<CharacterBody>();
            teamComponent = base.GetComponent<TeamComponent>();
            inputBank = base.GetComponent<InputBankTest>();

            this.indicatorCannotBefriend = new Indicator(base.gameObject, indicatorCannotBefriendPrefab);
            this.indicatorReadyToBefriend = new Indicator(base.gameObject, indicatorReadyToBefriendPrefab);
            this.indicatorFriend = new Indicator(base.gameObject, indicatorFriendPrefab);

            trackerUpdateStopwatch = 0f;

            //Is there a better way with onInventoryChangedGlobal?
            RoR2.Inventory.onServerItemGiven += UpdateMinionInventory;
        }

        private void Start()
        {
            if (ownerBody) ownerMaster = ownerBody.master;
        }

        private void OnDestroy()
        {
            RoR2.Inventory.onServerItemGiven -= UpdateMinionInventory;
        }

        private void UpdateMinionInventory(Inventory inventory, ItemIndex itemIndex, int count)
        {
            if (ownerMaster && this._hasFriend && this.targetMaster && this.targetMaster.inventory)
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

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                //Server updates what you're supposed to be targeting
                FixedUpdateServer();
            }

            //Client tries to figure out what the server wants it to target.
            ResolveTargetOnClient();
        }

        private void ResolveTargetOnClient()
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
                        targetBody = targetMaster.GetBody();
                        if (targetBody)
                        {
                            hasValidTarget = true;
                        }
                    }
                }
            }

            if (hasValidTarget)
            {
                this.indicatorCannotBefriend.targetTransform = targetBody.transform;
                this.indicatorReadyToBefriend.targetTransform = targetBody.transform;
                this.indicatorFriend.targetTransform = targetBody.transform;

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
            else
            {
                targetMaster = null;
                targetBody = null;

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
            if (trackingTarget && trackingTarget.healthComponent && trackingTarget.healthComponent.alive && targetMaster)
            {
                UpdateCanBefriendServer();
            }
            else
            {
                trackingTarget = null;
                if (_hasFriend) _hasFriend = false;
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
                bool befriendStatus = trackingTarget.healthComponent.combinedHealthFraction <= ChirrFriendController.befriendHealthFraction;
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
                        //bool isBoss = hbBody.isBoss;  //No need for this, seems CombatSquad code fixes the softlock problems.
                        bool isChampion = hbBody.isChampion;
                        bool isBlacklisted = blacklistedBodies.Contains(hbBody.bodyIndex);

                        if (!isPlayerControlled && (!isChampion || canBefriendChampion) && !isBlacklisted)// && !isBoss
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
                targetMaster.teamIndex = teamIndex;
                if (targetBody.teamComponent) targetBody.teamComponent.teamIndex = teamIndex;

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
                }

                if (targetMaster.inventory)
                {
                    targetMaster.inventory.RemoveItem(RoR2Content.Items.UseAmbientLevel, targetMaster.inventory.GetItemCount(RoR2Content.Items.UseAmbientLevel));
                }

                if (targetMaster.aiComponents != null)
                {
                    for (int i = 0; i< targetMaster.aiComponents.Length; i++)
                    {
                        targetMaster.aiComponents[i].currentEnemy.Reset();
                    }
                }

                List<CombatSquad> combatSquads = InstanceTracker.GetInstancesList<CombatSquad>();
                foreach (CombatSquad cs in combatSquads)
                {
                    if (cs.membersList.Contains(targetMaster))
                    {
                        //What about onMemberDeathServer?
                        cs.RemoveMember(targetMaster);
                        if (!cs.defeatedServer && cs.membersList.Count == 0)
                        {
                            cs.TriggerDefeat();
                        }
                    }
                }

                _canBefriendTarget = false;
                _hasFriend = true;
            }
            else
            {
                Debug.LogError("ChirrFriendController: Befriend called without valid target.");
            }
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
            return _hasFriend && targetBody && (targetBody.corePosition - ownerBody.corePosition).sqrMagnitude > 40f * 40f;
        }

        //Copied from minionLeash
        [Server]
        public void LeashFriend(Vector3 newPos)
        {
            if (CanLeash())
            {
                SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
                spawnCard.hullSize = targetBody.hullClassification;
                spawnCard.nodeGraphType = (targetBody.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground);
                spawnCard.prefab = ChirrFriendController.teleportHelperPrefab;
                GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.Approximate,
                    position = newPos,
                    minDistance = 5f,
                    maxDistance = 40f
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
}
