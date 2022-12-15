using EntityStates.SS2UStates.Common;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class Headbutt : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            this.damageType = DamageType.Generic;
            this.hitHopVelocity = 0f;
            this.scaleHitHopWithAttackSpeed = false;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "";
            this.swingSoundString = "Play_acrid_m1_bigSlash";
            this.hitboxName = "HeadbuttHitbox";
            this.damageCoefficient = 6f;
            this.procCoefficient = 1f;
            this.baseDuration = 0.65f;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.8f;
            this.pushForce = 0f;

            base.OnEnter();

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
        }
        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Additive", "Secondary", "Secondary.playbackRate", this.baseDuration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
