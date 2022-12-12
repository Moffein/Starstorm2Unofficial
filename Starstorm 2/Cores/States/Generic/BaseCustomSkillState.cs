using EntityStates;
using Starstorm2.Modules;

namespace Starstorm2.Cores.States
{
    public class BaseCustomSkillState : BaseSkillState
    {
        protected CustomEffectComponent effectComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.outOfCombatStopwatch = 0f;
            this.effectComponent = base.GetComponent<CustomEffectComponent>();
        }

        protected void PlaySwingEffect(string muzzleString)
        {
            if (this.effectComponent) this.effectComponent.PlaySwingEffect(muzzleString);
        }

        protected void PlayShootSound()
        {
            if (this.effectComponent) this.effectComponent.PlayShootSound(false);
        }

        protected void PlayScaledShootSound()
        {
            if (this.effectComponent) this.effectComponent.PlayShootSound(true);
        }
    }
}