using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EntityStates.SS2UStates.Nucleator.Primary
{
    public class FireIrradiate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);

            Util.PlaySound("SS2UNucleatorSkill1", base.gameObject);
            base.PlayAnimation("Gesture, Override", "PrimaryLight", "Primary.playbackRate", this.duration);

            if (muzzleflashEffectPrefab) EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "Forearm.R", false);

            if (base.isAuthority)
            {
                float chargeScaled = charge / BaseChargeState.overchargeFraction;

                float damageCoefficient = Mathf.Lerp(minDamageCoefficient, maxDamageCoefficient, chargeScaled);
                float force = Mathf.Lerp(minForce, maxForce, chargeScaled);
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, force, base.RollCrit(), DamageColorIndex.Default, null, Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargeScaled));
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

        public static GameObject muzzleflashEffectPrefab;
        public static GameObject projectilePrefab;

        public static float minDamageCoefficient = 2f;
        public static float maxDamageCoefficient = 8f;

        public static float minForce = 1000f;
        public static float maxForce = 2000f;

        public static float minProjectileSpeed = 60f;
        public static float maxProjectileSpeed = 90f;

        public static float baseDuration = 0.4f;

        public float charge = 0f;
        private float duration;
    }
}
