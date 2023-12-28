using EntityStates.SS2UStates.Common.Emotes;
using RoR2;
using Starstorm2Unofficial.Survivors.Nemmando;

namespace EntityStates.SS2UStates.Nemmando.Taunt
{
    public class NemmandoRestEmote : RestEmote
    {
        public override void SetParams()
        {
            this.animDuration = 8f;

            if (base.characterBody)
            {
                SkinDef currentSkin = SkinCatalog.GetBodySkinDef(base.characterBody.bodyIndex, (int)base.characterBody.skinIndex);
                if (currentSkin && currentSkin == NemmandoCore.SkinDefs.Vergil)
                {
                    this.animString = "SitChair";
                    this.animDuration = 0.6f;
                }
            }
        }
    }
}
