using RoR2;
using RoR2.Projectile;
using Starstorm2Unofficial.Survivors.Pyro.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Pyro
{
    public class SuppressiveFire : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = SuppressiveFire.baseDuration / this.attackSpeedStat;
            heatController = base.GetComponent<HeatController>();

            FireProjectile();
        }

        public void FireProjectile()
        {
            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            Util.PlaySound("Play_mage_m1_shoot", base.gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "Muzzle", false);

            if (heatController)
            {
                int stocks = 1;
                if (base.skillLocator && base.skillLocator.secondary) stocks = base.skillLocator.secondary.maxStock;

                heatController.ConsumeHeat(heatCost, stocks);
            }

            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * SuppressiveFire.damageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);

                if (base.characterMotor && !base.characterMotor.isGrounded)
                {
                    Vector3 selfForceDirection = base.GetAimRay().direction;
                    if (selfForceDirection.y < 0f)
                    {
                        selfForceDirection.x = 0f;
                        selfForceDirection.z = 0f;

                        base.characterMotor.ApplyForce(selfForceDirection * -baseSelfForce, true, false);
                    }
                }
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

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static float baseSelfForce = 600f;
        public static GameObject muzzleflashEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 0.84f;
        public static float baseDuration = 0.1f;
        public static float heatCost = 0.08f;

        private float duration;
        private HeatController heatController;
    }
}
