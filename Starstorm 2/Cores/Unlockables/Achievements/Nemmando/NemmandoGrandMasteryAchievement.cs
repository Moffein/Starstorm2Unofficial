using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Cores.Unlockables.Achievements.Nemmando
{
    [RegisterAchievement("SS2UNemmandoClearGameTyphoon", "Skins.SS2UNemmando.GrandMastery", null, null)]
    public class NemmandoGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Modules.Survivors.Nemmando.bodyIndex;
        }
	}
}
