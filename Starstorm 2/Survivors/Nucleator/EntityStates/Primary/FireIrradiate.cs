using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Nucleator.Primary
{
    public class FireIrradiate : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;// this.attackSpeedStat;
            this.damageStat *= this.attackSpeedStat;

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);

            Util.PlaySound("SS2UNucleatorSkill1", base.gameObject);

            bool isRight = step == 0;
            string animString = isRight ? "PrimaryLightR" : "PrimaryLightL";
            base.PlayAnimation("Gesture, Override", animString);

            string muzzleString = isRight ? "MuzzleR" : "MuzzleL";
            if (muzzleflashEffectPrefab) EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, muzzleString, false);

            if (base.isAuthority)
            {
                float chargeScaled = charge / BaseChargeState.overchargeFraction;

                float damageCoefficient = Mathf.Lerp(minDamageCoefficient, maxDamageCoefficient, chargeScaled);
                float force = Mathf.Lerp(minForce, maxForce, chargeScaled);
                ProjectileManager.instance.FireProjectile(projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * damageCoefficient, force, base.RollCrit(), DamageColorIndex.Default, null, Mathf.Lerp(minProjectileSpeed, maxProjectileSpeed, chargeScaled));
            }
            float recoil = 8f;
            base.AddRecoil(-0.5f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();
        public static GameObject projectilePrefab;

        public static float minDamageCoefficient = 2f;
        public static float maxDamageCoefficient = 7.2f;

        public static float minForce = 1000f;
        public static float maxForce = 2000f;

        public static float minProjectileSpeed = 60f;
        public static float maxProjectileSpeed = 90f;

        public static float baseDuration = 0.4f;

        public int step;
        public float charge = 0f;
        private float duration;
    }
}
