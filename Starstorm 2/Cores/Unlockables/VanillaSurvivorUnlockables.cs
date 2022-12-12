using R2API;
using RoR2;

namespace Starstorm2.Cores.Unlockables
{
    public static class VanillaSurvivorUnlockables
    {
        public static UnlockableDef commandoGrandMastery;
        public static UnlockableDef toolbotGrandMastery;
        public static UnlockableDef acridGrandMastery;

        public static void RegisterUnlockables()
        {
            // todo: make a base class for mastery achievements and simply inherit from it for each character 

            LanguageAPI.Add("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Commando: Grand Mastery");
            LanguageAPI.Add("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Commando, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add("COMMANDO_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Commando: Grand Mastery");

            LanguageAPI.Add("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "MUL-T: Grand Mastery");
            LanguageAPI.Add("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As MUL-T, beat the game or obliterate on Typhoon.");
            LanguageAPI.Add("TOOLBOT_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME", "MUL-T: Grand Mastery");

            //LanguageAPI.Add("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME", "Acrid: Grand Mastery");
            //LanguageAPI.Add("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC", "As Acrid, beat the game or obliterate on Typhoon.");
            //LanguageAPI.Add("CROCO_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME", "Acrid: Grand Mastery");

            commandoGrandMastery = Modules.Unlockables.AddUnlockable<Achievements.CommandoGrandMasteryAchievement>(true);
            toolbotGrandMastery = Modules.Unlockables.AddUnlockable<Achievements.ToolbotGrandMasteryAchievement>(true);
            //acridGrandMastery = Modules.Unlockables.AddUnlockable<Achievements.CrocoGrandMasteryAchievement>(true);
        }
    }
}