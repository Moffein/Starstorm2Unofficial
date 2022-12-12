using EntityStates;
using Starstorm2.Cores;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.States.Nucleator
{
    class ApplyRadionuclideSurge : BaseSkillState
    {
        private float stateDuration = 6F;
        private float buffDuration = 6f;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active) base.characterBody.AddTimedBuff(Modules.Buffs.nucleatorSpecialBuff, buffDuration);
            this.animator = base.GetModelAnimator();

            if (this.animator) this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Additive"), 1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.animator) this.animator.SetLayerWeight(this.animator.GetLayerIndex("Body, Additive"), 0f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.stateDuration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}