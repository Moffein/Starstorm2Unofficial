using BepInEx.Configuration;
using EntityStates;
using EntityStates.SS2UStates.Cyborg;
using EntityStates.SS2UStates.Cyborg.ChargeRifle;
using EntityStates.SS2UStates.Cyborg.Jetpack;
using EntityStates.SS2UStates.Cyborg.Secondary;
using EntityStates.SS2UStates.Cyborg.Special;
using R2API;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using RoR2.UI;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
using Starstorm2Unofficial.Survivors.Cyborg.Components.Crosshair;
using Starstorm2Unofficial.Survivors.Cyborg.Components.OverheatProjectile;
using Starstorm2Unofficial.Survivors.Cyborg.Components.ShockCoreProjectile;
using Starstorm2Unofficial.Survivors.Cyborg.Components.TeleportProjectile;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Cyborg
{
    //Would prefer for this to be the same as Nemmando/ExeCore, but I don't want to rewrite this so I'll leave it as-is.
    public class CyborgCore
    {
        public static GameObject cybPrefab;
        public static GameObject doppelganger;

        public static BodyIndex bodyIndex;

        public static class Skills
        {
            public static SkillDef Unmaker, ChargeRifle;
            public static EnergySkillDef DefenseMatrix, RisingStar, Recall, FlightMode, Overheat, OverheatScepter, ShockCore, ShockCoreScepter;
            public static CyborgTeleSkillDef RecallOverride;
        }

        public static ConfigEntry<bool> useEnergyRework;

        public CyborgCore() => Setup();
        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UCyborgBody");
            if (bodyIndex != BodyIndex.None) IgnoreSprintCrosshair.bodies.Add(bodyIndex);
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
            FireTriShot.tracerEffectPrefab = tracerEffectPrefab;

            LanguageAPI.Add("SS2UCYBORG_NAME", "Cyborg");
            LanguageAPI.Add("SS2UCYBORG_SUBTITLE", "Man Made Monstrosity");
            LanguageAPI.Add("SS2UCYBORG_OUTRO_FLAVOR", "..and so it left, programming releasing excess serotonin.");
            LanguageAPI.Add("SS2UCYBORG_OUTRO_FAILURE", "..and so it vanished, warp beacon left with no signal.");
            LanguageAPI.Add("SS2UCYBORG_DESCRIPTION", "Technology was to reach its moral bounds when the Cyborg was created. It's hard to know how much humanity is left inside them.<style=cSub>\r\n\r\n< ! > Cyborg's skills all draw from a single Energy Pool, allowing for flexibility with skill usage.\r\n\r\n< ! > Rising Star deals very high damage per second, but consumes energy quickly.\r\n\r\n< ! > Use Recall to place a Warp Point before engaging enemies so that you have an escape route.\r\n\r\n< ! > Overheat Redress is Cyborg's best tool for dealing with crowds.\r\n\r\n");
            LanguageAPI.Add("SS2UCYBORG_LORE", "<style=cMono>Audio file found under \"Dr. Rayell's Testimony\". Playing...</style>\n\n\"Recording started. Begin testing, Dr. Rayell.\"\n\n\"Thank you. After the EOD's smashing success with biomechanical augmentations, producing some of the finest combat engineers to ever come out of the department, they proceeded further with slightly more... shall we say, experimental designs.\"\n\n\"How so?\"\n\n\"The department began looking into mental augmentation. It was definitely a leap, to be sure. Augmentation was already very cutting edge, but supplementing - or even replacing a damaged brain? It was a tall order. But the heads thought we could do it, so we obliged.\"\n\n\"Hm. What came of your project?\"\n\n\"Once prototyping was complete, we began the human trials. We got all sorts of folks as test subjects, they were vegetative, mentally unstable, broken people. We were told to fix them as best would could.\"\n\n\"And how did that go?\"\n\n\"It was... It was a travesty. Some of them were simply too far gone. There was nothing we could do. Those were the best cases. Worse was when they rejected the replacements. Some died in a swift immune response. The more cognisant ones begged for us to end the headaches they were having. Some of them just started screaming. They'd scream for hours and hours, sometimes for days even. And then they'd stop. They'd literally scream themselves to death.\"\n\n\"That's, um, a bit unsettling. Is that all?\"\n\n\"There was one. One who made it through the replacement. And it worked wonderfully. We got him in a vegetative state, no hope whatsoever, and we took him from that, and we gave him - maybe not his full life back, but perhaps something resembling it. You have no idea how thankful I was to hear that he was doing well.\"\n\n\"Well, I suppose that's good to hear. So, what's happened to him?\"\n\n\"You know? I'm not sure. Other than some behavioral changes, which is to be expected, he was able to return to society rather gracefully. We've done check-ups on him every three months to analyze long-term effects, but... Actually? Now that I'm thinking about it, he didn't show up for it last month. Oh, well. I'm sure he's doing fine. I'll have to contact him about that.\"\n\n\"Um, alright, then. Thank you for your time, Dr. Rayell. Your testimony is... Well, it's certainly something. I hope to hear more about your projects in the future.\"\n\n\"Very well. I hope you find what you're looking for.\"\n\n<style=cMono>End of file.</style>");

            LanguageAPI.Add("SS2UCYBORG_PASSIVE_NAME", "Energy Core");
            LanguageAPI.Add("SS2UCYBORG_PASSIVE_DESCRIPTION", "The Cyborg's skills share a single <color=#8BEDE3>Energy Pool</color> for their cooldowns.\nHolding the Jump key causes the Cyborg to <style=cIsUtility>hover in the air</style>.");
            LanguageAPI.Add("SS2UCYBORG_PASSIVE_DESCRIPTION_NOENERGY", "Holding the Jump key causes the Cyborg to <style=cIsUtility>hover in the air</style>.");

            LanguageAPI.Add("SS2UCYBORG_PRIMARY_GUN_NAME", "Unmaker");
            LanguageAPI.Add("SS2UCYBORG_PRIMARY_GUN_DESCRIPTION", "Fire a beam at contenders for <style=cIsDamage>300% damage</style>.");

            LanguageAPI.Add("SS2UCYBORG_PRIMARY_CHARGE_NAME", "Piercing Shot");
            LanguageAPI.Add("SS2UCYBORG_PRIMARY_CHARGE_DESCRIPTION", "Charge up a beam that pierces for <style=cIsDamage>300%-900% damage</style>. Deals <style=cIsDamage>1200%</style> damage when <style=cIsDamage>perfectly charged</style>.");
            
            LanguageAPI.Add("SS2UCYBORG_SECONDARY_DEFENSEMATRIX_NAME", "Defense Matrix");
            LanguageAPI.Add("SS2UCYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION", "<color=#8BEDE3>15% Energy/s</color>. Project an energy field that <style=cIsUtility>neutralizes ranged attacks</style>.");
            LanguageAPI.Add("SS2UCYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION_NOENERGY", "Project an energy field that <style=cIsUtility>neutralizes ranged attacks</style>.");

            LanguageAPI.Add("SS2UCYBORG_SECONDARY_TRISHOT_NAME", "Rising Star");
            LanguageAPI.Add("SS2UCYBORG_SECONDARY_TRISHOT_DESCRIPTION", "<color=#8BEDE3>30% Energy/s</color>. Rapidly fire <style=cIsUtility>slowing</style> shots at contenders for <style=cIsDamage>3x140% damage</style>.");
            LanguageAPI.Add("SS2UCYBORG_SECONDARY_TRISHOT_DESCRIPTION_NOENERGY", "Rapidly fire <style=cIsUtility>slowing</style> shots at contenders for <style=cIsDamage>3x140% damage</style>.");
            
            LanguageAPI.Add("SS2UCYBORG_UTILITY_TELEPORT_NAME", "Recall");
            LanguageAPI.Add("SS2UCYBORG_UTILITY_TELEPORT_DESCRIPTION", "<color=#8BEDE3>50% Energy</color>. Create a <style=cIsUtility>warp point</style>. Reactivate to <style=cIsUtility>teleport to its location</style>. Hold to remove existing warp points.");
            LanguageAPI.Add("SS2UCYBORG_UTILITY_TELEPORT_DESCRIPTION_NOENERGY", "<style=cIsDamage>Shocking</style>. Create a <style=cIsUtility>warp point</style>. Reactivate to <style=cIsUtility>teleport to its location</style> and deal <style=cIsDamage>800% damage</style>. Hold to remove existing warp points.");

            LanguageAPI.Add("SS2UCYBORG_UTILITY_FLIGHT_NAME", "Flight Mode");
            LanguageAPI.Add("SS2UCYBORG_UTILITY_FLIGHT_DESCRIPTION", "<color=#8BEDE3>60% Energy/s</color>. <style=cIsUtility>Heavy</style>. Take flight, gaining <style=cIsUtility>200% movement speed</style>. Deals <style=cIsDamage>400% damage</style> to enemies on impact.");
            LanguageAPI.Add("SS2UCYBORG_UTILITY_FLIGHT_DESCRIPTION_NOENERGY", "<style=cIsUtility>Heavy</style>. Take flight, gaining <style=cIsUtility>200% movement speed</style>. Deals <style=cIsDamage>400% damage</style> to enemies on impact.");

            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_NAME", "Overheat Redress");
            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_DESCRIPTION", "<color=#8BEDE3>60% Energy</color>. <style=cIsUtility>Blast yourself backwards</style>, firing a greater energy bullet that deals a maximum of <style=cIsDamage>1200% damage per second</style>.");
            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_DESCRIPTION_NOENERGY", "<style=cIsUtility>Blast yourself backwards</style>, firing a greater energy bullet that deals a maximum of <style=cIsDamage>1200% damage per second</style>.");

            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_SCEPTER_NAME", "Gamma Overheat Redress");
            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_SCEPTER_DESCRIPTION", "<color=#8BEDE3>60% Energy</color>. <style=cIsUtility>Blast yourself backwards</style> and fire a greater energy bullet that deals a maximum of <style=cIsDamage>1800% damage per second</style>.");
            LanguageAPI.Add("SS2UCYBORG_OVERHEAT_SCEPTER_DESCRIPTION_NOENERGY", "<style=cIsUtility>Blast yourself backwards</style> and fire a greater energy bullet that deals a maximum of <style=cIsDamage>1800% damage per second</style>.");

            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_NAME", "Shock Core");
            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_DESCRIPTION", "<color=#8BEDE3>40% Energy</color>. <style=cIsDamage>Shocking</style>. Fire an energy core for <style=cIsDamage>400% damage</style>. Shoot the core to implode it for <style=cIsDamage>1200% damage</style>.");
            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_DESCRIPTION_NOENERGY", "<style=cIsDamage>Shocking</style>. Fire an energy core for <style=cIsDamage>400% damage</style>. Shoot the core to implode it for <style=cIsDamage>1200% damage</style>.");

            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_SCEPTER_NAME", "Gamma Shock Core");
            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_SCEPTER_DESCRIPTION", "<color=#8BEDE3>40% Energy</color>. <style=cIsDamage>Shocking</style>. Fire an energy core for <style=cIsDamage>600% damage</style>. Shoot the core to implode it for <style=cIsDamage>1800% damage</style>.");
            LanguageAPI.Add("SS2UCYBORG_SHOCKCORE_SCEPTER_DESCRIPTION_NOENERGY", "<style=cIsDamage>Shocking</style>. Fire an energy core for <style=cIsDamage>600% damage</style>. Shoot the core to implode it for <style=cIsDamage>1800% damage</style>.");

            RegisterProjectiles();
            RegisterStates();
            SetUpSkills();
            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;
            CyborgSkins.RegisterSkins();
            CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(cybPrefab, PrefabCore.CreateDisplayPrefab("CyborgDisplay", cybPrefab), Color.blue, "SS2UCYBORG", 40.1f);
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
                    if (item.bodyPrefab.name == "SS2UCyborgBody")
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
            Modules.States.AddState(typeof(CyborgFireOverheat));
            Modules.States.AddState(typeof(OverheatScepter));

            Modules.States.AddState(typeof(PrimaryLaser));
            Modules.States.AddState(typeof(DeployTeleporter));
            Modules.States.AddState(typeof(UseTeleporter));

            Modules.States.AddState(typeof(ChargeBeam));
            Modules.States.AddState(typeof(FireBeam));

            Modules.States.AddState(typeof(DefenseMatrix));
            Modules.States.AddState(typeof(FireTriShot));
            Modules.States.AddState(typeof(StopTriShot));
            Modules.States.AddState(typeof(ShockCore));
            Modules.States.AddState(typeof(ShockCoreScepter));
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
                        overheatParticles[i].startColor = Color.white;
                        break;
                    case 1: //beams
                        overheatParticles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
                        break;
                    case 2: //lightning
                        overheatParticles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
                        break;
                }
            }
            overheatGhost.AddComponent<BFGGhostReduceSizeOverTime>();
            CyborgFireOverheat.projectilePrefab = CreateOverheatProjectile("SS2UCyborgOverheatProjectile", overheatGhost, 0, -600f);

            GameObject overheatGhostScepter = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/BeamSphereGhost").InstantiateClone("SS2UCyborgOverheatGhostScepter", false);
            overheatParticles = overheatGhostScepter.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < overheatParticles.Length; i++)
            {
                switch (i)
                {
                    case 0: //fire
                        overheatParticles[i].startColor = Color.white;
                        break;
                    case 1: //beams
                        overheatParticles[i].startColor = new Color(246f / 255f, 169f / 255f, 237f / 255f);
                        break;
                    case 2: //lightning
                        overheatParticles[i].startColor = new Color(246f / 255f, 169f / 255f, 237f / 255f);
                        break;
                }
            }
            overheatGhostScepter.AddComponent<BFGGhostReduceSizeOverTime>();
            OverheatScepter.projectileOverride = CreateOverheatProjectile("SS2UCyborgOverheatScepterProjectile", overheatGhostScepter, 2, -900f);

            CyborgFireOverheat.projectilePrefab = CreateOverheatProjectile("SS2UCyborgOverheatProjectile", overheatGhost, 0, -600f);
            OverheatScepter.projectileOverride = CreateOverheatProjectile("SS2UCyborgOverheatScepterProjectile", overheatGhostScepter, 2, -900f);

            GameObject cyborgPylon = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EngiGrenadeProjectile"), "Prefabs/Projectiles/CyborgTPPylon", true);

            GameObject ghost = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("cyborgTeleGhost2");
            ghost.AddComponent<ProjectileGhostController>();

            ProjectileController pylonProjectileController = cyborgPylon.GetComponent<ProjectileController>();
            pylonProjectileController.ghostPrefab = ghost;
            pylonProjectileController.cannotBeDeleted = true;
            pylonProjectileController.allowPrediction = false;

            ProjectileSimple cyborgPylonProjectileSimple = cyborgPylon.GetComponent<ProjectileSimple>();
            cyborgPylonProjectileSimple.lifetime = 1000000f;
            cyborgPylonProjectileSimple.desiredForwardSpeed = 65f;

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


            GameObject shockGhostPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltGhost.prefab").WaitForCompletion().InstantiateClone("SS2UCyborgShockCoreGhost", false);
            shockGhostPrefab.transform.localScale = 2f * Vector3.one;  //vector3.one

            GameObject shockCoreExplosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/MageLightningBombExplosion.prefab").WaitForCompletion().InstantiateClone("SS2UShockCoreImplosionEffect", false);
            EffectComponent ec2 = shockCoreExplosionEffectPrefab.GetComponent<EffectComponent>();
            ec2.soundName = "Play_mage_m2_impact";
            Modules.Assets.effectDefs.Add(new EffectDef(shockCoreExplosionEffectPrefab));

            GameObject implosionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion();

            ShockCore.projectilePrefab = CreateShockCoreProjectile("SS2UCyborgShockCoreProjectile", shockGhostPrefab, shockCoreExplosionEffectPrefab, 20f, implosionEffectPrefab);
            ShockCoreScepter.scepterProjectilePrefab = CreateShockCoreProjectile("SS2UCyborgShockCoreScepterProjectile", shockGhostPrefab, shockCoreExplosionEffectPrefab, 30f, implosionEffectPrefab);
        }

        private GameObject CreateShockCoreProjectile(string prefabName, GameObject ghostPrefab, GameObject explosionEffectPrefab, float radius, GameObject implosionEffectPrefab)
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone(prefabName, true);
            //projectilePrefab.transform.localScale = 2f * Vector3.one; //0.1, 0.1, 1.0

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = ghostPrefab;
            pc.allowPrediction = false;

            UnityEngine.Object.Destroy(projectilePrefab.GetComponent<MineProximityDetonator>());

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 60;//lightning bomb 40, lightning bolt 80
            ps.lifetime = 10f;
            ps.updateAfterFiring = true;

            ProjectileDamage pd = projectilePrefab.GetComponent<ProjectileDamage>();
            pd.damageType = DamageType.Shock5s;

            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.blastRadius = 4f;

            TeamComponent tc = projectilePrefab.AddComponent<TeamComponent>();
            tc.hideAllyCardDisplay = true;

            projectilePrefab.AddComponent<SkillLocator>();
            CharacterBody cb = projectilePrefab.AddComponent<CharacterBody>();
            cb.rootMotionInMainState = false;
            cb.bodyFlags = CharacterBody.BodyFlags.Masterless;
            cb.baseMaxHealth = 1f;
            cb.baseCrit = 0f;
            cb.baseAcceleration = 0f;
            cb.baseArmor = 0f;
            cb.baseAttackSpeed = 0f;
            cb.baseDamage = 0f;
            cb.baseJumpCount = 0;
            cb.baseJumpPower = 0f;
            cb.baseMoveSpeed = 0f;
            cb.baseMaxShield = 0f;
            cb.baseRegen = 0f;
            cb.autoCalculateLevelStats = true;
            cb.levelArmor = 0f;
            cb.levelAttackSpeed = 0f;
            cb.levelCrit = 0f;
            cb.levelDamage = 0f;
            cb.levelJumpPower = 0f;
            cb.levelMaxHealth = 0f;
            cb.levelMaxShield = 0f;
            cb.levelMoveSpeed = 0f;
            cb.levelRegen = 0f;
            cb.hullClassification = HullClassification.Human;

            HealthComponent hc = projectilePrefab.AddComponent<HealthComponent>();
            hc.globalDeathEventChanceCoefficient = 0f;
            hc.body = cb;

            ShootableShockCore sp = projectilePrefab.AddComponent<ShootableShockCore>();
            sp.targetDamageType = DamageTypeCore.ModdedDamageTypes.CyborgCanDetonateShockCore;
            sp.explosionEffectPrefab = explosionEffectPrefab;
            sp.radius = radius;
            sp.pullRadius = radius * 1.5f;
            sp.implosionStartEffectPrefab = implosionEffectPrefab;

            AddSphereHurtbox(projectilePrefab, 1f);

            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);
            return projectilePrefab;
        }
        private void AddSphereHurtbox(GameObject go, float radius)
        {
            GameObject hbObject = new GameObject();
            hbObject.transform.parent = go.transform;
            //GameObject hbObject = go;

            hbObject.layer = LayerIndex.entityPrecise.intVal;
            SphereCollider goCollider = hbObject.AddComponent<SphereCollider>();
            goCollider.radius = radius;

            HurtBoxGroup goHurtBoxGroup = hbObject.AddComponent<HurtBoxGroup>();
            HurtBox goHurtBox = hbObject.AddComponent<HurtBox>();
            goHurtBox.isBullseye = false;
            goHurtBox.healthComponent = go.GetComponent<HealthComponent>();
            goHurtBox.damageModifier = HurtBox.DamageModifier.Normal;
            goHurtBox.hurtBoxGroup = goHurtBoxGroup;
            goHurtBox.indexInGroup = 0;

            HurtBox[] goHurtBoxArray = new HurtBox[]
            {
                goHurtBox
            };

            goHurtBoxGroup.bullseyeCount = 0;
            goHurtBoxGroup.hurtBoxes = goHurtBoxArray;
            goHurtBoxGroup.mainHurtBox = goHurtBox;

            DisableCollisionsBetweenColliders dc = go.AddComponent<DisableCollisionsBetweenColliders>();
            dc.collidersA = go.GetComponentsInChildren<Collider>();
            dc.collidersB = hbObject.GetComponents<Collider>();
        }

        private GameObject CreateOverheatProjectile(string name, GameObject ghostPrefab, int bounceCount, float pullStrength)
        {
            GameObject projectilePrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FMJ"), name, true);

            ProjectileController bfgProjectileController = projectilePrefab.GetComponent<ProjectileController>();
            bfgProjectileController.procCoefficient = 1f;
            bfgProjectileController.ghostPrefab = ghostPrefab;

            ProjectileDamage bfgDamage = projectilePrefab.GetComponent<ProjectileDamage>();
            bfgDamage.damage = 1f;
            bfgDamage.damageType = DamageType.Generic;
            bfgDamage.damageColorIndex = DamageColorIndex.Default;

            ProjectileSimple bfgProjectileSimple = projectilePrefab.GetComponent<ProjectileSimple>();
            bfgProjectileSimple.desiredForwardSpeed = 15f;
            bfgProjectileSimple.lifetime = 3f;

            float hitRate = 1f / 8f;
            ProjectileProximityBeamController bfgPbc = projectilePrefab.AddComponent<ProjectileProximityBeamController>();
            bfgPbc.attackRange = 15f;
            bfgPbc.listClearInterval = hitRate;
            bfgPbc.attackInterval = bfgPbc.listClearInterval;
            bfgPbc.damageCoefficient = hitRate;
            bfgPbc.procCoefficient = 0.7f;
            bfgPbc.inheritDamageType = true;
            bfgPbc.bounces = bounceCount;
            bfgPbc.attackFireCount = 30;
            bfgPbc.lightningType = RoR2.Orbs.LightningOrb.LightningType.Loader;

            RadialForce bfgRadialForce = projectilePrefab.AddComponent<RadialForce>();
            bfgRadialForce.radius = bfgPbc.attackRange;
            bfgRadialForce.damping = 0.5f;
            bfgRadialForce.forceMagnitude = pullStrength;
            bfgRadialForce.forceCoefficientAtEdge = 0.5f;

            //No clue how to get this working. Simpler to just rely on PBC
            ProjectileOverlapAttack bfgOverlap = projectilePrefab.GetComponent<ProjectileOverlapAttack>();
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

            projectilePrefab.AddComponent<OverheatReduceTickrateOverTime>();

            projectilePrefab.AddComponent<LightningSoundComponent>();
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            return projectilePrefab;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in cybPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillLocator skillLocator = cybPrefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = "SS2UCYBORG_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_PASSIVE_DESCRIPTION" : "SS2UCYBORG_PASSIVE_DESCRIPTION_NOENERGY";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgpassive");

            SetUpPrimaries(skillLocator);
            SetUpSecondaries(skillLocator);
            SetUpUtilities(skillLocator);
            SetUpSpecials(skillLocator);

            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;
            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
            On.RoR2.SkillLocator.DeductCooldownFromAllSkillsAuthority += SkillLocator_DeductCooldownFromAllSkillsAuthority;
        }

        private void SkillLocator_DeductCooldownFromAllSkillsAuthority(On.RoR2.SkillLocator.orig_DeductCooldownFromAllSkillsAuthority orig, SkillLocator self, float deduction)
        {
            orig(self, deduction);
            var energyComponent = self.gameObject.GetComponent<CyborgEnergyComponent>();
            if (energyComponent)
            {
                float energyRestoreFraction = 0f;
                if (self.secondary) energyRestoreFraction += deduction / self.secondary.CalculateFinalRechargeInterval();
                if (self.utility) energyRestoreFraction += deduction / self.utility.CalculateFinalRechargeInterval();
                if (self.special) energyRestoreFraction += deduction / self.special.CalculateFinalRechargeInterval();
                energyComponent.AddEnergyFraction(energyRestoreFraction);
            }
        }

        private void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);
            var energyComponent = self.gameObject.GetComponent<CyborgEnergyComponent>();
            if (energyComponent) energyComponent.ApplyAmmoPack();
        }

        private void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);
            var energyComponent = self.gameObject.GetComponent<CyborgEnergyComponent>();
            if (energyComponent) energyComponent.ResetEnergy();
        }

        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            SteppedSkillDef primaryDef1 = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(PrimaryLaser));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "SS2UCYBORG_PRIMARY_GUN_NAME";
            primaryDef1.skillNameToken = "SS2UCYBORG_PRIMARY_GUN_NAME";
            primaryDef1.skillDescriptionToken = "SS2UCYBORG_PRIMARY_GUN_DESCRIPTION";
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
            Skills.Unmaker = primaryDef1;

            SteppedSkillDef primaryDef2 = ScriptableObject.CreateInstance<SteppedSkillDef>();
            primaryDef2.activationState = new SerializableEntityStateType(typeof(ChargeBeam));
            primaryDef2.activationStateMachineName = "Weapon";
            primaryDef2.skillName = "SS2UCYBORG_PRIMARY_CHARGE_NAME";
            primaryDef2.skillNameToken = "SS2UCYBORG_PRIMARY_CHARGE_NAME";
            primaryDef2.skillDescriptionToken = "SS2UCYBORG_PRIMARY_CHARGE_DESCRIPTION";
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

            Skills.ChargeRifle = primaryDef2;

            skillLocator.primary = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { primaryVariant1, primaryVariant2 });
        }

        private void SetUpSecondaries(SkillLocator skillLocator)
        {
             EnergySkillDef defenseMatrixDef = ScriptableObject.CreateInstance<EnergySkillDef>();
             defenseMatrixDef.activationState = new SerializableEntityStateType(typeof(DefenseMatrix));
             defenseMatrixDef.activationStateMachineName = "DefenseMatrix";
             defenseMatrixDef.skillName = "SS2UCYBORG_SECONDARY_DEFENSEMATRIX_NAME";
             defenseMatrixDef.skillNameToken = "SS2UCYBORG_SECONDARY_DEFENSEMATRIX_NAME";
             defenseMatrixDef.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION" : "SS2UCYBORG_SECONDARY_DEFENSEMATRIX_DESCRIPTION_NOENERGY";
             defenseMatrixDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
             defenseMatrixDef.baseMaxStock = 1;
             defenseMatrixDef.baseRechargeInterval = useEnergyRework.Value ? 5f : 6f;
             defenseMatrixDef.beginSkillCooldownOnSkillEnd = true;
             defenseMatrixDef.canceledFromSprinting = false;
             defenseMatrixDef.fullRestockOnAssign = true;
             defenseMatrixDef.interruptPriority = EntityStates.InterruptPriority.Any;
             defenseMatrixDef.isCombatSkill = false;
             defenseMatrixDef.mustKeyPress = false;
             defenseMatrixDef.cancelSprintingOnActivation = true;
             defenseMatrixDef.rechargeStock = 0;
             defenseMatrixDef.requiredStock = 0;
             defenseMatrixDef.stockToConsume = 0;
             defenseMatrixDef.keywordTokens = new string[] {};
             Modules.Skills.FixSkillName(defenseMatrixDef);
             Utils.RegisterSkillDef(defenseMatrixDef);
             SkillFamily.Variant secondaryVariant1 = Utils.RegisterSkillVariant(defenseMatrixDef);
             Skills.DefenseMatrix = defenseMatrixDef;

            EnergySkillDef triShotDef = ScriptableObject.CreateInstance<EnergySkillDef>();
            triShotDef.activationState = new SerializableEntityStateType(typeof(FireTriShot));
            triShotDef.activationStateMachineName = "Weapon";
            triShotDef.skillName = "SS2UCYBORG_SECONDARY_TRISHOT_NAME";
            triShotDef.skillNameToken = "SS2UCYBORG_SECONDARY_TRISHOT_NAME";
            triShotDef.skillDescriptionToken = "SS2UCYBORG_SECONDARY_TRISHOT_DESCRIPTION";
            triShotDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
            triShotDef.baseMaxStock = 1;
            triShotDef.baseRechargeInterval = useEnergyRework.Value ? 5f : 6f;
            triShotDef.beginSkillCooldownOnSkillEnd = true;
            triShotDef.canceledFromSprinting = false;
            triShotDef.fullRestockOnAssign = true;
            triShotDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            triShotDef.isCombatSkill = false;
            triShotDef.mustKeyPress = false;
            triShotDef.cancelSprintingOnActivation = true;
            triShotDef.rechargeStock = 0;
            triShotDef.requiredStock = 0;
            triShotDef.stockToConsume = 0;
            triShotDef.keywordTokens = new string[] { };
            Modules.Skills.FixSkillName(triShotDef);
            Utils.RegisterSkillDef(triShotDef);
            SkillFamily.Variant secondaryVariant2 = Utils.RegisterSkillVariant(triShotDef);
            Skills.RisingStar = triShotDef;

            skillLocator.secondary = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { secondaryVariant2, secondaryVariant1 });
        }

        private void SetUpUtilities(SkillLocator skillLocator)
        {
            EnergySkillDef teleDeploy = ScriptableObject.CreateInstance<EnergySkillDef>();
            teleDeploy.activationState = new SerializableEntityStateType(typeof(DeployTeleporter));
            teleDeploy.activationStateMachineName = "Teleporter";
            teleDeploy.skillName = "SS2UCYBORG_UTILITY_TELEPORT_NAME";
            teleDeploy.skillNameToken = "SS2UCYBORG_UTILITY_TELEPORT_NAME";
            teleDeploy.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_UTILITY_TELEPORT_DESCRIPTION" : "SS2UCYBORG_UTILITY_TELEPORT_DESCRIPTION_NOENERGY";
            teleDeploy.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial");
            teleDeploy.baseMaxStock = 1;
            teleDeploy.baseRechargeInterval = useEnergyRework.Value ? 5f : 8f;
            teleDeploy.beginSkillCooldownOnSkillEnd = true;
            teleDeploy.canceledFromSprinting = false;
            teleDeploy.fullRestockOnAssign = true;
            teleDeploy.interruptPriority = EntityStates.InterruptPriority.Any;
            teleDeploy.isCombatSkill = false;
            teleDeploy.mustKeyPress = true;
            teleDeploy.cancelSprintingOnActivation = false;
            teleDeploy.rechargeStock = 1;
            teleDeploy.requiredStock = 1;
            teleDeploy.stockToConsume = useEnergyRework.Value ? 0 : 1;
            teleDeploy.keywordTokens = new string[] {};
            teleDeploy.energyFractionCost = 0.5f;
            Modules.Skills.skillDefs.Add(teleDeploy);
            Modules.Skills.FixSkillName(teleDeploy);
            SkillFamily.Variant variant1 = Utils.RegisterSkillVariant(teleDeploy);
            Skills.Recall = teleDeploy;

            CyborgTeleSkillDef teleActivate = ScriptableObject.CreateInstance<CyborgTeleSkillDef>();
            teleActivate.activationState = new SerializableEntityStateType(typeof(UseTeleporter));
            teleActivate.activationStateMachineName = "Teleporter";
            teleActivate.skillName = "SS2UCYBORG_UTILITY_TELEPORT_NAME";
            teleActivate.skillNameToken = "SS2UCYBORG_UTILITY_TELEPORT_NAME";
            teleActivate.skillDescriptionToken = teleDeploy.skillDescriptionToken;
            teleActivate.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial2");
            teleActivate.baseMaxStock = 1;
            teleActivate.baseRechargeInterval = 5f;
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
            Skills.RecallOverride = teleActivate;

            EnergySkillDef flightMode = ScriptableObject.CreateInstance<EnergySkillDef>();
            flightMode.activationState = new SerializableEntityStateType(typeof(FlightMode));
            flightMode.activationStateMachineName = "Jetpack";
            flightMode.skillName = "SS2UCYBORG_UTILITY_FLIGHT_NAME";
            flightMode.skillNameToken = "SS2UCYBORG_UTILITY_FLIGHT_NAME";
            flightMode.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_UTILITY_FLIGHT_DESCRIPTION" : "SS2UCYBORG_UTILITY_FLIGHT_DESCRIPTION_NOENERGY";
            flightMode.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgpassive");
            flightMode.baseMaxStock = 1;
            flightMode.baseRechargeInterval = useEnergyRework.Value ? 5f : 8f;
            flightMode.beginSkillCooldownOnSkillEnd = true;
            flightMode.canceledFromSprinting = false;
            flightMode.fullRestockOnAssign = true;
            flightMode.interruptPriority = EntityStates.InterruptPriority.Any;
            flightMode.isCombatSkill = false;
            flightMode.mustKeyPress = false;
            flightMode.cancelSprintingOnActivation = false;
            flightMode.rechargeStock = 1;
            flightMode.requiredStock = 1;
            flightMode.stockToConsume = useEnergyRework.Value ? 0 : 1;
            flightMode.forceSprintDuringState = true;
            flightMode.keywordTokens = new string[] { "KEYWORD_HEAVY" };
            Modules.Skills.skillDefs.Add(flightMode);
            Modules.Skills.FixSkillName(flightMode);
            SkillFamily.Variant variant2 = Utils.RegisterSkillVariant(flightMode);
            Skills.FlightMode = flightMode;

            skillLocator.utility = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { variant1, variant2 });
        }

        private void SetUpSpecials(SkillLocator skillLocator)
        {
            EnergySkillDef overheat = ScriptableObject.CreateInstance<EnergySkillDef>();
            overheat.activationState = new SerializableEntityStateType(typeof(CyborgFireOverheat));
            overheat.activationStateMachineName = "Special";
            overheat.skillName = "SS2UCYBORG_OVERHEAT_NAME";
            overheat.skillNameToken = "SS2UCYBORG_OVERHEAT_NAME";
            overheat.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_OVERHEAT_DESCRIPTION": "SS2UCYBORG_OVERHEAT_DESCRIPTION_NOENERGY";
            overheat.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutility");
            overheat.baseMaxStock = 1;
            overheat.baseRechargeInterval = useEnergyRework.Value ? 5f : 10f;
            overheat.beginSkillCooldownOnSkillEnd = false;
            overheat.canceledFromSprinting = false;
            overheat.fullRestockOnAssign = true;
            overheat.interruptPriority = EntityStates.InterruptPriority.Skill;
            overheat.isCombatSkill = true;
            overheat.mustKeyPress = false;
            overheat.cancelSprintingOnActivation = false;
            overheat.rechargeStock = 1;
            overheat.requiredStock = 1;
            overheat.stockToConsume = useEnergyRework.Value ? 0 : 1;
            overheat.energyFractionCost = 0.6f;
            Skills.Overheat = overheat;
            Modules.Skills.skillDefs.Add(overheat);
            Modules.Skills.FixSkillName(overheat);
            SkillFamily.Variant specialVariant = Utils.RegisterSkillVariant(overheat);

            EnergySkillDef shockCoreDef = ScriptableObject.CreateInstance<EnergySkillDef>();
            shockCoreDef.activationState = new SerializableEntityStateType(typeof(ShockCore));
            shockCoreDef.activationStateMachineName = "Special";
            shockCoreDef.skillName = "SS2UCYBORG_SHOCKCORE_NAME";
            shockCoreDef.skillNameToken = "SS2UCYBORG_SHOCKCORE_NAME";
            shockCoreDef.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_SHOCKCORE_DESCRIPTION" : "SS2UCYBORG_SHOCKCORE_DESCRIPTION_NOENERGY";
            shockCoreDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgshockcore");
            shockCoreDef.baseMaxStock = useEnergyRework.Value ? 1 : 2;
            shockCoreDef.baseRechargeInterval = 5f;
            shockCoreDef.beginSkillCooldownOnSkillEnd = false;
            shockCoreDef.canceledFromSprinting = false;
            shockCoreDef.fullRestockOnAssign = true;
            shockCoreDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            shockCoreDef.isCombatSkill = true;
            shockCoreDef.mustKeyPress = true;
            shockCoreDef.cancelSprintingOnActivation = true;
            shockCoreDef.rechargeStock = 1;
            shockCoreDef.requiredStock = 1;
            shockCoreDef.stockToConsume = useEnergyRework.Value ? 0 : 1;
            shockCoreDef.keywordTokens = new string[] { "KEYWORD_SHOCKING" };
            shockCoreDef.energyFractionCost = 0.4f;
            Modules.Skills.FixSkillName(shockCoreDef);
            Utils.RegisterSkillDef(shockCoreDef);
            SkillFamily.Variant shockVariant = Utils.RegisterSkillVariant(shockCoreDef);
            Skills.ShockCore = shockCoreDef;

            skillLocator.special = Utils.RegisterSkillsToFamily(cybPrefab, new SkillFamily.Variant[] { specialVariant, shockVariant });

            EnergySkillDef overheatScepter = ScriptableObject.CreateInstance<EnergySkillDef>();
            overheatScepter.activationState = new SerializableEntityStateType(typeof(OverheatScepter));
            overheatScepter.activationStateMachineName = "Special";
            overheatScepter.skillName = "SS2UCYBORG_OVERHEAT_SCEPTER_NAME";
            overheatScepter.skillNameToken = "SS2UCYBORG_OVERHEAT_SCEPTER_NAME";
            overheatScepter.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_OVERHEAT_SCEPTER_DESCRIPTION" : "SS2UCYBORG_OVERHEAT_SCEPTER_DESCRIPTION_NOENERGY";
            overheatScepter.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgutilityscepter");
            overheatScepter.baseMaxStock = 1;
            overheatScepter.baseRechargeInterval = overheat.baseRechargeInterval;
            overheatScepter.beginSkillCooldownOnSkillEnd = false;
            overheatScepter.canceledFromSprinting = false;
            overheatScepter.fullRestockOnAssign = true;
            overheatScepter.interruptPriority = EntityStates.InterruptPriority.Skill;
            overheatScepter.isCombatSkill = true;
            overheatScepter.mustKeyPress = false;
            overheatScepter.cancelSprintingOnActivation = false;
            overheatScepter.rechargeStock = 1;
            overheatScepter.requiredStock = 1;
            overheatScepter.stockToConsume = overheat.stockToConsume;
            overheatScepter.energyFractionCost = overheat.energyFractionCost;
            Modules.Skills.skillDefs.Add(overheatScepter);
            Skills.OverheatScepter = overheatScepter;
            Modules.Skills.FixSkillName(overheatScepter);

            EnergySkillDef shockCoreScepterDef = ScriptableObject.CreateInstance<EnergySkillDef>();
            shockCoreScepterDef.activationState = new SerializableEntityStateType(typeof(ShockCoreScepter));
            shockCoreScepterDef.activationStateMachineName = "Special";
            shockCoreScepterDef.skillName = "SS2UCYBORG_SHOCKCORE_SCEPTER_NAME";
            shockCoreScepterDef.skillNameToken = "SS2UCYBORG_SHOCKCORE_SCEPTER_NAME";
            shockCoreScepterDef.skillDescriptionToken = useEnergyRework.Value ? "SS2UCYBORG_SHOCKCORE_SCEPTER_DESCRIPTION" : "SS2UCYBORG_SHOCKCORE_SCEPTER_DESCRIPTION_NOENERGY";
            shockCoreScepterDef.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgshockcorescepter");
            shockCoreScepterDef.baseMaxStock = shockCoreDef.baseMaxStock;
            shockCoreScepterDef.baseRechargeInterval = shockCoreDef.baseRechargeInterval;
            shockCoreScepterDef.beginSkillCooldownOnSkillEnd = false;
            shockCoreScepterDef.canceledFromSprinting = false;
            shockCoreScepterDef.fullRestockOnAssign = true;
            shockCoreScepterDef.interruptPriority = EntityStates.InterruptPriority.Skill;
            shockCoreScepterDef.isCombatSkill = true;
            shockCoreScepterDef.mustKeyPress = true;
            shockCoreScepterDef.cancelSprintingOnActivation = true;
            shockCoreScepterDef.rechargeStock = 1;
            shockCoreScepterDef.requiredStock = 1;
            shockCoreScepterDef.stockToConsume = shockCoreDef.stockToConsume;
            shockCoreScepterDef.keywordTokens = new string[] { "KEYWORD_SHOCKING" };
            shockCoreScepterDef.energyFractionCost = shockCoreDef.energyFractionCost;
            Modules.Skills.FixSkillName(shockCoreScepterDef);
            Skills.ShockCoreScepter = shockCoreScepterDef;

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

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.OverheatScepter, "SS2UCyborgBody", Skills.Overheat);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.ShockCoreScepter, "SS2UCyborgBody", Skills.ShockCore);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.OverheatScepter, "SS2UCyborgBody", SkillSlot.Special, Skills.Overheat);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(Skills.ShockCoreScepter, "SS2UCyborgBody", SkillSlot.Special, Skills.ShockCore);
        }

        internal static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "SS2UCyborgMonsterMaster", true);
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
            GameObject crosshair = BuildChargeRifleCrosshair();//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion()

            GameObject cyborgPrefab = PrefabCore.CreatePrefab("SS2UCyborgBody", "mdlCyborg", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "SS2UCyborgBody",
                bodyNameToken = "SS2UCYBORG_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("cyborgicon"),
                bodyColor = new Color32(138, 183, 168, 255),
                crosshair = crosshair,
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "SS2UCYBORG_SUBTITLE",
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

            cyborgPrefab.AddComponent<CyborgTeleportTracker>();
            cyborgPrefab.AddComponent<CyborgEnergyComponent>();

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

            EntityStateMachine specialStateMachine = cyborgPrefab.AddComponent<EntityStateMachine>();
            specialStateMachine.customName = "Special";
            specialStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            specialStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            nsm.stateMachines = nsm.stateMachines.Append(specialStateMachine).ToArray();

            ModelLocator ml = cyborgPrefab.GetComponent<ModelLocator>();
            ChildLocator cl = ml.modelTransform.gameObject.GetComponent<ChildLocator>();
            PrefabCore.SetupHitbox(ml.modelTransform.gameObject, cl.FindChild("RamHitbox"), "RamHitbox");

            return cyborgPrefab;
        }
    }
}
