using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Projectile;
using R2API;
using Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Components.Projectiles;
using static R2API.DamageAPI;
using EntityStates.SS2UStates.Nucleator.Primary;

namespace Starstorm2Unofficial.Survivors.Nucleator
{
    //Projectile setup ends up cluttering the Survivor Core so I'd rather just do it in a separate place.
    internal static class NucleatorProjectiles
    {
        public static GameObject BuildPrimary()
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryProjectile", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            projectilePrefab.transform.localScale *= 1.5f;

            projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>(); //Does nothing until SpecialBuffComponent does its thing.
            projectilePrefab.AddComponent<ProjectileCheckSpecialBuffComponent>();

            GameObject projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion();


            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = projectileGhost;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 60f;
            ps.lifetime = 10f;

            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.impactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion();
            pie.blastRadius = 6f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.lifetime = ps.lifetime - 0.1f;

            PrimaryProjectileComponentSimple expand = projectilePrefab.AddComponent<PrimaryProjectileComponentSimple>();
            expand.endSizeMultiplier = 2f;
            expand.startDelay = 0.2f;
            expand.endSizeTime = 0.3f;
            expand.baseSpeed = FireIrradiate.minProjectileSpeed;

            Rigidbody rb = projectilePrefab.GetComponent<Rigidbody>();
            rb.useGravity = true;

            AntiGravityForce agf = projectilePrefab.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.5f;
            agf.rb = rb;

            return projectilePrefab;
        }

        public static GameObject BuildPrimaryOvercharge()
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryOverchargeProjectile", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            projectilePrefab.transform.localScale *= 3f;

            projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>(); //Does nothing until SpecialBuffComponent does its thing.
            projectilePrefab.AddComponent<ProjectileCheckSpecialBuffComponent>();

            GameObject projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereGhost.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryOverchargeGhost",false);

            //Copied from Cyborg
            ParticleSystem[] particles = projectileGhost.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                switch (i)
                {
                    case 0: //fire
                        particles[i].startColor = Color.white;
                        break;
                    case 1: //beams
                        particles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
                        break;
                    case 2: //lightning
                        particles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
                        break;
                }
            }

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = projectileGhost;
            pc.allowPrediction = false;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 20f;
            ps.lifetime = 2.1f;

            //"RoR2/Base/Loader/OmniImpactVFXLoaderLightning.prefab"
            GameObject impactEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderGroundSlam.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryOverchargeImpactEffect", false);
            VFXAttributes vfxAttributes = impactEffectPrefab.GetComponent<VFXAttributes>();
            if (vfxAttributes)
            {
                vfxAttributes.vfxPriority = VFXAttributes.VFXPriority.Always;
            }
            Modules.Assets.AddEffect(impactEffectPrefab, "Play_mage_m2_impact");

            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.impactEffect = impactEffectPrefab;
            pie.blastRadius = 10f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;
            pie.lifetime = ps.lifetime - 0.1f;
            pie.destroyOnWorld = false;

            PrimaryProjectileComponentSimple expand = projectilePrefab.AddComponent<PrimaryProjectileComponentSimple>();
            expand.endSizeMultiplier = 2f;
            expand.startDelay = 0.2f;
            expand.endSizeTime = 1.5f;
            expand.baseSpeed = FireIrradiateOvercharge.minProjectileSpeed;

            /*Rigidbody rb = projectilePrefab.GetComponent<Rigidbody>();
            rb.useGravity = true;

            AntiGravityForce agf = projectilePrefab.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.25f;
            agf.rb = rb;*/

            return projectilePrefab;
        }
    }
}
