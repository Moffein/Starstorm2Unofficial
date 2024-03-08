using UnityEngine;
using RoR2;
using System;

namespace EntityStates.SS2UStates.Nucleator.Primary
{
    public class ChargeIrradiate : BaseChargeState
    {
        private uint chargePlayID;
        public override void OnEnter()
        {
            base.OnEnter();
            //base.PlayCrossfade("Gesture, Override", "PrimaryCharge", "Primary.playbackRate", 0.8f * base.duration, 0.2f);
            this.chargePlayID = Util.PlaySound("SS2UNucleatorChargePrimary", this.gameObject);
        }

        public override void OnExit()
        {
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            AkSoundEngine.StopPlayingID(this.chargePlayID);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(base.GetAimRay(), 2f, false);
        }

        protected override bool GetInputPressed()
        {
            return base.inputBank && base.inputBank.skill1.down;
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new FireIrradiate() { charge = this.chargeFraction });
        }
        protected override void SetNextStateOvercharge()
        {
            this.outer.SetNextState(new FireIrradiateOvercharge() { charge = this.chargeFraction });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
