using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class ShockCore : BaseState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ShockCore.baseDuration / this.attackSpeedStat;

            Ray aimRay = base.GetAimRay();
            base.StartAimMode(aimRay, 2f, false);
            base.PlayAnimation("Gesture, Override", "FireM2", "FireArrow.playbackRate", this.duration);
            Util.PlaySound("Play_voidman_m2_shoot_fullCharge", base.gameObject);
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "Lowerarm.L_end", false);

            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(ShockCore.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageStat * ShockCore.damageCoefficient, 0f, base.RollCrit(), DamageColorIndex.Default, null, -1f);
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
            return InterruptPriority.PrioritySkill;
        }

        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainChargeTazer.prefab").WaitForCompletion();
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 3f;
        public static float baseDuration = 0.5f;

        private float duration;
    }
}
