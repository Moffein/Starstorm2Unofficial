using BepInEx.Configuration;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class ChargeLeap : BaseChargeState
    {
        public static ConfigEntry<bool> stationaryLeap;

        private bool isStationary = false;
        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("Play_loader_shift_activate", base.gameObject);

            if (stationaryLeap.Value)
            {
                isStationary = true;
                base.PlayAnimation("FullBody, Override", "UtilityCharge", "Utility.playbackRate", base.duration);
            }
            else
            {
                base.PlayAnimation("Gesture, Override", "UtilityChargeWalk", "Utility.playbackRate", base.duration);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.StartAimMode(base.GetAimRay(), 2f, false);
            //Don't need this if we're running this in the body state machine.
            if (base.isAuthority && isStationary)
            {
                base.characterMotor.moveDirection = Vector3.zero;
                base.characterMotor.jumpCount = base.characterBody.maxJumpCount;
            }
        }

        public override void OnExit()
        {
            Util.PlaySound("Play_loader_shift_release", base.gameObject);

            if (isStationary)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                base.characterMotor.jumpCount = 0;
            }
            else
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
            }

            base.OnExit();
        }

        protected override void SetNextState()
        {
            EntityStateMachine bodyMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Body");
            if (bodyMachine)
            {
                bodyMachine.SetNextState(new FireLeap() {
                    charge = this.chargeFraction,
                    animString = isStationary ? "UtilityRelease" : "UtilityReleaseWalk"
                });
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
