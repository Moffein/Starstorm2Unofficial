using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using Starstorm2Unofficial.Survivors.Chirr.Components;

namespace Starstorm2Unofficial.Survivors.Chirr
{
	public class BefriendSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new BefriendSkillDef.InstanceData
			{
				targetingController = skillSlot.GetComponent<ChirrFriendController>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return BefriendSkillDef.CanBefriend(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && BefriendSkillDef.CanBefriend(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public ChirrFriendController targetingController;
		}

		private static bool CanBefriend([NotNull] GenericSkill skillSlot)
		{
			ChirrFriendController targetingController = ((BefriendSkillDef.InstanceData)skillSlot.skillInstanceData).targetingController;
			if (targetingController != null)
            {
				return targetingController.CanBefriend();
            }
			return false;
		}
	}
}
