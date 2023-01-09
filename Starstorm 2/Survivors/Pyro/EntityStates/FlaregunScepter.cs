using UnityEngine;

namespace EntityStates.SS2UStates.Pyro
{
    public class FlaregunScepter : Flaregun
    {
        public static GameObject scepterProjectilePrefab;
        public static int scepterMaxExplosions = 16;

        public override GameObject GetProjectilePrefab()
        {
            return scepterProjectilePrefab;
        }

        public override int SetExplosionCount(float heatPercent)
        {
            int totalExplosions = Mathf.FloorToInt(Mathf.Lerp(0, scepterMaxExplosions, heatPercent));
            return (Mathf.Max(1, totalExplosions));
        }

        public override float SetCostPerExplosion()
        {
            return 1f / scepterMaxExplosions;
        }
    }
}
