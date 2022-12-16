using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using Starstorm2.Survivors.Chirr.Components;

namespace Starstorm2.Survivors.Chirr
{
	public class LeashSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new LeashSkillDef.InstanceData
			{
				targetingController = skillSlot.GetComponent<ChirrFriendController>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return LeashSkillDef.HasFriend(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && LeashSkillDef.HasFriend(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public ChirrFriendController targetingController;
		}

		private static bool HasFriend([NotNull] GenericSkill skillSlot)
		{
			ChirrFriendController targetingController = ((LeashSkillDef.InstanceData)skillSlot.skillInstanceData).targetingController;
			if (targetingController != null)
			{
				return targetingController.HasFriend();
			}
			return false;
		}
	}
}
