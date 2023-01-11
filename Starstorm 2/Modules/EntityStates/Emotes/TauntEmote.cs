using Starstorm2Unofficial.Modules;

namespace EntityStates.SS2UStates.Common.Emotes
{
    public class TauntEmote : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "Taunt";
            this.duration = 4f;

            CustomEffectComponent effectComponent = base.GetComponent<CustomEffectComponent>();
            if (effectComponent)
            {
                if (effectComponent.hasSheath)
                {
                    this.soundString = "SS2USpawnMGR";
                    this.duration = 12f;
                }
            }

            base.OnEnter();
        }
    }
}