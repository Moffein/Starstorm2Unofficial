using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Cores.Unlockables.Achievements.Executioner
{
    [RegisterAchievement("SS2UExecutionerClearGameTyphoon", "Skins.SS2UExecutioner.GrandMastery", null, null)]
    public class ExecutionerGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Modules.Survivors.Executioner.bodyIndex;
        }
	}
}
