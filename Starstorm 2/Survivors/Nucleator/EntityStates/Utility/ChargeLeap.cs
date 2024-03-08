using System;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class ChargeLeap : BaseChargeState
    {
        protected override void SetNextState()
        {
            this.outer.SetNextState(new FireLeap() { charge = this.chargeFraction });
        }

        protected override void SetNextStateOvercharge()
        {
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
