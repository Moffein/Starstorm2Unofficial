using RoR2;
using RoR2.Achievements;

namespace Starstorm2Unofficial.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoClearGameMonsoon", "Skins.SS2UNemmando.Mastery", null, 10, null)]
    public class NemmandoMasteryAchievement : BasePerSurvivorClearGameMonsoonAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Survivors.Nemmando.NemmandoCore.bodyIndex;
        }
    }
}
