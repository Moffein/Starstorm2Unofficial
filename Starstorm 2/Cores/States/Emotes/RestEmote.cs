using Starstorm2.Modules;

namespace Starstorm2.Cores.States.Emotes
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
                if (effectComponent.hasSheath && effectComponent.chargeAttackEffect == Modules.Assets.nemChargedSlashStartFXBlue)
                {
                    this.animString = "SitChair";
                    this.animDuration = 0.6f;
                }
            }

            base.OnEnter();
        }
    }
}