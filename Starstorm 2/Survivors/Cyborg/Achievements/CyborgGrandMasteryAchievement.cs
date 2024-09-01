using RoR2;
using RoR2.Achievements;
using Starstorm2Unofficial.Modules.Achievements;
using UnityEngine;
namespace Starstorm2Unofficial.Survivors.Cyborg.Achievements
{
    [RegisterAchievement("SS2UCyborgClearGameTyphoon", "Skins.SS2UCyborg.GrandMastery", null, 15, null)]
    public class CyborgGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return CyborgCore.bodyIndex;
        }
	}
}
