using RoR2;
using RoR2.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2.Cores.Unlockables
{
	public class BaseGrandMasteryAchievement : BaseAchievement
	{
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
					bool isTyphoon = difficultyDef.scalingValue >= 3.5f;//hardcoded number to be consistent with other mods

					if (isTyphoon || isInferno || isEclipse) base.Grant();
				}
			}
		}
	}
}
