using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using R2API;
using Starstorm2Unofficial.Cores;
using System.Runtime.CompilerServices;
using EntityStates;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using Starstorm2Unofficial.Survivors.Pyro.Components;
using EntityStates.SS2UStates.Pyro;
using RoR2.UI;
using Starstorm2Unofficial.Survivors.Pyro.Components.Crosshair;
using RoR2.Projectile;
using Starstorm2Unofficial.Survivors.Pyro.Components.Projectile;
using RoR2.CharacterAI;

namespace Starstorm2Unofficial.Survivors.Pyro
{
    public class PyroCore
    {
        public static GameObject bodyPrefab;
        public static BodyIndex bodyIndex;

        public static class Skills
        {
            public static SkillDef primaryScorch;
            public static SkillDef secondaryFire;
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
            LanguageAPI.Add("SS2UPYRO_DESCRIPTION", "The Pyro will stop at nothing to burn down everything in sight.\r\n\r\n< ! > Scorch generates Heat, which is required to use Pyro's abilities.\r\n\r\n< ! > Projectiles reflected by Return to Sender deal massive damage.\r\n\r\n< ! > Plan B allows you to quickly get in and out of fights.\r\n\r\n< ! > Blaze Flare is strongest when used at high Heat.\r\n\r\n");
            LanguageAPI.Add("SS2UPYRO_LORE", "<style=cMono>//--AUTO-TRANSCRIPTION FROM PATROL SECTION 7 OF RALLYPOINT SIGMA--//</style>\n\nTwo patrol guards walk into the sight of the camera, the sounds of their argument slowly fading in as they take their positions at the patrol point.\n\n\"No! For the last time, no! He is not going out to deal with the bugs.\"\n\n\"Come on, Alan! Those bugs have been harassing the rallypoint for far too long now. I mean, Hell, just last night those things almost killed me, for crying out loud!\"\n\n\"Okay, first of all, Jess, stop exaggerating. You were fine. They might be getting feister, but it's nothing we can't handle. He'd just be a liability.\"\n\n\"What do you mean, a liability?\" Jess waves his hand out to the side. \"As far as I know, he's the single best suited person to deal with those bugs.\"\n\n\"Oh, gee, I dunno.\" Alan sets down his weapon to start a count. \"He's threatened people, gone out on 'hunting trips' in the middle of the night with NO ONE ELSE, he's started fires in the dorms while futzing around with his flamethrower, and that's just all before we took his weapons away.\" He lets out a subdued groan. \"Quite frankly, with his behavior, I'm stunned he was even allowed on the ship.\"\n\n\"He just wants to kill things, is all.\" Jess replied, before shrugging. \"And yeah, that's pretty screwed up, but if you gave him his stuff back and told him to run in the direction of the forest and burn anything that moves, he'd probably do it gladly.\"\n\n\"Ehh...\"\n\n\"Besides, if he does that, he'll probably just get himself killed.\" Jess added with a chuckle, \"Then he's no one's problem.\"");

            RegisterStates();
            SetUpSkills();
            PyroSkins.RegisterSkins();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(bodyPrefab, PrefabCore.CreateDisplayPrefab("PyroDisplay", bodyPrefab), Color.red, "SS2UPYRO", 40.3f);
            RoR2.RoR2Application.onLoad += SetBodyIndex;

            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();

            NetworkSoundEventDef sound = Modules.Assets.CreateNetworkSoundEventDef("Play_item_proc_behemoth");
            BuildFlaregunProjectile(sound);
            BuildFlaregunScepterProjectile(sound);
            BuildSuppressiveFireProjectile();
            Airblast.reflectSound = Modules.Assets.CreateNetworkSoundEventDef("Play_captain_drone_zap");
        }

        private void BuildSuppressiveFireProjectile()
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion().InstantiateClone("SS2UPyroSuppressiveFireProjectile", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);
            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<ProjectileSingleTargetImpact>());

            projectilePrefab.transform.localScale *= 1.5f;

            GameObject ghostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/FireballGhost.prefab").WaitForCompletion().InstantiateClone("SS2UPyroSuppressiveFireGhost", false);
            ghostPrefab.transform.localScale *= 0.5f;

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = ghostPrefab;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 100f;
            ps.lifetime = 0.5f;

            ProjectileImpactExplosion pie = projectilePrefab.AddComponent<ProjectileImpactExplosion>();
            pie.explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.blastDamageCoefficient = 1f;
            pie.blastProcCoefficient = 1f;
            pie.blastRadius = 3f;
            pie.bonusBlastForce = Vector3.zero;
            pie.canRejectForce = true;
            pie.destroyOnEnemy = true;
            pie.destroyOnWorld = true;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.lifetime = 0.5f;

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.IgniteOnHit;

