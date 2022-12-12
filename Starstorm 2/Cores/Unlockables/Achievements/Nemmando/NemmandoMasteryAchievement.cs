using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Cores.Unlockables.Achievements.Nemmando
{
    [RegisterAchievement("SS2UNemmandoClearGameMonsoon", "Skins.SS2UNemmando.Mastery", null, null)]
    public class NemmandoMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Modules.Survivors.Nemmando.bodyIndex;
        }
    }
}
