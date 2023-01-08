using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Pyro.Components
{
    public class FlamethrowerController : MonoBehaviour
    {
        public static GameObject flamethrowerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Drones/DroneFlamethrowerEffect.prefab").WaitForCompletion();
        public static float effectRefreshDuration = 1.5f;

        private float effectRefreshTimer;   //refresh the effect since it doesnt have built-in looping
        private GameObject flamethrowerInstance;
        private float flamethrowerTimer;    //timer before flamethrower effect is destroyed
        private ChildLocator childLocator;

        public void Awake()
        {
            effectRefreshTimer = 0f;
            flamethrowerTimer = 0f;
            flamethrowerInstance = null;
        }

        public void Start()
        {
            ModelLocator ml = base.GetComponent<ModelLocator>();
            if (ml && ml.modelTransform)
            {
                GameObject model = ml.modelTransform.gameObject;
                if (model)
                {
                    childLocator = model.GetComponent<ChildLocator>();
                }
            }
        }

        public void FireFlamethrower(float duration)
        {
            if (flamethrowerTimer < duration) flamethrowerTimer = duration;

            if (!flamethrowerInstance)
            {
                if (childLocator)
                {

                }
            }
        }
    }
}
