using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile
{
    [RequireComponent(typeof(ProjectileImpactExplosion))]
    public class PrimaryProjectileComponentSimple : MonoBehaviour
    {
        public float startDelay = 0f;   //Delay before expansion starts;
        public float endSizeMultiplier = 1f;    //Max size multiplier
        public float endSizeTime = 1f;  //Time it takes to expand to full (after start delay)
        public float baseSpeed = 60f;   //If projectile is faster than base speed, speed up time scaling to match;

        //Hacky thing to make the VFX look less plain
        public static GameObject secondaryVFX = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoaderLightning.prefab").WaitForCompletion();

        private float stopwatch;
        private float initialRadius;  //Always calculate size off of initial size
        private ProjectileImpactExplosion pie;
        private float storedRadius;

        public void Awake()
        {
            stopwatch = 0f;
            pie = base.GetComponent<ProjectileImpactExplosion>();
            initialRadius = pie.blastRadius;

            ProjectileSimple ps = base.GetComponent<ProjectileSimple>();
            if (ps)
            {
                float speedFactor = ps.desiredForwardSpeed / baseSpeed;
                if (speedFactor > 0f)
                {
                    startDelay /= speedFactor;
                    endSizeTime /= speedFactor;
                }
            }
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            stopwatch += Time.fixedDeltaTime;
            if (stopwatch <= startDelay) return;

            float multiplier = Mathf.Lerp(1f, endSizeMultiplier, (stopwatch - startDelay) / endSizeTime);
            pie.blastRadius = initialRadius * multiplier;
            storedRadius = pie.blastRadius;
        }

        private void OnDestroy()
        {
            if (NetworkServer.active)
            {
                EffectManager.SpawnEffect(secondaryVFX, new EffectData
                {
                    scale = storedRadius * 0.5f,
                    origin = base.transform.position,
                    rotation = base.transform.rotation
                }, true);
            }
        }
    }
}
