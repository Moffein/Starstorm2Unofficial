using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile
{
    public class GhostScaleOverTime : MonoBehaviour
    {
        public float maxScale = 5f;
        public float delayBeforeScaling = 1f / 3f;
        public float timeToScale = 1.5f - 1f / 3f;

        private float stopwatch;
        private Vector3 initialScale;
        private void Awake()
        {
            stopwatch = 0f;
            initialScale = base.transform.localScale;
        }

        private void FixedUpdate()
        {
            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= delayBeforeScaling)
            {
                float scaleTime = (stopwatch - delayBeforeScaling) / timeToScale;
                float newScale = Mathf.Lerp(1f, maxScale, scaleTime);
                base.transform.localScale = newScale * initialScale;
            }
        }
    }
}
