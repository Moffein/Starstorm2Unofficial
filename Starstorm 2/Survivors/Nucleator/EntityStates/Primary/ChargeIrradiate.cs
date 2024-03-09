using UnityEngine;
using RoR2;
using System;

namespace EntityStates.SS2UStates.Nucleator.Primary
{
    public class ChargeIrradiate : BaseChargeState, RoR2.Skills.SteppedSkillDef.IStepSetter
    {
        private bool playedChargeAnim;

        private int step;
        private uint chargePlayID;
        public override void OnEnter()
        {
            base.OnEnter();
            playedChargeAnim = false;
            this.chargePlayID = Util.PlaySound("SS2UNucleatorChargePrimary", this.gameObject);
        }

        public override void OnExit()
        {
            if (playedChargeAnim) base.PlayAnimation("Gesture, Override", "BufferEmpty");
            AkSoundEngine.StopPlayingID(this.chargePlayID);
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(base.GetAimRay(), 2f, false);
        }

        protected override void OnOverchargeStart()
        {
            playedChargeAnim = true;
            base.PlayCrossfade("Gesture, Override", "PrimaryCharge", "Primary.playbackRate", base.duration * (1f - overchargeFraction), 0.2f);
        }

        protected override bool GetInputPressed()
        {
            //Manually handle AIs
            if (base.characterBody && !base.characterBody.isPlayerControlled)
            {
                return true;
            }
            return base.inputBank && base.inputBank.skill1.down;
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new FireIrradiate() { charge = this.chargeFraction, step = this.step });
        }
        protected override void SetNextStateOvercharge()
        {
            this.outer.SetNextState(new FireIrradiateOvercharge() { charge = this.chargeFraction });
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
