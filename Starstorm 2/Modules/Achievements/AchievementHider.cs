using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2Unofficial.Modules.Achievements
{
    internal static class AchievementHider
    {
        public static List<string> unlockableRewardIdentifiers =
        [
            "Skins.SS2UCommando.GrandMastery",
            "Skins.SS2UToolbot.GrandMastery",
            "Skins.SS2UCyborg.GrandMastery",
            "Skins.SS2UCyborg.Mastery",
            "Skins.SS2UExecutioner.GrandMastery",
            "Skins.SS2UExecutioner.Mastery",
            "Skins.SS2UExecutioner.Wastelander",
            "Skins.SS2UNemmando.Commando",
            "Characters.SS2UNemmando",
            "Skins.SS2UNemmando.Mastery",
            "Skins.SS2UNemmando.GrandMastery",
            "Skins.SS2UChirr.Mastery"
        ];
        public static bool enabled = true;

        public static void Init()
        {
            if (!enabled) return;
            On.RoR2.UI.LogBook.LogBookController.BuildAchievementEntries += LogBookController_BuildAchievementEntries;
        }

        private static RoR2.UI.LogBook.Entry[] LogBookController_BuildAchievementEntries(On.RoR2.UI.LogBook.LogBookController.orig_BuildAchievementEntries orig, Dictionary<RoR2.ExpansionManagement.ExpansionDef, bool> expansionAvailability)
        {
            var entries = orig(expansionAvailability);
            entries = entries.Where(entry =>
            {
                AchievementDef achievementDef = (AchievementDef)entry.extraData;
                return !unlockableRewardIdentifiers.Contains(achievementDef.unlockableRewardIdentifier);
            }).ToArray();
            return entries;
        }
    }
}
