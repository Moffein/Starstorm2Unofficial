using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2Unofficial.Survivors.Executioner.Achievements
{
    [RegisterAchievement("SS2UExecutionerClearGameMonsoon", "Skins.SS2UExecutioner.Mastery", null, 10, null)]
    public class ExecutionerMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return ExecutionerCore.bodyIndex;
        }
    }
}
