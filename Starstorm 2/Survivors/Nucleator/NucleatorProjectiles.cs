using UnityEngine;
using UnityEngine.AddressableAssets;
using RoR2;
using RoR2.Projectile;
using R2API;

namespace Starstorm2Unofficial.Survivors.Nucleator
{
    //Projectile setup ends up cluttering the Survivor Core so I'd rather just do it in a separate place.
    internal static class NucleatorProjectiles
    {
        public static GameObject BuildPrimary()
        {
            GameObject projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningboltBasic.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryProjectile", true);
            Modules.Prefabs.projectilePrefabs.Add(projectilePrefab);

            GameObject projectileGhost = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleQueenSpitGhost.prefab").WaitForCompletion().InstantiateClone("SS2UNucleatorPrimaryGhost", false);

            ProjectileController pc = projectilePrefab.GetComponent<ProjectileController>();
            pc.ghostPrefab = projectileGhost;

            return projectilePrefab;
        }
    }
}
