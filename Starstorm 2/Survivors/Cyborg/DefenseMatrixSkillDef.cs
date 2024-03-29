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
	public class DefenseMatrixSkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new DefenseMatrixSkillDef.InstanceData
			{
				chargeComponent = skillSlot.GetComponent<CyborgChargeComponent>(),
                chargeFractionCost = this.chargeFractionCost
            };
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return DefenseMatrixSkillDef.CanUseShield(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && DefenseMatrixSkillDef.CanUseShield(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public CyborgChargeComponent chargeComponent;
			public float chargeFractionCost;
        }

        private static bool CanUseShield([NotNull] GenericSkill skillSlot)
        {
			DefenseMatrixSkillDef.InstanceData instanceData = (DefenseMatrixSkillDef.InstanceData)skillSlot.skillInstanceData;
            CyborgChargeComponent chargeComponent = instanceData.chargeComponent;
            return (chargeComponent != null) ? !chargeComponent.shieldDepleted && chargeComponent.chargeFraction >= instanceData.chargeFractionCost : false;
        }

        public float chargeFractionCost = 0f;
	}
}