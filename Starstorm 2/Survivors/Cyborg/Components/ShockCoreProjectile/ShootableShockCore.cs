using R2API;
using RoR2;
using RoR2.Projectile;
using Starstorm2.Components.Projectiles;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2.Survivors.Cyborg.Components.ShockCoreProjectile
{
    public class ShootableShockCore : ShootableProjectileComponent
    {
        public static GameObject explosionEffectPrefab;

        public override void OnShootActions(DamageInfo damageInfo)
        {
            base.OnShootActions(damageInfo);

            damageInfo.crit = true;
            damageInfo.procCoefficient = 0f;
            damageInfo.force = Vector3.zero;

            ProjectileDamage pd = base.gameObject.GetComponent<ProjectileDamage>();
            if (pd)
            {
                pd.damage *= 4f;
            }

            DamageAPI.ModdedDamageTypeHolderComponent mdc = base.gameObject.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (!mdc) mdc = base.gameObject.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(this.targetDamageType);

            ProjectileImpactExplosion pie = base.gameObject.GetComponent<ProjectileImpactExplosion>();
            if (pie)
            {
                pie.blastRadius = 14f;
                pie.falloffModel = BlastAttack.FalloffModel.None;
                pie.explosionEffect = ShootableShockCore.explosionEffectPrefab;

                //Why doesn't BanditDynamite need to call this?
                pie.Detonate();
            }
        }
    }
}
