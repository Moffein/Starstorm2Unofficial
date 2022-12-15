using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Survivors.Cyborg.Achievements
{
    [RegisterAchievement("SS2UCyborgClearGameMonsoon", "Skins.SS2UCyborg.Mastery", null, null)]
    public class CyborgMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return CyborgCore.bodyIndex;
        }
    }
}
