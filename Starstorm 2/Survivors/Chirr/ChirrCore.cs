using R2API;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using EntityStates.SS2UStates.Chirr;
using EntityStates;
using System.Linq;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using EntityStates.SS2UStates.Chirr.Special;
using System.Runtime.CompilerServices;
using Starstorm2Unofficial.Cores;
using System.Collections.Generic;
using RoR2.CharacterAI;
using EntityStates.SS2UStates.Chirr.Taunt;

namespace Starstorm2Unofficial.Survivors.Chirr
{
    public class ChirrCore
    {
        public static bool convertedMithrixThisRun = false;

        public static BodyIndex bodyIndex;

        public static GameObject chirrPrefab;
        public static GameObject doppelganger;

        public static GameObject chirrHeal;

        public static SkillDef specialDef;
        public static SkillDef specialScepterDef;

        public static SurvivorDef survivorDef;

        public static List<string> brotherKillChirrTokens = new List<string>();

        public ChirrCore() => Setup();

        private void OnLoadActions()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UChirrBody");
            ChirrFriendController.BlacklistBody(bodyIndex);

            BodyIndex brotherBodyIndex = BodyCatalog.FindBodyIndex("BrotherBody");
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("VoidRaidCrabBody"));
            ChirrFriendController.BlacklistBody(brotherBodyIndex);
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("UrchinTurretBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("WispSoulBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("ShopkeeperBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("VoidInfestorBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MinorConstructAttachableBody"));

            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("VoidRaidCrabBody"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyBase"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase1"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase2"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase3"));
            ChirrFriendController.BlacklistBody(BodyCatalog.FindBodyIndex("MiniVoidRaidCrabBodyPhase4"));

            //ChirrFriendController.bodyDamageValueOverrides.Add(BodyCatalog.FindBodyIndex("ClayBruiserBody"), 1f);
            ChirrFriendController.bodyDamageValueOverrides.Add(brotherBodyIndex, 10f);
            ChirrFriendController.bodyDamageValueOverrides.Add(BodyCatalog.FindBodyIndex("BrotherHurtBody"), 10f);


            ModCompat.SurvariantsCompat.SetVariant(survivorDef, "ChirrBody");
        }
        private void Setup()
        {
            chirrPrefab = CreateChirrPrefab();
            R2API.ItemAPI.DoNotAutoIDRSFor(chirrPrefab);
            chirrPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(ChirrMain));

            brotherKillChirrTokens.Add("SS2UBROTHER_KILL_CHIRR1");
            brotherKillChirrTokens.Add("SS2UBROTHER_KILL_CHIRR2");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;
            CreateDoppelganger();

            survivorDef = Modules.Prefabs.RegisterNewSurvivor(chirrPrefab, Cores.PrefabCore.CreateDisplayPrefab("ChirrDisplay", chirrPrefab), Color.green, "SS2UCHIRR", 40.2f);

