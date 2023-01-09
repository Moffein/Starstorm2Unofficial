using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using R2API;
using Starstorm2.Cores;
using System.Runtime.CompilerServices;
using EntityStates;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using Starstorm2.Survivors.Pyro.Components;
using EntityStates.SS2UStates.Pyro;
using RoR2.UI;
using Starstorm2.Survivors.Pyro.Components.Crosshair;
using RoR2.Projectile;
using Starstorm2.Survivors.Pyro.Components.Projectile;

namespace Starstorm2.Survivors.Pyro
{
    public class PyroCore
    {
        public static GameObject bodyPrefab;
        public static BodyIndex bodyIndex;

        public static class Skills
        {
            public static SkillDef primaryScorch;
            public static SkillDef secondaryAirblast;
            public static SkillDef utilityHeatJetpack;
            public static SkillDef specialFlare;
            public static SkillDef specialFlareScepter;
        }

        public PyroCore() => Setup();

        private void Setup()
        {
            bodyPrefab = CreatePyroPrefab();
            R2API.ItemAPI.DoNotAutoIDRSFor(bodyPrefab);

            LanguageAPI.Add("SS2UPYRO_NAME", "Pyro");
            LanguageAPI.Add("SS2UPYRO_SUBTITLE", "Pest Control");
            LanguageAPI.Add("SS2UPYRO_OUTRO_FLAVOR", "..and so he left, in a blaze of glory.");
            LanguageAPI.Add("SS2UPYRO_OUTRO_FAILURE", "..and so he vanished, with nothing but ashes left behind.");
            LanguageAPI.Add("SS2UPYRO_DESCRIPTION", "");

            RegisterStates();
            SetUpSkills();
            PyroSkins.RegisterSkins();
            //CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(bodyPrefab, PrefabCore.CreateDisplayPrefab("PyroDisplay", bodyPrefab), Color.red, "SS2UPYRO", 40.3f);
            RoR2.RoR2Application.onLoad += SetBodyIndex;

            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();

            NetworkSoundEventDef sound = Modules.Assets.CreateNetworkSoundEventDef("Play_mage_m1_impact");
            BuildFlaregunProjectile(sound);
            BuildFlaregunScepterProjectile(sound);
            Airblast.reflectSound = Modules.Assets.CreateNetworkSoundEventDef("Play_captain_drone_zap");
        }

        private void BuildFlaregunProjectile(NetworkSoundEventDef initialImpactSound)
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion().InstantiateClone("SS2UPyroFlare", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;

            ProjectileStickOnImpact pst = projectilePrefab.AddComponent<ProjectileStickOnImpact>();
            pst.ignoreCharacters = false;
            pst.ignoreWorld = false;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
            ps.desiredForwardSpeed = 100f;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            Rigidbody rb = pc.GetComponent<Rigidbody>();
            rb.useGravity = true;

            AntiGravityForce agf = projectilePrefab.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.7f;
            agf.rb = rb;

            FlareProjectileController fpc = projectilePrefab.AddComponent<FlareProjectileController>();
            fpc.explosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();
            fpc.initialImpactSound = initialImpactSound;

            Flaregun.projectilePrefab = projectilePrefab;
        }

        private void BuildFlaregunScepterProjectile(NetworkSoundEventDef initialImpactSound)
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion().InstantiateClone("SS2UPyroFlareScepter", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<ProjectileSingleTargetImpact>());

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;

            ProjectileStickOnImpact pst = projectilePrefab.AddComponent<ProjectileStickOnImpact>();
            pst.ignoreCharacters = false;
            pst.ignoreWorld = false;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.lifetime = 10f;
            ps.desiredForwardSpeed = 100f;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.allowPrediction = false;

            Rigidbody rb = pc.GetComponent<Rigidbody>();
            rb.useGravity = true;

            AntiGravityForce agf = projectilePrefab.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.7f;
            agf.rb = rb;

