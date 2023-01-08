using EntityStates;
using EntityStates.Captain.Weapon;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nucleator
{
    class ChargeQuarantine : NucleatorSkillStateBase
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Gesture, Override", "SecondaryCharge", "Secondary.playbackRate", 0.9f * this.maxChargeTime);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.maxChargeTime || !base.inputBank || !base.inputBank.skill2.down) && base.isAuthority)
            {
                FireQuarantine fireQuarantine = new FireQuarantine();
                fireQuarantine.charge = this.charge;

                this.outer.SetNextState(fireQuarantine);
                return;
            }
        }



        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

