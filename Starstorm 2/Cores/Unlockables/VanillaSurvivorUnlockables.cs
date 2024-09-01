using R2API;
using RoR2;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Achievements;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Unlockables
{
    public static class VanillaSurvivorUnlockables
    {
        public static UnlockableDef commandoGrandMastery;
        public static UnlockableDef toolbotGrandMastery;
        public static UnlockableDef acridGrandMastery;

        public static void RegisterUnlockables()
        {
            // todo: make a base class for mastery achievements and simply inherit from it for each character 

            if (Modules.Config.EnableGrandMasteryCommando.Value)
            {
                commandoGrandMastery = ScriptableObject.CreateInstance<UnlockableDef>();
                commandoGrandMastery.cachedName = "Skins.SS2UCommando.GrandMastery";
                commandoGrandMastery.nameToken = "ACHIEVEMENT_SS2UCOMMANDOCLEARGAMETYPHOON_NAME";
                commandoGrandMastery.achievementIcon = Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCommandoSkinGrandMaster");
                Modules.Unlockables.unlockableDefs.Add(commandoGrandMastery);

                AchievementHider.unlockableRewardIdentifiers.Remove(commandoGrandMastery.cachedName);
            }

            if (Modules.Config.EnableGrandMasteryToolbot.Value)
            {
                toolbotGrandMastery = ScriptableObject.CreateInstance<UnlockableDef>();
                toolbotGrandMastery.cachedName = "Skins.SS2UToolbot.GrandMastery";
                toolbotGrandMastery.nameToken = "ACHIEVEMENT_SS2UTOOLBOTCLEARGAMETYPHOON_NAME";
                toolbotGrandMastery.achievementIcon = Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texToolbotSkinGrandMaster");
                Modules.Unlockables.unlockableDefs.Add(toolbotGrandMastery);

                AchievementHider.unlockableRewardIdentifiers.Remove(toolbotGrandMastery.cachedName);
            }

            //acridGrandMastery = Modules.Unlockables.AddUnlockable<Achievements.CrocoGrandMasteryAchievement>(true);//Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAcridSkinGrandMaster");
        }
    }
}