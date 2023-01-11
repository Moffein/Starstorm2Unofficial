using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile
{
    public class GhostScaleOverTime : MonoBehaviour
    {
        public float maxScale = 6f;
        public float delayBeforeScaling = 0.5f;
        public float timeToScale = 1f;

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
                float scaleTime = stopwatch - delayBeforeScaling;
                float newScale = Mathf.Lerp(1f, maxScale, scaleTime / timeToScale);
                base.transform.localScale = newScale * initialScale;
            }
        }
    }
}
