using RoR2;
using System;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class ChargeLeap : BaseChargeState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            }
        }
        public override void OnExit()
        {
            if (NetworkServer.active && base.characterBody)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }
            base.OnExit();
        }

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
