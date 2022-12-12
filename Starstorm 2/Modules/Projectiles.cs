using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Starstorm2.Modules
{
    internal static class Projectiles
    {
        internal static GameObject swordBeam;
        internal static GameObject laserTracer;

        internal static void Initialize()
        {
            swordBeam = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FMJ"), "NemmandoSwordBeam", true);
            swordBeam.transform.localScale = new Vector3(4.5f, 2.5f, 2.5f);

            GameObject swordBeamGhost = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EvisProjectile").GetComponent<ProjectileController>().ghostPrefab, "NemmandoSwordBeamGhost", false);
            foreach (ParticleSystemRenderer i in swordBeamGhost.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (i)
                {
                    Material mat = UnityEngine.Object.Instantiate<Material>(i.material);
                    mat.SetColor("_TintColor", Color.red);
                    i.material = mat;
                }
            }
            foreach (Light i in swordBeamGhost.GetComponentsInChildren<Light>())
            {
                if (i)
                {
                    i.color = Color.red;
                }
            }

            swordBeam.GetComponent<ProjectileController>().ghostPrefab = swordBeamGhost;
            swordBeam.GetComponent<ProjectileDamage>().damageType = DamageType.BlightOnHit;

            Starstorm.Destroy(swordBeam.transform.Find("SweetSpotBehavior").gameObject);

            Modules.Prefabs.projectilePrefabs.Add(swordBeam);

            laserTracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("NemmandoLaserTracer", true);

            foreach (LineRenderer i in laserTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    Material material = UnityEngine.Object.Instantiate<Material>(i.material);
                    material.SetColor("_TintColor", Color.red);
                    i.material = material;
                    i.startColor = new Color(0.8f, 0.19f, 0.19f);
                    i.endColor = new Color(0.8f, 0.19f, 0.19f);
                }
            }

            Modules.Assets.AddEffect(laserTracer);
        }
    }
}
