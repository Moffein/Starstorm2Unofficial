using EntityStates;
using Starstorm2.Modules;

namespace Starstorm2.Cores.States
{
    public class BaseCustomMainState : GenericCharacterMain
    {
        protected CustomEffectComponent effectComponent;

        public override void OnEnter()
        {
            base.OnEnter();
            this.effectComponent = base.GetComponent<CustomEffectComponent>();
        }

        protected bool hasSheath
        {
            get
            {
                return this.effectComponent.hasSheath;
            }
        }
    }
}