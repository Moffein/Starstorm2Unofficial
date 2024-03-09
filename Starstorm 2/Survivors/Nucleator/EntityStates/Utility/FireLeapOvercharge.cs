using Starstorm2Unofficial.Survivors.Nucleator.Components;
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
        public static float shockDamageCoefficient = 4f;
        public static float shockRadius = 24f;

        protected override float CalculateChargeMultiplier()
        {
            float mult = Mathf.Lerp(1.5f, 2f, (this.charge - BaseChargeState.overchargeFraction) / (1f - BaseChargeState.overchargeFraction));
            return mult;
        }

        protected override void DetonateAuthority()
        {
            base.DetonateAuthority();
            NucleatorNetworkComponent nnc = base.GetComponent<NucleatorNetworkComponent>();
            if (nnc)
            {
                nnc.UtilityShockAuthority(base.transform.position, isCrit);
            }
        }
    }
}
