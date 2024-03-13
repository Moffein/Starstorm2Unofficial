using BepInEx.Configuration;
using EntityStates;
using EntityStates.SS2UStates.Nucleator;
using EntityStates.SS2UStates.Nucleator.Primary;
using EntityStates.SS2UStates.Nucleator.Secondary;
using EntityStates.SS2UStates.Nucleator.Special;
using EntityStates.SS2UStates.Nucleator.Utility;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Survivors;
using Starstorm2Unofficial.Survivors.Nemmando;
using Starstorm2Unofficial.Survivors.Nucleator;
using Starstorm2Unofficial.Survivors.Nucleator.Components;
using Starstorm2Unofficial.Survivors.Nucleator.Components.Crosshair;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Starstorm2Unofficial.Survivors.Nucleator
{
    internal class NucleatorCore : SurvivorBase
    {
        public static class SkillDefs
        {
            public static SkillDef Primary, Secondary, Utility, Special, SpecialScepter;
        }

        public static BodyIndex bodyIndex;
        internal override string bodyName { get; set; } = "SS2UNucleator";
        internal override string modelName { get; set; } = "mdlNucleator";
        internal override string displayName { get; set; } = "NucleatorDisplay";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 40.4f;

        internal override StarstormBodyInfo bodyInfo { get; set; } = new StarstormBodyInfo
        {
            armor = 20f,
            bodyName = "SS2UNucleatorBody",
            bodyNameToken = "SS2UNUCLEATOR_NAME",
            bodyColor = new Color(0.8039216f, 0.482352942f, 0.843137264f),
            characterPortrait = Modules.Assets.LoadCharacterIcon("Nucleator"),
            crosshair = Modules.Assets.LoadCrosshair("ToolbotGrenadeLauncherCrosshair"),
            damage = 12f,
            healthGrowth = 48f,
            healthRegen = 1.0f,
            jumpCount = 1,
            maxHealth = 160f,
            subtitleNameToken = "SS2UNUCLEATOR_SUBTITLE",
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
        };

        internal static Material nucleatorMat = Modules.Assets.CreateMaterial("matNucleator", 2f, new Color32(219, 201, 245, 255));
        internal override int mainRendererIndex { get; set; } = 0;

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
        new CustomRendererInfo
        {
            childName = "Model",
            material = nucleatorMat,
        }};

        internal override Type characterMainState { get; set; } = typeof(EntityStates.GenericCharacterMain);

        // item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }
        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;

        internal static GameObject chargeCrosshair { get; set; }
        internal static GameObject primaryProjectile { get; set; }

        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();
            R2API.ItemAPI.DoNotAutoIDRSFor(bodyPrefab);

            EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(bodyPrefab, "Body");
            bodyMachine.mainStateType = new SerializableEntityStateType(typeof(NucleatorMain));

            NetworkStateMachine nsm = bodyPrefab.GetComponent<NetworkStateMachine>();

            EntityStateMachine specialStateMachine = bodyPrefab.AddComponent<EntityStateMachine>();
            specialStateMachine.customName = "SpecialBuff";
            specialStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            specialStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            
            nsm.stateMachines = nsm.stateMachines.Append(specialStateMachine).ToArray();

            CharacterBody cb = bodyPrefab.GetComponent<CharacterBody>();
            cb._defaultCrosshairPrefab = BuildCrosshair();

            CharacterMotor cm = bodyPrefab.GetComponent<CharacterMotor>();
            cm.mass = 300f;

            bodyPrefab.AddComponent<NucleatorChargeComponent>();
            bodyPrefab.AddComponent<NucleatorNetworkComponent>();

            /*CameraTargetParams cameraTargetParams = bodyPrefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams.data.idealLocalCameraPos = new Vector3(0f, 1.2f, -11f);*/

            CameraTargetParams ctp = bodyPrefab.GetComponent<CameraTargetParams>();

            var _cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
            _cameraParams.data.minPitch = -70;
            _cameraParams.data.maxPitch = 70;
            _cameraParams.data.wallCushion = 0.1f;
            _cameraParams.data.pivotVerticalOffset = 1.7f;
            _cameraParams.data.idealLocalCameraPos = new Vector3(0f, 1.2f, -12f);

            ctp.cameraParams = _cameraParams;

            FireIrradiate.projectilePrefab = NucleatorProjectiles.BuildPrimary();
            FireIrradiateOvercharge.projectilePrefab = NucleatorProjectiles.BuildPrimaryOvercharge();

            FireSecondary.coneEffectPrefab = BuildSecondaryVFX("SS2UNucleatorSecondaryEffect", new Color(244f / 255f, 243f / 255f, 183f / 255f));
            FireSecondaryOvercharge.overchargeEffectPrefab = BuildSecondaryVFX("SS2UNucleatorSecondaryOverchargeEffect", new Color(1f, 0f, 1f));

            RoR2Application.onLoad += SetBodyIndex;
            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();
        }

        private GameObject BuildSecondaryVFX(string name, Color color)
        {
            GameObject effect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoLeapExplosion.prefab").WaitForCompletion().InstantiateClone(name, false);

            UnityEngine.Object.Destroy(effect.GetComponent<ShakeEmitter>());

            EffectComponent ec = effect.GetComponent<EffectComponent>();
            ec.soundName = "";

            VFXAttributes vfx = effect.GetComponent<VFXAttributes>();
            vfx.vfxPriority = VFXAttributes.VFXPriority.Always;

            ParticleSystem[] particles = effect.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                if (particles[i].name == "AreaIndicator")
                {
                    UnityEngine.Object.Destroy(particles[i]);
                }
                else
                {
                    particles[i].startColor = color;
                }
            }

            Modules.Assets.AddEffect(effect);
            return effect;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SS2UNucleatorBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animNucleatorEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }

        internal override void InitializeUnlockables()
        {
            //masterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Cores.Unlockables.Achievements.NucleatorMasteryAchievement>(true);
            //grandMasterySkinUnlockableDef = Modules.Unlockables.AddUnlockable<Cores.Unlockables.Achievements.NucleatorGrandMasteryAchievement>(true);
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(NucleatorMain));

            Modules.States.AddState(typeof(ChargeIrradiate));
            Modules.States.AddState(typeof(FireIrradiate));
            Modules.States.AddState(typeof(FireIrradiateOvercharge));

            Modules.States.AddState(typeof(ChargeSecondary));
            Modules.States.AddState(typeof(FireSecondary));
            Modules.States.AddState(typeof(FireSecondaryOvercharge));

            Modules.States.AddState(typeof(ChargeLeap));
            Modules.States.AddState(typeof(FireLeap));
            Modules.States.AddState(typeof(FireLeapOvercharge));

            Modules.States.AddState(typeof(BuffSelf));
            Modules.States.AddState(typeof(BuffSelfScepter));
        }


        internal override void InitializeSkills()
        {
            RegisterStates();
            foreach (GenericSkill sk in bodyPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorPassive");   //Threw together a placeholder. Probably want an actual icon later?
            skillLocator.passiveSkill.skillNameToken = "SS2UNUCLEATOR_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "SS2UNUCLEATOR_PASSIVE_DESCRIPTION";

            SetupPrimaries(skillLocator);
            SetupSecondaries(skillLocator);
            SetupUtilities(skillLocator);
            SetupSpecials(skillLocator);
        }

        private void SetupPrimaries(SkillLocator skillLocator)
        {
            SteppedSkillDef primaryDef = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef.activationState = new SerializableEntityStateType(typeof(ChargeIrradiate));
            primaryDef.activationStateMachineName = "Weapon";
            primaryDef.skillName = "SS2UNUCLEATOR_PRIMARY_NAME";
            primaryDef.skillNameToken = "SS2UNUCLEATOR_PRIMARY_NAME";
            primaryDef.skillDescriptionToken = "SS2UNUCLEATOR_PRIMARY_DESCRIPTION";
            primaryDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorPrimary");
            primaryDef.baseMaxStock = 1;
            primaryDef.baseRechargeInterval = 0f;
            primaryDef.beginSkillCooldownOnSkillEnd = false;
            primaryDef.canceledFromSprinting = false;
            primaryDef.fullRestockOnAssign = true;
            primaryDef.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryDef.isCombatSkill = true;
            primaryDef.mustKeyPress = true;
            primaryDef.cancelSprintingOnActivation = true;
            primaryDef.rechargeStock = 1;
            primaryDef.requiredStock = 1;
            primaryDef.stockToConsume = 1;

            //Steps juste used to alternate which arm uncharged primary fires from.
            primaryDef.stepCount = 2;
            primaryDef.stepGraceDuration = 60f;

            Modules.Skills.FixSkillName(primaryDef);
            Modules.Skills.skillDefs.Add(primaryDef);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef);
            skillLocator.primary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { primaryVariant1 });
            SkillDefs.Primary = primaryDef;
        }

        private void SetupSecondaries(SkillLocator skillLocator)
        {
            SkillDef secondaryDef = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef.activationState = new SerializableEntityStateType(typeof(ChargeSecondary));
            secondaryDef.activationStateMachineName = "Weapon";
            secondaryDef.skillName = "SS2UNUCLEATOR_SECONDARY_NAME";
            secondaryDef.skillNameToken = "SS2UNUCLEATOR_SECONDARY_NAME";
            secondaryDef.skillDescriptionToken = "SS2UNUCLEATOR_SECONDARY_DESCRIPTION";
            secondaryDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorSecondary");
            secondaryDef.baseMaxStock = 1;
            secondaryDef.baseRechargeInterval = 5f;
            secondaryDef.beginSkillCooldownOnSkillEnd = true;
            secondaryDef.canceledFromSprinting = false;
            secondaryDef.fullRestockOnAssign = true;
            secondaryDef.interruptPriority = EntityStates.InterruptPriority.Any;
            secondaryDef.isCombatSkill = true;
            secondaryDef.mustKeyPress = true;
            secondaryDef.cancelSprintingOnActivation = true;
            secondaryDef.rechargeStock = 1;
            secondaryDef.requiredStock = 1;
            secondaryDef.stockToConsume = 1;
            secondaryDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixSkillName(secondaryDef);
            Modules.Skills.skillDefs.Add(secondaryDef);
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef);
            skillLocator.secondary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { secondaryVariant1 });
            SkillDefs.Secondary = secondaryDef;
        }

        private void SetupUtilities(SkillLocator skillLocator)
        {
            SkillDef utilityDef = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef.activationState = new SerializableEntityStateType(typeof(ChargeLeap));
            utilityDef.activationStateMachineName = "Weapon";   //Hacky, state forces the body machine to change
            utilityDef.skillName = "SS2UNUCLEATOR_UTILITY_NAME";
            utilityDef.skillNameToken = "SS2UNUCLEATOR_UTILITY_NAME";
            utilityDef.skillDescriptionToken = "SS2UNUCLEATOR_UTILITY_DESCRIPTION";
            utilityDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorUtility");
            utilityDef.baseMaxStock = 1;
            utilityDef.baseRechargeInterval = 7f;
            utilityDef.beginSkillCooldownOnSkillEnd = true;
            utilityDef.canceledFromSprinting = false;
            utilityDef.fullRestockOnAssign = true;
            utilityDef.interruptPriority = EntityStates.InterruptPriority.Any;
            utilityDef.isCombatSkill = true;
            utilityDef.mustKeyPress = true;
            utilityDef.cancelSprintingOnActivation = true;
            utilityDef.rechargeStock = 1;
            utilityDef.requiredStock = 1;
            utilityDef.stockToConsume = 1;
            utilityDef.keywordTokens = new string[] { "KEYWORD_STUNNING" };
            Modules.Skills.FixSkillName(utilityDef);
            Modules.Skills.skillDefs.Add(utilityDef);
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef);
            skillLocator.utility = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { utilityVariant1 });
            SkillDefs.Utility = utilityDef;
        }

        private void SetupSpecials(SkillLocator skillLocator)
        {
            SkillDef specialDef = ScriptableObject.CreateInstance<SkillDef>();
            specialDef.activationState = new SerializableEntityStateType(typeof(BuffSelf));
            specialDef.activationStateMachineName = "SpecialBuff";
            specialDef.skillName = "SS2UNUCLEATOR_SPECIAL_NAME";
            specialDef.skillNameToken = "SS2UNUCLEATOR_SPECIAL_NAME";
            specialDef.skillDescriptionToken = "SS2UNUCLEATOR_SPECIAL_DESCRIPTION";
            specialDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorSpecial");
            specialDef.baseMaxStock = 1;
            specialDef.baseRechargeInterval = 10f;
            specialDef.beginSkillCooldownOnSkillEnd = true;
            specialDef.canceledFromSprinting = false;
            specialDef.fullRestockOnAssign = true;
            specialDef.interruptPriority = EntityStates.InterruptPriority.Any;
            specialDef.isCombatSkill = false;
            specialDef.mustKeyPress = true;
            specialDef.cancelSprintingOnActivation = true;
            specialDef.rechargeStock = 1;
            specialDef.requiredStock = 1;
            specialDef.stockToConsume = 1;
            specialDef.keywordTokens = new string[] { "KEYWORD_SS2U_RADIATION" };
            Modules.Skills.FixSkillName(specialDef);
            Modules.Skills.skillDefs.Add(specialDef);
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(specialDef);
            skillLocator.special = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { specialVariant1 });
            SkillDefs.Special = specialDef;

            SkillDef specialScepterDef = ScriptableObject.CreateInstance<SkillDef>();
            specialScepterDef.activationState = new SerializableEntityStateType(typeof(BuffSelfScepter));
            specialScepterDef.activationStateMachineName = "SpecialBuff";
            specialScepterDef.skillName = "SS2UNUCLEATOR_SPECIAL_SCEPTER_NAME";
            specialScepterDef.skillNameToken = "SS2UNUCLEATOR_SPECIAL_SCEPTER_NAME";
            specialScepterDef.skillDescriptionToken = "SS2UNUCLEATOR_SPECIAL_SCEPTER_DESCRIPTION";
            specialScepterDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNucleatorSpecialScepter");
            specialScepterDef.baseMaxStock = 1;
            specialScepterDef.baseRechargeInterval = 10f;
            specialScepterDef.beginSkillCooldownOnSkillEnd = true;
            specialScepterDef.canceledFromSprinting = false;
            specialScepterDef.fullRestockOnAssign = true;
            specialScepterDef.interruptPriority = EntityStates.InterruptPriority.Any;
            specialScepterDef.isCombatSkill = false;
            specialScepterDef.mustKeyPress = true;
            specialScepterDef.cancelSprintingOnActivation = true;
            specialScepterDef.rechargeStock = 1;
            specialScepterDef.requiredStock = 1;
            specialScepterDef.stockToConsume = 1;
            specialScepterDef.keywordTokens = new string[] { "KEYWORD_SS2U_RADIATION"};
            Modules.Skills.FixSkillName(specialScepterDef);
            SkillDefs.SpecialScepter = specialScepterDef;

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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(SkillDefs.SpecialScepter, bodyInfo.bodyName, SkillDefs.Special);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(SkillDefs.SpecialScepter, bodyInfo.bodyName, SkillSlot.Special, SkillDefs.Special);
        }

        internal override void RegisterTokens()
        {
            LanguageAPI.Add("SS2UNUCLEATOR_NAME", "Nucleator");
            LanguageAPI.Add("SS2UNUCLEATOR_SUBTITLE", "Walking Fallout");
            LanguageAPI.Add("SS2UNUCLEATOR_DESCRIPTION", "The Nucleator is a radioactive juggernaut with rad-proof armor, which allows him to manipulate nuclear components for long periods of time.<style=cSub>\r\n\r\n< ! > Nucleator can charge his skills for maximum output, but be careful as overcharging them may lead to self-harm!\r\n\r\n< ! > Irradiate's projectiles gain increased blast radius the further they travel.\r\n\r\n< ! > Quarantine always applies the same knockback force regardless of charge level.\r\n\r\n< ! > Strategic use of Radionuclide Surge will allow you to recover health lost from Overcharging skills.\r\n\r\n");
            LanguageAPI.Add("SS2UNUCLEATOR_OUTRO_FLAVOR", "..and so he left, health status undisclosed.");
            LanguageAPI.Add("SS2UNUCLEATOR_OUTRO_FAILURE", "..and so he vanished, an uninhabitable wasteland in his wake.");
            LanguageAPI.Add("SS2UNUCLEATOR_LORE", "laugh and grow fat");

            LanguageAPI.Add("SS2UNUCLEATOR_PASSIVE_NAME", "Total Meltdown");
            LanguageAPI.Add("SS2UNUCLEATOR_PASSIVE_DESCRIPTION", "The Nucleator's skills are unaffected <style=cIsDamage>Attack Speed</style>. <style=cIsDamage>Attack Speed</style> is converted to <style=cIsDamage>Damage</style> instead.");

            LanguageAPI.Add("SS2UNUCLEATOR_PRIMARY_NAME", "Irradiate");
            LanguageAPI.Add("SS2UNUCLEATOR_PRIMARY_DESCRIPTION", "Charge and fire a glob of nuclear waste for <style=cIsDamage>200%-720% damage</style>, up to <style=cIsDamage>1080% damage</style> on <style=cIsHealth>Overcharge</style>.");

            LanguageAPI.Add("SS2UNUCLEATOR_SECONDARY_NAME", "Quarantine");
            LanguageAPI.Add("SS2UNUCLEATOR_SECONDARY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Charge up and <style=cIsUtility>push</style> enemies for <style=cIsDamage>400%-800% damage</style>, deals up to <style=cIsDamage>1200% damage</style> and <style=cIsDamage>roots</style> on <style=cIsHealth>Overcharge</style>.");

            LanguageAPI.Add("SS2UNUCLEATOR_UTILITY_NAME", "Fission Impulse");
            LanguageAPI.Add("SS2UNUCLEATOR_UTILITY_DESCRIPTION", "<style=cIsDamage>Stunning</style>. Charge up a leap and gain <style=cIsUtility>200 armor</style>. Deals <style=cIsDamage>400%-800% damage</style> on impact, up to <style=cIsDamage>1200% damage</style> on <style=cIsHealth>Overcharge</style>.");

            LanguageAPI.Add("SS2UNUCLEATOR_SPECIAL_NAME", "Radionuclide Surge");
            LanguageAPI.Add("SS2UNUCLEATOR_SPECIAL_DESCRIPTION", "Go nuclear for <style=cIsUtility>6 seconds</style>, <style=cIsHealing>healing from <style=cIsHealth>Overcharge</style></style> and adding <style=cIsDamage>Radiation Poisoning</style> to every attack.");

            LanguageAPI.Add("SS2UNUCLEATOR_SPECIAL_SCEPTER_NAME", "Radionuclide Efflux");
            LanguageAPI.Add("SS2UNUCLEATOR_SPECIAL_SCEPTER_DESCRIPTION", $"Enter a nuclear state for <style=cIsUtility>12 seconds</style>, becoming <style=cIsUtility>immune to <style=cIsHealth>Overcharge</style> damage</style> and adding <style=cIsDamage>Radiation Poisoning</style> to every attack.");

            LanguageAPI.Add("SS2UNUCLEATOR_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME", "Overkill");
            LanguageAPI.Add("SS2UNUCLEATOR_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC", "Collect 5 Legendary items in one run.");
            LanguageAPI.Add("SS2UNUCLEATOR_UNLOCKUNLOCKABLE_UNLOCKABLE_NAME", "Overkill");

            LanguageAPI.Add("KEYWORD_SS2U_RADIATION", "<style=cKeywordName>Radiation Poisoning</style><style=cSub>Deal damage equivalent to <style=cIsDamage>5%</style> of their maximum health over 5s.</style>");

            // todo: make a base class for mastery achievements and simply inherit from it for each character 
            LanguageAPI.Add("SS2UNUCLEATOR_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Nucleator: Mastery");
            LanguageAPI.Add("SS2UNUCLEATOR_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Nucleator, beat the game or obliterate on Monsoon or harder.");
            LanguageAPI.Add("SS2UNUCLEATOR_MASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Nucleator: Mastery");

            LanguageAPI.Add("SS2UNUCLEATOR_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Nucleator: Grand Mastery");
            LanguageAPI.Add("SS2UNUCLEATOR_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Nucleator, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add("SS2UNUCLEATOR_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Nucleator: Grand Mastery");
        }

        internal override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("SS2UNUCLEATOR_DEFAULT_SKIN",
                LoadoutAPI.CreateSkinIcon(new Color32(219, 201, 245, 255), new Color32(92, 97, 69, 255), new Color32(71, 59, 63, 255), new Color32(180, 174, 104, 255)),
                defaultRenderers,
                mainRenderer,
                model);
            defaultSkin.nameToken = "DEFAULT_SKIN";

            skins.Add(defaultSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }
        internal override void InitializeDoppelganger()
        {
            GameObject doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "SS2UNucleatorMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Special", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.MoveDirection,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                true,
                null);


            Modules.Prefabs.AddAISkillDriver(doppelganger, "Leap", SkillSlot.Utility, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                30f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.MoveDirection,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Secondary", SkillSlot.Secondary, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 0f, 20f,
                 true, false, true, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 true, true, true,
                 AISkillDriver.MovementType.StrafeMovetarget, 1f,
                 AISkillDriver.AimType.AtCurrentEnemy,
                 false,
                 false,
                 false,
                 AISkillDriver.ButtonPressType.Hold,
                 -1f,
                 false,
                 true,
                 null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1f,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Strafe", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
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
                40f, Mathf.Infinity,
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

        internal override void SetItemDisplays()
        {

            instance.itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            NucleatorItemDisplays.RegisterDisplays();

            itemDisplayRuleSet.keyAssetRuleGroups = instance.itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];

            return newRendererInfos;
        }

        private static GameObject BuildCrosshair()
        {
            GameObject crosshairPrefab = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("crosshairNucleator.prefab").InstantiateClone("SS2UNucleatorCrosshair", false);
            crosshairPrefab.AddComponent<HudElement>();

            CrosshairController cc = crosshairPrefab.AddComponent<CrosshairController>();
            cc.maxSpreadAngle = 0f;

            crosshairPrefab.AddComponent<NucleatorCrosshairController>();

            return crosshairPrefab;
        }

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UNucleatorBody");
            if (bodyIndex != BodyIndex.None) IgnoreSprintCrosshair.bodies.Add(bodyIndex);
        }
    }
}
