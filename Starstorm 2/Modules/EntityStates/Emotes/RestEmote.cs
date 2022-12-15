using Starstorm2.Modules;

namespace EntityStates.SS2UStates.Common.Emotes
{
    public class RestEmote : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "Rest";
            this.animDuration = 8f;
            this.normalizeModel = true;

            CustomEffectComponent effectComponent = this.GetComponent<CustomEffectComponent>();
            if (effectComponent)
            {
                if (effectComponent.hasSheath && effectComponent.chargeAttackEffect == Starstorm2.Modules.Assets.nemChargedSlashStartFXBlue)
                {
                    this.animString = "SitChair";
                    this.animDuration = 0.6f;
                }
            }

            base.OnEnter();
        }
    }
}