            SuppressiveFire.projectilePrefab = projectilePrefab;
        }

        private void BuildFlaregunProjectile(NetworkSoundEventDef initialImpactSound)
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/Fireball.prefab").WaitForCompletion().InstantiateClone("SS2UPyroFlareProjectile", true);
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
            fpc.initialEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFXQuick.prefab").WaitForCompletion();
            fpc.explosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/IgniteOnKill/IgniteExplosionVFX.prefab").WaitForCompletion();
            fpc.explosionSound = initialImpactSound;

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
            fpc.explosionSound = initialImpactSound;
            fpc.explosionRadius = 18f;

            FlaregunScepter.scepterProjectilePrefab = projectilePrefab;
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(FireFlamethrower));
            Modules.States.AddState(typeof(SuppressiveFire));
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
            LanguageAPI.Add("SS2UPYRO_SECONDARY_DESCRIPTION", $"<color=#D78326>Consume 25% heat</color> and fire a nonlethal blast of air that <style=cIsUtility>pushes</style> enemies and <style=cIsUtility>reflects projectiles</style>.");

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
            airblast.cancelSprintingOnActivation = false;
            airblast.forceSprintDuringState = false;
            airblast.rechargeStock = 1;
            airblast.requiredStock = 1;
            airblast.stockToConsume = 1;
            airblast.baseHeatRequirement = 0.25f;
            Modules.Skills.FixSkillName(airblast);
            Modules.Skills.skillDefs.Add(airblast);
            SkillFamily.Variant airblastVariant = Utils.RegisterSkillVariant(airblast);
            Skills.secondaryAirblast = airblast;

            LanguageAPI.Add("SS2UPYRO_SECONDARY_ALT_NAME", "\"Suppressive Fire\"");
            LanguageAPI.Add("SS2UPYRO_SECONDARY_ALT_DESCRIPTION", $"<color=#D78326>Rapidly consume heat</color> to fire a stream of fireballs that deal <style=cIsDamage>840% damage per second</style> and <style=cIsDamage>ignite</style>.");

            HeatSkillDef suppressiveFire = ScriptableObject.CreateInstance<HeatSkillDef>();
            suppressiveFire.activationState = new SerializableEntityStateType(typeof(SuppressiveFire));
            suppressiveFire.activationStateMachineName = "Weapon";
            suppressiveFire.skillName = "SS2UPYRO_SECONDARY_ALT_NAME";
            suppressiveFire.skillNameToken = "SS2UPYRO_SECONDARY_ALT_NAME";
            suppressiveFire.skillDescriptionToken = "SS2UPYRO_SECONDARY_ALT_DESCRIPTION";
            suppressiveFire.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill2");
            suppressiveFire.baseMaxStock = 1;
            suppressiveFire.baseRechargeInterval = 0f;
            suppressiveFire.beginSkillCooldownOnSkillEnd = true;
            suppressiveFire.canceledFromSprinting = false;
            suppressiveFire.fullRestockOnAssign = true;
            suppressiveFire.interruptPriority = EntityStates.InterruptPriority.Any;
            suppressiveFire.isCombatSkill = true;
            suppressiveFire.mustKeyPress = false;
            suppressiveFire.cancelSprintingOnActivation = true;
            suppressiveFire.forceSprintDuringState = false;
            suppressiveFire.rechargeStock = 1;
            suppressiveFire.requiredStock = 1;
            suppressiveFire.stockToConsume = 1;
            suppressiveFire.baseHeatRequirement = 0.08f;
            Modules.Skills.FixSkillName(suppressiveFire);
            Modules.Skills.skillDefs.Add(suppressiveFire);
            SkillFamily.Variant fireVariant = Utils.RegisterSkillVariant(suppressiveFire);
            Skills.secondaryFire = suppressiveFire;

            skillLocator.secondary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { fireVariant, airblastVariant });
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_UTILITY_NAME", "Plan B");
            LanguageAPI.Add("SS2UPYRO_UTILITY_DESCRIPTION", $"<color=#D78326>Consume 33% heat</color> and <style=cIsUtility>fly forwards</style>. Hold the button to fly further at the cost of more <color=#D78326>heat</color>.");

            HeatSkillDef utilityDef1 = ScriptableObject.CreateInstance<HeatSkillDef>();
            utilityDef1.activationState = new SerializableEntityStateType(typeof(HeatJetpack));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "SS2UPYRO_UTILITY_NAME";
            utilityDef1.skillNameToken = "SS2UPYRO_UTILITY_NAME";
            utilityDef1.skillDescriptionToken = "SS2UPYRO_UTILITY_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill3");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 5f;
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
            LanguageAPI.Add("SS2UPYRO_SPECIAL_SCEPTER_DESCRIPTION", $"<color=#D78326>Consume all Heat</color> and fire a flare for <style=cIsDamage>600% damage</style>. On impact, it explodes for up to <style=cIsDamage>16x150% damage</style> based on <color=#D78326>heat</color> consumed.");

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
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("texPortraitPyro"),
                bodyColor = new Color32(215, 131, 38, 255),
                crosshair = crosshair,
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
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

        internal static void CreateDoppelganger()
        {
            GameObject doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "SS2UPyroMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Flare", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1,
                false,
                true,
                null);


            Modules.Prefabs.AddAISkillDriver(doppelganger, "Jetpack", SkillSlot.Utility, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 24f, Mathf.Infinity,
                 false, false, false, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 false, false, false,
                 AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                 AISkillDriver.AimType.AtCurrentEnemy,
                 false,
                 true,
                 false,
                 AISkillDriver.ButtonPressType.Hold,
                 1f,
                 false,
                 true,
                 null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "W+M1", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 24f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                3f,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
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
