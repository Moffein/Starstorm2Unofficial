using RoR2;
using RoR2.Achievements;
using UnityEngine;
namespace Starstorm2.Cores.Unlockables.Achievements.Nemmando
{
    [RegisterAchievement("SS2UNemmandoClearGameTyphoon", "Skins.SS2UNemmando.GrandMastery", null, null)]
    public class NemmandoGrandMasteryAchievement : BaseAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Modules.Survivors.Nemmando.bodyIndex;
        }
		public override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
			Run.onClientGameOverGlobal += this.OnClientGameOverGlobal;
		}

		public override void OnBodyRequirementBroken()
		{
			Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
			base.OnBodyRequirementBroken();
		}

		private void OnClientGameOverGlobal(Run run, RunReport runReport)
		{
			if (!runReport.gameEnding)
			{
				return;
			}
			if (runReport.gameEnding.isWin)
			{
				DifficultyIndex difficultyIndex = runReport.ruleBook.FindDifficulty();
				DifficultyDef difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
				if (difficultyDef != null)
				{
					DifficultyDef infernoDef = Starstorm.GetInfernoDef();
					bool isInferno = infernoDef != null && difficultyDef == infernoDef;
					bool isEclipse = (difficultyIndex >= DifficultyIndex.Eclipse1 && difficultyIndex <= DifficultyIndex.Eclipse8);
					bool isTyphoon = difficultyDef.scalingValue >= TyphoonCore.difScale;

					if (isTyphoon || isInferno || isEclipse) base.Grant();
				}
			}
		}
	}
}
