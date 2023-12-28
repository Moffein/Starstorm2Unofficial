using EntityStates.SS2UStates.Common.Emotes;
using Starstorm2Unofficial.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStates.SS2UStates.Nemmando.Taunt
{
    public class NemmandoTauntEmote : TauntEmote
    {
        public override void SetParams()
        {
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
        }
    }
}
