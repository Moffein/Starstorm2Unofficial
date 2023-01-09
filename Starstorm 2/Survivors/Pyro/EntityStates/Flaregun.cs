using RoR2;
using RoR2.Projectile;
using Starstorm2Unofficial.Survivors.Pyro.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Pyro
{
    public class Flaregun : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Flaregun.baseDuration / this.attackSpeedStat;

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            Util.PlaySound("Play_bandit_M2_shot", base.gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "Muzzle", false);

            costPerExplosion = SetCostPerExplosion();
            explosionCount = 1;
            HeatController hc = base.GetComponent<HeatController>();
            if (hc)
            {
                float heatPercent = hc.GetHeatPercent();
                explosionCount = SetExplosionCount(heatPercent);

                int stocks = 1;
                if (base.skillLocator && base.skillLocator.special) stocks = base.skillLocator.special.maxStock;

                hc.ConsumeHeat(explosionCount * costPerExplosion, stocks);
            }

            if (base.isAuthority)
            {
                //Do a hacky thing where force is converted to explosion count through FlareProjectileController
                ProjectileManager.instance.FireProjectile(GetProjectilePrefab(), aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * Flaregun.damageCoefficient, explosionCount, base.RollCrit(), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public virtual GameObject GetProjectilePrefab()
        {
            return projectilePrefab;
        }

        public virtual int SetExplosionCount(float heatPercent)
        {
            int totalExplosions = Mathf.FloorToInt(Mathf.Lerp(0, maxExplosions, heatPercent));
            return (Mathf.Max(1, totalExplosions));
        }

        public virtual float SetCostPerExplosion()
        {
            return 1f / Flaregun.maxExplosions;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject muzzleflashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 6f;
        public static float baseDuration = 0.5f;
        public static int maxExplosions = 8;

        public int explosionCount;
        private float duration;
        private float costPerExplosion;
    }
}
