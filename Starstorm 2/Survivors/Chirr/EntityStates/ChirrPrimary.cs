using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrPrimary : BaseState
    {
        public static float damageCoefficient = 0.9f;
        public static float force = 100f;
        public static float baseDuration = 0.5f;
        public static float baseShotDuration = 0.1f;
        public static string attackSoundString = "SS2UChirrPrimary";
        public static int baseShotCount = 3;
        public static float spreadBloom = 0.4f;
        public static float recoil = 1f;

        public static GameObject projectilePrefab;
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/MuzzleflashCroco.prefab").WaitForCompletion();

        private int shotCount;
        private float duration;
        private float shotDuration;
        private float shotStopwatch;
        private bool crit;

        public override void OnEnter()
        {
            base.OnEnter();

            crit = base.RollCrit();
            shotCount = 0;
            shotStopwatch = 0f;
            duration = ChirrPrimary.baseDuration / this.attackSpeedStat;
            shotDuration = ChirrPrimary.baseShotDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleWingL", false);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleWingR", false);

            base.PlayCrossfade("Gesture, Override", "Primary", "Primary.playbackRate", this.duration, 0.1f);
            base.PlayCrossfade("Gesture, Additive", "Primary", "Primary.playbackRate", this.duration, 0.1f);

            FireBullet();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool isAirborne = base.characterMotor && !base.characterMotor.isGrounded;
            bool isSprinting = base.characterBody && base.characterBody.isSprinting;
            if (isAirborne && isSprinting && base.characterDirection)
            {
                base.characterDirection.forward = base.GetAimRay().direction;
            }

            if (shotCount < ChirrPrimary.baseShotCount)
            {
                shotStopwatch += Time.fixedDeltaTime;
                if (shotStopwatch >= shotDuration)
                {
                    FireBullet();
                }
            }
            else
            {
                if (base.isAuthority && base.fixedAge >= this.duration && shotCount >= ChirrPrimary.baseShotCount)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                base.characterBody.SetSpreadBloom(0f, false);
            }
            base.OnExit();
        }

        private void FireBullet()
        {
            shotStopwatch = 0f;
            shotCount++;
            Util.PlaySound(ChirrPrimary.attackSoundString, base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                ProjectileManager.instance.FireProjectile(projectilePrefab,
                    aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction),
                    base.gameObject, base.damageStat * damageCoefficient,
                    0f,
                    crit,
                    DamageColorIndex.Default, null, -1f);
            }
            base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            if (base.characterBody) base.characterBody.AddSpreadBloom(spreadBloom); //Spread is cosmetic. Skill is always perfectly accurate.
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
