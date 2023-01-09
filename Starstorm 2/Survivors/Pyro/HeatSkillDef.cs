using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using Starstorm2Unofficial.Survivors.Pyro.Components;

namespace Starstorm2Unofficial.Survivors.Pyro
{
	public class HeatSkillDef : SkillDef
	{
		public float baseHeatRequirement = 0f;
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new HeatSkillDef.InstanceData
			{
				heatController = skillSlot.GetComponent<HeatController>()
			};
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return HeatSkillDef.HasEnoughHeat(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && HeatSkillDef.HasEnoughHeat(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public HeatController heatController;
		}

		private static bool HasEnoughHeat([NotNull] GenericSkill skillSlot)
		{
			HeatController heatController = ((HeatSkillDef.InstanceData)skillSlot.skillInstanceData).heatController;
			if (heatController)
            {
				float requiredHeat = ((HeatSkillDef)skillSlot.skillDef).baseHeatRequirement / (Mathf.Max(1f, 0.5f + 0.5f * skillSlot.maxStock));
				return heatController.GetHeatPercent() >= requiredHeat;
            }
			return false;
		}
	}
}
