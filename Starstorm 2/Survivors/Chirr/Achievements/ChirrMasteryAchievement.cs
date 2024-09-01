using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Chirr.Achievements
{
    [RegisterAchievement("SS2UChirrClearGameMonsoon", "Skins.SS2UChirr.Mastery", null, 10, null)]
    public class ChirrMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return ChirrCore.bodyIndex;
        }
    }
}
