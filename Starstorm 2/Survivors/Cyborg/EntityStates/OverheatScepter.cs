using UnityEngine;

namespace EntityStates.Starstorm2States.Cyborg
{
    public class OverheatScepter : CyborgFireOverheat
    {
        public static GameObject projectileOverride;

        public override void OverrideStats()
        {
            damageCoefficientInternal *= 1.5f;
            projectilePrefabInternal = OverheatScepter.projectileOverride;
        }
    }
}
