using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class ChargeLeap : BaseChargeState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("FullBody, Override", "UtilityCharge", "Utility.playbackRate", base.duration);
            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            }
        }
        public override void OnExit()
        {
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }
            base.OnExit();
        }

        protected override void SetNextState()
        {
            Vector3 direction = base.GetAimRay().direction;
            if (base.characterDirection)
            {
                Vector3 initialDirection = direction;
                initialDirection.y = 0f;
                initialDirection.Normalize();
                base.characterDirection.forward = initialDirection;
            }

            this.outer.SetNextState(new FireLeap() { charge = this.chargeFraction });
        }

        protected override void SetNextStateOvercharge()
        {
            Vector3 direction = base.GetAimRay().direction;
            if (base.characterDirection)
            {
                Vector3 initialDirection = direction;
                initialDirection.y = 0f;
                initialDirection.Normalize();
                base.characterDirection.forward = initialDirection;
            }

            this.outer.SetNextState(new FireLeap() { charge = this.chargeFraction });
        }

        protected override bool GetInputPressed()
        {
            return base.inputBank && base.inputBank.skill3.down;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
