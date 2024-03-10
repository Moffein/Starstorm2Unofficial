using RoR2;

namespace EntityStates.SS2UStates.Nucleator.Secondary
{
    public class ChargeSecondary : BaseChargeState
    {
        private uint chargePlayID;
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayCrossfade("Gesture, Override", "SecondaryCharge", "Secondary.playbackRate", base.duration, 0.1f);
            this.chargePlayID = Util.PlaySound("SS2UNucleatorChargePrimary", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(base.GetAimRay(), 2f, false);
        }

        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(this.chargePlayID);
            base.PlayAnimation("Gesture, Override", "BufferEmpty");
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new FireSecondary() { charge = this.chargeFraction });
        }

        protected override void SetNextStateOvercharge()
        {
            this.outer.SetNextState(new FireSecondaryOvercharge() { charge = this.chargeFraction });
        }

        protected override bool GetInputPressed()
        {
            //Manually handle AIs
            if (base.characterBody && !base.characterBody.isPlayerControlled)
            {
                return true;
            }
            return base.inputBank && base.inputBank.skill2.down;
        }
    }
}
