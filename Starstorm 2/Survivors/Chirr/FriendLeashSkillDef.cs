using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using Starstorm2.Survivors.Chirr.Components;

namespace Starstorm2.Survivors.Chirr
{
	public class FriendLeashSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new FriendLeashSkillDef.InstanceData
			{
				targetingController = skillSlot.GetComponent<ChirrFriendController>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return FriendLeashSkillDef.CanLeash(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && FriendLeashSkillDef.CanLeash(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public ChirrFriendController targetingController;
		}

		private static bool CanLeash([NotNull] GenericSkill skillSlot)
		{
			ChirrFriendController targetingController = ((FriendLeashSkillDef.InstanceData)skillSlot.skillInstanceData).targetingController;
			if (targetingController != null)
			{
				return targetingController.CanLeash();
			}
			return false;
		}
	}
}
