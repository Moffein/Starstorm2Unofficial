using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;
using System.Linq;
using EntityStates.SS2UStates.Cyborg;
using Starstorm2.Survivors.Cyborg.Components;
using Starstorm2.Cores;
using Starstorm2.Modules;
using EntityStates.SS2UStates.Cyborg.Special;
using Starstorm2.Survivors.Cyborg.Components.TeleportProjectile;
using Starstorm2.Survivors.Cyborg.Components.OverheatProjectile;
using System.Runtime.CompilerServices;
using EntityStates.SS2UStates.Cyborg.Secondary;
using RoR2.CharacterAI;
using RoR2.UI;
using Starstorm2.Survivors.Cyborg.Components.Crosshair;
using EntityStates.SS2UStates.Cyborg.ChargeRifle;
using EntityStates.SS2UStates.Cyborg.Jetpack;

namespace Starstorm2.Survivors.Cyborg
{
    //Would prefer for this to be the same as Nemmando/ExeCore, but I don't want to rewrite this so I'll leave it as-is.
    public class CyborgCore
    {
        public static GameObject cybPrefab;
        public static GameObject doppelganger;

        public static BodyIndex bodyIndex;

        public static SkillDef chargeRifleDef;
        public static SkillDef defenseMatrixDef;
        public static SkillDef overheatDef;
        public static SkillDef overheatScepterDef;

        public CyborgCore() => Setup();
        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("CyborgBody");
        }

        private void Setup()
        {
            cybPrefab = CreateCyborgPrefab();
            R2API.ItemAPI.DoNotAutoIDRSFor(cybPrefab);

            JetpackOn.activationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/EngiHarpoonExplosion.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgJetpackEffect", false);
            EffectComponent jetpackEC = JetpackOn.activationEffectPrefab.GetComponent<EffectComponent>();
            jetpackEC.soundName = "";
            Modules.Assets.effectDefs.Add(new EffectDef(JetpackOn.activationEffectPrefab));

            GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageIceLaser.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgTracer", false);
            tracerEffectPrefab.AddComponent<DestroyOnTimer>().duration = 0.3f;
            Modules.Assets.effectDefs.Add(new EffectDef(tracerEffectPrefab));
            PrimaryLaser.tracerEffectPrefab = tracerEffectPrefab;
            FireBeam.tracerEffectPrefab = tracerEffectPrefab;

            LanguageAPI.Add("CYBORG_NAME", "Cyborg");
            LanguageAPI.Add("CYBORG_SUBTITLE", "Man Made Monstrosity");
            LanguageAPI.Add("CYBORG_OUTRO_FLAVOR", "..and so it left, programming releasing excess serotonin.");
            LanguageAPI.Add("CYBORG_OUTRO_FAILURE", "..and so it vanished, warp beacon left with no signal.");
            LanguageAPI.Add("CYBORG_DESCRIPTION", "Technology was to reach its moral bounds when the Cyborg was created. It's hard to know how much humanity is left inside them.\r\n\r\n< ! > Unmaker and Rising Star deal consistent damage at all ranges.\r\n\r\n< ! > Overheat Redress fizzles out over distance, so use it up close to deal the most damage!\r\n\r\n< ! > Use Recall to place warp points to return to while exploring a stage.\r\n\r\n< ! > Hold down the Recall button to destroy unwanted warp points.\r\n\r\n");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;
            CyborgSkins.RegisterSkins();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(cybPrefab, PrefabCore.CreateDisplayPrefab("CyborgDisplay", cybPrefab), Color.blue, "CYBORG", 40.1f);
            RoR2.RoR2Application.onLoad += SetBodyIndex;
            SetupDefenseMatrix();

            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();
        }

        private void SetupDefenseMatrix()
        {
            DefenseMatrix.matrixPrefab = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("DefenseMatrix.prefab");

            GameObject projectileDeletionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgDeleteProjectileEffect", false);
            EffectComponent ec = projectileDeletionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_captain_drone_zap";
            Modules.Assets.effectDefs.Add(new EffectDef(projectileDeletionEffect));
            DefenseMatrix.projectileDeletionEffectPrefab = projectileDeletionEffect;

            DefenseMatrixManager.Initialize();
        }

