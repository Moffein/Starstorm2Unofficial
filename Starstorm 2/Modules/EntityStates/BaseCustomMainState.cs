using EntityStates;
using Starstorm2.Modules;

namespace EntityStates.SS2UStates.Common
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