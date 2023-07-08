using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Cores.NemesisInvasion.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace Starstorm2Unofficial.Cores.NemesisInvasion
{
    public class NemesisInvasionCore
    {
        public static List<BodyIndex> prioritizePlayersList = new List<BodyIndex>();
        public static ItemDef NemesisMarkerItem;

        public static float hpMult = 1f;
        public static float damageMult = 1f;
        public static float speedMult = 1f;
        public static float hpMultPerPlayer = 0.3f;
        public static float bonusArmor = 0f;
        public static bool scaleHPWithPlayercount = true;

        public NemesisInvasionCore()
        {
            NemesisInvasionManager.Initialize();

            LanguageAPI.Add("NEMESIS_MODE_ACTIVE_WARNING", "<style=cIsHealth>An unnatural force emanates from the void...</style>");
            LanguageAPI.Add("NEMESIS_MODE_DEACTIVATED", "<style=cWorldEvent>The void's influence fades...</style>");

            BuildNemesisItem();

            RoR2.RoR2Application.onLoad += NemforcerMinibossCompat;
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += NemesisInvasionCore.AttemptTargetPlayer;

            ApplyInfestationFix();
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
                ItemTag.BrotherBlacklist,
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
                }
            };

            On.RoR2.HealthComponent.TakeDamage += (orig, self, damageInfo) =>
            {
                if (NetworkServer.active)
                {
                    if (self.body && self.body.inventory && self.body.inventory.GetItemCount(NemesisMarkerItem) > 0)
                    {
                        if (damageInfo.damageType.HasFlag(DamageType.FallDamage))
                        {
                            damageInfo.damage = 0f;
                            damageInfo.rejected = true;
                        }
                    }
                }

                orig(self, damageInfo);
            };
        }

        private void NemforcerMinibossCompat()
        {
            GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemesisEnforcerMiniBossMaster");  //This is mislabeled as requiring BodyName instead of MasterName. HOPOOOOOOOOOOOOOOOOOOOO
            if (masterPrefab)
            {
                Debug.Log("Starstorm 2 Unofficial: Adding Nemforcer Miniboss to Nemesis invader list.");
                AddNemesisBoss(masterPrefab, null, string.Empty, true, true);

                CharacterMaster cm = masterPrefab.GetComponent<CharacterMaster>();
                if (cm && cm.bodyPrefab)
                {
                    CharacterBody cb = cm.bodyPrefab.GetComponent<CharacterBody>();
                    if (cb)
                    {
                        cb.baseMaxHealth = 3800f;
                        cb.levelMaxHealth = 1140f;

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
        }

        public static void AddNemesisBoss(GameObject masterPrefab, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems, bool autoMusicSetup)
        {
            NemesisCard nc = CreateNemesisBossCard(masterPrefab, itemStacks, itemDropName, shouldGrantRandomItems, autoMusicSetup);
            AddNemesisBoss(nc);
        }

        public static void AddNemesisBoss(NemesisCard card)
        {
            if (card != null) NemesisInvasionManager.nemesisCards.Add(card);
        }

        public static NemesisCard CreateNemesisBossCard(GameObject masterPrefab, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems, bool autoMusicSetup)
        {
            if (!masterPrefab) return null;

            if (autoMusicSetup)
            {
                NemesisMusicComponentMaster masterMusic = masterPrefab.GetComponent<NemesisMusicComponentMaster>();
                if (!masterMusic) masterMusic = masterPrefab.AddComponent<NemesisMusicComponentMaster>();
                masterMusic.musicString = "Play_SS2U_NemesisTheme";    //Lazy workaround since I can't figure out how to get the music system to work.
            }

            CharacterSpawnCard csc = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            csc.forbiddenAsBoss = false;
            csc.noElites = true;
            csc.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            csc.prefab = masterPrefab;

            NemesisCard nc = new NemesisCard(csc, itemStacks, itemDropName, shouldGrantRandomItems);
            return nc;
        }

        private static void AttemptTargetPlayer(On.RoR2.CharacterAI.BaseAI.orig_UpdateTargets orig, BaseAI self)
        {
            orig(self);
            if (self.body && self.body.teamComponent && (prioritizePlayersList.Contains(self.body.bodyIndex)))
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

        public NemesisCard(CharacterSpawnCard spawnCard, int[] itemStacks, string itemDropName, bool shouldGrantRandomItems)
        {
            this.spawnCard = spawnCard;
            this.itemStacks = itemStacks;
            this.itemDropName = itemDropName;
            this.shouldGrantRandomItems = shouldGrantRandomItems;
        }
    }
}
