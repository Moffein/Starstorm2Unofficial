using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EntityStates.AffixVoid;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Cores.NemesisInvasion.Components;
using Starstorm2Unofficial.Modules;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.VFX;

namespace Starstorm2Unofficial.Cores.NemesisInvasion
{
    public class NemesisInvasionCore
    {
        public static List<BodyIndex> prioritizePlayersList = new List<BodyIndex>();
        public static ItemDef NemesisMarkerItem;
        public static GameObject NemesisMusicPrefab;

        public static float hpMult = 1f;
        public static float damageMult = 1f;
        public static float speedMult = 1f;
        public static float hpMultPerPlayer = 0.5f;
        public static float bonusArmor = 0f;
        public static bool scaleHPWithPlayercount = true;
        public static float moveSpeedCap = 0f;

        public NemesisInvasionCore()
        {
            BuildNemesisMusicController();
            NemesisInvasionManager.Initialize();

            BuildNemesisItem();

            RoR2.RoR2Application.onLoad += NemforcerMinibossCompat;
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += NemesisInvasionCore.AttemptTargetPlayer;
            if (moveSpeedCap > 0f) On.RoR2.CharacterBody.FixedUpdate += CapMoveSpeed;

            ApplyInfestationFix();
        }

        private void BuildNemesisMusicController()
        {
            if (NemesisMusicPrefab) return;
            NemesisMusicPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>("EmptyGameObject").InstantiateClone("NemesisMusicObject", false);
            NemesisMusicPrefab.AddComponent<NetworkIdentity>();
            NemesisMusicPrefab.AddComponent<NemesisMusicController>();
            NemesisMusicPrefab.RegisterNetworkPrefab();
            //Assets.networkedObjectPrefabs.Add(NemesisMusicPrefab);
        }

        private void CapMoveSpeed(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);
            if (self.inventory && self.inventory.GetItemCount(NemesisInvasionCore.NemesisMarkerItem) > 0)
            {
                if (self.moveSpeed > moveSpeedCap) self.moveSpeed = moveSpeedCap;
            }
        }

