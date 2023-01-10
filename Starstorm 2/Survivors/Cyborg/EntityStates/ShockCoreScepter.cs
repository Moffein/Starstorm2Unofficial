using UnityEngine;

namespace EntityStates.SS2UStates.Cyborg
{
    public class ShockCoreScepter : ShockCore
    {
        public static GameObject scepterProjectilePrefab;

        public override float GetDamageCoefficient()
        {
            return base.GetDamageCoefficient() * 1.5f;
        }

        public override GameObject GetProjectilePrefab()
        {
            return ShockCoreScepter.scepterProjectilePrefab;
        }
    }
}
