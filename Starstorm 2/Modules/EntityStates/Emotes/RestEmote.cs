using Starstorm2Unofficial.Modules;

namespace EntityStates.SS2UStates.Common.Emotes
{
    public class RestEmote : BaseEmote
    {
        public override void OnEnter()
        {
            this.animString = "Rest";
            this.normalizeModel = true;

            base.OnEnter();
        }

        public override void SetParams()
        {
            this.animDuration = 8f;
        }
    }
}