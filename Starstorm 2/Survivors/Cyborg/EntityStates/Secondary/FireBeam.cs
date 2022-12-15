using RoR2.UI;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class FireBeam : BaseState
    {
        public static string muzzleString = "Lowerarm.L_end";
        public static float perfectChargeDamageMultiplier = 1.5f;
        public static float minDamageCoefficient = 3f;
        public static float maxDamageCoefficient = 10f;
        public static float minForce = 2000f;
        public static float maxForce = 4000f;
        public static float baseDuration = 0.5f;

        public float charge;
        public bool perfectCharge;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        private float duration;

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
