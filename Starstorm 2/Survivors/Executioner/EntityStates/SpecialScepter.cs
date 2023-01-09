using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityStates.SS2UStates.Executioner
{
    public class ExecutionerAxeScepter : ExecutionerAxe
    {
        public override void SetNextState()
        {
            this.outer.SetNextState(new ExecutionerAxeSlamScepter());
        }
    }

    public class ExecutionerAxeSlamScepter : ExecutionerAxeSlam
    {
        public override void OverrideStats()
        {
            damageCoefficientInternal *= 1.5f;
            radiusInternal = 20f;
        }

        public override BlastAttack ModifyBlastAttack(BlastAttack blast)
        {
            blast = base.ModifyBlastAttack(blast);
            blast.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.GuaranteedFearOnHit);
            return blast;
        }
    }
}
