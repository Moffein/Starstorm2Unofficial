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
	public class EnergySkillDef : SkillDef
	{
		public override SkillDef.BaseSkillInstanceData OnAssigned([NotNull] GenericSkill skillSlot)
		{
			return new EnergySkillDef.InstanceData
			{
				energyComponent = skillSlot.GetComponent<CyborgEnergyComponent>(),
                energyFractionCost = this.energyFractionCost
            };
		}

		public override bool CanExecute([NotNull] GenericSkill skillSlot)
		{
			return EnergySkillDef.HasEnoughEnergy(skillSlot) && base.CanExecute(skillSlot);
		}

		public override bool IsReady([NotNull] GenericSkill skillSlot)
		{
			return base.IsReady(skillSlot) && EnergySkillDef.HasEnoughEnergy(skillSlot);
		}

		protected class InstanceData : SkillDef.BaseSkillInstanceData
		{
			public CyborgEnergyComponent energyComponent;
			public float energyFractionCost;
        }

        private static bool HasEnoughEnergy([NotNull] GenericSkill skillSlot)
        {
			EnergySkillDef.InstanceData instanceData = (EnergySkillDef.InstanceData)skillSlot.skillInstanceData;
            CyborgEnergyComponent chargeComponent = instanceData.energyComponent;
            return (chargeComponent != null) ? !chargeComponent.energyDepleted && chargeComponent.rifleChargeFraction >= instanceData.energyFractionCost : false;
        }

        public float energyFractionCost = 0f;
	}
}