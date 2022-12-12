using System;
using RoR2;
using UnityEngine;
using Starstorm2.Modules;

namespace Starstorm2.Cores.Unlockables.Achievements
{
    internal class ExecutionerUnlockAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "EXECUTIONER_UNLOCKUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerIconUnlock");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_UNLOCKUNLOCKABLE_ACHIEVEMENT_DESC")
        });

        public override void OnInstall()
        {
            base.OnInstall();

            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            GlobalEventManager.onCharacterDeathGlobal -= GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport rpt)
        {
            var attackerMaster = rpt.attackerMaster;
            if (attackerMaster && attackerMaster.playerCharacterMasterController)
            {
                var victimBody = rpt.victimBody;
                var damage = rpt.damageDealt;
                if (victimBody && damage >= victimBody.maxHealth * 10 && victimBody.master)
                    base.Grant();
            }
        }
    }

    // todo: make a base class for mastery achievements and simply inherit from it for each character   
    internal class ExecutionerMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "EXECUTIONER_MASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_MASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });


        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("ExecutionerBody");
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

    internal class ExecutionerGrandMasteryAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "EXECUTIONER_GRANDMASTERYUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerSkinGrandMaster");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_GRANDMASTERYUNLOCKABLE_ACHIEVEMENT_DESC")
        });


        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("ExecutionerBody");
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

    internal class WastelanderAchievement : SS2Unlockable
    {
        public override String AchievementIdentifier { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_ID";
        public override String UnlockableIdentifier { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_REWARD_ID";
        public override String PrerequisiteUnlockableIdentifier { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_PREREQ_ID";
        public override String AchievementNameToken { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_NAME";
        public override String AchievementDescToken { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_DESC";
        public override String UnlockableNameToken { get; } = "EXECUTIONER_WASTELANDERUNLOCKABLE_UNLOCKABLE_NAME";
        public override Sprite IconSprite { get; } = Assets.mainAssetBundle.LoadAsset<Sprite>("texExecutionerWastelander");
        public override Func<string> GetHowToUnlock => () => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_DESC")
        });
        public override Func<string> GetUnlocked { get; } = () => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
        {
            Language.GetString("EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_NAME"),
            Language.GetString("EXECUTIONER_WASTELANDERUNLOCKABLE_ACHIEVEMENT_DESC")
        });


        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("ExecutionerBody");
        }

        public void StoneGate_Opening(On.EntityStates.Interactables.StoneGate.Opening.orig_OnEnter orig, EntityStates.Interactables.StoneGate.Opening self)
        {
            orig(self);

            if (base.meetsBodyRequirement)
            {
                base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            On.EntityStates.Interactables.StoneGate.Opening.OnEnter += this.StoneGate_Opening;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            On.EntityStates.Interactables.StoneGate.Opening.OnEnter -= this.StoneGate_Opening;
        }
    }
}