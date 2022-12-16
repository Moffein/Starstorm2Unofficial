using UnityEngine;
using RoR2;
using RoR2.Projectile;

namespace EntityStates.SS2UStates.Chirr
{
    public class DartSpread : BaseState
    {
        public static GameObject centerProjectilePrefab;
        public static GameObject spreadProjectilePrefab;
        public static float damageCoefficient = 1.5f;
        public static int totalProjectiles = 3;
        public static float baseDuration = 0.4f;

        private float duration;
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
