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

            Util.PlaySound("Play_loader_shift_activate", base.gameObject);
            base.PlayAnimation("FullBody, Override", "UtilityCharge", "Utility.playbackRate", base.duration);

            if (base.characterBody)
            {
                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(base.GetAimRay(), 2f, false);
            //Don't need this if we're running this in the body state machine.
            if (base.isAuthority)
            {
                base.characterMotor.moveDirection = Vector3.zero;
                base.characterMotor.jumpCount = base.characterBody.maxJumpCount;
            }
        }

        public override void OnExit()
        {
            Util.PlaySound("Play_loader_shift_release", base.gameObject);
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }
            base.characterMotor.jumpCount = 0;
            base.OnExit();
        }

        protected override void SetNextState()
        {
            EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (bodyMachine)
            {
                bodyMachine.SetNextState(new FireLeap() { charge = this.chargeFraction });
            }
            this.outer.SetNextStateToMain();
        }

        protected override void SetNextStateOvercharge()
        {
            EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (bodyMachine)
            {
                bodyMachine.SetNextState(new FireLeapOvercharge() { charge = this.chargeFraction });
            }
            this.outer.SetNextStateToMain();
        }

        protected override bool GetInputPressed()
        {
            //Manually handle AIs
            if (base.characterBody && !base.characterBody.isPlayerControlled)
            {
                return true;
            }
            return base.inputBank && base.inputBank.skill3.down;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
