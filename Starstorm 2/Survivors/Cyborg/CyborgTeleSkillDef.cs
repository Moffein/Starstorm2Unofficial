using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial.Survivors.Cyborg
{
	public class CyborgTeleSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new CyborgTeleSkillDef.InstanceData
			{
				teleTracker = skillSlot.GetComponent<CyborgTeleportTracker>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return CyborgTeleSkillDef.HasTeleDestination(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && CyborgTeleSkillDef.HasTeleDestination(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public CyborgTeleportTracker teleTracker;
		}

		private static bool HasTeleDestination([NotNull] GenericSkill skillSlot)
		{
			CyborgTeleportTracker teleTracker = ((CyborgTeleSkillDef.InstanceData)skillSlot.skillInstanceData).teleTracker;
			return (teleTracker != null) ? teleTracker.GetTeleportCoordinates() != null : false;
		}
	}
}
