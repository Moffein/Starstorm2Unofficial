using R2API;
using RoR2;
using RoR2.Projectile;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;
using System.Linq;
using EntityStates.Starstorm2States.Cyborg;
using Starstorm2.Survivors.Cyborg.Components;
using Starstorm2.Cores;
using Starstorm2.Modules;
using EntityStates.Starstorm2States.Cyborg.Special;
using Starstorm2.Survivors.Cyborg.Components.TeleportProjectile;

namespace Starstorm2.Survivors.Cyborg
{
    //Would prefer for this to be the same as Nemmando/ExeCore, but I don't want to rewrite this so I'll leave it as-is.
    public class CyborgCore
    {
        public static GameObject cybPrefab;
        public static GameObject doppelganger;

        public static GameObject bfgProjectile;

        public static BodyIndex bodyIndex;

        public CyborgCore() => Setup();
        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("CyborgBody");
        }

        private void Setup()
        {
            cybPrefab = CreateCyborgPrefab();
            cybPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(CyborgMain));

            EntityStateMachine jetpackStateMachine = cybPrefab.AddComponent<EntityStateMachine>();
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            NetworkStateMachine nsm = cybPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            SetStateOnHurt ssoh = cybPrefab.GetComponent<SetStateOnHurt>();
            ssoh.idleStateMachine.Append(jetpackStateMachine);

            EntityStateMachine teleStateMachine = cybPrefab.AddComponent<EntityStateMachine>();
            teleStateMachine.customName = "Teleporter";
            teleStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            teleStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(teleStateMachine).ToArray();

            GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageIceLaser.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgTracer", false);
            tracerEffectPrefab.AddComponent<DestroyOnTimer>().duration = 0.3f;
            Modules.Assets.effectDefs.Add(new EffectDef(tracerEffectPrefab));
            PrimaryLaser.tracerEffectPrefab = tracerEffectPrefab;

