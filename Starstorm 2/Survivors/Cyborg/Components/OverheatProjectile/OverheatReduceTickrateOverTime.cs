using RoR2;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg.Components.OverheatProjectile
{
    [RequireComponent(typeof(ProjectileSimple))]
    [RequireComponent(typeof(ProjectileProximityBeamController))]
    public class OverheatReduceTickrateOverTime : MonoBehaviour
    {
        public static float fullDPSDurationPercent = 0f;
        public static float finalDPSMultiplier = 0.1f;

        private float initialOverlapInterval;
        private float initialResetInterval;
        private float startDecayTime;
        private float totalLifetime;
        private float stopwatch = 0f;
        private ProjectileProximityBeamController pbc;
        public void Start()
        {
            pbc = base.GetComponent<ProjectileProximityBeamController>();
            initialResetInterval = pbc.listClearInterval;
            initialOverlapInterval = pbc.attackInterval;

            ProjectileSimple ps = base.GetComponent<ProjectileSimple>();
            totalLifetime = ps.lifetime;
            startDecayTime = ps.lifetime * fullDPSDurationPercent;
        }
        

        public void FixedUpdate()
        {
            if (NetworkServer.active && pbc)
            {
                stopwatch += Time.fixedDeltaTime;
                if (stopwatch >= startDecayTime)
                {
                    float decayLerp = Mathf.Lerp(1f, finalDPSMultiplier, (stopwatch - startDecayTime) / (totalLifetime - startDecayTime));
                    pbc.listClearInterval = initialResetInterval / decayLerp;
                    pbc.attackInterval = initialResetInterval / decayLerp;
                }
            }
        }
    }
}
