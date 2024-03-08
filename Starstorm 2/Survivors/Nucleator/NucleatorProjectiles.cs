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

            DamageAPI.ModdedDamageTypeHolderComponent mdc = projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypeCore.ModdedDamageTypes.NucleatorCanApplyRadiation);

            GameObject projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion();
            //projectileGhost.AddComponent<GhostScaleOverTime>();

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

            PrimaryProjectileComponentSimple expand = projectilePrefab.AddComponent<PrimaryProjectileComponentSimple>();
            expand.endSizeMultiplier = 2f;
            expand.startDelay = 0.1f;
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

            projectilePrefab.transform.localScale *= 4f;

            DamageAPI.ModdedDamageTypeHolderComponent mdc = projectilePrefab.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(DamageTypeCore.ModdedDamageTypes.NucleatorCanApplyRadiation);

            GameObject projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereGhost.prefab").WaitForCompletion();
            //projectileGhost.AddComponent<GhostScaleOverTime>();

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = projectileGhost;

            ProjectileSimple ps = projectilePrefab.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 60f;
            ps.lifetime = 10f;

            GameObject impactEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/OmniImpactVFXLightningMage.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryOverchargeImpactEffect", false);

            //Debug.Log("Modifying Nucleator overcharge primary particle color");
            //ParticleSystem[] particles = impactEffectPrefab.GetComponentsInChildren<ParticleSystem>();
            /*for (int i = 0; i < particles.Length; i++)
            {
                Debug.Log(particles[i].name + ": " + particles[i].startColor);
                particles[i].startColor = new Color(244f / 255f, 243f / 255f, 183f / 255f);
            }*/
            /*
                [Info   : Unity Log] Flash, Blue: RGBA(0.000, 0.523, 1.000, 1.000)
                [Info   : Unity Log] Matrix, Dynamic: RGBA(0.279, 0.000, 0.279, 0.000)
                [Info   : Unity Log] Matrix, Directional: RGBA(0.000, 0.000, 0.000, 0.000)
                [Info   : Unity Log] Flash, Directional: RGBA(0.000, 0.000, 0.000, 0.000)
                [Info   : Unity Log] Flash, Distortion: RGBA(1.000, 1.000, 1.000, 1.000)
                [Info   : Unity Log] Matrix, Billboard: RGBA(0.000, 0.523, 1.000, 1.000)
                [Info   : Unity Log] Sphere, Expanding: RGBA(0.190, 0.653, 0.708, 1.000)
             */

            Modules.Assets.AddEffect(impactEffectPrefab);

            ProjectileImpactExplosion pie = projectilePrefab.GetComponent<ProjectileImpactExplosion>();
            pie.impactEffect = impactEffectPrefab;
            pie.blastRadius = 30f;
            pie.falloffModel = BlastAttack.FalloffModel.None;
            pie.blastAttackerFiltering = AttackerFiltering.NeverHitSelf;

            /*Rigidbody rb = projectilePrefab.GetComponent<Rigidbody>();
            rb.useGravity = true;

            AntiGravityForce agf = projectilePrefab.AddComponent<AntiGravityForce>();
            agf.antiGravityCoefficient = 0.25f;
            agf.rb = rb;*/

            //projectilePrefab.AddComponent<PrimaryProjectileComponent>();

            return projectilePrefab;
        }
    }
}
