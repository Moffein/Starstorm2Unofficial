using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class FireLeapOvercharge : FireLeap
    {
        protected override float CalculateChargeMultiplier()
        {
            float mult = Mathf.Lerp(1.5f, 2f, (this.charge - BaseChargeState.overchargeFraction) / (1f - BaseChargeState.overchargeFraction));
            return mult;
        }
    }
}
