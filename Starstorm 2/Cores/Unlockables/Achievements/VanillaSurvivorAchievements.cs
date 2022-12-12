using System;
using RoR2;
using UnityEngine;
using Starstorm2.Modules;

namespace Starstorm2.Cores.Unlockables.Achievements
{
    // todo: make a base class for mastery achievements and simply inherit from it for each character   
    internal class CommandoGrandMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "COMMANDO_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texCommandoSkinGrandMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("COMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("CommandoBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && runReport.ruleBook.FindDifficulty() == TyphoonCore.diffIdxTyphoon)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }

    internal class ToolbotGrandMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "TOOLBOT_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texToolbotSkinGrandMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("TOOLBOT_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("ToolbotBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && runReport.ruleBook.FindDifficulty() == TyphoonCore.diffIdxTyphoon)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }

    internal class CrocoGrandMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "CROCO_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texAcridSkinGrandMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("CROCO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("CrocoBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && runReport.ruleBook.FindDifficulty() == TyphoonCore.diffIdxTyphoon)
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Run.onClientGameOverGlobal += this.ClearCheck;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Run.onClientGameOverGlobal -= this.ClearCheck;
        }
    }
}