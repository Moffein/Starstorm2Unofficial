using BepInEx.Configuration;
using EntityStates;
using EntityStates.AI.Walker;
using EntityStates.Executioner;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using RoR2.Skills;
using Starstorm2.Cores;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2.Modules.Survivors
{
    internal class Executioner : SurvivorBase
    {
        public static BodyIndex bodyIndex;

        private static GameObject fearKillEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2KillEffect.prefab").WaitForCompletion();

        internal override string bodyName { get; set; } = "Executioner";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal override float sortPosition { get; set; } = 4.001f;

        internal override ConfigEntry<bool> characterEnabled { get; set; }

        internal override StarstormBodyInfo bodyInfo { get; set; } = new StarstormBodyInfo
        {
            armor = 0f,
            bodyName = "ExecutionerBody",
            bodyNameToken = "EXECUTIONER_NAME",
            bodyColor = new Color(0.69f, 0.44f, 0.49f),
            characterPortrait = Modules.Assets.LoadCharacterIcon("Executioner"),
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            damage = 12f,
            healthGrowth = 33f,
            healthRegen = 1f,
            jumpCount = 1,
            maxHealth = 110f,
            subtitleNameToken = "EXECUTIONER_SUBTITLE",
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

        internal override Type characterMainState { get; set; } = typeof(Cores.States.Executioner.ExecutionerMain);

        // item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; }// = Modules.Unlockables.AddUnlockable<Cores.Unlockables.Achievements.ExecutionerUnlockAchievement>(true);
        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;
        private static UnlockableDef wastelanderSkinUnlockableDef;

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("ExecutionerBody");
        }

        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();

            RoR2.RoR2Application.onLoad += SetBodyIndex;

            if (characterEnabled.Value)
            {
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
            }
        }

        internal override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinMaster");
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);

            grandMasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandMasterySkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.GrandMastery";
            grandMasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERCLEARGAMETYPHOON_NAME";
            grandMasterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinGrandMaster");
            Unlockables.unlockableDefs.Add(grandMasterySkinUnlockableDef);

            wastelanderSkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            wastelanderSkinUnlockableDef.cachedName = "Skins.SS2UExecutioner.Wastelander";
            wastelanderSkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UEXECUTIONERWASTELANDER_NAME";
            wastelanderSkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerWastelander");
            Unlockables.unlockableDefs.Add(wastelanderSkinUnlockableDef);
        }

        internal override void InitializeSkills()
        {
            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            #region Primary
            //Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(EntityStates.Executioner.ExecutionerPistol)), "Weapon", "EXECUTIONER_PISTOL_NAME", "EXECUTIONER_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(EntityStates.Executioner.ExecutionerBurstPistol)), "Weapon", "EXECUTIONER_PISTOL_NAME", "EXECUTIONER_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            //if (Modules.Config.ss_test.Value) Modules.Skills.AddPrimarySkill(bodyPrefab, Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(EntityStates.Executioner.ExecutionerTaser)), "Weapon", "EXECUTIONER_TASER_NAME", "EXECUTIONER_TASER_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerPrimary"), false));
            #endregion

            #region Secondary
            SkillDef ionGunSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "EXECUTIONER_IONGUN_NAME",
                skillNameToken = "EXECUTIONER_IONGUN_NAME",
                skillDescriptionToken = "EXECUTIONER_IONGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSecondary"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ExecutionerIonGun)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 10,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0,
                keywordTokens = new string[] { "KEYWORD_SHOCKING" }
            });

            Modules.Skills.AddSecondarySkills(bodyPrefab, ionGunSkillDef);
            #endregion

            #region Utility
            SkillDef dashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "EXECUTIONER_DASH_NAME",
                skillNameToken = "EXECUTIONER_DASH_NAME",
                skillDescriptionToken = "EXECUTIONER_DASH_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerUtility"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Executioner.ExecutionerDash)),
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
                    "KEYWORD_FEAR"
                }
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, dashSkillDef);
            #endregion

            #region Special
            SkillDef executionSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "EXECUTIONER_AXE_NAME",
                skillNameToken = "EXECUTIONER_AXE_NAME",
                skillDescriptionToken = "EXECUTIONER_AXE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSpecial"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(ExecutionerAxe)),
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

            Modules.Skills.AddSpecialSkills(bodyPrefab, executionSkillDef);
            #endregion
        }

        internal override void RegisterTokens()
        {
            LanguageAPI.Add("EXECUTIONER_NAME", "Executioner");
            LanguageAPI.Add("EXECUTIONER_SUBTITLE", "Dreaded Guillotine");
            LanguageAPI.Add("EXECUTIONER_DESCRIPTION", "The Executioner is a high-risk, high-reward survivor that's all about racking up an endless kill count.\n\n" +
                "<color=#CCD3E0> < ! > Use Service Pistol to score some kills, with each kill building up an even stronger Ion Burst.\n\n" +
                " < ! > Some enemies provide multiple charges for Ion burst - and bosses fully charge it!\n\n" +
                " < ! > Execution is a great crowd control AND single target tool. Don't forget that its damage depends on how many targets it hits!\n\n" +
                " < ! > If you find yourself getting swarmed, Crowd Dispersion can get enemies off your back fast.\n\n");
            LanguageAPI.Add("EXECUTIONER_OUTRO_FLAVOR", "..and so he left, an empty shell of armor drenched in blood.");
            LanguageAPI.Add("EXECUTIONER_OUTRO_FAILURE", "..and so he vanished, escaping what he'd believed was inevitable.");
            LanguageAPI.Add("EXECUTIONER_LORE", "Death is inevitable. It comes for us all. Some may try to evade it, or run from it. But death arrives all the same. Death, however, is simply the cost of war. And death in glorious combat is one of the best deaths a man could ask for.\n\n" +
                "But, as always, some would seek to run from death. We call them by many names. Deserters, cowards, turncoats. Each and every one of them, traitors to the cause. These traitors must be punished for their crime. And that punishment: in an ironic twist, is the very thing they tried to avoid.\n\n" +
                "Death will come for them. Not as a robed spectre with a scythe. Not in a trench coat, holding a revolver. Not as a sickness, nor the wear of time. No. For them, death will come bearing lustrous battlegarb, with service pistol loaded, and ion manipulators fully charged.");

            LanguageAPI.Add("EXECUTIONER_DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add("EXECUTIONER_MASTERY_SKIN_NAME", "Vigilante");
            LanguageAPI.Add("EXECUTIONER_KNIGHT_SKIN_NAME", "Gladiator");
            LanguageAPI.Add("EXECUTIONER_WASTELANDER_SKIN_NAME", "Wastelander");

            //float dmg = ExecutionerPistol.damageCoefficient * 100f;
            float dmg = ExecutionerBurstPistol.damageCoefficient * 100f;
            int shotCount = ExecutionerBurstPistol.baseShotCount;

            LanguageAPI.Add("EXECUTIONER_PISTOL_NAME", "Service Pistol");
            LanguageAPI.Add("EXECUTIONER_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{shotCount}x{dmg}% damage</style>.");
            //LanguageAPI.Add("EXECUTIONER_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{dmg}% damage</style>.");

            dmg = ExecutionerTaser.damageCoefficient * 100f;

            LanguageAPI.Add("EXECUTIONER_TASER_NAME", "Deadly Voltage");
            LanguageAPI.Add("EXECUTIONER_TASER_DESCRIPTION", $"Release an arc of lightning forward for <style=cIsDamage>{dmg}% damage</style>.");

            dmg = ExecutionerIonGun.damageCoefficient * 100f;

            LanguageAPI.Add("EXECUTIONER_IONGUN_NAME", "Ion Burst");
            LanguageAPI.Add("EXECUTIONER_IONGUN_DESCRIPTION", $"<style=cIsDamage>Shocking</style>. Unload a barrage of ionized bullets for <style=cIsDamage>{dmg}% damage</style> each. Every slain enemy <style=cIsUtility>adds a bullet</style>.");

            LanguageAPI.Add("KEYWORD_FEAR", "<style=cKeywordName>Fear</style><style=cSub>Reduce movement speed by <style=cIsDamage>50%</style>. Feared enemies are <style=cIsHealth>instantly killed</style> if below <style=cIsHealth>15%</style> health.</style>");

            LanguageAPI.Add("EXECUTIONER_DASH_NAME", "Crowd Dispersion");
            LanguageAPI.Add("EXECUTIONER_DASH_DESCRIPTION", $"<style=cIsUtility>Dash forward</style> and <style=cIsDamage>Fear</style> nearby enemies.");

            dmg = ExecutionerAxeSlam.baseDamageCoefficient * 100f;

            LanguageAPI.Add("EXECUTIONER_AXE_NAME", "Execution");
            LanguageAPI.Add("EXECUTIONER_AXE_DESCRIPTION", $"<style=cIsDamage>Slayer</style>. <style=cIsUtility>Launch into the air</style>, then slam downwards with your ion axe for <style=cIsDamage>{dmg}% damage</style>.");

            LanguageAPI.Add("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME", "Overkill");
            LanguageAPI.Add("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC", "Defeat an enemy by dealing 1000% of its max health in damage. <color=#c11>Host only</color>");
            LanguageAPI.Add("EXECUTIONER_UNLOCKUNLOCKABLE_UNLOCKABLE_NAME", "Overkill");

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
                        if (attackerBody.bodyIndex == Executioner.bodyIndex)
                        {
                            //self.body.AddTimedBuff(Starstorm2.Cores.BuffCoreexeAssistBuff, 5f);
                            Components.ExecutionerKillComponent killComponent = self.GetComponent<Components.ExecutionerKillComponent>();
                            if (!killComponent)
                            {
                                self.AddComponent<Components.ExecutionerKillComponent>().AddTimer(attackerBody, 5f);
                            }
                            else killComponent.AddTimer(attackerBody, 5f);
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
                if (self.GetBody().bodyIndex == Executioner.bodyIndex)
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
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("EXECUTIONER_DEFAULT_SKIN_NAME",
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

            SkinDef masterySkin = Modules.Skins.CreateSkinDef("EXECUTIONER_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinMaster"),
                masteryRendererInfos,
                mainRenderer,
                model,
                masterySkinUnlockableDef);

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

            SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef("EXECUTIONER_KNIGHT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinGrandMaster"),
                grandMasteryRendererInfos,
                mainRenderer,
                model,
                grandMasterySkinUnlockableDef);

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

            SkinDef wastelanderSkin = Modules.Skins.CreateSkinDef("EXECUTIONER_WASTELANDER_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerWastelander"),
                wastelanderRendererInfos,
                mainRenderer,
                model,
                wastelanderSkinUnlockableDef);

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

            #region Display Rules
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0, 0.0027f, 0.0012f), new Vector3(330, 0, 0), new Vector3(0.0036f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "ShoulderL", new Vector3(0.00f, 0.00f, -0.00f), new Vector3(13.00001f, 110, 210f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Gun", new Vector3(-0.0015f, 0.0025f, 0.00f), new Vector3(0, 0, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Gun", new Vector3(-0.001f, 0.0035f, 0f), new Vector3(295, 90, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(-0.003f, 0.00065f, 0f), new Vector3(2.1344340f, 9.99999f, 20f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "CalfR", new Vector3(0, 0.0037f, -0.001f), new Vector3(75, 0, 0), new Vector3(0.001f, 0.0012f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.0007f, 0.0035f, -0.0027f), new Vector3(0f, 180f, 56f), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0f, 0.0025f, 0.0008f), new Vector3(330, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "ShoulderR", new Vector3(-0.0008f, 0f, 0.0005f), new Vector3(0f, 340f, 90f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(0, 0.0035f, 0.0005f), new Vector3(310, 0, 0), new Vector3(0.006f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Muzzle", new Vector3(-0f, -0.0005f, -0.0f), new Vector3(0, 0, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(0, 0.003f, -0.003f), new Vector3(270, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ElbowR", new Vector3(0.00f, 0.0007f, 0.0003f), new Vector3(90, 0, 2), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Gun", new Vector3(0, 0.00220f, 0), new Vector3(0f, 0f, 300f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "ElbowL", new Vector3(0f, 0f, -0.0005f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(0, 0.0035f, -0.0003f), new Vector3(345f, 0f, 0f), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(0.001f, -0.00f, 0), new Vector3(0f, 70f, 270f), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "ElbowR", new Vector3(-0.0008f, 0.0005f, 0), new Vector3(0, 330, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Muzzle", new Vector3(0.0f, 0, -0.00028f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Muzzle", new Vector3(0.0f, 0, -0.0003f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(0.00f, -0.0009f, -0.001f), new Vector3(0, 0, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Axe", new Vector3(0, 0.006f, 0), new Vector3(270, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "ThighR", new Vector3(0.00f, 0.003f, -0.001f), new Vector3(90, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "ShoulderR", new Vector3(0, 0.001f, 0.00f), new Vector3(0, 180, 180), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Pelvis", new Vector3(-0.002f, 0.00f, 0.001f), new Vector3(310, 30, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "Axe", new Vector3(0.00f, 0.007f, -0.0002f), new Vector3(0, 270, 270), new Vector3(0.01f, 0.015f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "ThighL", new Vector3(0.00f, 0.0023f, -0.0015f), new Vector3(80, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(0.001f, 0.0023f, -0.0003f), new Vector3(0, 90, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            //https://discord.com/channels/753709254598328400/755273415719387146/793188879557328916
            //https://discord.com/channels/753709254598328400/757459787117101096/785685039177793547
            //https://discord.com/channels/753709254598328400/757459787117101096/785641674977706034
            //this could've been done once but now must be done twice
            //:damnation:
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Chest", new Vector3(0.0f, 0.002f, 0.0023f), new Vector3(0, 20, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "Pelvis", new Vector3(0.0025f, 0.0006f, 0), new Vector3(70, 250, 5), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(0, 0.0023f, -0.0006f), new Vector3(45, 80, 0), new Vector3(0.006f, 0.008f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(-0.0013f, 0.0035f, -0.003f), new Vector3(0, 0, 4), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "Pelvis", new Vector3(-0.0023f, 0.001f, 0.001f), new Vector3(80, 20, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "ThighR", new Vector3(-0.0005f, 0.002f, -0.0015f), new Vector3(90f, 230f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Chest", new Vector3(-0.002f, 0.003f, -0.0023f), new Vector3(270, 0, 10), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Pelvis", new Vector3(0.0026f, -0.001f, -0.001f), new Vector3(0, 0, 260), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(0.00f, 0.002f, 0.0018f), new Vector3(0, 0, 0), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Pelvis", new Vector3(-0.003f, 0.0007f, -0.00f), new Vector3(0, 20, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Pelvis", new Vector3(0.0028f, 0.001f, -0.001f), new Vector3(85, 180, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Gun", new Vector3(0.00f, 0, 0.00008f), new Vector3(15, 270, 0), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Pelvis", new Vector3(-0.0026f, 0.002f, -0.0014f), new Vector3(345, 0, 11), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.0015f, 0.003f, 0.002f), new Vector3(20, 90, 80), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "ShoulderR", new Vector3(0.0f, 0.0015f, -0.001f), new Vector3(270, 20, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Pelvis", new Vector3(0.0016f, 0.008f, 0.006f), new Vector3(0, 90, 180), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.00f, 0.003f, -0.003f), new Vector3(0, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0, 0.002f, 0), new Vector3(345, 0, 0), new Vector3(0.003f, 0.004f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "Head", new Vector3(0.0012f, 0.002f, -0.001f), new Vector3(90, 100, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(0, 0f, 0.002f), new Vector3(0f, 90f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.001f, 0.002f, -0.0005f), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            //at around this point i scrolled down to check how many more items i had to do then lay my head on my desk at the realization
            //remember when i mentioned multiple times to wait for classic model
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.0006f, 0.002f, 0.002f), new Vector3(0, 0, 20), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "ShoulderL", new Vector3(0.0004f, 0.0015f, 0), new Vector3(0, 90, 180), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.001f, 0.0023f, 0.0017f), new Vector3(90, 0, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Chest", new Vector3(0.002f, 0.0f, 0.0006f), new Vector3(0, 70, 90), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "CalfR", new Vector3(0f, 0.002f, 0.0021f), new Vector3(0, 270, 280), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "ShoulderL", new Vector3(-0.0005f, 0, 0), new Vector3(270, 0, 0), new Vector3(0.008f, 0.006f, 0.003f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("LunarPrimaryReplacement", "DisplayBirdEye", "Head", new Vector3(0, 0.004f, 0.001f), new Vector3(90, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Head", new Vector3(-0.0005f, 0.0004f, -0.00f), new Vector3(310, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Pelvis", new Vector3(0.003f, 0.0005f, 0.001f), new Vector3(0, 0, 270), new Vector3(0.01f, 0.01f, 0.0f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(0.0018f, 0.0043f, -0.002f), new Vector3(290, 180, 180), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Chest", new Vector3(0, 0.002f, 0.002f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "HandR", new Vector3(-0.001f, 0.0005f, -0.00f), new Vector3(300, 0, 270), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Pelvis", new Vector3(-0.0025f, 0.0005f, 0.002f), new Vector3(20, 315, 180), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "CalfL", new Vector3(0, 0.002f, -0.001f), new Vector3(0, 90, 45), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Pelvis", new Vector3(0.0032f, 0.0025f, -0.001f), new Vector3(0, 0, 160), new Vector3(0.006f, 0.006f, 0.006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "ElbowR", new Vector3(-0.00f, 0.001f, 0.001f), new Vector3(90, 180, 0), new Vector3(0.003f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "ShoulderR", new Vector3(-0.00f, 0.001f, 0.00f), new Vector3(280, 180, 0), new Vector3(0.002f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Chest", new Vector3(0, 0.002f, -0.002f), new Vector3(270, 0, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Pelvis", new Vector3(-0.0026f, 0, 0), new Vector3(90, 90, 0), new Vector3(0.012f, 0.012f, 0.012f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullCrown", "Head", new Vector3(0, 0.0024f, -0.0003f), new Vector3(320, 0, 0), new Vector3(0.005f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Pelvis", new Vector3(0, 0, 0.0023f), new Vector3(0, 0, 180), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Chest", new Vector3(0f, 0.0026f, 0.0026f), new Vector3(330f, 0f, 0f), new Vector3(0.0016f, 0.0016f, 0.0016f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, "DisplayToothMeshLarge", "Chest", new Vector3(-0.002f, 0.0055f, -0.001f), new Vector3(340f, 0f, 0f), new Vector3(0.07f, 0.07f, 0.07f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Muzzle", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Muzzle", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "Pelvis", new Vector3(0f, 0f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "ShoulderL", new Vector3(0.001f, -0.00f, -0.001f), new Vector3(0, 35, 250), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Chest", new Vector3(0f, 0.0035f, -0.0025f), new Vector3(15f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Gun", new Vector3(-0.003f, 0.003f, 0f), new Vector3(330f, 270f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "ThighL", new Vector3(0.0006f, 0.002f, -0.001f), new Vector3(0f, 45f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.0005f, 0.0024f, 0.0025f), new Vector3(0f, 0f, 45f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "ThighR", new Vector3(-0.00085f, 0.0033f, 0f), new Vector3(30f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "Gun", new Vector3(-0.002f, 0.0015f, 0.000f), new Vector3(270f, 270f, 270f), new Vector3(0.0004f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0.002f, 0.003f, 0f), new Vector3(70f, 180f, 190f), new Vector3(0.01f, 0.01f, 0.01f)));
            //set up thing to make this work (needs mirror that mirrors z - rotation)
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "ThighR", new Vector3(-0.000f, 0.0003f, -0.00f), new Vector3(0f, 0f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(-0.001f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0f, 0.0042f, 0.0005f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Gun", new Vector3(0f, 0.0032f, 0.00f), new Vector3(0f, 90f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0f, 0.0012f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Pelvis", new Vector3(-0.0013f, -0.00f, 0.002f), new Vector3(0f, 345f, 195f), new Vector3(0.006f, 0.006f, 0.006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, ItemDisplayCore.capacitorPrefab, "Chest", new Vector3(-0.001f, 0f, -0.004f), new Vector3(0f, 0f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            //no clue how to do this rob........ :(meru
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Pelvis", new Vector3(0.0015f, 0.0002f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Pelvis", new Vector3(-0.0016f, 0.0018f, -0.0016f), new Vector3(0f, 30f, 180f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Head", new Vector3(0f, 0.0022f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.019f, 0.018f, 0.012f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0f, 0.0034f, -0.003f), new Vector3(0f, 90f, 15f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Pelvis", new Vector3(0.002f, 0.0008f, -0.0014f), new Vector3(85f, 180f, 0f), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(0f, 0.0015f, -0.0025f), new Vector3(0f, 180f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Pelvis", new Vector3(0.0015f, -0.001f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Pelvis", new Vector3(-0.0024f, 0.0005f, 0.001f), new Vector3(0f, 30f, 170f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Chest", new Vector3(0.0015f, 0.0035f, 0f), new Vector3(280f, 0f, 15f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Pelvis", new Vector3(0.000f, 0.00f, -0.002f), new Vector3(0f, 0f, 180f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", new Vector3(0f, 0.004f, -0.0028f), new Vector3(30f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(0f, 0.0018f, 0.002f), new Vector3(15f, 0f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //equipmentRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Lightning", "???", "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //I have no clue what the model name is for the Capacitator, and both the Miner / Enforcer gits do some weird fucky shit.

            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(-0.00f, 0.013f, -0.008f), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", new Vector3(0.007f, 0.02f, -0.0f), new Vector3(0f, 0, 0), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", new Vector3(-0.002f, 0.02f, -0.01f), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", new Vector3(0.003f, 0.022f, -0.003f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", new Vector3(0.003f, 0.022f, -0.003f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));

            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(0, 0.002f, -0.002f), new Vector3(0, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(0.003f, 0.0058f, 0), new Vector3(20, 90, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Chest", new Vector3(0.0017f, 0.003f, 0), new Vector3(0, 0, 330), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(0, 0.002f, -0.0033f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", new Vector3(0, 0.015f, -0.015f), new Vector3(0, 90, 0), new Vector3(0.2f, 0.2f, 0.2f)));

            instance.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfL",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.0028f),
                            limbMask = LimbFlags.None
                        },
                        //For some reason, only appears on one leg?
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfR",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.0028f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            instance.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBirdEye"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.004f, 0.001f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Modules.Assets.needlerPrefab,
                            childName = "Gun",
                            localPos = new Vector3(-0.001f, 0.001f, 0f),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.02f, 0.02f, 0.02f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion

            itemDisplayRuleSet.keyAssetRuleGroups = instance.itemDisplayRules.ToArray();
            itemDisplayRuleSet.GenerateRuntimeValues();
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
            CharacterBody victimBody = damageReport.victimBody;
            if (victimBody)
            {
                bool victimFeared = victimBody.HasBuff(BuffCore.fearDebuff);
                if (victimFeared)
                {
                    EffectManager.SpawnEffect(Executioner.fearKillEffect, new EffectData
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
                    if (damageReport.attackerBody && damageReport.attackerBody.bodyIndex == Executioner.bodyIndex)
                    {
                        int orbCount = GetIonCountFromBody(victimBody);
                        if (victimFeared) orbCount *= 2;
                            //if (damageReport.damageInfo.damageType.HasFlag(DamageType.BypassOneShotProtection)) orbCount *= 2;    //Was used to make exe axe give double charges on kill. Unnecessary.

                            for (int i = 0; i < orbCount; i++)
                        {
                            Modules.Orbs.ExecutionerIonOrb ionOrb = new Modules.Orbs.ExecutionerIonOrb();
                            ionOrb.origin = victimBody.corePosition;
                            ionOrb.target = Util.FindBodyMainHurtBox(damageReport.attackerBody);
                            OrbManager.instance.AddOrb(ionOrb);
                        }

                        //These aren't used anymore because the hardcoded Ion Burst value list was ridiculous.
                        /*if (orbCount >= 50 && orbCount < 110)
                        {
                            Modules.Orbs.ExecutionerIonTempSuperOrb tempSuperIonOrb = new Modules.Orbs.ExecutionerIonTempSuperOrb();
                            tempSuperIonOrb.origin = victimBody.corePosition;
                            tempSuperIonOrb.target = Util.FindBodyMainHurtBox(damageReport.attackerBody);
                            OrbManager.instance.AddOrb(tempSuperIonOrb);
                        }

                        if (orbCount >= 110)
                        {
                            Modules.Orbs.ExecutionerIonSuperOrb superIonOrb = new Modules.Orbs.ExecutionerIonSuperOrb();
                            superIonOrb.origin = victimBody.corePosition;
                            superIonOrb.target = Util.FindBodyMainHurtBox(damageReport.attackerBody);
                            OrbManager.instance.AddOrb(superIonOrb);
                        }*/
                    }
                }
            }
        }

        internal static int GetIonCountFromBody(CharacterBody body)
        {
            if (body.isChampion) return 10;
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
    }
}

public class IonGunChargeComponent : NetworkBehaviour
{
    public int storedCharges = 0;
    public SkillLocator skillLocator;

    [ClientRpc]
    public void RpcAddIonCharge()
    {
        if (this.hasAuthority)
        {
            if (!skillLocator) skillLocator = this.gameObject.GetComponent<SkillLocator>();
            GenericSkill ionGunSkill = skillLocator?.secondary;
            if (ionGunSkill && ionGunSkill.stock < ionGunSkill.maxStock)
                ionGunSkill.AddOneStock();
        }
    }
}
