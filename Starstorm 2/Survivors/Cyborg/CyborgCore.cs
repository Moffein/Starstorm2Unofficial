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

namespace Starstorm2.Survivors.Cyborg
{
    //Would prefer for this to be the same as Nemmando/ExeCore, but I don't want to rewrite this so I'll leave it as-is.
    public class CyborgCore
    {
        public static GameObject cybPrefab;
        public static GameObject doppelganger;

        public static BodyIndex bodyIndex;

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

            GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageIceLaser.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgTracer", false);
            tracerEffectPrefab.AddComponent<DestroyOnTimer>().duration = 0.3f;
            Modules.Assets.effectDefs.Add(new EffectDef(tracerEffectPrefab));
            PrimaryLaser.tracerEffectPrefab = tracerEffectPrefab;

            LanguageAPI.Add("CYBORG_NAME", "CYBORG");
            LanguageAPI.Add("CYBORG_SUBTITLE", "Man Made Monstrosity");
            LanguageAPI.Add("CYBORG_OUTRO_FLAVOR", "..and so it left, programming releasing excess serotonin.");
            LanguageAPI.Add("CYBORG_OUTRO_FAILURE", "..and so it vanished, teleportation beacon left with no signal.");
            LanguageAPI.Add("CYBORG_DESCRIPTION", "The CYBORG is a versatile and highly-efficient fusion of man and machine.\r\n\r\n< ! > Unmaker and Rising Star deal consistent damage at all ranges.\r\n\r\n< ! > Overheat Redress fizzles out over distance, so use it up close to deal the most damage!\r\n\r\n< ! > Use Recall to place warp points to return to while exploring a stage.\r\n\r\n< ! > Hold down the Recall button to destroy unwanted warp points.\r\n\r\n");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;
            CyborgSkins.RegisterSkins();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(cybPrefab, PrefabCore.CreateDisplayPrefab("CyborgDisplay", cybPrefab), Color.blue, "CYBORG", 40.1f);
            RoR2.RoR2Application.onLoad += SetBodyIndex;
        }

        private void LateSetup(HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            CyborgItemDisplays.RegisterDisplays();
        }

