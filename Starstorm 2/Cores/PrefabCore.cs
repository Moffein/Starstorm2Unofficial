using R2API;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2.Cores
{
    public class PrefabCore
    {
        internal static List<SurvivorDef> survivorDefs = new List<SurvivorDef>();

        public static GameObject executionerPrefab;
        public static GameObject executionerDisplayPrefab;

        public static GameObject nemmandoPrefab;
        public static GameObject nemmandoDisplayPrefab;

        public static GameObject pyroPrefab;
        public static GameObject pyroDisplayPrefab;

        public static GameObject cyborgPrefab;
        public static GameObject cyborgDisplayPrefab;

        public static GameObject chirrPrefab;
        public static GameObject chirrDisplayPrefab;

        public static GameObject spooderPrefab;

        // cache this just to give our ragdolls the same physic material as vanilla stuff
        private static PhysicMaterial ragdollMaterial;

        // list of new bodies to add
        public static List<GameObject> bodyList = new List<GameObject>();
        //public static GameObject[] bodyArray;

        public PrefabCore()
        {
            //CreatePrefabs();
            //RegisterSurvivors();
        }

        public static void RegisterSurvivors()
        {
            //here are your funny numbers now shut your fuck up
            //if (Starstorm.EnableCyborg.Value) RegisterNewSurvivor(cyborgPrefab, cyborgDisplayPrefab, 202, "CYBORG", "");
            //if (Starstorm.EnableNucleator.Value) RegisterNewSurvivor(nucleatorPrefab, nucleatorDisplayPrefab, 204, "NUCLEATOR", "");
            //if (Starstorm.EnableChirr.Value) RegisterNewSurvivor(chirrPrefab, chirrDisplayPrefab, 206, "CHIRR", "");
            //if (Starstorm.EnablePyro.Value) RegisterNewSurvivor(pyroPrefab, pyroDisplayPrefab, 207, "PYRO");
        }

        public static void CreatePrefabs()
        {
        }

        private static void RegisterNewSurvivor(GameObject bodyPrefab, GameObject displayPrefab, float sortPosition, string namePrefix, UnlockableDef unlockDef)
        {
            SurvivorDef survivorDef = ScriptableObject.CreateInstance<SurvivorDef>();
            survivorDef.displayNameToken = namePrefix + "_NAME";
            survivorDef.unlockableDef = unlockDef;
            survivorDef.descriptionToken = namePrefix + "_DESCRIPTION";
            survivorDef.bodyPrefab = bodyPrefab;
            survivorDef.displayPrefab = displayPrefab;
            survivorDef.outroFlavorToken = namePrefix + "_OUTRO_FLAVOR";
            survivorDef.mainEndingEscapeFailureFlavorToken = namePrefix + "_OUTRO_FAILURE";
            survivorDef.desiredSortPosition = sortPosition;

            survivorDefs.Add(survivorDef);
        }

        
        #region Cyborg
        internal static GameObject CreateCyborgPrefab()
        {
            cyborgPrefab = CreatePrefab("CyborgBody", "mdlCyborg", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "CyborgBody",
                bodyNameToken = "CYBORG_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("cyborgicon"),
                bodyColor = new Color32(138, 183, 168, 255),
                crosshair = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1.5f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "CYBORG_SUBTITLE",
                podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            });

            SetupCharacterModel(cyborgPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matCyborg", 1f, new Color(0.839f, 0.812f, 0.812f))
                }
            }, 0);

            cyborgPrefab.AddComponent<Components.CyborgController>();
            cyborgPrefab.AddComponent<CyborgInfoComponent>();
            cyborgDisplayPrefab = CreateDisplayPrefab("CyborgDisplay", cyborgPrefab);

            return cyborgPrefab;
        }
        #endregion

        #region Chirr
        internal static GameObject CreateChirrPrefab()
        {
            chirrPrefab = CreatePrefab("ChirrBody", "mdlChirr", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "ChirrBody",
                bodyNameToken = "CHIRR_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("ChirrIcon"),
                bodyColor = new Color32(129, 167, 98, 255),
                crosshair = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1.5f,
                jumpCount = 1,
                jumpPower = 22.5f,
                maxHealth = 110f,
                subtitleNameToken = "CHIRR_SUBTITLE",
                podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            });

            SetupCharacterModel(chirrPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matChirr")
                }
            }, 0);

            chirrPrefab.AddComponent<Components.ChirrController>();
            chirrPrefab.AddComponent<ChirrInfoComponent>();
            chirrDisplayPrefab = CreateDisplayPrefab("ChirrDisplay", chirrPrefab);
            // create hitboxes

            GameObject model = chirrPrefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SetupHitbox(model, childLocator.FindChild("HeadbuttHitbox"), "HeadbuttHitbox");

            return chirrPrefab;
        }
        #endregion
        /*
        #region Pyro
        private static void CreatePyro()
        {
            pyroPrefab = CreatePrefab("PyroBody", "mdlPyro", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "PyroBody",
                bodyNameToken = "PYRO_NAME",
                characterPortrait = AssetsCore.mainAssetBundle.LoadAsset<Texture2D>("texExecutionerIcon"),
                bodyColor = new Color32(215, 131, 38, 255),
                crosshair = Resources.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair"),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1.5f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "PYRO_SUBTITLE",
                podPrefab = Resources.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod")
            });

            SetupCharacterModel(pyroPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = AssetsCore.CreateMaterial("matPyro", 1, Color.white, 0)
                }
            }, 0);

            pyroDisplayPrefab = CreateDisplayPrefab("PyroDisplay", pyroPrefab);
        }
        #endregion

        #region Spooder
        private static void CreateSpooder()
        {
            spooderPrefab = CreatePrefab("SpooderBody", "mdlSpooder", new BodyInfo
            {
                armor = 20f,
                armorGrowth = 0f,
                bodyName = "SpooderBody",
                bodyNameToken = "SPOODER_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("texNemmandoIcon"),
                crosshair = Resources.Load<GameObject>("Prefabs/Crosshair/SimpleDotCrosshair"),
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "SPOODER_SUBTITLE"
            });

            SetupCharacterModel(spooderPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matSpooder")
                }
            }, 0);
        }
        #endregion*/

        public static GameObject CreateDisplayPrefab(string modelName, GameObject prefab)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), modelName + "Prefab");

            GameObject model = CreateModel(newPrefab, modelName);
            Transform modelBaseTransform = SetupModel(newPrefab, model.transform);

            model.AddComponent<CharacterModel>().baseRendererInfos = prefab.GetComponentInChildren<CharacterModel>().baseRendererInfos;

            return model.gameObject;
        }

        public static GameObject CreatePrefab(string bodyName, string modelName, BodyInfo bodyInfo)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), bodyName);

            GameObject model = CreateModel(newPrefab, modelName);
            Transform modelBaseTransform = SetupModel(newPrefab, model.transform);

            #region CharacterBody
            CharacterBody bodyComponent = newPrefab.GetComponent<CharacterBody>();

            bodyComponent.name = bodyInfo.bodyName;
            bodyComponent.baseNameToken = bodyInfo.bodyNameToken;
            bodyComponent.subtitleNameToken = bodyInfo.subtitleNameToken;
            bodyComponent.portraitIcon = bodyInfo.characterPortrait;
            bodyComponent.crosshairPrefab = bodyInfo.crosshair;
            bodyComponent.bodyColor = bodyInfo.bodyColor;

            bodyComponent.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
            bodyComponent.rootMotionInMainState = false;

            bodyComponent.baseMaxHealth = bodyInfo.maxHealth;
            bodyComponent.levelMaxHealth = bodyInfo.healthGrowth;

            bodyComponent.baseRegen = bodyInfo.healthRegen;
            bodyComponent.levelRegen = bodyComponent.baseRegen * 0.2f;

            bodyComponent.baseMaxShield = bodyInfo.shield;
            bodyComponent.levelMaxShield = bodyInfo.shieldGrowth;

            bodyComponent.baseMoveSpeed = bodyInfo.moveSpeed;
            bodyComponent.levelMoveSpeed = bodyInfo.moveSpeedGrowth;

            bodyComponent.baseAcceleration = bodyInfo.acceleration;

            bodyComponent.baseJumpPower = bodyInfo.jumpPower;
            bodyComponent.levelJumpPower = bodyInfo.jumpPowerGrowth;

            bodyComponent.baseDamage = bodyInfo.damage;
            bodyComponent.levelDamage = bodyComponent.baseDamage * 0.2f;

            bodyComponent.baseAttackSpeed = bodyInfo.attackSpeed;
            bodyComponent.levelAttackSpeed = bodyInfo.attackSpeedGrowth;

            bodyComponent.baseArmor = bodyInfo.armor;
            bodyComponent.levelArmor = bodyInfo.armorGrowth;

            bodyComponent.baseCrit = bodyInfo.crit;
            bodyComponent.levelCrit = bodyInfo.critGrowth;

            bodyComponent.baseJumpCount = bodyInfo.jumpCount;

            bodyComponent.sprintingSpeedMultiplier = 1.45f;

            bodyComponent.hideCrosshair = false;
            bodyComponent.aimOriginTransform = modelBaseTransform.Find("AimOrigin");
            bodyComponent.hullClassification = HullClassification.Human;

            bodyComponent.preferredPodPrefab = bodyInfo.podPrefab;

            bodyComponent.isChampion = false;
            #endregion

            SetupCharacterDirection(newPrefab, modelBaseTransform, model.transform);
            SetupCameraTargetParams(newPrefab);
            SetupModelLocator(newPrefab, modelBaseTransform, model.transform);
            SetupRigidbody(newPrefab);
            SetupCapsuleCollider(newPrefab);
            SetupMainHurtbox(newPrefab, model);
            SetupFootstepController(model);
            SetupRagdoll(model);
            SetupAimAnimator(newPrefab, model);

            Modules.Prefabs.bodyPrefabs.Add(newPrefab);
            bodyList.Add(newPrefab);

            return newPrefab;
        }

        #region ModelSetup
        private static Transform SetupModel(GameObject prefab, Transform modelTransform)
        {
            GameObject modelBase = new GameObject("ModelBase");
            modelBase.transform.parent = prefab.transform;
            modelBase.transform.localPosition = new Vector3(0f, -0.92f, 0f);
            modelBase.transform.localRotation = Quaternion.identity;
            modelBase.transform.localScale = new Vector3(1f, 1f, 1f);

            GameObject cameraPivot = new GameObject("CameraPivot");
            cameraPivot.transform.parent = modelBase.transform;
            cameraPivot.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            cameraPivot.transform.localRotation = Quaternion.identity;
            cameraPivot.transform.localScale = Vector3.one;

            GameObject aimOrigin = new GameObject("AimOrigin");
            aimOrigin.transform.parent = modelBase.transform;
            aimOrigin.transform.localPosition = new Vector3(0f, 2.2f, 0f);
            aimOrigin.transform.localRotation = Quaternion.identity;
            aimOrigin.transform.localScale = Vector3.one;
            prefab.GetComponent<CharacterBody>().aimOriginTransform = aimOrigin.transform;

            modelTransform.parent = modelBase.transform;
            modelTransform.localPosition = Vector3.zero;
            modelTransform.localRotation = Quaternion.identity;

            return modelBase.transform;
        }

        private static GameObject CreateModel(GameObject main, string modelName)
        {
            Starstorm.DestroyImmediate(main.transform.Find("ModelBase").gameObject);
            Starstorm.DestroyImmediate(main.transform.Find("CameraPivot").gameObject);
            Starstorm.DestroyImmediate(main.transform.Find("AimOrigin").gameObject);

            return Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(modelName);
        }

        public static void SetupCharacterModel(GameObject prefab, CustomRendererInfo[] rendererInfo, int mainRendererIndex)
        {
            CharacterModel characterModel = prefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();
            ChildLocator childLocator = characterModel.GetComponent<ChildLocator>();
            characterModel.body = prefab.GetComponent<CharacterBody>();

            List<CharacterModel.RendererInfo> rendererInfos = new List<CharacterModel.RendererInfo>();

            for (int i = 0; i < rendererInfo.Length; i++)
            {
                rendererInfos.Add(new CharacterModel.RendererInfo
                {
                    renderer = childLocator.FindChild(rendererInfo[i].childName).GetComponent<Renderer>(),
                    defaultMaterial = rendererInfo[i].material,
                    ignoreOverlays = rendererInfo[i].ignoreOverlays,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On
                });
            }

            characterModel.baseRendererInfos = rendererInfos.ToArray();

            characterModel.autoPopulateLightInfos = true;
            characterModel.invisibilityCount = 0;
            characterModel.temporaryOverlays = new List<TemporaryOverlay>();

            characterModel.mainSkinnedMeshRenderer = characterModel.baseRendererInfos[mainRendererIndex].renderer.GetComponent<SkinnedMeshRenderer>();
        }
        #endregion

        #region ComponentSetup
        private static void SetupCharacterDirection(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            CharacterDirection characterDirection = prefab.GetComponent<CharacterDirection>();
            characterDirection.targetTransform = modelBaseTransform;
            characterDirection.overrideAnimatorForwardTransform = null;
            characterDirection.rootMotionAccumulator = null;
            characterDirection.modelAnimator = modelTransform.GetComponent<Animator>();
            characterDirection.driveFromRootRotation = false;
            characterDirection.turnSpeed = 720f;
        }

        private static void SetupCameraTargetParams(GameObject prefab)
        {
            CameraTargetParams cameraTargetParams = prefab.GetComponent<CameraTargetParams>();
            cameraTargetParams.cameraParams = Resources.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponent<CameraTargetParams>().cameraParams;
            cameraTargetParams.cameraPivotTransform = prefab.transform.Find("ModelBase").Find("CameraPivot");
            cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            cameraTargetParams.recoil = Vector2.zero;
            cameraTargetParams.idealLocalCameraPos = Vector3.zero;
            cameraTargetParams.dontRaycastToPivot = false;
        }

        private static void SetupModelLocator(GameObject prefab, Transform modelBaseTransform, Transform modelTransform)
        {
            ModelLocator modelLocator = prefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = modelTransform;
            modelLocator.modelBaseTransform = modelBaseTransform;
        }

        private static void SetupRigidbody(GameObject prefab)
        {
            Rigidbody rigidbody = prefab.GetComponent<Rigidbody>();
            rigidbody.mass = 100f;
        }

        private static void SetupCapsuleCollider(GameObject prefab)
        {
            CapsuleCollider capsuleCollider = prefab.GetComponent<CapsuleCollider>();
            capsuleCollider.center = new Vector3(0f, 0f, 0f);
            capsuleCollider.radius = 0.5f;
            capsuleCollider.height = 1.82f;
            capsuleCollider.direction = 1;
        }

        private static void SetupMainHurtbox(GameObject prefab, GameObject model)
        {
            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            HurtBox mainHurtbox = childLocator.FindChild("MainHurtbox").gameObject.AddComponent<HurtBox>();
            mainHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
            mainHurtbox.healthComponent = prefab.GetComponent<HealthComponent>();
            mainHurtbox.isBullseye = true;
            mainHurtbox.damageModifier = HurtBox.DamageModifier.Normal;
            mainHurtbox.hurtBoxGroup = hurtBoxGroup;
            mainHurtbox.indexInGroup = 0;

            hurtBoxGroup.hurtBoxes = new HurtBox[]
            {
                mainHurtbox
            };

            hurtBoxGroup.mainHurtBox = mainHurtbox;
            hurtBoxGroup.bullseyeCount = 1;
        }

        private static void SetupFootstepController(GameObject model)
        {
            FootstepHandler footstepHandler = model.AddComponent<FootstepHandler>();
            footstepHandler.baseFootstepString = "Play_player_footstep";
            footstepHandler.sprintFootstepOverrideString = "";
            footstepHandler.enableFootstepDust = true;
            footstepHandler.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericFootstepDust");
        }

        private static void SetupRagdoll(GameObject model)
        {
            RagdollController ragdollController = model.GetComponent<RagdollController>();

            if (!ragdollController) return;

            if (ragdollMaterial == null) ragdollMaterial = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<RagdollController>().bones[1].GetComponent<Collider>().material;

            foreach (Transform i in ragdollController.bones)
            {
                if (i)
                {
                    i.gameObject.layer = LayerIndex.ragdoll.intVal;
                    Collider j = i.GetComponent<Collider>();
                    if (j)
                    {
                        j.material = ragdollMaterial;
                        j.sharedMaterial = ragdollMaterial;
                    }
                }
            }
        }

        private static void SetupAimAnimator(GameObject prefab, GameObject model)
        {
            AimAnimator aimAnimator = model.AddComponent<AimAnimator>();
            aimAnimator.directionComponent = prefab.GetComponent<CharacterDirection>();
            aimAnimator.pitchRangeMax = 60f;
            aimAnimator.pitchRangeMin = -60f;
            aimAnimator.yawRangeMin = -80f;
            aimAnimator.yawRangeMax = 80f;
            aimAnimator.pitchGiveupRange = 30f;
            aimAnimator.yawGiveupRange = 10f;
            aimAnimator.giveupDuration = 3f;
            aimAnimator.inputBank = prefab.GetComponent<InputBankTest>();
        }

        private static void SetupHitbox(GameObject prefab, Transform hitboxTransform, string hitboxName)
        {
            HitBoxGroup hitBoxGroup = prefab.AddComponent<HitBoxGroup>();

            HitBox hitBox = hitboxTransform.gameObject.AddComponent<HitBox>();
            hitboxTransform.gameObject.layer = LayerIndex.projectile.intVal;

            hitBoxGroup.hitBoxes = new HitBox[]
            {
                hitBox
            };

            hitBoxGroup.groupName = hitboxName;
        }
        #endregion
    }
}

// for simplifying characterbody creation
public class BodyInfo
{
    public string bodyName = "";
    public string bodyNameToken = "";
    public string subtitleNameToken = "";

    public Texture characterPortrait = null;
    public Color32 bodyColor = Color.gray;

    public GameObject crosshair = null;
    public GameObject podPrefab = null;

    public float maxHealth = 100f;
    public float healthGrowth = 2f;

    public float healthRegen = 0f;

    public float shield = 0f;// base shield is a thing apparently. neat
    public float shieldGrowth = 0f;

    public float moveSpeed = 7f;
    public float moveSpeedGrowth = 0f;

    public float acceleration = 80f;

    public float jumpPower = 15f;
    public float jumpPowerGrowth = 0f;// jump power per level exists for some reason

    public float damage = 12f;

    public float attackSpeed = 1f;
    public float attackSpeedGrowth = 0f;

    public float armor = 0f;
    public float armorGrowth = 0f;

    public float crit = 1f;
    public float critGrowth = 0f;

    public int jumpCount = 1;
}

// for simplifying rendererinfo creation
public class CustomRendererInfo
{
    public string childName;
    public Material material;
    public bool ignoreOverlays;
}