            FlareProjectileController fpc = projectilePrefab.AddComponent<FlareProjectileController>();
            fpc.explosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();
            fpc.initialImpactSound = initialImpactSound;
            fpc.explosionRadius = 12f;

            FlaregunScepter.scepterProjectilePrefab = projectilePrefab;
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(FireFlamethrower));
            Modules.States.AddState(typeof(Airblast));
            Modules.States.AddState(typeof(HeatJetpack));
            Modules.States.AddState(typeof(Flaregun));
            Modules.States.AddState(typeof(FlaregunScepter));
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in bodyPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillDef squawkDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();
            SkillFamily.Variant squawkVariant =  Utils.RegisterSkillVariant(squawkDef);
            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);
        }
        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_PRIMARY_NAME", "Scorch");
            LanguageAPI.Add("SS2UPYRO_PRIMARY_DESCRIPTION", "<color=#D78326>Build heat</color> and burn enemies for <style=cIsDamage>420% damage per second</style>. <style=cIsDamage>Ignites</style> at <color=#D78326>high heat</color>.");

            SkillDef primaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(FireFlamethrower));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "SS2UPYRO_PRIMARY_NAME";
            primaryDef1.skillNameToken = "SS2UPYRO_PRIMARY_NAME";
            primaryDef1.skillDescriptionToken = "SS2UPYRO_PRIMARY_DESCRIPTION";
            primaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill1");
            primaryDef1.baseMaxStock = 1;
            primaryDef1.baseRechargeInterval = 0f;
            primaryDef1.beginSkillCooldownOnSkillEnd = false;
            primaryDef1.canceledFromSprinting = false;
            primaryDef1.fullRestockOnAssign = true;
            primaryDef1.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryDef1.isCombatSkill = true;
            primaryDef1.mustKeyPress = false;
            primaryDef1.cancelSprintingOnActivation = true;
            primaryDef1.rechargeStock = 1;
            primaryDef1.requiredStock = 1;
            primaryDef1.stockToConsume = 1;
            Modules.Skills.FixSkillName(primaryDef1);
            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            Skills.primaryScorch = primaryDef1;

            skillLocator.primary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { primaryVariant1});
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_SECONDARY_NAME", "Return to Sender");
            LanguageAPI.Add("SS2UPYRO_SECONDARY_DESCRIPTION", $"<color=#D78326>Consume 33% heat</color> and fire a nonlethal blast of air that <style=cIsUtility>pushes</style> enemies and <style=cIsUtility>reflects projectiles</style>.");

            HeatSkillDef airblast = ScriptableObject.CreateInstance<HeatSkillDef>();
            airblast.activationState = new SerializableEntityStateType(typeof(Airblast));
            airblast.activationStateMachineName = "Weapon";
            airblast.skillName = "SS2UPYRO_SECONDARY_NAME";
            airblast.skillNameToken = "SS2UPYRO_SECONDARY_NAME";
            airblast.skillDescriptionToken = "SS2UPYRO_SECONDARY_DESCRIPTION";
            airblast.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill2");
            airblast.baseMaxStock = 1;
            airblast.baseRechargeInterval = 0f;
            airblast.beginSkillCooldownOnSkillEnd = true;
            airblast.canceledFromSprinting = false;
            airblast.fullRestockOnAssign = true;
            airblast.interruptPriority = EntityStates.InterruptPriority.Skill;
            airblast.isCombatSkill = false;
            airblast.mustKeyPress = false;
            airblast.cancelSprintingOnActivation = true;
            airblast.forceSprintDuringState = false;
            airblast.rechargeStock = 1;
            airblast.requiredStock = 1;
            airblast.stockToConsume = 1;
            airblast.baseHeatRequirement = 0.33f;
            Modules.Skills.FixSkillName(airblast);
            Modules.Skills.skillDefs.Add(airblast);
            SkillFamily.Variant heatWaveVariant = Utils.RegisterSkillVariant(airblast);

            Skills.secondaryAirblast = airblast;

            skillLocator.secondary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { heatWaveVariant });
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_UTILITY_NAME", "Plan B");
            LanguageAPI.Add("SS2UPYRO_UTILITY_DESCRIPTION", $"<color=#D78326>Consume 33% heat</color> and <style=cIsUtility>fly forwards</style>. Hold the button to fly further.");

            HeatSkillDef utilityDef1 = ScriptableObject.CreateInstance<HeatSkillDef>();
            utilityDef1.activationState = new SerializableEntityStateType(typeof(HeatJetpack));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "SS2UPYRO_UTILITY_NAME";
            utilityDef1.skillNameToken = "SS2UPYRO_UTILITY_NAME";
            utilityDef1.skillDescriptionToken = "SS2UPYRO_UTILITY_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill3");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 4f;
            utilityDef1.beginSkillCooldownOnSkillEnd = true;
            utilityDef1.canceledFromSprinting = false;
            utilityDef1.fullRestockOnAssign = true;
            utilityDef1.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            utilityDef1.isCombatSkill = false;
            utilityDef1.mustKeyPress = true;
            utilityDef1.cancelSprintingOnActivation = false;
            utilityDef1.forceSprintDuringState = true;
            utilityDef1.rechargeStock = 1;
            utilityDef1.requiredStock = 1;
            utilityDef1.stockToConsume = 1;
            utilityDef1.baseHeatRequirement = 0.33f;
            Modules.Skills.FixSkillName(utilityDef1);
            Modules.Skills.skillDefs.Add(utilityDef1);
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            Skills.utilityHeatJetpack = utilityDef1;

            skillLocator.utility = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { utilityVariant1 });
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_SPECIAL_NAME", "Blaze Flare");
            LanguageAPI.Add("SS2UPYRO_SPECIAL_DESCRIPTION", $"<color=#D78326>Consume all heat</color> and fire a flare for <style=cIsDamage>600% damage</style>. On impact, explodes for up to <style=cIsDamage>8x150% damage</style> based on <color=#D78326>heat</color> consumed.");

            HeatSkillDef flareGun = ScriptableObject.CreateInstance<HeatSkillDef>();
            flareGun.activationState = new SerializableEntityStateType(typeof(Flaregun));
            flareGun.activationStateMachineName = "Weapon";
            flareGun.skillName = "SS2UPYRO_SPECIAL_NAME";
            flareGun.skillNameToken = "SS2UPYRO_SPECIAL_NAME";
            flareGun.skillDescriptionToken = "SS2UPYRO_SPECIAL_DESCRIPTION";
            flareGun.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill4");
            flareGun.baseMaxStock = 1;
            flareGun.baseRechargeInterval = 7f;
            flareGun.beginSkillCooldownOnSkillEnd = false;
            flareGun.canceledFromSprinting = false;
            flareGun.fullRestockOnAssign = true;
            flareGun.interruptPriority = EntityStates.InterruptPriority.Skill;
            flareGun.isCombatSkill = true;
            flareGun.mustKeyPress = false;
            flareGun.cancelSprintingOnActivation = true;
            flareGun.forceSprintDuringState = false;
            flareGun.rechargeStock = 1;
            flareGun.requiredStock = 1;
            flareGun.stockToConsume = 1;
            flareGun.baseHeatRequirement = 0.125f;
            Modules.Skills.FixSkillName(flareGun);
            Modules.Skills.skillDefs.Add(flareGun);
            SkillFamily.Variant flareGunVariant = Utils.RegisterSkillVariant(flareGun);

            Skills.specialFlare = flareGun;

            skillLocator.special = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { flareGunVariant });

            SetUpScepters(skillLocator);
        }

        private void SetUpScepters(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_SPECIAL_SCEPTER_NAME", "Hell Flare");
            LanguageAPI.Add("SS2UPYRO_SPECIAL_SCEPTER_DESCRIPTION", $"<color=#D78326>Consume all heat</color> and fire a flare for <style=cIsDamage>600% damage</style>. On impact, it explodes for up to <style=cIsDamage>16x150% damage</style> based on <color=#D78326>heat</color> consumed.");

            HeatSkillDef flareScepter = ScriptableObject.CreateInstance<HeatSkillDef>();
            flareScepter.activationState = new SerializableEntityStateType(typeof(Flaregun));
            flareScepter.activationStateMachineName = "Weapon";
            flareScepter.skillName = "SS2UPYRO_SPECIAL_NAME";
            flareScepter.skillNameToken = "SS2UPYRO_SPECIAL_NAME";
            flareScepter.skillDescriptionToken = "SS2UPYRO_SPECIAL_DESCRIPTION";
            flareScepter.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill4_scepter");
            flareScepter.baseMaxStock = 1;
            flareScepter.baseRechargeInterval = 7f;
            flareScepter.beginSkillCooldownOnSkillEnd = false;
            flareScepter.canceledFromSprinting = false;
            flareScepter.fullRestockOnAssign = true;
            flareScepter.interruptPriority = EntityStates.InterruptPriority.Skill;
            flareScepter.isCombatSkill = true;
            flareScepter.mustKeyPress = false;
            flareScepter.cancelSprintingOnActivation = true;
            flareScepter.forceSprintDuringState = false;
            flareScepter.rechargeStock = 1;
            flareScepter.requiredStock = 1;
            flareScepter.stockToConsume = 1;
            flareScepter.baseHeatRequirement = 0.0625f;
            Modules.Skills.FixSkillName(flareScepter);
            Modules.Skills.skillDefs.Add(flareScepter);

            Skills.specialFlareScepter = flareScepter;

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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.specialFlareScepter, "SS2UPyroBody", SkillSlot.Special, 0);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.specialFlareScepter, "SS2UPyroBody", SkillSlot.Special, Skills.specialFlare);
        }

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UPyroBody");
            if (bodyIndex != BodyIndex.None) IgnoreSprintCrosshair.bodies.Add(bodyIndex);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SS2UPyroBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animPyroEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }
        internal static GameObject CreatePyroPrefab()
        {
            GameObject crosshair = BuildCrosshair();//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

            GameObject pyroPrefab = PrefabCore.CreatePrefab("SS2UPyroBody", "mdlPyro", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "SS2UPyroBody",
                bodyNameToken = "SS2UPYRO_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("pyroicon"),
                bodyColor = new Color32(215, 131, 38, 255),
                crosshair = crosshair,
                damage = 12f,
                healthGrowth = 30f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 100f,
                subtitleNameToken = "SS2UPYRO_SUBTITLE",
                podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                acceleration = 80f
            });

            pyroPrefab.AddComponent<HeatController>();
            pyroPrefab.AddComponent<FlamethrowerController>();

            PrefabCore.SetupCharacterModel(pyroPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matPyro", 1f, new Color(0.839f, 0.812f, 0.812f))
                }
            }, 0);

            //pyroPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(PyroMain));

            /*bool hadSlide = true;
            EntityStateMachine jetpackStateMachine = EntityStateMachine.FindByCustomName(pyroPrefab, "Slide");
            if (!jetpackStateMachine)
            {
                hadSlide = false;
                jetpackStateMachine = pyroPrefab.AddComponent<EntityStateMachine>();
            }
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            NetworkStateMachine nsm = pyroPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            if (!hadSlide)
            {
                SetStateOnHurt ssoh = pyroPrefab.GetComponent<SetStateOnHurt>();
                ssoh.idleStateMachine.Append(jetpackStateMachine);
            }*/

            return pyroPrefab;
        }

        private static GameObject BuildCrosshair()
        {
            GameObject crosshairPrefab = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("crosshairPyro.prefab").InstantiateClone("SS2UPyroCrosshair", false);
            crosshairPrefab.AddComponent<HudElement>();

            CrosshairController cc = crosshairPrefab.AddComponent<CrosshairController>();
            cc.maxSpreadAngle = 0f;

            crosshairPrefab.AddComponent<PyroCrosshairController>();

            return crosshairPrefab;
        }
    }
}
