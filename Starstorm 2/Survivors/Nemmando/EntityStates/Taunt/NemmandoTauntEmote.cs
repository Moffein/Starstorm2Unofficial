using EntityStates.SS2UStates.Common.Emotes;
using RoR2;
using Starstorm2Unofficial.Survivors.Nemmando;

namespace EntityStates.SS2UStates.Nemmando.Taunt
{
    public class NemmandoTauntEmote : TauntEmote
    {
        public override void SetParams()
        {
            this.duration = 4f;

            if (base.characterBody)
            {
                SkinDef currentSkin = SkinCatalog.GetBodySkinDef(base.characterBody.bodyIndex, (int)base.characterBody.skinIndex);
                if (currentSkin && (currentSkin == NemmandoCore.SkinDefs.Mastery || currentSkin == NemmandoCore.SkinDefs.Vergil))
                {
                    this.soundString = "SS2USpawnMGR";
                    this.duration = 12f;
                }
            }
        }
    }
}
