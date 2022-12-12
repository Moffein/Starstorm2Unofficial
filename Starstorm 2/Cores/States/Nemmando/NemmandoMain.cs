using EntityStates;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando
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
                if (Input.GetKeyDown(Modules.Config.restKeybind))
                {
                    this.outer.SetInterruptState(new Emotes.RestEmote(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Modules.Config.tauntKeybind))
                {
                    this.outer.SetInterruptState(new Emotes.TauntEmote(), InterruptPriority.Any);
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