        private void ApplyInfestationFix()
        {
            IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                    );
                c.Emit(OpCodes.Ldloc_3);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                {
                    return playerControlled || (body.inventory && body.inventory.GetItemCount(NemesisMarkerItem) > 0);
                });
            };
        }

        public delegate void NemesisItemHook(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args);
        public static NemesisItemHook NemesisItemActions;

        //TODO: Move this to Items section if I ever get around to doing them. (probably not)
        private void BuildNemesisItem()
        {
            NemesisMarkerItem = ScriptableObject.CreateInstance<ItemDef>();
            NemesisMarkerItem.name = "SS2UNemesisMarkerItem";
            NemesisMarkerItem.deprecatedTier = ItemTier.NoTier;
            NemesisMarkerItem.nameToken = "SS2UNemesisMarkerItem";
            NemesisMarkerItem.pickupToken = "Applies Nemesis Invader stat boosts.";
            NemesisMarkerItem.descriptionToken = "Applies Nemesis Invader stat boosts.";
            NemesisMarkerItem.tags = new[]
            {
                ItemTag.WorldUnique,
                ItemTag.CannotSteal
            };
            (NemesisMarkerItem as ScriptableObject).name = "SS2UNemesisMarkerItem";
            ItemDisplayRule[] idr = new ItemDisplayRule[0];
            ItemAPI.Add(new CustomItem(NemesisMarkerItem, idr)); //TODO: Make this use the ContentPack instead.

            R2API.RecalculateStatsAPI.GetStatCoefficients += (sender, args) =>
            {
                if (sender.inventory && sender.inventory.GetItemCount(NemesisMarkerItem) > 0)
                {
                    float desiredHPMult = 1f;
                    if (scaleHPWithPlayercount)
                    {
                        desiredHPMult += hpMultPerPlayer * Mathf.Max(0, Run.instance.participatingPlayerCount - 1);
                    }
                    desiredHPMult *= hpMult;
                    if (desiredHPMult > 1f)
                    {
                        args.healthMultAdd += desiredHPMult - 1f;
                    }

                    if (damageMult > 1f)
                    {
                        args.damageMultAdd += damageMult - 1f;
                    }

                    if (speedMult > 1f)
                    {
                        args.moveSpeedMultAdd += speedMult - 1f;
                    }

                    args.armorAdd += bonusArmor;

                    NemesisItemActions?.Invoke(sender, args);
                }
            };

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (NetworkServer.active)
                {
                    if (self.body && self.body.inventory && self.body.inventory.GetItemCount(NemesisMarkerItem) > 0)
                    {
                        bool isFall = damageInfo.damageType.HasFlag(DamageType.FallDamage);
                        bool isVoidFog = !damageInfo.attacker && !damageInfo.inflictor
                            && damageInfo.damageColorIndex == DamageColorIndex.Void
                            && damageInfo.damageType == (DamageType.BypassArmor | DamageType.BypassBlock);
                        if (isFall || isVoidFog)
                        {
                            damageInfo.damage = 0f;
                            damageInfo.rejected = true;
                        }
                    }
                }

                orig(self, damageInfo);
            };

            RoR2.CharacterBody.onBodyInventoryChangedGlobal += CharacterBody_onBodyInventoryChangedGlobal;
        }

        private void CharacterBody_onBodyInventoryChangedGlobal(CharacterBody self)
        {
            if (self.inventory) self.AddItemBehavior<NemesisItemBehavior>(self.inventory.GetItemCount(NemesisMarkerItem));
        }

        private void NemforcerMinibossCompat()
        {
            GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemesisEnforcerMiniBossMaster");  //This is mislabeled as requiring BodyName instead of MasterName. HOPOOOOOOOOOOOOOOOOOOOO
            if (!masterPrefab) return;
            Debug.Log("Starstorm 2 Unofficial: Adding Nemforcer Miniboss to Nemesis invader list.");
            AddNemesisBoss(masterPrefab, null, string.Empty, true, true);

            CharacterMaster cm = masterPrefab.GetComponent<CharacterMaster>();
            if (cm && cm.bodyPrefab)
            {
                CharacterBody cb = cm.bodyPrefab.GetComponent<CharacterBody>();
                if (cb)
                {
                    cb.baseMaxHealth = 5400f;
                    cb.levelMaxHealth = 1620f;

                    cb.baseDamage = 8f;
                    cb.levelDamage = 1.6f;

                    cb.baseRegen = 0f;
                    cb.levelRegen = 0f;

                    cb.isChampion = true;

                    cb.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                    cb.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
                    cb.bodyFlags |= CharacterBody.BodyFlags.Void;
                    cb.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;

                    BodyIndex bi = BodyCatalog.FindBodyIndex(cb);
                    if (bi != BodyIndex.None)
                    {
                        prioritizePlayersList.Add(bi);
                    }
                }
            }
        }

        public static void AddNemesisBoss(GameObject masterPrefab, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems, bool addEffect = false)
        {
            NemesisCard nc = CreateNemesisBossCard(masterPrefab, itemStacks, itemDropName, shouldGrantRandomItems, addEffect);
            AddNemesisBoss(nc);
        }

        public static void AddNemesisBoss(NemesisCard card)
        {
            if (card != null) NemesisInvasionManager.nemesisCards.Add(card);
        }

        public static NemesisCard CreateNemesisBossCard(GameObject masterPrefab, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems, bool addEffect)
        {
            if (!masterPrefab) return null;

            CharacterSpawnCard csc = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            csc.forbiddenAsBoss = false;
            csc.noElites = true;
            csc.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            csc.prefab = masterPrefab;

            NemesisCard nc = new NemesisCard(csc, itemStacks, itemDropName, shouldGrantRandomItems, addEffect);
            return nc;
        }

        private static void AttemptTargetPlayer(On.RoR2.CharacterAI.BaseAI.orig_UpdateTargets orig, BaseAI self)
        {
            orig(self);
            if (self.body && self.body.teamComponent && (self.body.inventory && self.body.inventory.GetItemCount(NemesisMarkerItem) > 0) && (prioritizePlayersList.Contains(self.body.bodyIndex)))
            {
                if (self.currentEnemy != null
                && self.currentEnemy.characterBody
                && !self.currentEnemy.characterBody.isPlayerControlled
                && self.currentEnemy.characterBody.teamComponent)
                {
                    TeamMask enemyTeams = TeamMask.GetEnemyTeams(self.body.teamComponent.teamIndex);

                    List<CharacterBody> targetList = new List<CharacterBody>();
                    foreach (PlayerCharacterMasterController pc in PlayerCharacterMasterController.instances)
                    {
                        if (pc.body && pc.body.isPlayerControlled && pc.body.teamComponent && enemyTeams.HasTeam(pc.body.teamComponent.teamIndex) && pc.body.healthComponent && pc.body.healthComponent.alive && ((int)pc.body.GetVisibilityLevel(self.body) >= (int)VisibilityLevel.Revealed))
                        {
                            targetList.Add(pc.body);
                        }
                    }

                    if (targetList.Count > 0)
                    {
                        Vector3 myPos = self.body.corePosition;
                        float shortestDistSqr = Mathf.Infinity;
                        CharacterBody newTarget = null;

                        foreach (CharacterBody cb in targetList)
                        {
                            float sqrDist = (myPos - cb.corePosition).sqrMagnitude;
                            if (sqrDist < shortestDistSqr)
                            {
                                shortestDistSqr = sqrDist;
                                newTarget = cb;
                            }
                        }

                        if (newTarget)
                        {
                            self.currentEnemy.gameObject = newTarget.gameObject;
                            self.currentEnemy.bestHurtBox = newTarget.mainHurtBox;
                            self.enemyAttention = self.enemyAttentionDuration;
                            self.targetRefreshTimer = 10f;
                            self.BeginSkillDriver(self.EvaluateSkillDrivers());
                        }
                    }
                }
            }
        }
    }

    public class NemesisCard
    {
        public CharacterSpawnCard spawnCard;
        public int[] itemStacks;
        public string itemDropName;
        public bool shouldGrantRandomItems;
        public bool addEffect;

        public NemesisCard(CharacterSpawnCard spawnCard, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems, bool addEffect = false)
        {
            this.spawnCard = spawnCard;
            this.itemStacks = itemStacks;
            this.itemDropName = itemDropName;
            this.shouldGrantRandomItems = shouldGrantRandomItems;
            this.addEffect = addEffect;
        }
    }

    public class NemesisItemBehavior : RoR2.CharacterBody.ItemBehavior
    {
        public static GameObject effectPrefab;

        public delegate void OnNemStart(NemesisItemBehavior self, CharacterBody body);
        public static OnNemStart NemStartActions;

        public delegate void OnNemDisable(NemesisItemBehavior self, CharacterBody body);
        public static OnNemDisable NemDisableActions;

        private NemesisCard nemesisCard = null;
        private bool addEffect = false;
        CharacterBody.BodyFlags origFlags = CharacterBody.BodyFlags.None;

        private GameObject effectInstance;

        private void Start()
        {
            if (!body) return;
            origFlags = body.bodyFlags;
            body.bodyFlags = body.bodyFlags | CharacterBody.BodyFlags.IgnoreFallDamage | CharacterBody.BodyFlags.ImmuneToVoidDeath | CharacterBody.BodyFlags.Void | CharacterBody.BodyFlags.OverheatImmune;

            //Find Nemesis Card
            //Probably a better way to do this
            if (nemesisCard == null)
            {
                foreach (NemesisCard nc in NemesisInvasionManager.nemesisCards)
                {
                    if (nc.spawnCard && nc.spawnCard.prefab)
                    {
                        CharacterMaster cm = nc.spawnCard.prefab.GetComponent<CharacterMaster>();
                        if (cm && cm.bodyPrefab)
                        {
                            CharacterBody cb = cm.bodyPrefab.GetComponent<CharacterBody>();
                            if (cb && cb.bodyIndex == body.bodyIndex)
                            {
                                if (nc.addEffect) addEffect = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (addEffect && effectPrefab)
            {
                effectInstance = GameObject.Instantiate(effectPrefab, body.corePosition, Quaternion.identity, body.transform);
                effectInstance.transform.localPosition = Vector3.zero;
                effectInstance.transform.localScale = Vector3.one * 4f;
            }

            NemStartActions?.Invoke(this, body);
        }

        private void OnDisable()
        {
            if (body)
            {
                body.bodyFlags = origFlags;
                origFlags = CharacterBody.BodyFlags.None;
                NemDisableActions?.Invoke(this, body);
            }

            if (effectInstance) Destroy(effectInstance);
        }
    }
}
