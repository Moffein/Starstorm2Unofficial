using UnityEngine;

namespace Starstorm2.Survivors.Cyborg.Components.OverheatProjectile
{
    public class BFGReduceSizeOverTime : MonoBehaviour
    {
        public float lifetime = 3f;

        private ParticleSystem[] particles = null;
        private float stopwatch = 0f;



        private float origScaleFire;
        private float origScaleBeams;
        private float origScaleLightning;

        private float scaleMultFire = 0f;
        private float scaleMultBeams = 0f;
        private float scaleMultLightning = 0f;

        private void Awake()
        {
            particles = base.GetComponentsInChildren<ParticleSystem>();
            for (int i = 0; i < particles.Length; i++)
            {
                switch(i)
                {
                    case 0: //fire
                        origScaleFire = particles[i].startSize;
                        break;
                    case 1: //beams
                        origScaleBeams = particles[i].startSize;
                        break;
                    case 2: //lightning
                        origScaleLightning = particles[i].startSize;
                        break;
                }
            }
        }

        private void FixedUpdate()
        {
            if (particles != null)
            {
                stopwatch += Time.fixedDeltaTime;
                for (int i = 0; i < particles.Length; i++)
                {
                    switch (i)
                    {
                        case 0: //fire
                            particles[i].startSize = Mathf.Lerp(origScaleFire, scaleMultFire * origScaleFire, stopwatch / lifetime);
                            break;
                        case 1: //beams
                            particles[i].startSize = Mathf.Lerp(origScaleBeams, scaleMultBeams * origScaleBeams, stopwatch / lifetime);
                            break;
                        case 2: //lightning
                            particles[i].startSize = Mathf.Lerp(origScaleLightning, scaleMultLightning * origScaleLightning, stopwatch / lifetime);
                            break;
                    }
                }
            }
        }

        /*Overheat partiucle scales
        [Info   : Unity Log] Fire - 4
        [Info   : Unity Log] Beams - 1
        [Info   : Unity Log] Lightning - 4*/
    }
}
