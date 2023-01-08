using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Pyro.Components
{
    public class FlamethrowerController : MonoBehaviour
    {
        public static GameObject flamethrowerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/DroneFlamethrowerEffect.prefab").WaitForCompletion();
        public static float effectRefreshDuration = 1.8f;

        private Animator animator;
        private ModelLocator modelLocator;
        private CharacterBody characterBody;
        private Transform muzzleTransform;
        private float effectRefreshTimer;   //refresh the effect since it doesnt have built-in looping
        private GameObject flamethrowerInstance;
        private float flamethrowerTimer;    //timer before flamethrower effect is destroyed
        private bool firing;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            effectRefreshTimer = 0f;
            flamethrowerTimer = 0f;
            flamethrowerInstance = null;
            firing = false;
        }

        private void Start()
        {
            modelLocator = base.GetComponent<ModelLocator>();
            if (modelLocator && modelLocator.modelTransform)
            {
                GameObject model = modelLocator.modelTransform.gameObject;
                if (model)
                {
                    animator = model.GetComponent<Animator>();
                    ChildLocator cl = model.GetComponent<ChildLocator>();
                    if (cl)
                    {
                        muzzleTransform = cl.FindChild("Muzzle");
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (firing)
            {
                if (flamethrowerTimer > 0f)
                {
                    flamethrowerTimer -= Time.fixedDeltaTime;
                    RefreshFlamethrowerEffect();
                }
                else
                {
                    Util.PlaySound("Play_mage_R_end", base.gameObject);
                    DestroyFlamethrowerInstsance();
                    firing = false;
                    StopAnim();
                }
            }
        }

        private void Update()
        {
            UpdateFlamethrowerEffect();
        }

        public void FireFlamethrower(float duration)
        {
            if (flamethrowerTimer < duration) flamethrowerTimer = duration;

            if (!firing)
            {
                firing = true;
                Util.PlaySound("Play_mage_R_start", base.gameObject);
                StartAnim();
            }

            CreateFlamethrowerInstance();
        }

        private void StartAnim()
        {
            PlayAnimation(animator, "FullBody, Override", "Firing", "", 0.5f);
        }

        private void StopAnim()
        {
            PlayAnimation(animator, "FullBody, Override", "BufferEmpty", "", 0.5f);
        }

        private void PlayAnimation(Animator modelAnimator, string layerName, string animationStateName, string playbackRateParam, float duration)
        {
            if (animator)
            {
                modelAnimator.speed = 1f;
                modelAnimator.Update(0f);
                int layerIndex = modelAnimator.GetLayerIndex(layerName);
                if (layerIndex >= 0)
                {
                    modelAnimator.SetFloat(playbackRateParam, 1f);
                    modelAnimator.PlayInFixedTime(animationStateName, layerIndex, 0f);
                    modelAnimator.Update(0f);
                    float length = modelAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;
                    modelAnimator.SetFloat(playbackRateParam, length / duration);
                }
            }
        }

        private void CreateFlamethrowerInstance()
        {
            if (!flamethrowerInstance)
            {
                effectRefreshTimer = FlamethrowerController.effectRefreshDuration;
                if (muzzleTransform)
                {
                    flamethrowerInstance = UnityEngine.Object.Instantiate<GameObject>(FlamethrowerController.flamethrowerPrefab);//, muzzleTransform    //Effect scaling is messed up when parented.
                    if (flamethrowerInstance && flamethrowerInstance.transform)
                    {
                        flamethrowerInstance.transform.GetComponent<ScaleParticleSystemDuration>().newDuration = 2f;
                    }
                    UpdateFlamethrowerEffect();
                }
            }
        }

        private void DestroyFlamethrowerInstsance()
        {
            if (flamethrowerInstance)
            {
                Destroy(flamethrowerInstance);
                flamethrowerInstance = null;
            }
        }

        private void UpdateFlamethrowerEffect()
        {
            if (flamethrowerInstance)
            {
                if (muzzleTransform)flamethrowerInstance.transform.position = muzzleTransform.position;
                if (characterBody && characterBody.inputBank) flamethrowerInstance.transform.forward = characterBody.inputBank.GetAimRay().direction;
            }
        }

        private void RefreshFlamethrowerEffect()
        {
            effectRefreshTimer -= Time.fixedDeltaTime;
            if (effectRefreshTimer <= 0f)
            {
                DestroyFlamethrowerInstsance();
                CreateFlamethrowerInstance();
            }
        }

        private void OnDestroy()
        {
            DestroyFlamethrowerInstsance();
        }
    }
}
