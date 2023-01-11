using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile
{
    public class PrimaryProjectileComponent : MonoBehaviour
    {
        public float maxScale = 5f;
        public float maxRadiusScale = 2.5f;
        public float maxDamageScale = 1.5f;
        public float delayBeforeScaling = 1f/3f;
        public float timeToScale = 1.5f - 1f/3f;

        private float stopwatch;
        private Vector3 initialScale;
        private float initialRadius;
        private float initialDamageCoefficient;
        private ProjectileImpactExplosion pie;

        private void Awake()
        {
            initialScale = base.transform.localScale;
            stopwatch = 0f;
            pie = base.GetComponent<ProjectileImpactExplosion>();
            if (pie)
            {
                initialRadius = pie.blastRadius;
                initialDamageCoefficient = pie.blastDamageCoefficient;
            }
        }

        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= delayBeforeScaling)
            {
                float scaleTime = (stopwatch - delayBeforeScaling)/timeToScale;
                base.transform.localScale = initialScale * Mathf.Lerp(1f, maxScale, scaleTime);
                if (pie)
                {
                    pie.blastRadius = initialRadius * Mathf.Lerp(1f, maxRadiusScale, scaleTime);
                    pie.blastDamageCoefficient = initialDamageCoefficient * Mathf.Lerp(1f, maxDamageScale, scaleTime);
                }
            }
        }
    }
}