        private static GameObject BuildChargeRifleCrosshair()
        {
            GameObject crosshairPrefab = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("crosshairCyborgChargeRifle.prefab").InstantiateClone("SS2UCyborgCrosshair", false);
            crosshairPrefab.AddComponent<HudElement>();

            CrosshairController cc = crosshairPrefab.AddComponent<CrosshairController>();
            cc.maxSpreadAngle = 0f;

            crosshairPrefab.AddComponent<CyborgCrosshairChargeController>();

            return crosshairPrefab;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "CyborgBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animCyborg1Emote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }

        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            CyborgItemDisplays.RegisterDisplays();
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(JetpackOn));
            Modules.States.AddState(typeof(FlightMode));

            Modules.States.AddState(typeof(CyborgMain));
            Modules.States.AddState(typeof(CyborgFireTrackshot));
            Modules.States.AddState(typeof(CyborgFireOverheat));
            Modules.States.AddState(typeof(OverheatScepter));

            Modules.States.AddState(typeof(PrimaryLaser));
            Modules.States.AddState(typeof(DeployTeleporter));
            Modules.States.AddState(typeof(UseTeleporter));

            Modules.States.AddState(typeof(ChargeBeam));
            Modules.States.AddState(typeof(FireBeam));
            Modules.States.AddState(typeof(DefenseMatrix));
        }

        private void RegisterProjectiles()
        {

            GameObject overheatGhost = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/BeamSphereGhost").InstantiateClone("SS2UCyborgOverheatGhost", false);
            ParticleSystem[] overheatParticles = overheatGhost.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < overheatParticles.Length; i++)
            {
                switch (i)
                {
                    case 0: //fire
                        overheatParticles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
                        break;
                    case 1: //beams
                        overheatParticles[i].startColor = Color.white;
                        break;
                    case 2: //lightning
                        overheatParticles[i].startColor = Color.white;
                        break;
                }
            }
            overheatGhost.AddComponent<BFGGhostReduceSizeOverTime>();

            LightningSoundComponent.lightningSound = Modules.Assets.CreateNetworkSoundEventDef("Play_SS2U_RoR1Lightning");
            CyborgFireOverheat.projectilePrefab = CreateOverheatProjectile("SS2UCyborgOverheatProjectile", overheatGhost, 0, -300f);
            OverheatScepter.projectileOverride = CreateOverheatProjectile("SS2UCyborgOverheatScepterProjectile", overheatGhost, 2, -900f);

            GameObject cyborgPylon = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"), "Prefabs/Projectiles/CyborgTPPylon", true);

            GameObject ghost = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("cyborgTeleGhost2");
            ghost.AddComponent<ProjectileGhostController>();

            ProjectileController pylonProjectileController = cyborgPylon.GetComponent<ProjectileController>();
            pylonProjectileController.ghostPrefab = ghost;
            pylonProjectileController.cannotBeDeleted = true;
            pylonProjectileController.allowPrediction = false;

            ProjectileSimple cyborgPylonProjectileSimple = cyborgPylon.GetComponent<ProjectileSimple>();
            cyborgPylonProjectileSimple.lifetime = 1000000f;

            ProjectileImpactExplosion pylonProjectileImpactExplosion = cyborgPylon.GetComponent<ProjectileImpactExplosion>();
            pylonProjectileImpactExplosion.lifetime = 1000000f;
            pylonProjectileImpactExplosion.lifetimeAfterImpact = 1000000f;
            pylonProjectileImpactExplosion.destroyOnEnemy = false;
            pylonProjectileImpactExplosion.destroyOnWorld = false;
            //cyborgPylon.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.SweetSpot;

            Rigidbody pylonRigidBody = cyborgPylon.GetComponent<Rigidbody>();
            pylonRigidBody.drag = 2f;   //loader is 3
            pylonRigidBody.angularDrag = 2f;   //loader is 3

            AntiGravityForce pylonAntiGrav = cyborgPylon.GetComponent<AntiGravityForce>();
            pylonAntiGrav.antiGravityCoefficient = 1;

            cyborgPylon.AddComponent<AssignToTeleportTracker>();

            Modules.Prefabs.projectilePrefabs.Add(cyborgPylon);

            DeployTeleporter.projectilePrefab = cyborgPylon;

            GameObject telefragExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/OmniImpactVFXLightningMage.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgTelefragEffect", false);
            EffectComponent ec = telefragExplosionEffect.GetComponent<EffectComponent>();
            ec.soundName = "Play_mage_R_lightningBlast";
            Modules.Assets.effectDefs.Add(new EffectDef(telefragExplosionEffect));
            UseTeleporter.explosionEffectPrefab = telefragExplosionEffect;
        }

        private GameObject CreateOverheatProjectile(string name, GameObject ghostPrefab, int bounceCount, float pullStrength)
        {
            GameObject bfgProjectile = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FMJ"), name, true);

            ProjectileController bfgProjectileController = bfgProjectile.GetComponent<ProjectileController>();
            bfgProjectileController.procCoefficient = 1f;
            bfgProjectileController.ghostPrefab = ghostPrefab;

            ProjectileDamage bfgDamage = bfgProjectile.GetComponent<ProjectileDamage>();
            bfgDamage.damage = 1f;
            bfgDamage.damageType = DamageType.Generic;
            bfgDamage.damageColorIndex = DamageColorIndex.Default;

            ProjectileSimple bfgProjectileSimple = bfgProjectile.GetComponent<ProjectileSimple>();
            bfgProjectileSimple.desiredForwardSpeed = 15f;
            bfgProjectileSimple.lifetime = 3f;

            ProjectileProximityBeamController bfgPbc = bfgProjectile.AddComponent<ProjectileProximityBeamController>();
            bfgPbc.attackRange = 12f;
            bfgPbc.listClearInterval = 0.2f;
            bfgPbc.attackInterval = bfgPbc.listClearInterval;
            bfgPbc.damageCoefficient = 0.2f;
            bfgPbc.procCoefficient = 0.7f;
            bfgPbc.inheritDamageType = true;
            bfgPbc.bounces = bounceCount;
            bfgPbc.attackFireCount = 30;
            bfgPbc.lightningType = RoR2.Orbs.LightningOrb.LightningType.Loader;

            RadialForce bfgRadialForce = bfgProjectile.AddComponent<RadialForce>();
            bfgRadialForce.radius = bfgPbc.attackRange;
            bfgRadialForce.damping = 0.5f;
            bfgRadialForce.forceMagnitude = pullStrength;
            bfgRadialForce.forceCoefficientAtEdge = 0.5f;

            //No clue how to get this working. Simpler to just rely on PBC
            ProjectileOverlapAttack bfgOverlap = bfgProjectile.GetComponent<ProjectileOverlapAttack>();
            UnityEngine.Object.Destroy(bfgOverlap);
            /*bfgOverlap.resetInterval = 5f / 30f;
            bfgOverlap.fireFrequency = 5f / 30f;
            bfgOverlap.damageCoefficient = 5f / 30f;*/

            /*ProjectileImpactExplosion bfgExplosion = bfgProjectile.AddComponent<ProjectileImpactExplosion>();
            bfgExplosion.impactEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BeamSphereExplosion");
            bfgExplosion.destroyOnEnemy = false;
            bfgExplosion.destroyOnWorld = false;
            bfgExplosion.timerAfterImpact = false;
            bfgExplosion.falloffModel = BlastAttack.FalloffModel.None;
            bfgExplosion.lifetime = bfgProjectileSimple.lifetime;
            bfgExplosion.lifetimeAfterImpact = 0;
            bfgExplosion.lifetimeRandomOffset = 0;
            bfgExplosion.blastRadius = bfgPbc.attackRange;
            bfgExplosion.blastDamageCoefficient = 1f;
            bfgExplosion.blastProcCoefficient = 1f;
            bfgExplosion.blastAttackerFiltering = AttackerFiltering.Default;*/

            bfgProjectile.AddComponent<OverheatReduceTickrateOverTime>();
            bfgProjectile.AddComponent<LightningSoundComponent>();

            Modules.Prefabs.projectilePrefabs.Add(bfgProjectile);

            return bfgProjectile;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in cybPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_PASSIVE_NAME", "Thrusters");
            LanguageAPI.Add("CYBORG_PASSIVE_DESCRIPTION", "Holding the Jump key causes the Cyborg to <style=cIsUtility>hover in the air</style>.");
            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "CYBORG_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = "CYBORG_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgpassive");

            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            var dmg = PrimaryLaser.damageCoefficient * 100f;

            LanguageAPI.Add("CYBORG_PRIMARY_GUN_NAME", "Unmaker");
            LanguageAPI.Add("CYBORG_PRIMARY_GUN_DESCRIPTION", $"Fire a <style=cIsUtility>slowing</style> beam at contenders for <style=cIsDamage>{dmg}% damage</style>.");

            SteppedSkillDef primaryDef1 = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(PrimaryLaser));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "CYBORG_PRIMARY_GUN_NAME";
            primaryDef1.skillNameToken = "CYBORG_PRIMARY_GUN_NAME";
            primaryDef1.skillDescriptionToken = "CYBORG_PRIMARY_GUN_DESCRIPTION";
            primaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgprimary");
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
            primaryDef1.stepCount = 2;
            Modules.Skills.FixSkillName(primaryDef1);
            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            LanguageAPI.Add("CYBORG_PRIMARY_CHARGE_NAME", "Rising Star");
            LanguageAPI.Add("CYBORG_PRIMARY_CHARGE_DESCRIPTION", $"Charge up a <style=cIsUtility>slowing</style> beam that pierces for <style=cIsDamage>250%-750% damage</style>. Deals <style=cIsDamage>33%</style> more damage when <style=cIsDamage>perfectly charged</style>.");
            SteppedSkillDef primaryDef2 = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef2.activationState = new SerializableEntityStateType(typeof(ChargeBeam));
            primaryDef2.activationStateMachineName = "Weapon";
            primaryDef2.skillName = "CYBORG_PRIMARY_CHARGE_NAME";
            primaryDef2.skillNameToken = "CYBORG_PRIMARY_CHARGE_NAME";
            primaryDef2.skillDescriptionToken = "CYBORG_PRIMARY_CHARGE_DESCRIPTION";
            primaryDef2.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgprimary");
            primaryDef2.baseMaxStock = 1;
            primaryDef2.baseRechargeInterval = 0f;
            primaryDef2.beginSkillCooldownOnSkillEnd = false;
            primaryDef2.canceledFromSprinting = false;
            primaryDef2.fullRestockOnAssign = true;
            primaryDef2.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryDef2.isCombatSkill = true;
            primaryDef2.mustKeyPress = false;
            primaryDef2.cancelSprintingOnActivation = true;
            primaryDef2.rechargeStock = 1;
            primaryDef2.requiredStock = 1;
            primaryDef2.stockToConsume = 1;
            primaryDef2.stepCount = 2;
            Modules.Skills.FixSkillName(primaryDef2);
            Modules.Skills.skillDefs.Add(primaryDef2);
            SkillFamily.Variant primaryVariant2 = Utils.RegisterSkillVariant(primaryDef2);

            chargeRifleDef = primaryDef2;

            skillLocator.primary = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { primaryVariant1, primaryVariant2 });
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
             LanguageAPI.Add("CYBORG_SECONDARY_DEFENSEMATRIX_NAME", "Defense Matrix");
             LanguageAPI.Add("CYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION", "Project an energy field that <style=cIsUtility>neutralizes ranged attacks</style>. Reduce skill cooldowns by <style=cIsUtility>0.5s</style> for every <style=cIsUtility>projectile</style> deleted.");
             SkillDef defenseMatrixDef = ScriptableObject.CreateInstance<SkillDef>();
             defenseMatrixDef.activationState = new SerializableEntityStateType(typeof(DefenseMatrix));
             defenseMatrixDef.activationStateMachineName = "DefenseMatrix";
             defenseMatrixDef.skillName = "CYBORG_SECONDARY_DEFENSEMATRIX_NAME";
             defenseMatrixDef.skillNameToken = "CYBORG_SECONDARY_DEFENSEMATRIX_NAME";
             defenseMatrixDef.skillDescriptionToken = "CYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION";
             defenseMatrixDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
             defenseMatrixDef.baseMaxStock = 1;
             defenseMatrixDef.baseRechargeInterval = 6f;
             defenseMatrixDef.beginSkillCooldownOnSkillEnd = true;
             defenseMatrixDef.canceledFromSprinting = false;
             defenseMatrixDef.fullRestockOnAssign = true;
             defenseMatrixDef.interruptPriority = EntityStates.InterruptPriority.Any;
             defenseMatrixDef.isCombatSkill = false;
             defenseMatrixDef.mustKeyPress = false;
             defenseMatrixDef.cancelSprintingOnActivation = true;
             defenseMatrixDef.rechargeStock = 1;
             defenseMatrixDef.requiredStock = 1;
             defenseMatrixDef.stockToConsume = 1;
             defenseMatrixDef.keywordTokens = new string[] {};
             Modules.Skills.FixSkillName(defenseMatrixDef);
             Utils.RegisterSkillDef(defenseMatrixDef);
             SkillFamily.Variant secondaryVariant2 = Utils.RegisterSkillVariant(defenseMatrixDef);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { secondaryVariant2 });

            CyborgCore.defenseMatrixDef = defenseMatrixDef;
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_UTILITY_TELEPORT_NAME", "Recall");
            LanguageAPI.Add("CYBORG_UTILITY_TELEPORT_DESCRIPTION", "<style=cIsDamage>Shocking</style>. Create a <style=cIsUtility>warp point</style>. Reactivate to <style=cIsUtility>teleport to its location</style> and deal <style=cIsDamage>1200% damage</style>. Hold to remove existing warp points.");
            SkillDef teleDeploy = ScriptableObject.CreateInstance<SkillDef>();
            teleDeploy.activationState = new SerializableEntityStateType(typeof(DeployTeleporter));
            teleDeploy.activationStateMachineName = "Teleporter";
            teleDeploy.skillName = "CYBORG_UTILITY_TELEPORT_NAME";
            teleDeploy.skillNameToken = "CYBORG_UTILITY_TELEPORT_NAME";
            teleDeploy.skillDescriptionToken = "CYBORG_UTILITY_TELEPORT_DESCRIPTION";
            teleDeploy.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial");
            teleDeploy.baseMaxStock = 1;
            teleDeploy.baseRechargeInterval = 12f;
            teleDeploy.beginSkillCooldownOnSkillEnd = true;
            teleDeploy.canceledFromSprinting = false;
            teleDeploy.fullRestockOnAssign = true;
            teleDeploy.interruptPriority = EntityStates.InterruptPriority.Any;
            teleDeploy.isCombatSkill = false;
            teleDeploy.mustKeyPress = true;
            teleDeploy.cancelSprintingOnActivation = false;
            teleDeploy.rechargeStock = 1;
            teleDeploy.requiredStock = 1;
            teleDeploy.stockToConsume = 1;
            teleDeploy.keywordTokens = new string[] { "KEYWORD_SHOCKING" };
            Modules.Skills.skillDefs.Add(teleDeploy);
            Modules.Skills.FixSkillName(teleDeploy);
            SkillFamily.Variant variant1 = Utils.RegisterSkillVariant(teleDeploy);

            CyborgTeleSkillDef teleActivate = ScriptableObject.CreateInstance<CyborgTeleSkillDef>();
            teleActivate.activationState = new SerializableEntityStateType(typeof(UseTeleporter));
            teleActivate.activationStateMachineName = "Teleporter";
            teleActivate.skillName = "CYBORG_UTILITY_TELEPORT_NAME";
            teleActivate.skillNameToken = "CYBORG_UTILITY_TELEPORT_NAME";
            teleActivate.skillDescriptionToken = "CYBORG_UTILITY_TELEPORT_DESCRIPTION";
            teleActivate.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial2");
            teleActivate.baseMaxStock = 1;
            teleActivate.baseRechargeInterval = 3f;
            teleActivate.beginSkillCooldownOnSkillEnd = true;
            teleActivate.canceledFromSprinting = false;
            teleActivate.fullRestockOnAssign = true;
            teleActivate.interruptPriority = EntityStates.InterruptPriority.Skill;
            teleActivate.isCombatSkill = false;
            teleActivate.mustKeyPress = true;
            teleActivate.cancelSprintingOnActivation = false;
            teleActivate.rechargeStock = 0;
            teleActivate.requiredStock = 1;
            teleActivate.stockToConsume = 1;
            teleActivate.keywordTokens = new string[] { };
            Modules.Skills.FixSkillName(teleActivate);
            Modules.Skills.skillDefs.Add(teleActivate);
            DeployTeleporter.teleportSkillDef = teleActivate;


            LanguageAPI.Add("CYBORG_UTILITY_FLIGHT_NAME", "Flight Mode");
            LanguageAPI.Add("CYBORG_UTILITY_FLIGHT_DESCRIPTION", "<style=cIsUtility>Heavy</style>. Take flight, gaining <style=cIsUtility>200% movement speed</style>. Deals <style=cIsDamage>400% damage</style> to enemies on impact.");
            SkillDef flightMode = ScriptableObject.CreateInstance<SkillDef>();
            flightMode.activationState = new SerializableEntityStateType(typeof(FlightMode));
            flightMode.activationStateMachineName = "Jetpack";
            flightMode.skillName = "CYBORG_UTILITY_FLIGHT_NAME";
            flightMode.skillNameToken = "CYBORG_UTILITY_FLIGHT_NAME";
            flightMode.skillDescriptionToken = "CYBORG_UTILITY_FLIGHT_DESCRIPTION";
            flightMode.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgpassive");
            flightMode.baseMaxStock = 1;
            flightMode.baseRechargeInterval = 12f;
            flightMode.beginSkillCooldownOnSkillEnd = true;
            flightMode.canceledFromSprinting = false;
            flightMode.fullRestockOnAssign = true;
            flightMode.interruptPriority = EntityStates.InterruptPriority.Any;
            flightMode.isCombatSkill = false;
            flightMode.mustKeyPress = false;
            flightMode.cancelSprintingOnActivation = false;
            flightMode.rechargeStock = 1;
            flightMode.requiredStock = 1;
            flightMode.stockToConsume = 1;
            flightMode.forceSprintDuringState = true;
            flightMode.keywordTokens = new string[] { "KEYWORD_HEAVY" };
            Modules.Skills.skillDefs.Add(flightMode);
            Modules.Skills.FixSkillName(flightMode);
            SkillFamily.Variant variant2 = Utils.RegisterSkillVariant(flightMode);

            skillLocator.utility = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { variant1, variant2 });
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            var zapDmg = CyborgFireOverheat.damageCoefficient * 100f;

            //var dur = ExecutionerDash.debuffDuration;

            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_OVERHEAT_NAME", "Overheat Redress");
            LanguageAPI.Add("CYBORG_OVERHEAT_DESCRIPTION", $"<style=cIsUtility>Blast yourself backwards</style>, firing a greater energy bullet that deals a maximum of <style=cIsDamage>{zapDmg}% damage per second</style>.");

            zapDmg = CyborgFireOverheat.damageCoefficient * 100f * 1.5f;
            LanguageAPI.Add("CYBORG_OVERHEAT_SCEPTER_NAME", "Gamma Overheat Redress");
            LanguageAPI.Add("CYBORG_OVERHEAT_SCEPTER_DESCRIPTION", $"<style=cIsUtility>Blast yourself backwards</style> and fire a greater energy bullet that deals a maximum of <style=cIsDamage>{zapDmg}% damage per second</style>.");

            SkillDef overheat = ScriptableObject.CreateInstance<SkillDef>();
            overheat.activationState = new SerializableEntityStateType(typeof(CyborgFireOverheat));
            overheat.activationStateMachineName = "Weapon";
            overheat.skillName = "CYBORG_OVERHEAT_NAME";
            overheat.skillNameToken = "CYBORG_OVERHEAT_NAME";
            overheat.skillDescriptionToken = "CYBORG_OVERHEAT_DESCRIPTION";
            overheat.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutility");
            overheat.baseMaxStock = 1;
            overheat.baseRechargeInterval = 12f;
            overheat.beginSkillCooldownOnSkillEnd = false;
            overheat.canceledFromSprinting = false;
            overheat.fullRestockOnAssign = true;
            overheat.interruptPriority = EntityStates.InterruptPriority.Skill;
            overheat.isCombatSkill = true;
            overheat.mustKeyPress = false;
            overheat.cancelSprintingOnActivation = false;
            overheat.rechargeStock = 1;
            overheat.requiredStock = 1;
            overheat.stockToConsume = 1;
            overheatDef = overheat;
            Modules.Skills.skillDefs.Add(overheat);
            Modules.Skills.FixSkillName(overheat);
            SkillFamily.Variant specialVariant = Utils.RegisterSkillVariant(overheat);

            skillLocator.special = Utils.RegisterSkillsToFamily(cybPrefab, specialVariant);

            SkillDef scepterDef = ScriptableObject.CreateInstance<SkillDef>();
            scepterDef.activationState = new SerializableEntityStateType(typeof(OverheatScepter));
            scepterDef.activationStateMachineName = "Weapon";
            scepterDef.skillName = "CYBORG_OVERHEAT_SCEPTER_NAME";
            scepterDef.skillNameToken = "CYBORG_OVERHEAT_SCEPTER_NAME";
            scepterDef.skillDescriptionToken = "CYBORG_OVERHEAT_SCEPTER_DESCRIPTION";
            scepterDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutilityscepter");
            scepterDef.baseMaxStock = 1;
            scepterDef.baseRechargeInterval = 12f;
            scepterDef.beginSkillCooldownOnSkillEnd = false;
            scepterDef.canceledFromSprinting = false;
            scepterDef.fullRestockOnAssign = true;
            scepterDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            scepterDef.isCombatSkill = true;
            scepterDef.mustKeyPress = false;
            scepterDef.cancelSprintingOnActivation = false;
            scepterDef.rechargeStock = 1;
            scepterDef.requiredStock = 1;
            scepterDef.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(scepterDef);
            overheatScepterDef = scepterDef;
            Modules.Skills.FixSkillName(scepterDef);

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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(overheatScepterDef, "CyborgBody", SkillSlot.Special, 0);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(overheatScepterDef, "CyborgBody", SkillSlot.Special, overheatDef);
        }

        internal static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "CyborgMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = cybPrefab;
            Modules.Prefabs.RemoveAISkillDrivers(doppelganger);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Overheat", SkillSlot.Special, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 30f,
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


            Modules.Prefabs.AddAISkillDriver(doppelganger, "Teleport", SkillSlot.Utility, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 0f, 30f,
                 true, false, false, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 false, false, false,
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

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Shield", SkillSlot.Secondary, null,
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
                 -1f,
                 false,
                 true,
                 null);

            Modules.Prefabs.AddAISkillDriver(doppelganger, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 50f,
                true, false, true, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, true, true,
                AISkillDriver.MovementType.StrafeMovetarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                false,
                false,
                AISkillDriver.ButtonPressType.Hold,
                1,
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


        internal static GameObject CreateCyborgPrefab()
        {
            GameObject crosshair = BuildChargeRifleCrosshair();
            CyborgMain.chargeRifleCrosshair = crosshair;

            GameObject cyborgPrefab = PrefabCore.CreatePrefab("CyborgBody", "mdlCyborg", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "CyborgBody",
                bodyNameToken = "CYBORG_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("cyborgicon"),
                bodyColor = new Color32(138, 183, 168, 255),
                crosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion(),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "CYBORG_SUBTITLE",
                podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                acceleration = 40f
            });

            PrefabCore.SetupCharacterModel(cyborgPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matCyborg", 1f, new Color(0.839f, 0.812f, 0.812f))
                }
            }, 0);

            cyborgPrefab.AddComponent<CyborgController>();
            cyborgPrefab.AddComponent<CyborgTeleportTracker>();
            cyborgPrefab.AddComponent<CyborgChargeComponent>();

            cyborgPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(CyborgMain));

            bool hadSlide = true;
            EntityStateMachine jetpackStateMachine = EntityStateMachine.FindByCustomName(cyborgPrefab, "Slide");
            if (!jetpackStateMachine)
            {
                hadSlide = false;
                jetpackStateMachine = cyborgPrefab.AddComponent<EntityStateMachine>();
            }
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            NetworkStateMachine nsm = cyborgPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            if (!hadSlide)
            {
                SetStateOnHurt ssoh = cyborgPrefab.GetComponent<SetStateOnHurt>();
                ssoh.idleStateMachine.Append(jetpackStateMachine);
            }

            EntityStateMachine teleStateMachine = cyborgPrefab.AddComponent<EntityStateMachine>();
            teleStateMachine.customName = "Teleporter";
            teleStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            teleStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(teleStateMachine).ToArray();

            EntityStateMachine defenseStateMachine = cyborgPrefab.AddComponent<EntityStateMachine>();
            defenseStateMachine.customName = "DefenseMatrix";
            defenseStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            defenseStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(defenseStateMachine).ToArray();

            ModelLocator ml = cyborgPrefab.GetComponent<ModelLocator>();
            ChildLocator cl = ml.modelTransform.gameObject.GetComponent<ChildLocator>();
            PrefabCore.SetupHitbox(ml.modelTransform.gameObject, cl.FindChild("RamHitbox"), "RamHitbox");

            return cyborgPrefab;
        }
    }
}
