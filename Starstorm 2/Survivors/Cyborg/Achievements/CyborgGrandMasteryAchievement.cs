using RoR2;
using RoR2.Achievements;
using Starstorm2.Modules.Achievements;
using UnityEngine;
namespace Starstorm2.Survivors.Cyborg.Achievements
{
    [RegisterAchievement("SS2UCyborgClearGameTyphoon", "Skins.SS2UCyborg.GrandMastery", null, null)]
    public class CyborgGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return CyborgCore.bodyIndex;
        }
	}
}
