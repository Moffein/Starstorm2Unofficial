using EntityStates.SS2UStates.Common.Emotes;
using Starstorm2Unofficial.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStates.SS2UStates.Nemmando.Taunt
{
    public class NemmandoRestEmote : RestEmote
    {
        public override void SetParams()
        {
            this.animDuration = 8f;
            CustomEffectComponent effectComponent = this.GetComponent<CustomEffectComponent>();
            if (effectComponent)
            {
                if (effectComponent.hasSheath && effectComponent.chargeAttackEffect == Starstorm2Unofficial.Modules.Assets.nemChargedSlashStartFXBlue)
                {
                    this.animString = "SitChair";
                    this.animDuration = 0.6f;
                }
            }
        }
    }
}
