using Starstorm2Unofficial.Modules;

namespace EntityStates.SS2UStates.Common.Emotes
{
    public class TauntEmote : BaseEmote
    {
        //this.duration = 4f;   //old default: 4s
        public override void OnEnter()
        {
            this.animString = "Taunt";
            base.OnEnter();
        }

        public override void SetParams()
        {
            this.duration = 4f;
        }
    }
}