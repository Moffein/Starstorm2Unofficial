using EntityStates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStates.SS2UStates.Nucleator
{
    public class NucleatorMain : GenericCharacterMain
    {

        public override void Update()
        {
            base.Update();

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Starstorm2Unofficial.Modules.Config.GetKeyPressed(Starstorm2Unofficial.Modules.Config.RestKeybind))
                {
                    this.outer.SetInterruptState(new EntityStates.SS2UStates.Common.Emotes.RestEmote(), InterruptPriority.Any);
                    return;
                }
            }
        }
    }
}