        private void RegisterStates()
        {
            Modules.States.AddSkill(typeof(JetpackOn));
            Modules.States.AddSkill(typeof(CyborgMain));
            Modules.States.AddSkill(typeof(CyborgFireTrackshot));
            Modules.States.AddSkill(typeof(CyborgFireOverheat));
            Modules.States.AddSkill(typeof(OverheatScepter));

            Modules.States.AddSkill(typeof(PrimaryLaser));
            Modules.States.AddSkill(typeof(DeployTeleporter));
            Modules.States.AddSkill(typeof(UseTeleporter));

            Modules.States.AddSkill(typeof(ChargeBeam));
            Modules.States.AddSkill(typeof(FireBeam));
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
            CyborgFireOverheat.projectilePrefab = CreateOverheatProjectile("SS2UCyborgOverheatProjectile", overheatGhost, 1, -300f);
            OverheatScepter.projectileOverride = CreateOverheatProjectile("SS2UCyborgOverheatScepterProjectile", overheatGhost, 3, -600f);

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
            bfgProjectileSimple.desiredForwardSpeed = 12f;
            bfgProjectileSimple.lifetime = 3f;

            ProjectileProximityBeamController bfgPbc = bfgProjectile.AddComponent<ProjectileProximityBeamController>();
            bfgPbc.attackRange = 15f;
            bfgPbc.listClearInterval = 1f / 5f;
            bfgPbc.attackInterval = bfgPbc.listClearInterval;
            bfgPbc.damageCoefficient = 1f * bfgPbc.listClearInterval;
            bfgPbc.procCoefficient = 1f;
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
            LanguageAPI.Add("CYBORG_PASSIVE_DESCRIPTION", "Holding the Jump key causes the CYBORG to <style=cIsUtility>hover in the air</style>.");
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

            skillLocator.primary = Utils.RegisterSkillsToFamily(cybPrefab, primaryVariant1);
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
            var dmg = CyborgFireTrackshot.damageCoefficient * 100f;

            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_SECONDARY_CHARGERIFLE_NAME", "Rising Star");
            //LanguageAPI.Add("CYBORG_SECONDARY_CHARGERIFLE_DESCRIPTION", $"Quickly fire three seeking shots at contenders in front for <style=cIsDamage>3x{dmg}% damage</style>. <style=cKeywordName>Stunning</style><style=cSub>.");
            LanguageAPI.Add("CYBORG_SECONDARY_CHARGERIFLE_DESCRIPTION", $"<style=cIsDamage>Stunning</style>. Charge up a piercing beam that deals <style=cIsDamage>400%-800% damage</style>. Deals <style=cIsDamage>+50% damage</style> when <style=cIsDamage>perfectly charged</style>.");

            SkillDef secondaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            secondaryDef1.activationState = new SerializableEntityStateType(typeof(ChargeBeam));
            secondaryDef1.activationStateMachineName = "Weapon";
            secondaryDef1.skillName = "CYBORG_SECONDARY_CHARGERIFLE_NAME";
            secondaryDef1.skillNameToken = "CYBORG_SECONDARY_CHARGERIFLE_NAME";
            secondaryDef1.skillDescriptionToken = "CYBORG_SECONDARY_CHARGERIFLE_DESCRIPTION";
            secondaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
            secondaryDef1.baseMaxStock = 1;
            secondaryDef1.baseRechargeInterval = 4f;
            secondaryDef1.beginSkillCooldownOnSkillEnd = true;
            secondaryDef1.canceledFromSprinting = false;
            secondaryDef1.fullRestockOnAssign = true;
            secondaryDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            secondaryDef1.isCombatSkill = true;
            secondaryDef1.mustKeyPress = false;
            secondaryDef1.cancelSprintingOnActivation = true;
            secondaryDef1.rechargeStock = 1;
            secondaryDef1.requiredStock = 1;
            secondaryDef1.stockToConsume = 1;
            secondaryDef1.keywordTokens = new string[] { "KEYWORD_STUNNING" };

            Utils.RegisterSkillDef(secondaryDef1, typeof(ChargeBeam));
            SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(secondaryDef1);

            skillLocator.secondary = Utils.RegisterSkillsToFamily(cybPrefab, secondaryVariant1);
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            var zapDmg = CyborgFireOverheat.damageCoefficient * 100f;

            //var dur = ExecutionerDash.debuffDuration;

            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_UTILITY_NAME", "Overheat Redress");
            LanguageAPI.Add("CYBORG_UTILITY_DESCRIPTION", $"<style=cIsUtility>Blast yourself backwards</style>, firing a greater energy bullet that deals a maximum of <style=cIsDamage>{zapDmg}% damage per second</style>.");

            zapDmg = CyborgFireOverheat.damageCoefficient * 100f * 1.5f;
            LanguageAPI.Add("CYBORG_UTILITY_SCEPTER_NAME", "Gamma Overheat Redress");
            LanguageAPI.Add("CYBORG_UTILITY_SCEPTER_DESCRIPTION", $"<style=cIsUtility>Blast yourself backwards</style>, firing a greater energy bullet that deals a maximum of <style=cIsDamage>{zapDmg}% damage per second</style>.");

            SkillDef utilityDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityDef1.activationState = new SerializableEntityStateType(typeof(CyborgFireOverheat));
            utilityDef1.activationStateMachineName = "Weapon";
            utilityDef1.skillName = "CYBORG_UTILITY_NAME";
            utilityDef1.skillNameToken = "CYBORG_UTILITY_NAME";
            utilityDef1.skillDescriptionToken = "CYBORG_UTILITY_DESCRIPTION";
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
            overheatDef = utilityDef1;
            Modules.Skills.skillDefs.Add(utilityDef1);
            Modules.Skills.FixSkillName(utilityDef1);
            SkillFamily.Variant utilityVariant1 = Utils.RegisterSkillVariant(utilityDef1);

            skillLocator.utility = Utils.RegisterSkillsToFamily(cybPrefab, utilityVariant1);

            SkillDef utilityScepterDef1 = ScriptableObject.CreateInstance<SkillDef>();
            utilityScepterDef1.activationState = new SerializableEntityStateType(typeof(OverheatScepter));
            utilityScepterDef1.activationStateMachineName = "Weapon";
            utilityScepterDef1.skillName = "CYBORG_UTILITY_SCEPTER_NAME";
            utilityScepterDef1.skillNameToken = "CYBORG_UTILITY_SCEPTER_NAME";
            utilityScepterDef1.skillDescriptionToken = "CYBORG_UTILITY_SCEPTER_DESCRIPTION";
            utilityScepterDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutilityscepter");
            utilityScepterDef1.baseMaxStock = 1;
            utilityScepterDef1.baseRechargeInterval = 10f;
            utilityScepterDef1.beginSkillCooldownOnSkillEnd = false;
            utilityScepterDef1.canceledFromSprinting = false;
            utilityScepterDef1.fullRestockOnAssign = true;
            utilityScepterDef1.interruptPriority = EntityStates.InterruptPriority.Skill;
            utilityScepterDef1.isCombatSkill = true;
            utilityScepterDef1.mustKeyPress = false;
            utilityScepterDef1.cancelSprintingOnActivation = false;
            utilityScepterDef1.rechargeStock = 1;
            utilityScepterDef1.requiredStock = 1;
            utilityScepterDef1.stockToConsume = 1;
            Modules.Skills.skillDefs.Add(utilityScepterDef1);
            overheatScepterDef = utilityScepterDef1;
            Modules.Skills.FixSkillName(utilityScepterDef1);

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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(overheatScepterDef, "CyborgBody", SkillSlot.Utility, 0);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(overheatScepterDef, "CyborgBody", SkillSlot.Utility, overheatDef);
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            SkillLocator skill = cybPrefab.GetComponent<SkillLocator>();

            LanguageAPI.Add("CYBORG_SPECIAL_TELEPORT_NAME", "Recall");
            LanguageAPI.Add("CYBORG_SPECIAL_TELEPORT_DESCRIPTION", "<style=cIsDamage>Shocking</style>. Create a <style=cIsUtility>warp point</style>. Reactivate to <style=cIsUtility>teleport to its location</style> and deal <style=cIsDamage>1200% damage</style>. Hold to remove existing warp points.");
            SkillDef specialDeploy = ScriptableObject.CreateInstance<SkillDef>();
            specialDeploy.activationState = new SerializableEntityStateType(typeof(DeployTeleporter));
            specialDeploy.activationStateMachineName = "Teleporter";
            specialDeploy.skillName = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialDeploy.skillNameToken = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialDeploy.skillDescriptionToken = "CYBORG_SPECIAL_TELEPORT_DESCRIPTION";
            specialDeploy.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial");
            specialDeploy.baseMaxStock = 1;
            specialDeploy.baseRechargeInterval = 20f;
            specialDeploy.beginSkillCooldownOnSkillEnd = true;
            specialDeploy.canceledFromSprinting = false;
            specialDeploy.fullRestockOnAssign = true;
            specialDeploy.interruptPriority = EntityStates.InterruptPriority.Any;
            specialDeploy.isCombatSkill = false;
            specialDeploy.mustKeyPress = true;
            specialDeploy.cancelSprintingOnActivation = false;
            specialDeploy.rechargeStock = 1;
            specialDeploy.requiredStock = 1;
            specialDeploy.stockToConsume = 1;
            specialDeploy.keywordTokens = new string[] { "KEYWORD_SHOCKING" };
            Modules.Skills.skillDefs.Add(specialDeploy);
            Modules.Skills.FixSkillName(specialDeploy);
            SkillFamily.Variant specialVariant1 = Utils.RegisterSkillVariant(specialDeploy);
            skillLocator.special = Utils.RegisterSkillsToFamily(cybPrefab, specialVariant1);

            CyborgTeleSkillDef specialTeleport = ScriptableObject.CreateInstance<CyborgTeleSkillDef>();
            specialTeleport.activationState = new SerializableEntityStateType(typeof(UseTeleporter));
            specialTeleport.activationStateMachineName = "Teleporter";
            specialTeleport.skillName = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialTeleport.skillNameToken = "CYBORG_SPECIAL_TELEPORT_NAME";
            specialTeleport.skillDescriptionToken = "CYBORG_SPECIAL_TELEPORT_DESCRIPTION";
            specialTeleport.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial2");
            specialTeleport.baseMaxStock = 1;
            specialTeleport.baseRechargeInterval = 0f;
            specialTeleport.beginSkillCooldownOnSkillEnd = false;
            specialTeleport.canceledFromSprinting = false;
            specialTeleport.fullRestockOnAssign = true;
            specialTeleport.interruptPriority = EntityStates.InterruptPriority.Skill;
            specialTeleport.isCombatSkill = false;
            specialTeleport.mustKeyPress = true;
            specialTeleport.cancelSprintingOnActivation = false;
            specialTeleport.rechargeStock = 0;
            specialTeleport.requiredStock = 1;
            specialTeleport.stockToConsume = 1;
            specialTeleport.keywordTokens = new string[] { };
            Modules.Skills.FixSkillName(specialTeleport);
            Modules.Skills.skillDefs.Add(specialTeleport);
            DeployTeleporter.teleportSkillDef = specialTeleport;
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

            return cyborgPrefab;
        }
    }
}
