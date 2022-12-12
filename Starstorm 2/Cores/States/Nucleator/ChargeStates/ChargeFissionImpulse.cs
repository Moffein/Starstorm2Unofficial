using EntityStates;
using EntityStates.Captain.Weapon;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nucleator
{
    class ChargeFissionImpulse : NucleatorSkillStateBase
    {
        private Transform modelTransform;
        private HurtBoxGroup hurtboxGroup;
        private CharacterModel characterModel;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = base.GetModelTransform();
            if (this.modelTransform)
            {
                this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
                this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }

            base.PlayAnimation("FullBody, Override", "UtilityCharge", "Utility.playbackRate", 0.9f * this.maxChargeTime);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.maxChargeTime || !base.inputBank || !base.inputBank.skill3.down) && base.isAuthority)
            {                
                this.outer.SetNextState(new FireFissionImpulse(this.hurtboxGroup, this.characterModel, this.charge));
                return;
            }
        }



        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

