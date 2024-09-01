using RoR2;
using RoR2.Achievements;
using Starstorm2Unofficial.Modules.Achievements;
using UnityEngine;
namespace Starstorm2Unofficial.Survivors.Executioner.Achievements
{
    [RegisterAchievement("SS2UExecutionerClearGameTyphoon", "Skins.SS2UExecutioner.GrandMastery", null, 15, null)]
    public class ExecutionerGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return ExecutionerCore.bodyIndex;
        }
	}
}
