using BepInEx.Configuration;
using EntityStates;
using EntityStates.SS2UStates.Executioner;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using RoR2.Skills;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Achievements;
using Starstorm2Unofficial.Modules.Survivors;
using Starstorm2Unofficial.Survivors.Executioner.Components;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Executioner
{
    internal class ExecutionerCore : SurvivorBase
    {
        private static GameObject fearKillEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2KillEffect.prefab").WaitForCompletion();

        public static BodyIndex bodyIndex;

        internal override string bodyName { get; set; } = "SS2UExecutioner";
        internal override string modelName { get; set; } = "mdlExecutioner";
        internal override string displayName { get; set; } = "ExecutionerDisplay";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 40f;

        internal override StarstormBodyInfo bodyInfo { get; set; } = new StarstormBodyInfo
        {
            armor = 0f,
            bodyName = "SS2UExecutionerBody",
            bodyNameToken = "SS2UEXECUTIONER_NAME",
            bodyColor = new Color(0.69f, 0.44f, 0.49f),
            characterPortrait = Modules.Assets.LoadCharacterIcon("Executioner"),
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            damage = 12f,
            healthGrowth = 33f,
            healthRegen = 1f,
            jumpCount = 1,
            maxHealth = 110f,
            subtitleNameToken = "SS2UEXECUTIONER_SUBTITLE",
            podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
        };

        internal override int mainRendererIndex { get; set; } = 0;

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matExecutioner", 1f)
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                    material = Modules.Assets.CreateMaterial("matExecutioner"),
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "AxeModel",
                    material = Modules.Assets.CreateMaterial("matExecutionerAxe", 1f),
                    ignoreOverlays = true
                }};

        internal override Type characterMainState { get; set; } = typeof(EntityStates.SS2UStates.Executioner.ExecutionerMain);

        // item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }// = Modules.Unlockables.AddUnlockable<Cores.Unlockables.Achievements.ExecutionerUnlockAchievement>(true);
        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;
        private static UnlockableDef wastelanderSkinUnlockableDef;

        public static SkillDef specialDef, specialScepterDef;

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UExecutionerBody");
        }

        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();
            R2API.ItemAPI.DoNotAutoIDRSFor(bodyPrefab);

            RoR2.RoR2Application.onLoad += SetBodyIndex;

            Modules.Assets.LoadExecutionerEffects();

            bodyPrefab.AddComponent<IonGunChargeComponent>();
            bodyPrefab.AddComponent<Components.ExecutionerController>();
            bodyPrefab.AddComponent<CustomEffectComponent>();

            // hate that i have to do this but it works so whatever
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            childLocator.FindChild("AxeSpawnEffect").Find("Lightning").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matJellyfishLightningLarge;

            childLocator.FindChild("DashEffect").Find("DashSkull").Find("Lightning").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matMageMatrixDirectionalLightning;
            childLocator.FindChild("DashEffect").Find("DashSkull").Find("LightningRound").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matJellyfishLightningLarge;
            childLocator.FindChild("DashEffect").Find("DashSkull").Find("EffectRadius").gameObject.SetActive(false);//GetComponent<ParticleSystemRenderer>().material = Modules.Assets.matMoonbatteryCrippleRadius;

            childLocator.FindChild("SuperchargePassiveEffect").Find("Lightning").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matBlueLightningLong;
            childLocator.FindChild("SuperchargePassiveEffect").Find("Lightning").Find("LightningRound").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matBlueLightningLong;
            childLocator.FindChild("SuperchargePassiveEffect").Find("Lightning").Find("LightningRound").Find("LightningRound (1)").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matBlueLightningLong;

            childLocator.FindChild("MaxChargeEffect").Find("Lightning").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matJellyfishLightningLarge;

            childLocator.FindChild("SuperchargeEffect").Find("Lightning").GetComponent<ParticleSystemRenderer>().trailMaterial = Modules.Assets.matJellyfishLightningLarge;
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
                    if (item.bodyPrefab.name == "SS2UExecutionerBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animExecutionerEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }

        internal override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinMaster");
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Remove(masterySkinUnlockableDef.cachedName);

            grandMasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandMasterySkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.GrandMastery";
            grandMasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMETYPHOON_NAME";
            grandMasterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinGrandMaster");
            Unlockables.unlockableDefs.Add(grandMasterySkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Remove(grandMasterySkinUnlockableDef.cachedName);

            wastelanderSkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            wastelanderSkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.Wastelander";
            wastelanderSkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERWASTELANDER_NAME";
            wastelanderSkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerWastelander");
            Unlockables.unlockableDefs.Add(wastelanderSkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Remove(wastelanderSkinUnlockableDef.cachedName);
        }

        internal override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            RegisterStates();

            #region Primary
            //Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(EntityStates.Starstorm2States.Executioner.ExecutionerPistol)), "Weapon", "EXECUTIONER_PISTOL_NAME", "EXECUTIONER_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(EntityStates.SS2UStates.Executioner.ExecutionerBurstPistol)), "Weapon", "SS2UEXECUTIONER_PISTOL_NAME", "SS2UEXECUTIONER_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(EntityStates.SS2UStates.Executioner.ExecutionerSinglePistol)), "Weapon", "SS2UEXECUTIONER_PISTOL_SINGLE_NAME", "SS2UEXECUTIONER_PISTOL_SINGLE_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            //if (Modules.Config.ss_test.Value) Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(EntityStates.Starstorm2States.Executioner.ExecutionerTaser)), "Weapon", "EXECUTIONER_TASER_NAME", "EXECUTIONER_TASER_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            #endregion

            #region Secondary
            SkillDef ionGunSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UEXECUTIONER_IONGUN_NAME",
                skillNameToken = "SS2UEXECUTIONER_IONGUN_NAME",
                skillDescriptionToken = "SS2UEXECUTIONER_IONGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSecondary"),
                activationState = new SerializableEntityStateType(typeof(ExecutionerIonGun)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 5,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "KEYWORD_SHOCKING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, ionGunSkillDef);
            #endregion

            #region Utility
            SkillDef dashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UEXECUTIONER_DASH_NAME",
                skillNameToken = "SS2UEXECUTIONER_DASH_NAME",
                skillDescriptionToken = "SS2UEXECUTIONER_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerUtility"),
                activationState = new SerializableEntityStateType(typeof(EntityStates.SS2UStates.Executioner.ExecutionerDash)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_STUNNING",
                    "KEYWORD_SS2U_FEAR"
                }
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, dashSkillDef);
            #endregion

            #region Special
            SkillDef executionSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UEXECUTIONER_AXE_NAME",
                skillNameToken = "SS2UEXECUTIONER_AXE_NAME",
                skillDescriptionToken = "SS2UEXECUTIONER_AXE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSpecial"),
                activationState = new SerializableEntityStateType(typeof(ExecutionerAxe)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SLAYER" }
            });
            specialDef = executionSkillDef;
            Modules.Skills.AddSpecialSkills(bodyPrefab, executionSkillDef);
            #endregion


            SkillDef scepterSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UEXECUTIONER_AXE_SCEPTER_NAME",
                skillNameToken = "SS2UEXECUTIONER_AXE_SCEPTER_NAME",
                skillDescriptionToken = "SS2UEXECUTIONER_AXE_SCEPTER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSpecialScepter"),
                activationState = new SerializableEntityStateType(typeof(ExecutionerAxeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SLAYER", "KEYWORD_SS2U_FEAR" }
            });
            specialScepterDef = scepterSkillDef;
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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(specialScepterDef, bodyInfo.bodyName, specialDef);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(specialScepterDef, bodyInfo.bodyName, SkillSlot.Special, specialDef);
        }


        private void RegisterStates()
        {
            Modules.States.AddState(typeof(ExecutionerMain));
            Modules.States.AddState(typeof(ExecutionerBurstPistol));
            Modules.States.AddState(typeof(ExecutionerSinglePistol));
            Modules.States.AddState(typeof(ExecutionerIonGun));
            Modules.States.AddState(typeof(ExecutionerDash));
            Modules.States.AddState(typeof(ExecutionerAxe));
            Modules.States.AddState(typeof(ExecutionerAxeSlam));
            Modules.States.AddState(typeof(ExecutionerAxeScepter));
            Modules.States.AddState(typeof(ExecutionerAxeSlamScepter));
        }

        internal override void RegisterTokens()
        {
            LanguageAPI.Add("SS2UEXECUTIONER_NAME", "Executioner");
            LanguageAPI.Add("SS2UEXECUTIONER_SUBTITLE", "Dreaded Guillotine");
            LanguageAPI.Add("SS2UEXECUTIONER_DESCRIPTION", "The Executioner's goal is to spill as much blood as possible in the shortest amount of time. Bullets loaded, ion manipulators charged.<style=cSub>\r\n\r\n" +
                " < ! > Ion Burst deals massive damage but regenerates stocks slowly. Kill weak enemies to charge it up quickly, then unload it into stronger enemies.\r\n\r\n" +
                " < ! > Crowd Disperion interrupts enemy attacks, providing you with a window of opportunity.\r\n\r\n" +
                " < ! > Feared enemies are executed at low health.\r\n\r\n" +
                " < ! > Combine Execution with Crowd Dispersion to quickly kill groups of enemies.\r\n\r\n");
            LanguageAPI.Add("SS2UEXECUTIONER_OUTRO_FLAVOR", "..and so he left, bloodlust unfulfilled.");
            LanguageAPI.Add("SS2UEXECUTIONER_OUTRO_FAILURE", "..and so he vanished, escaping what he'd believed was inevitable.");
            LanguageAPI.Add("SS2UEXECUTIONER_LORE", "Death is inevitable. It comes for us all. Some may try to evade it, or run from it. But death arrives all the same. Death, however, is simply the cost of war. And death in glorious combat is one of the best deaths a man could ask for.\n\n" +
                "But, as always, some would seek to run from death. We call them by many names. Deserters, cowards, turncoats. Each and every one of them, traitors to the cause. These traitors must be punished for their crime. And that punishment: in an ironic twist, is the very thing they tried to avoid.\n\n" +
                "Death will come for them. Not as a robed spectre with a scythe. Not in a trench coat, holding a revolver. Not as a sickness, nor the wear of time. No. For them, death will come bearing lustrous battlegarb, with service pistol loaded, and ion manipulators fully charged.");

            LanguageAPI.Add("SS2UEXECUTIONER_DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add("SS2UEXECUTIONER_MASTERY_SKIN_NAME", "Vigilante");
            LanguageAPI.Add("SS2UEXECUTIONER_KNIGHT_SKIN_NAME", "Gladiator");
            LanguageAPI.Add("SS2UEXECUTIONER_WASTELANDER_SKIN_NAME", "Wastelander");

            //float dmg = ExecutionerPistol.damageCoefficient * 100f;
            float dmg = ExecutionerBurstPistol.damageCoefficient * 100f;
            int shotCount = ExecutionerBurstPistol.baseShotCount;

            LanguageAPI.Add("SS2UEXECUTIONER_PISTOL_NAME", "Service Pistol");
            LanguageAPI.Add("SS2UEXECUTIONER_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{shotCount}x{dmg}% damage</style>.");

            dmg = ExecutionerSinglePistol.damageCoefficient * 100f;
            LanguageAPI.Add("SS2UEXECUTIONER_PISTOL_SINGLE_NAME", "Standard-Issue Pistol");
            LanguageAPI.Add("SS2UEXECUTIONER_PISTOL_SINGLE_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{dmg}% damage</style>.");

            dmg = ExecutionerIonGun.damageCoefficient * 100f;
            int shots = ExecutionerIonGun.shotCount;
            LanguageAPI.Add("SS2UEXECUTIONER_IONGUN_NAME", "Ion Burst");
            LanguageAPI.Add("SS2UEXECUTIONER_IONGUN_DESCRIPTION", $"<style=cIsDamage>Shocking</style>. Unload a barrage of ionized bullets for <style=cIsDamage>{shots}x{dmg}% damage</style> each. Every slain enemy <style=cIsUtility>adds a stock</style>. Hold up to 5.");

            LanguageAPI.Add("KEYWORD_SS2U_FEAR", "<style=cKeywordName>Fear</style><style=cSub>Reduce movement speed by <style=cIsDamage>50%</style>. Feared enemies are <style=cIsHealth>instantly killed</style> if below <style=cIsHealth>15%</style> health.</style>");

            LanguageAPI.Add("SS2UEXECUTIONER_DASH_NAME", "Crowd Dispersion");
            LanguageAPI.Add("SS2UEXECUTIONER_DASH_DESCRIPTION", $"<style=cIsDamage>Stunning</style>. <style=cIsUtility>Dash forward</style> and <style=cIsDamage>Fear</style> nearby enemies.");

            dmg = ExecutionerAxeSlam.damageCoefficient * 100f;

            LanguageAPI.Add("SS2UEXECUTIONER_AXE_NAME", "Execution");
            LanguageAPI.Add("SS2UEXECUTIONER_AXE_DESCRIPTION", $"<style=cIsDamage>Slayer</style>. <style=cIsUtility>Launch into the air</style>, then slam downwards with your ion axe for <style=cIsDamage>{dmg}% damage</style>.");

            dmg = ExecutionerAxeSlam.damageCoefficient * 100f * 1.5f;
            LanguageAPI.Add("SS2UEXECUTIONER_AXE_SCEPTER_NAME", "Crowd Execution");
            LanguageAPI.Add("SS2UEXECUTIONER_AXE_SCEPTER_DESCRIPTION", $"<style=cIsDamage>Slayer</style>. <style=cIsUtility>Launch into the air</style>, then slam downwards with your ion axe and <style=cIsUtility>fear</style> nearby enemies while dealing <style=cIsDamage>{dmg}% damage</style>.");

            LanguageAPI.Add("SS2UEXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME", "Overkill");
            LanguageAPI.Add("SS2UEXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC", "Defeat an enemy by dealing 1000% of its max health in damage. <color=#c11>Host only</color>");
            LanguageAPI.Add("SS2UEXECUTIONER_UNLOCKUNLOCKABLE_UNLOCKABLE_NAME", "Overkill");

            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMEMONSOON_NAME", "Executioner: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMEMONSOON_DESCRIPTION", "As Executioner, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMETYPHOON_NAME", "Executioner: Grand Mastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMETYPHOON_DESCRIPTION", "As Executioner, beat the game or obliterate on Typhoon.");

            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERWASTELANDER_NAME", "Executioner: Crack the Vault");
            LanguageAPI.Add("ACHIEVEMENT_SS2UEXECUTIONERWASTELANDER_DESCRIPTION", "As Executioner, open the ancient gate on Abandoned Aqueduct.");
        }

        internal override void Hook()
        {
            On.RoR2.CharacterMaster.OnInventoryChanged += CharacterMaster_OnInventoryChanged;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            SetupFearExecute();
        }
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool wasAlive = self.alive;

            orig(self, damageInfo);

            if (self)
            {
                if (damageInfo.attacker && !damageInfo.rejected)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        if (attackerBody.bodyIndex == ExecutionerCore.bodyIndex)
                        {
                            Components.ExecutionerKillComponent killComponent = self.GetComponent<Components.ExecutionerKillComponent>();
                            if (!killComponent)
                            {
                                killComponent = self.AddComponent<Components.ExecutionerKillComponent>();
                            }
                            killComponent.AddTimer(attackerBody, 5f);
                        }
                    }
                }
            }
        }

        private void CharacterMaster_OnInventoryChanged(On.RoR2.CharacterMaster.orig_OnInventoryChanged orig, CharacterMaster self)
        {
            orig(self);

            if (self.hasBody)
            {
                if (self.GetBody().bodyIndex == ExecutionerCore.bodyIndex)
                {
                    var execComponent = self.GetBody().GetComponent<Components.ExecutionerController>();
                    if (execComponent)
                    {
                        execComponent.CheckInventory();
                    }
                }
            }
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
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("SS2UEXECUTIONER_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutioner"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerGun"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerAxe"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            string materialName = UnityEngine.Random.value < 0.01f ? "matExecutionerMastery" : "matExecutionerMasteryAlt";
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                Modules.Assets.CreateMaterial(materialName, 1f, Color.white),
                Modules.Assets.CreateMaterial(materialName, 1f, Color.white),
                Modules.Assets.CreateMaterial("matExecutionerAxe", 1f)
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef("SS2UEXECUTIONER_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinMaster"),
                masteryRendererInfos,
                mainRenderer,
                model,
                Modules.Config.ForceUnlockSkins.Value ? null : masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerMastery"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerGun"),
                    renderer = defaultRenderers[1].renderer
                }
            };

            skins.Add(masterySkin);
            #endregion

            #region GrandMasterySkin
            CharacterModel.RendererInfo[] grandMasteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                Modules.Assets.CreateMaterial("matExecutionerKnight"),
                Modules.Assets.CreateMaterial("matExecutionerKnight"),
                Modules.Assets.CreateMaterial("matExecutionerAxe", 1f)
            });

            SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef("SS2UEXECUTIONER_KNIGHT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinGrandMaster"),
                grandMasteryRendererInfos,
                mainRenderer,
                model,
                Modules.Config.ForceUnlockSkins.Value ? null : grandMasterySkinUnlockableDef);

            grandMasterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerKnight"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshExecutionerGun"),
                    renderer = defaultRenderers[1].renderer
                }
            };

            skins.Add(grandMasterySkin);
            #endregion

            #region WastelanderSkin
            CharacterModel.RendererInfo[] wastelanderRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                Modules.Assets.CreateMaterial("matWastelander", 1f, Color.white),
                Modules.Assets.CreateMaterial("matWastelander", 1f, Color.white),
                Modules.Assets.CreateMaterial("matWastelanderAxe", 1, Color.red)
            });

            SkinDef wastelanderSkin = Modules.Skins.CreateSkinDef("SS2UEXECUTIONER_WASTELANDER_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerWastelander"),
                wastelanderRendererInfos,
                mainRenderer,
                model,
                Modules.Config.ForceUnlockSkins.Value ? null : wastelanderSkinUnlockableDef);

            wastelanderSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshWastelander"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshWastelanderGun"),
                    renderer = defaultRenderers[1].renderer
                }
            };

            skins.Add(wastelanderSkin);
            #endregion

            skinController.skins = skins.ToArray();
        }

        internal override void SetItemDisplays()
        {
            instance.itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            ExecutionerItemDisplays.RegisterDisplays();
            
            itemDisplayRuleSet.keyAssetRuleGroups = instance.itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[2].defaultMaterial = materials[2];

            return newRendererInfos;
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            // create the orb for ion gun stock
            if (!damageReport.victimBody) return;
            CharacterBody victimBody = damageReport.victimBody;

            bool victimFeared = victimBody.HasBuff(BuffCore.fearDebuff);
            if (victimFeared)
            {
                EffectManager.SpawnEffect(ExecutionerCore.fearKillEffect, new EffectData
                {
                    origin = damageReport.damageInfo.position
                }, true);
            }

            Components.ExecutionerKillComponent killComponent = victimBody.GetComponent<Components.ExecutionerKillComponent>();
            if (killComponent)
            {
                killComponent.TriggerEffects(damageReport.attackerBody, damageReport.damageInfo.damageType);
            }
            else
            {
                if (damageReport.attackerBody && damageReport.attackerBody.bodyIndex == ExecutionerCore.bodyIndex)
                {
                    int orbCount = GetIonCountFromBody(victimBody);
                    //if (victimFeared) orbCount *= 2;

                    for (int i = 0; i < orbCount; i++)
                    {
                        Modules.Orbs.ExecutionerIonOrb ionOrb = new Modules.Orbs.ExecutionerIonOrb();
                        ionOrb.origin = victimBody.corePosition;
                        ionOrb.target = Util.FindBodyMainHurtBox(damageReport.attackerBody);
                        OrbManager.instance.AddOrb(ionOrb);
                    }
                }
            }
        }

        internal static int GetIonCountFromBody(CharacterBody body)
        {
            if (body.isChampion) return 5;
            return 1;
        }

        private void SetupFearExecute()
        {
            On.RoR2.HealthComponent.GetHealthBarValues += FearExecuteHealthbar;

            //Prone to breaking when the game updates
            IL.RoR2.HealthComponent.TakeDamage += (il) =>
            {
                bool error = true;
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                    x => x.MatchStloc(53)   //num17 = float.NegativeInfinity, stloc53 = Execute Fraction, first instance it is used
                    ))
                {
                    if (c.TryGotoNext(MoveType.After,
                    x => x.MatchLdloc(8)   //flag 5, this is checked before final Execute damage calculations.
                    ))
                    {
                        c.Emit(OpCodes.Ldarg_0);//self
                        c.Emit(OpCodes.Ldloc, 53);//execute fraction
                        c.EmitDelegate <Func<HealthComponent, float, float>>((self, executeFraction) =>
                        {
                            if (self.body.HasBuff(BuffCore.fearDebuff))
                            {
                                if (executeFraction < 0f) executeFraction = 0f;
                                executeFraction += 0.15f;
                            }
                            return executeFraction;
                        });
                        c.Emit(OpCodes.Stloc, 53);

                        error = false;
                    }
                }

                if (error)
                {
                    UnityEngine.Debug.LogError("Starstorm 2 Unofficial: Fear Execute IL Hook failed.");
                }

            };
            IL.RoR2.CharacterBody.UpdateAllTemporaryVisualEffects += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                      x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Cripple")
                     ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || self.HasBuff(BuffCore.fearDebuff);
                    });
                }
                else
                {
                    UnityEngine.Debug.LogError("Starstorm 2 Unofficial: Fear VFX IL Hook failed.");
                }    
            };
        }

        private HealthComponent.HealthBarValues FearExecuteHealthbar(On.RoR2.HealthComponent.orig_GetHealthBarValues orig, HealthComponent self)
        {
            var hbv = orig(self);
            if (!self.body.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToExecutes) && self.body.HasBuff(BuffCore.fearDebuff))
            {
                hbv.cullFraction += 0.15f;//(self.body && self.body.isChampion) ? 0.15f : 0.3f; //might stack too crazy if it's 30% like Freeze
            }
            return hbv;
        }

        internal override void InitializeDoppelganger()
        {
            GameObject doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), bodyName + "MonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Execute", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 15f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtMoveTarget,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "DashCloser", SkillSlot.Utility, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 40f,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtMoveTarget,
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
                0f, 30f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
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
    }
}