            ChirrSkins.RegisterSkins();
            RoR2.RoR2Application.onLoad += OnLoadActions;
            if (brotherKillChirrTokens.Count > 0) RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            RoR2.Run.onRunStartGlobal += ResetMithrixConvertedTracker;

            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SS2UChirrBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animChirrEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }

        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            ChirrItemDisplays.RegisterDisplays();
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport report)
        {
            if (EnemyCore.IsMoon() && report.victimBody && report.victimBody.bodyIndex == bodyIndex && report.attackerBody && (report.attackerBody.bodyIndex == EnemyCore.brotherHurtIndex || report.attackerBody.bodyIndex == EnemyCore.brotherIndex))
            {
                int index = UnityEngine.Random.Range(0, brotherKillChirrTokens.Count);
                EnemyCore.FakeMithrixChatMessageServer(brotherKillChirrTokens[index]);
            }
        }

        private void ResetMithrixConvertedTracker(Run obj)
        {
            survivorDef.outroFlavorToken = "SS2UCHIRR_OUTRO_FLAVOR";
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(JetpackOn));
            Modules.States.AddState(typeof(ChirrMain));

            Modules.States.AddState(typeof(ChirrPrimary));
            Modules.States.AddState(typeof(Headbutt));
            Modules.States.AddState(typeof(ChirrHeal));

            Modules.States.AddState(typeof(Befriend));
            Modules.States.AddState(typeof(BefriendScepter));
            Modules.States.AddState(typeof(Leash));


            Modules.States.AddState(typeof(ChirrRestEmote));
            Modules.States.AddState(typeof(ChirrTauntLoopEmote));
        }

        private void RegisterProjectiles()
        {

            GameObject chirrTargetIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "SS2UChirrTargetIndicator", false);
            chirrTargetIndicator.AddComponent<NetworkIdentity>();
            chirrTargetIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrTargetCrosshair");
            chirrTargetIndicator.transform.localScale = new Vector3(.04f,.04f,.04f);
            chirrTargetIndicator.GetComponentInChildren<RoR2.UI.MPEventSystemProvider>().transform.rotation = Quaternion.Euler(0,0,-45);
            chirrTargetIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrTargetIndicator.GetComponentInChildren<TextMeshPro>().enabled = false;
            SpriteRenderer sr = chirrTargetIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            GameObject chirrBefriendIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "SS2UChirrTargetIndicator", false);
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrBefriendCrosshair");
            chirrBefriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().transform.rotation = Quaternion.Euler(0, 0, 0);
            chirrBefriendIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrBefriendIndicator.GetComponentInChildren<RoR2.InputBindingDisplayController>().actionName = "SpecialSkill";
            sr = chirrBefriendIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            GameObject chirrFriendIndicator = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/WoodSpriteIndicator"), "SS2UChirrFriendIndicator", false);
            chirrFriendIndicator.AddComponent<NetworkIdentity>();
            chirrFriendIndicator.GetComponentInChildren<UnityEngine.SpriteRenderer>().sprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texChirrFriendCrosshair");
            chirrFriendIndicator.transform.localScale = new Vector3(.04f, .04f, .04f);
            chirrFriendIndicator.GetComponentInChildren<RoR2.UI.MPEventSystemProvider>().transform.rotation = Quaternion.Euler(0, 0, -45);
            chirrFriendIndicator.GetComponentInChildren<Rewired.ComponentControls.Effects.RotateAroundAxis>().enabled = false;
            chirrFriendIndicator.GetComponentInChildren<TextMeshPro>().enabled = false;
            sr = chirrFriendIndicator.GetComponentInChildren<SpriteRenderer>();
            sr.color = Color.white;

            ChirrFriendController.indicatorCannotBefriendPrefab = chirrTargetIndicator;
            ChirrFriendController.indicatorReadyToBefriendPrefab = chirrBefriendIndicator;
            ChirrFriendController.indicatorFriendPrefab = chirrFriendIndicator;

            //RoR2/Base/Treebot/SeedpodMortarGhost.prefab
            //"RoR2/Base/Treebot/SyringeProjectile.prefab"
            GameObject chirrDart = BuildChirrDart();
            ChirrPrimary.projectilePrefab = chirrDart;

           /*GameObject chirrDartCenter = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/SyringeProjectileHealing.prefab").WaitForCompletion(), "SS2UChirrDartCenterProjectile", true);
            UnityEngine.Object.Destroy(chirrDartCenter.GetComponent<RoR2.Projectile.ProjectileHealOwnerOnDamageInflicted>());
            ProjectileDamage dartCenterDamage = chirrDartCenter.GetComponent<ProjectileDamage>();
            dartCenterDamage.damageType = DamageType.Generic;
            Modules.Prefabs.projectilePrefabs.Add(chirrDartCenter);
            ChirrPrimary.centerProjectilePrefab = chirrDartCenter;*/
        }

        private GameObject BuildChirrDart()
        {
            GameObject projectilePrefab = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ElitePoison/UrchinSeekingProjectile.prefab").WaitForCompletion(), "SS2UChirrDartProjectile", true);

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 130f;
            ps.lifetime = 2f;

            ProjectileSteerTowardTarget pst = projectilePrefab.GetComponent<ProjectileSteerTowardTarget>();
            pst.rotationSpeed = 20f;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            ProjectileDirectionalTargetFinder pdtf = projectilePrefab.GetComponent<ProjectileDirectionalTargetFinder>();
            pdtf.lookRange = 45f;
            pdtf.lookCone = 12f;
            pdtf.targetSearchInterval = 0.1f;

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Generic;

            ProjectileSingleTargetImpact psti = projectilePrefab.GetComponent<ProjectileSingleTargetImpact>();
            psti.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/OmniImpactVFXSlashSyringe.prefab").WaitForCompletion();

            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);
            return projectilePrefab;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in chirrPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = chirrPrefab.GetComponent<SkillLocator>();

            SetUpPassive(skillLocator);
            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);
        }

        private void SetUpPassive(SkillLocator skillLocator)
        {
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "SS2UCHIRR_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "SS2UCHIRR_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrPassive");
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            SkillDef primaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(ChirrPrimary));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "SS2UCHIRR_DARTS_NAME";
            primaryDef1.skillNameToken = "SS2UCHIRR_DARTS_NAME";
            primaryDef1.skillDescriptionToken = "SS2UCHIRR_DARTS_DESCRIPTION";
            primaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrPrimary");
            primaryDef1.baseMaxStock = 1;
            primaryDef1.baseRechargeInterval = 0f;
            primaryDef1.beginSkillCooldownOnSkillEnd = false;
            primaryDef1.canceledFromSprinting = false;
            primaryDef1.fullRestockOnAssign = true;
            primaryDef1.interruptPriority = InterruptPriority.Any;
            primaryDef1.isCombatSkill = true;
            primaryDef1.mustKeyPress = false;
            primaryDef1.cancelSprintingOnActivation = true;
            primaryDef1.rechargeStock = 1;
            primaryDef1.requiredStock = 1;
            primaryDef1.stockToConsume = 1;
            primaryDef1.keywordTokens = new string[] {};
            Modules.Skills.FixSkillName(primaryDef1);

            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            skillLocator.primary = Utils.RegisterSkillsToFamily(chirrPrefab, primaryVariant1);
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            SkillDef secondaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef1.activationState = new SerializableEntityStateType(typeof(Headbutt));
            secondaryDef1.activationStateMachineName = "Weapon";
            secondaryDef1.skillName = "SS2UCHIRR_HEADBUTT_NAME";
            secondaryDef1.skillNameToken = "SS2UCHIRR_HEADBUTT_NAME";
            secondaryDef1.skillDescriptionToken = "SS2UCHIRR_HEADBUTT_DESCRIPTION";
            secondaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSecondary");
            secondaryDef1.baseMaxStock = 1;
            secondaryDef1.baseRechargeInterval = 6f;
            secondaryDef1.beginSkillCooldownOnSkillEnd = false;
            secondaryDef1.canceledFromSprinting = false;
            secondaryDef1.fullRestockOnAssign = true;
            secondaryDef1.interruptPriority = InterruptPriority.Skill;
            secondaryDef1.isCombatSkill = true;
            secondaryDef1.mustKeyPress = false;
            secondaryDef1.cancelSprintingOnActivation = false;
            secondaryDef1.rechargeStock = 1;
            secondaryDef1.requiredStock = 1;
            secondaryDef1.stockToConsume = 1;
            secondaryDef1.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixSkillName(secondaryDef1);

            Modules.Skills.skillDefs.Add(secondaryDef1);
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef1);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(chirrPrefab, secondaryVariant1);
        }
        
        private void SetUpUtilities(SkillLocator skillLocator)
        {
            SkillDef utilityDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef1.activationState = new SerializableEntityStateType(typeof(ChirrHeal));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "SS2UCHIRR_HEAL_NAME";
            utilityDef1.skillNameToken = "SS2UCHIRR_HEAL_NAME";
            utilityDef1.skillDescriptionToken = "SS2UCHIRR_HEAL_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrUtility");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 12f;
            utilityDef1.beginSkillCooldownOnSkillEnd = true;
            utilityDef1.canceledFromSprinting = false;
            utilityDef1.fullRestockOnAssign = true;
            utilityDef1.interruptPriority = InterruptPriority.Skill;
            utilityDef1.isCombatSkill = false;
            utilityDef1.mustKeyPress = false;
            utilityDef1.cancelSprintingOnActivation = false;
            utilityDef1.rechargeStock = 1;
            utilityDef1.requiredStock = 1;
            utilityDef1.stockToConsume = 1;
            Modules.Skills.FixSkillName(utilityDef1);

            Modules.Skills.skillDefs.Add(utilityDef1);
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            skillLocator.utility = Utils.RegisterSkillsToFamily(chirrPrefab, utilityVariant1);
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            BefriendSkillDef befriendDef = ScriptableObject.CreateInstance<BefriendSkillDef>();
            befriendDef.activationState = new SerializableEntityStateType(typeof(Befriend));
            befriendDef.activationStateMachineName = "Befriend";
            befriendDef.skillName = "SS2UCHIRR_BEFRIEND_NAME";
            befriendDef.skillNameToken = "SS2UCHIRR_BEFRIEND_NAME";
            befriendDef.skillDescriptionToken = "SS2UCHIRR_BEFRIEND_DESCRIPTION";
            befriendDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial1");
            befriendDef.baseMaxStock = 1;
            befriendDef.baseRechargeInterval = 3f;
            befriendDef.beginSkillCooldownOnSkillEnd = false;
            befriendDef.canceledFromSprinting = false;
            befriendDef.fullRestockOnAssign = true;
            befriendDef.interruptPriority = InterruptPriority.Any;
            befriendDef.isCombatSkill = false;
            befriendDef.mustKeyPress = true;
            befriendDef.cancelSprintingOnActivation = false;
            befriendDef.rechargeStock = 1;
            befriendDef.requiredStock = 1;
            befriendDef.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(befriendDef);
            ChirrCore.specialDef = befriendDef;
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(befriendDef);
            skillLocator.special = Utils.RegisterSkillsToFamily(chirrPrefab, specialVariant1);
            Modules.Skills.FixSkillName(befriendDef);

            SkillDef leashDef = ScriptableObject.CreateInstance<SkillDef>();    //FriendLeashSkillDef   //Don't need the min leash distance.
            leashDef.activationState = new SerializableEntityStateType(typeof(Leash));
            leashDef.activationStateMachineName = "Leash";
            leashDef.skillName = "SS2UCHIRR_LEASH_NAME";
            leashDef.skillNameToken = "SS2UCHIRR_LEASH_NAME";
            leashDef.skillDescriptionToken = "SS2UCHIRR_LEASH_DESCRIPTION";
            leashDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecial2");
            leashDef.baseMaxStock = 1;
            leashDef.baseRechargeInterval = 12f;
            leashDef.beginSkillCooldownOnSkillEnd = false;
            leashDef.canceledFromSprinting = false;
            leashDef.fullRestockOnAssign = true;
            leashDef.interruptPriority = InterruptPriority.Skill;
            leashDef.isCombatSkill = false;
            leashDef.mustKeyPress = true;
            leashDef.cancelSprintingOnActivation = false;
            leashDef.rechargeStock = 1;
            leashDef.requiredStock = 1;
            leashDef.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(leashDef);
            Befriend.leashOverrideSkillDef = leashDef;
            Modules.Skills.FixSkillName(leashDef);

            BefriendSkillDef befriendScepterDef = ScriptableObject.CreateInstance<BefriendSkillDef>();
            befriendScepterDef.activationState = new SerializableEntityStateType(typeof(BefriendScepter));
            befriendScepterDef.activationStateMachineName = "Befriend";
            befriendScepterDef.skillName = "SS2UCHIRR_BEFRIEND_SCEPTER_NAME";
            befriendScepterDef.skillNameToken = "SS2UCHIRR_BEFRIEND_SCEPTER_NAME";
            befriendScepterDef.skillDescriptionToken = "SS2UCHIRR_BEFRIEND_SCEPTER_DESCRIPTION";
            befriendScepterDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("ChirrSpecialScepter");
            befriendScepterDef.baseMaxStock = 1;
            befriendScepterDef.baseRechargeInterval = 3f;
            befriendScepterDef.beginSkillCooldownOnSkillEnd = true;
            befriendScepterDef.canceledFromSprinting = false;
            befriendScepterDef.fullRestockOnAssign = true;
            befriendScepterDef.interruptPriority = InterruptPriority.Any;
            befriendScepterDef.isCombatSkill = false;
            befriendScepterDef.mustKeyPress = true;
            befriendScepterDef.cancelSprintingOnActivation = false;
            befriendScepterDef.rechargeStock = 1;
            befriendScepterDef.requiredStock = 1;
            befriendScepterDef.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(befriendScepterDef);
            ChirrCore.specialScepterDef = befriendScepterDef;
            Modules.Skills.FixSkillName(befriendScepterDef);

            if (StarstormPlugin.scepterPluginLoaded)
            {
                ScepterSetup();
            }
            if (StarstormPlugin.classicItemsLoaded)
            {
                ClassicScepterSetup();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSetup()
        {

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialScepterDef, "SS2UChirrBody", specialDef);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(specialScepterDef, "SS2UChirrBody", SkillSlot.Special, specialDef);
        }

        public static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "SS2UChirrMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = chirrPrefab;

            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);


            Modules.Prefabs.AddAISkillDriver(doppelganger, "Secondary", SkillSlot.Secondary, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 0f, 8f,
                 false, false, false, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 false, false, false,
                 AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                 AISkillDriver.AimType.AtMoveTarget,
                 false,
                 true,
                 false,
                 AISkillDriver.ButtonPressType.Hold,
                 -1f,
                 false,
                 false,
                 null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "HealSelf", SkillSlot.Utility, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 false, false, false, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 false, false, false,
                 AISkillDriver.MovementType.FleeMoveTarget, 1f,
                 AISkillDriver.AimType.AtMoveTarget,
                 false,
                 true,
                 false,
                 AISkillDriver.ButtonPressType.Abstain,
                 -1f,
                 true,
                 true,
                 null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 45f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Strafe", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                30f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.masterPrefabs.Add(doppelganger);
        }

        internal static GameObject CreateChirrPrefab()
        {
            chirrPrefab = Cores.PrefabCore.CreatePrefab("SS2UChirrBody", "mdlChirr", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "SS2UChirrBody",
                bodyNameToken = "SS2UCHIRR_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("ChirrIcon"),
                bodyColor = new Color32(129, 167, 98, 255),
                crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotCrosshair.prefab").WaitForCompletion(),
                damage = 12f,
                healthGrowth = 30f,
                healthRegen = 1f,
                jumpCount = 1,
                jumpPower = 22.5f,    //15f is standard
                maxHealth = 100f,
                subtitleNameToken = "SS2UCHIRR_SUBTITLE",
                podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            });

            Cores.PrefabCore.SetupCharacterModel(chirrPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.LoadMaterialFromAssetBundle("matChirr")
                },
                new CustomRendererInfo
                {
                    childName = "ModelDress",
                    material = Modules.Assets.LoadMaterialFromAssetBundle("matChirrMaidDress")
                }
            }, 0);

            // create hitboxes

            GameObject model = chirrPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            Cores.PrefabCore.SetupHitbox(model, childLocator.FindChild("HeadbuttHitbox"), "HeadbuttHitbox");

            NetworkStateMachine nsm = chirrPrefab.GetComponent<NetworkStateMachine>();

            bool hadSlide = true;
            EntityStateMachine jetpackStateMachine = EntityStateMachine.FindByCustomName(chirrPrefab, "Slide");
            if (!jetpackStateMachine)
            {
                hadSlide = false;
                jetpackStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            }
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            if (!hadSlide)
            {
                SetStateOnHurt ssoh = chirrPrefab.GetComponent<SetStateOnHurt>();
                ssoh.idleStateMachine.Append(jetpackStateMachine);
            }

            chirrPrefab.AddComponent<ChirrFriendController>();
            chirrPrefab.AddComponent<ChirrLeashSkillOverrideController>();
            EntityStateMachine befriendStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            befriendStateMachine.customName = "Befriend";
            befriendStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            befriendStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(befriendStateMachine).ToArray();

            EntityStateMachine leashStateMachine = chirrPrefab.AddComponent<EntityStateMachine>();
            leashStateMachine.customName = "Leash";
            leashStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            leashStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(leashStateMachine).ToArray();

            CameraTargetParams ctp = chirrPrefab.GetComponent<CameraTargetParams>();
            if (ctp && ctp.cameraPivotTransform)
            {
                ctp.cameraPivotTransform.localPosition = new Vector3(0f, 1.8f, 0f); //was (0f, 1.6f, 0f)
            }

            return chirrPrefab;
        }
    }
}
