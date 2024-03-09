using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;

namespace EntityStates.SS2UStates.Nucleator.Secondary
{
    public class FireSecondary : BaseState
    {
        public static float baseDuration = 0.4f;
        public static GameObject muzzleflashEffectPrefab;

        public float charge;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;// this.attackSpeedStat;

            if (muzzleflashEffectPrefab)
            {
                //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleR", false);
                //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleL", false);

                Transform muzzleTransform = null;
                ChildLocator cl = base.GetModelChildLocator();
                if (cl) muzzleTransform = cl.FindChild("MuzzleCenter");

                if (muzzleTransform)
                {
                    Ray aimRay = base.GetAimRay();
                    EffectManager.SpawnEffect(muzzleflashEffectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = Quaternion.LookRotation(aimRay.direction) * Quaternion.Euler(90f, 0f, 0f),
                        origin = muzzleTransform.position + aimRay.direction * 3.5f
                    }, false);
                }
            }

            base.PlayAnimation("Gesture, Override", "SecondaryRelease", "Secondary.playbackRate", duration * 2f);
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
    }
}
