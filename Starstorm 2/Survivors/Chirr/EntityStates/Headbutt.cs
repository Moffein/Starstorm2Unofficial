using EntityStates.SS2UStates.Common;
using RoR2;
using UnityEngine;

namespace EntityStates.SS2UStates.Chirr
{
    public class Headbutt : BaseState
    {
        public static float baseDuration = 0.65f;    //Don't scale with attack speed due to the lunge
        public static float damageCoefficient = 6f;

        public static float momentumStartTime = 0.3f * Headbutt.baseDuration;
        public static float momentumEndTime = 0.5f * Headbutt.baseDuration;

        private OverlapAttack attack;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("Play_acrid_m1_bigSlash", base.gameObject);
            base.PlayAnimation("Gesture, Additive", "Secondary", "Secondary.playbackRate", Headbutt.baseDuration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (base.fixedAge >= Headbutt.baseDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
