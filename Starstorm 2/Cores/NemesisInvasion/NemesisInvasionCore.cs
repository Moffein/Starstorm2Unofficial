using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public NemesisInvasionCore()
        {
            NemesisInvasionManager.Initialize();

            LanguageAPI.Add("NEMESIS_MODE_ACTIVE_WARNING", "<style=cIsHealth>An unnatural force emanates from the void...</style>");
            LanguageAPI.Add("NEMESIS_MODE_DEACTIVATED", "<style=cWorldEvent>The void's influence fades...</style>");

            RoR2.RoR2Application.onLoad += NemforcerMinibossCompat;
            RoR2.RoR2Application.onLoad += BlacklistItemsFromNemesisInvader;
            On.RoR2.CharacterAI.BaseAI.UpdateTargets += NemesisInvasionCore.AttemptTargetPlayer;
        }

        private void NemforcerMinibossCompat()
        {
            GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemesisEnforcerMiniBossMaster");  //This is mislabeled as requiring BodyName instead of MasterName. HOPOOOOOOOOOOOOOOOOOOOO
            if (masterPrefab)
            {
                Debug.Log("Starstorm 2 Unofficial: Adding Nemforcer Miniboss to Nemesis invader list.");
                AddNemesisBoss(masterPrefab, null, "ShinyPearl", true, true);

                CharacterMaster cm = masterPrefab.GetComponent<CharacterMaster>();
                if (cm && cm.bodyPrefab)
                {
                    CharacterBody cb = cm.bodyPrefab.GetComponent<CharacterBody>();
                    if (cb)
                    {
                        cb.baseMaxHealth = 3200f;
                        cb.levelMaxHealth = 960f;

                        cb.baseDamage = 4f;
                        cb.levelDamage = 0.8f;

                        cb.baseRegen = 0f;
                        cb.levelRegen = 0f;

                        cb.isChampion = true;

                        BodyIndex bi = BodyCatalog.FindBodyIndex(cb);
                        if (bi != BodyIndex.None)
                        {
                            prioritizePlayersList.Add(bi);
                        }
                    }
                }
            }
        }

        private void BlacklistItemsFromNemesisInvader()
        {
            NemesisInvasionManager.BlacklistItem("IceRing");
            NemesisInvasionManager.BlacklistItem("FireRing");
            NemesisInvasionManager.BlacklistItem("ElementalRingVoid");
            NemesisInvasionManager.BlacklistItem("FlatHealth");
            NemesisInvasionManager.BlacklistItem("PersonalShield");
            NemesisInvasionManager.BlacklistItem("ArmorPlate");
            NemesisInvasionManager.BlacklistItem("MushroomVoid");
            NemesisInvasionManager.BlacklistItem("Bear");
            NemesisInvasionManager.BlacklistItem("BearVoid");
            NemesisInvasionManager.BlacklistItem("ITEM_BLOODMASK");
            NemesisInvasionManager.BlacklistItem("BleedOnHit");
            NemesisInvasionManager.BlacklistItem("BleedOnHitVoid");
            NemesisInvasionManager.BlacklistItem("BleedOnHitAndExplode");
            NemesisInvasionManager.BlacklistItem("Missile");
            NemesisInvasionManager.BlacklistItem("MissileVoid");
            NemesisInvasionManager.BlacklistItem("PrimarySkillShuriken");
            NemesisInvasionManager.BlacklistItem("ShockNearby");
            NemesisInvasionManager.BlacklistItem("NovaOnHeal");
            NemesisInvasionManager.BlacklistItem("Thorns");
            NemesisInvasionManager.BlacklistItem("DroneWeapons");
            NemesisInvasionManager.BlacklistItem("Icicle");
            NemesisInvasionManager.BlacklistItem("ImmuneToDebuff");
            NemesisInvasionManager.BlacklistItem("CaptainDefenseMatrix");
            NemesisInvasionManager.BlacklistItem("ExtraLife");
            NemesisInvasionManager.BlacklistItem("ExtraLifeVoid");
            NemesisInvasionManager.BlacklistItem("ExplodeOnDeathVoid");
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
