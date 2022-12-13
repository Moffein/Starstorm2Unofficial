using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Survivors.Executioner.Achievements
{
    [RegisterAchievement("SS2UExecutionerClearGameMonsoon", "Skins.SS2UExecutioner.Mastery", null, null)]
    public class ExecutionerMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return ExecutionerCore.bodyIndex;
        }
    }
}
