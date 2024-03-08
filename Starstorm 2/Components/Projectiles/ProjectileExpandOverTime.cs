using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Components.Projectiles
{
    public class ProjectileExpandOverTime : MonoBehaviour
    {
        public float startDelay = 0f;   //Delay before expansion starts;
        public float endSizeMultiplier = 1f;    //Max size multiplier
        public float endSizeTime = 1f;  //Time it takes to expand to full (after start delay)

        private float stopwatch;
        private float initialRadius;  //Always calculate size off of initial size
        private ProjectileImpactExplosion pie;

        public void Awake()
        {
            stopwatch = 0f;
            pie = base.GetComponent<ProjectileImpactExplosion>();
            if (!pie)
            {
                Destroy(this);
                return;
            }
            initialRadius = pie.blastRadius;
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            stopwatch += Time.fixedDeltaTime;
            if (stopwatch <= startDelay) return;

            float multiplier = Mathf.Lerp(1f, endSizeMultiplier, (stopwatch - startDelay) / endSizeTime);
            pie.blastRadius = initialRadius * multiplier;
        }
    }
}
