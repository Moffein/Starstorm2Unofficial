using RoR2;
using Starstorm2.Modules.Achievements;
namespace Starstorm2.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoClearGameTyphoon", "Skins.SS2UNemmando.GrandMastery", null, null)]
    public class NemmandoGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Survivors.Nemmando.NemmandoCore.bodyIndex;
        }
	}
}
