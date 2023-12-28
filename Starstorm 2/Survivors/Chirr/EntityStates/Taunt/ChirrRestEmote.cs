using EntityStates.SS2UStates.Common.Emotes;

namespace EntityStates.SS2UStates.Chirr.Taunt
{
    public class ChirrRestEmote : RestEmote
    {
        public override void SetParams()
        {
            this.animDuration = 33f;
            this.duration = 0f;
        }
    }
}
