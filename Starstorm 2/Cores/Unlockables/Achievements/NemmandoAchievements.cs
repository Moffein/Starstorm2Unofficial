using System;
using RoR2;
using UnityEngine;
using Starstorm2.Modules;

namespace Starstorm2.Cores.Unlockables.Achievements
{
    internal class NemmandoUnlockAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "NEMMANDO_UNLOCKUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoIconUnlock");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC")
        });


        private void Check(Run run)
        {
            if (base.isUserAlive)
                base.Grant();
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Components.NemmandoUnlockComponent.OnDeath += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Components.NemmandoUnlockComponent.OnDeath -= this.Check;
        }
    }

    // todo: make a base class for mastery achievements and simply inherit from it for each character   
    internal class NemmandoMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "NEMMANDO_MASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "NEMMANDO_MASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "NEMMANDO_MASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("NemmandoBody");
        }

        public void ClearCheck(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(runReport.ruleBook.FindDifficulty());

                if (difficultyDef != null && difficultyDef.countsAsHardMode)
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

    internal class NemmandoGrandMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "NEMMANDO_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinGrandMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("NemmandoBody");
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

    internal class NemmandoSingleTapAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "NEMMANDO_SINGLETAPUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texSingleTapUnlockIcon");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_SINGLETAPUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("NemmandoBody");
        }

        private void Check(Run run)
        {
            if (base.isUserAlive)
            {
                if (base.meetsBodyRequirement) base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            Components.NemmandoUnlockComponent.OnDeath += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            Components.NemmandoUnlockComponent.OnDeath -= this.Check;
        }
    }

    internal class NemmandoDecisiveStrikeAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "NEMMANDO_EPICUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "NEMMANDO_EPICUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "NEMMANDO_EPICUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texDecisiveStrike");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("NEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("NemmandoBody");
        }

        private void Check(On.RoR2.CharacterBody.orig_AddBuff_BuffIndex orig, CharacterBody self, BuffIndex buff)
        {
            if (self)
            {
                if (self.activeBuffsList != null)
                {
                    if (self.GetBuffCount(BuffCore.gougeBuff) >= 50)
                    {
                        if (base.meetsBodyRequirement)
                        {
                            base.Grant();
                        }
                    }
                }
            }

            orig(self, buff);
        }

        private void Check(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, CharacterBody self, BuffDef buff)
        {
            if (self)
            {
                if (self.activeBuffsList != null)
                {
                    if (self.GetBuffCount(BuffCore.gougeBuff) >= 50)
                    {
                        if (base.meetsBodyRequirement)
                        {
                            base.Grant();
                        }
                    }
                }
            }

            orig(self, buff);
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.RoR2.CharacterBody.AddBuff_BuffDef += this.Check;
            On.RoR2.CharacterBody.AddBuff_BuffIndex += this.Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.RoR2.CharacterBody.AddBuff_BuffDef -= this.Check;
            On.RoR2.CharacterBody.AddBuff_BuffIndex -= this.Check;
        }
    }
}