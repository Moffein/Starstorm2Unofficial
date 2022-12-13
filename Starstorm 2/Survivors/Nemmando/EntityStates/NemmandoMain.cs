using EntityStates;
using EntityStates.Starstorm2States.Common;
using UnityEngine;

namespace EntityStates.Starstorm2States.Nemmando
{
    public class NemmandoMain : BaseCustomMainState
    {
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Starstorm2.Modules.Config.restKeybind))
                {
                    this.outer.SetInterruptState(new Common.Emotes.RestEmote(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Starstorm2.Modules.Config.tauntKeybind))
                {
                    this.outer.SetInterruptState(new Common.Emotes.TauntEmote(), InterruptPriority.Any);
                    return;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                if (base.hasSheath) this.animator.SetBool("isAttacking", !base.characterBody.outOfCombat);
                else this.animator.SetBool("isAttacking", true);
                //this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));
            }
        }
    }
}