            LanguageAPI.Add("CYBORG_NAME", "Cyborg");
            LanguageAPI.Add("CYBORG_SUBTITLE", "Man Made Monstrosity");
            LanguageAPI.Add("CYBORG_OUTRO_FLAVOR", "..and so he left, programming releasing excess serotonin.");
            LanguageAPI.Add("CYBORG_OUTRO_FAILURE", "..and so he vanished, teleportation beacon left with no signal.");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            CyborgItemDisplays.RegisterDisplays();
            CyborgSkins.RegisterSkins();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(cybPrefab, PrefabCore.CreateDisplayPrefab("CyborgDisplay", cybPrefab), Color.blue, "CYBORG");
            RoR2.RoR2Application.onLoad += SetBodyIndex;
        }

        private void RegisterStates()
        {
            Modules.States.AddSkill(typeof(JetpackOn));
            Modules.States.AddSkill(typeof(CyborgMain));
            Modules.States.AddSkill(typeof(CyborgFireTrackshot));
            Modules.States.AddSkill(typeof(CyborgFireBFG));

            Modules.States.AddSkill(typeof(PrimaryLaser));
            Modules.States.AddSkill(typeof(DeployTeleporter));
            Modules.States.AddSkill(typeof(UseTeleporter));
        }

        private void RegisterProjectiles()
        {
            bfgProjectile = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FMJ"), "Prefabs/Projectiles/CyborgbfgProjectile", true);

            ProjectileController bfgProjectileController = bfgProjectile.GetComponent<ProjectileController>();
            bfgProjectileController.procCoefficient = 1f;
            bfgProjectileController.ghostPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/BeamSphereGhost");

            ProjectileDamage bfgDamage = bfgProjectile.GetComponent<ProjectileDamage>();
            bfgDamage.damage = 1f;
            bfgDamage.damageType = DamageType.Generic;
            bfgDamage.damageColorIndex = DamageColorIndex.Default;

            ProjectileSimple bfgProjectileSimple = bfgProjectile.GetComponent<ProjectileSimple>();
            bfgProjectileSimple.desiredForwardSpeed = 13;
            bfgProjectileSimple.lifetime = 4;

            ProjectileProximityBeamController bfgPbc = bfgProjectile.AddComponent<ProjectileProximityBeamController>();
            bfgPbc.attackRange = 8;
            bfgPbc.listClearInterval = .2f;
            bfgPbc.attackInterval = .2f;
            bfgPbc.damageCoefficient = 0.8f;
            bfgPbc.procCoefficient = .2f;
            bfgPbc.inheritDamageType = true;

            RadialForce bfgRadialForce = bfgProjectile.AddComponent<RadialForce>();
            bfgRadialForce.radius = 18;
            bfgRadialForce.damping = 0.5f;
            bfgRadialForce.forceMagnitude = -1500;
            bfgRadialForce.forceCoefficientAtEdge = 0.5f;

            ProjectileImpactExplosion bfgExplosion = bfgProjectile.AddComponent<ProjectileImpactExplosion>();
            bfgExplosion.impactEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BeamSphereExplosion");
            bfgExplosion.destroyOnEnemy = true;
            bfgExplosion.destroyOnWorld = true;
            bfgExplosion.timerAfterImpact = false;
            bfgExplosion.falloffModel = BlastAttack.FalloffModel.SweetSpot;
            bfgExplosion.lifetime = 3;
            bfgExplosion.lifetimeAfterImpact = 0;
            bfgExplosion.lifetimeRandomOffset = 0;
            bfgExplosion.blastRadius = 20;
            bfgExplosion.blastDamageCoefficient = 12f;
            bfgExplosion.blastProcCoefficient = 1;
            bfgExplosion.blastAttackerFiltering = AttackerFiltering.Default;
            bfgProjectile.GetComponent<ProjectileOverlapAttack>().enabled = false;

            //bfgProjectile.GetComponent<ProjectileProximityBeamController>().enabled = false;

            GameObject cyborgPylon = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"), "Prefabs/Projectiles/CyborgTPPylon", true);

            GameObject ghost = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("cyborgTeleGhost2");
            ghost.AddComponent<ProjectileGhostController>();

            ProjectileController pylonProjectileController = cyborgPylon.GetComponent<ProjectileController>();
            pylonProjectileController.ghostPrefab = ghost;

            cyborgPylon.GetComponent<ProjectileSimple>().lifetime = 1000000f;

            ProjectileImpactExplosion pylonProjectileImpactExplosion = cyborgPylon.GetComponent<ProjectileImpactExplosion>();
            pylonProjectileImpactExplosion.lifetime = 1000000f;
            pylonProjectileImpactExplosion.lifetimeAfterImpact = 1000000f;
            pylonProjectileImpactExplosion.destroyOnEnemy = false;
            //cyborgPylon.GetComponent<ProjectileImpactExplosion>().falloffModel = BlastAttack.FalloffModel.SweetSpot;

            Rigidbody pylonRigidBody = cyborgPylon.GetComponent<Rigidbody>();
            pylonRigidBody.drag = 2;
            pylonRigidBody.angularDrag = 2f;

            AntiGravityForce pylonAntiGrav = cyborgPylon.AddComponent<AntiGravityForce>();
            pylonAntiGrav.rb = pylonRigidBody;
            pylonAntiGrav.antiGravityCoefficient = 1;

            cyborgPylon.AddComponent<AssignToTeleportTracker>();

            Modules.Prefabs.projectilePrefabs.Add(bfgProjectile);
            Modules.Prefabs.projectilePrefabs.Add(cyborgPylon);

            DeployTeleporter.projectilePrefab = cyborgPylon;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in cybPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = cybPrefab.GetComponent<SkillLocator>();

            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            var dmg = PrimaryLaser.damageCoefficient * 100f;

            LanguageAPI.Add("CYBORG_PRIMARY_GUN_NAME", "Unmaker");
            LanguageAPI.Add("CYBORG_PRIMARY_GUN_DESCRIPTION", $"Shoot an enemy for <style=cIsDamage>{dmg}% damage</style>.");

            SteppedSkillDef primaryDef1 = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(PrimaryLaser));
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

            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            skillLocator.primary = Utils.RegisterSkillsToFamily(cybPrefab, primaryVariant1);
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            var dmg = CyborgFireTrackshot.damageCoefficient * 100f;

            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_SECONDARY_AIMBOT_NAME", "Rising Star");
            //LanguageAPI.Add("CYBORG_SECONDARY_AIMBOT_DESCRIPTION", $"Quickly fire three seeking shots at contenders in front for <style=cIsDamage>3x{dmg}% damage</style>. <style=cKeywordName>Stunning</style><style=cSub>.");
            LanguageAPI.Add("CYBORG_SECONDARY_AIMBOT_DESCRIPTION", $"Quickly fire three seeking shots at contenders in front for <style=cIsDamage>3x{dmg}% damage</style>. Stunning.");

            SkillDef secondaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(CyborgFireTrackshot));
            secondaryDef1.activationStateMachineName = "Weapon";
            secondaryDef1.skillName = "CYBORG_SECONDARY_AIMBOT_NAME";
            secondaryDef1.skillNameToken = "CYBORG_SECONDARY_AIMBOT_NAME";
            secondaryDef1.skillDescriptionToken = "CYBORG_SECONDARY_AIMBOT_DESCRIPTION";
            secondaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
            secondaryDef1.baseMaxStock = 1;
            secondaryDef1.baseRechargeInterval = 3f;
            secondaryDef1.beginSkillCooldownOnSkillEnd = false;
            secondaryDef1.canceledFromSprinting = false;
            secondaryDef1.fullRestockOnAssign = true;
            secondaryDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            secondaryDef1.isCombatSkill = true;
            secondaryDef1.mustKeyPress = false;
            secondaryDef1.cancelSprintingOnActivation = true;
            secondaryDef1.rechargeStock = 1;
            secondaryDef1.requiredStock = 1;
            secondaryDef1.stockToConsume = 1;

            Utils.RegisterSkillDef(secondaryDef1, typeof(CyborgFireTrackshot));
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef1);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(cybPrefab, secondaryVariant1);
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            var zapDmg = 0.8f * 100f * 5f;
            var explosionDmg = 12f * 100f;

            

            //var dur = ExecutionerDash.debuffDuration;

            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_SPECIAL_NOTPREON_NAME", "Overheat Redress");
            LanguageAPI.Add("CYBORG_SPECIAL_NOTPREON_DESCRIPTION", $"Blast yourself backwards, firing a greater energy bullet, dealing up to <style=cIsDamage>{zapDmg}%</style> damage per second. " +
                $"Explodes at the end dealing <style=cIsDamage>{explosionDmg}%</style> in an area.");

            SkillDef utilityDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(CyborgFireBFG));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "CYBORG_SPECIAL_NOTPREON_NAME";
            utilityDef1.skillNameToken = "CYBORG_SPECIAL_NOTPREON_NAME";
            utilityDef1.skillDescriptionToken = "CYBORG_SPECIAL_NOTPREON_DESCRIPTION";
            utilityDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutility");
            utilityDef1.baseMaxStock = 1;
            utilityDef1.baseRechargeInterval = 10f;
            utilityDef1.beginSkillCooldownOnSkillEnd = false;
            utilityDef1.canceledFromSprinting = false;
            utilityDef1.fullRestockOnAssign = true;
            utilityDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityDef1.isCombatSkill = true;
            utilityDef1.mustKeyPress = false;
            utilityDef1.cancelSprintingOnActivation = false;
            utilityDef1.rechargeStock = 1;
            utilityDef1.requiredStock = 1;
            utilityDef1.stockToConsume = 1;

            Utils.RegisterSkillDef(utilityDef1, typeof(CyborgFireBFG));
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            skillLocator.utility = Utils.RegisterSkillsToFamily(cybPrefab, utilityVariant1);
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_SPECIAL_TELEPORT_NAME", "Recall");
            LanguageAPI.Add("CYBORG_SPECIAL_TELEPORT_DESCRIPTION", "Create a warp point. Once a warp point is set, teleport to its location. Teleporting <style=cIsDamage>reduces skill cooldowns by 4 seconds</style>. " +
                $"Telefragging.");
            SkillDef specialDef1 = ScriptableObject.CreateInstance<SkillDef>();
            specialDef1.activationState = new EntityStates.SerializableEntityStateType(typeof(DeployTeleporter));
            specialDef1.activationStateMachineName = "Teleporter";
            specialDef1.skillName = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialDef1.skillNameToken = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialDef1.skillDescriptionToken = "CYBORG_SPECIAL_TELEPORT_DESCRIPTION";
            specialDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial");
            specialDef1.baseMaxStock = 1;
            specialDef1.baseRechargeInterval = 3f;
            specialDef1.beginSkillCooldownOnSkillEnd = false;
            specialDef1.canceledFromSprinting = false;
            specialDef1.fullRestockOnAssign = true;
            specialDef1.interruptPriority = EntityStates.InterruptPriority.Pain;
            specialDef1.isCombatSkill = false;
            specialDef1.mustKeyPress = false;
            specialDef1.cancelSprintingOnActivation = false;
            specialDef1.rechargeStock = 1;
            specialDef1.requiredStock = 1;
            specialDef1.stockToConsume = 1;
            specialDef1.keywordTokens = new string[] {};

            Modules.Skills.skillDefs.Add(specialDef1);
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(specialDef1);

            skillLocator.special = Utils.RegisterSkillsToFamily(cybPrefab, specialVariant1);
        }

        internal static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "CyborgMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = cybPrefab;

            Modules.Prefabs.masterPrefabs.Add(doppelganger);
        }


        internal static GameObject CreateCyborgPrefab()
        {
            GameObject cyborgPrefab = PrefabCore.CreatePrefab("CyborgBody", "mdlCyborg", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "CyborgBody",
                bodyNameToken = "CYBORG_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("cyborgicon"),
                bodyColor = new Color32(138, 183, 168, 255),
                crosshair = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
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

            return cyborgPrefab;
        }
    }
}
