using RoR2;
using RoR2.Achievements;
using Starstorm2Unofficial.Modules.Achievements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial.Cores.Unlockables.Achievements.Vanilla
{
	[RegisterAchievement("SS2UCommandoClearGameTyphoon", "Skins.SS2UCommando.GrandMastery", null, 15, null)]
	public class CommandoGrandMasteryAchievement : BaseGrandMasteryAchievement
	{
		public override BodyIndex LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("CommandoBody");
		}
	}

	[RegisterAchievement("SS2UToolbotClearGameTyphoon", "Skins.SS2UToolbot.GrandMastery", null, 15, null)]
	public class ToolbotGrandMasteryAchievement : BaseGrandMasteryAchievement
	{
		public override BodyIndex LookUpRequiredBodyIndex()
		{
			return BodyCatalog.FindBodyIndex("ToolbotBody");
		}
	}
}
