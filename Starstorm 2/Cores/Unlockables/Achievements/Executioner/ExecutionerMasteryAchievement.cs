using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Cores.Unlockables.Achievements.Executioner
{
    [RegisterAchievement("SS2UExecutionerClearGameMonsoon", "Skins.SS2UExecutioner.Mastery", null, null)]
    public class ExecutionerMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Modules.Survivors.Executioner.bodyIndex;
        }
    }
}
