using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Cores
{
    public class SpooderCore
    {
        public static GameObject bodyPrefab;

        public SpooderCore() => Setup();

        private void Setup()
        {
            bodyPrefab = PrefabCore.CreatePrefab("SpooderBody", "mdlSpooder", new BodyInfo
            {
                armor = 20f,
                armorGrowth = 0f,
                bodyName = "SpooderBody",
                bodyNameToken = "SPOODER_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("texNemmandoIcon"),
                crosshair = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair"),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "SPOODER_SUBTITLE"
            });

            PrefabCore.SetupCharacterModel(bodyPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matSpooder")
                }
            }, 0);

            //insert custom spawn state here
            //bodyPrefab.GetComponent<EntityStateMachine>().initialStateType = new SerializableEntityStateType(typeof(SpooderMain));
        }
    }
}