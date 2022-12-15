using EntityStates.SS2UStates.Common;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class Headbutt : BaseMeleeAttack
    {
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
