using EntityStates;
using EntityStates.SS2UStates.Common;
using EntityStates.SS2UStates.Nemmando.Taunt;
using UnityEngine;

namespace EntityStates.SS2UStates.Nemmando
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
                if (Starstorm2Unofficial.Modules.Config.GetKeyPressed(Starstorm2Unofficial.Modules.Config.RestKeybind))
                {
                    this.outer.SetInterruptState(new NemmandoRestEmote(), InterruptPriority.Any);
                    return;
                }
                else if (Starstorm2Unofficial.Modules.Config.GetKeyPressed(Starstorm2Unofficial.Modules.Config.TauntKeybind))
                {
                    this.outer.SetInterruptState(new NemmandoTauntEmote(), InterruptPriority.Any);
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