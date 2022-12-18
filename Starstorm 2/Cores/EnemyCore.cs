using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using RoR2.Skills;
using EntityStates;
using UnityEngine;
using UnityEngine.Networking;
using static R2API.DirectorAPI;
using R2API.Utils;
using RoR2.CharacterAI;
using KinematicCharacterController;
using UnityEngine.AddressableAssets;

namespace Starstorm2.Cores
{
    public class EnemyCore
    {
        public static GameObject wayfarerPrefab;
        public static GameObject masterPrefab;
        public static GameObject wayfarerBuffWardPrefab;

        public static BodyIndex brotherHurtIndex;
        public static BodyIndex brotherIndex;
        public static BodyIndex scavLunar1Index, scavLunar2Index, scavLunar3Index, scavLunar4Index;

        private static SceneDef sceneMoon = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/moon/moon.asset").WaitForCompletion();
        private static SceneDef sceneMoon2 = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/moon2/moon2.asset").WaitForCompletion();

        public static bool IsMoon()
        {
            SceneDef sd = SceneCatalog.GetSceneDefForCurrentScene();
            return sd && (sd == sceneMoon || sd == sceneMoon2);
        }

        public static void StoreBodyIndexes()
        {
            brotherHurtIndex = BodyCatalog.FindBodyIndex("BrotherHurtBody");
            brotherIndex = BodyCatalog.FindBodyIndex("BrotherBody");

            scavLunar1Index = BodyCatalog.FindBodyIndex("ScavLunar1Body");
            scavLunar2Index = BodyCatalog.FindBodyIndex("ScavLunar2Body");
            scavLunar3Index = BodyCatalog.FindBodyIndex("ScavLunar3Body");
            scavLunar4Index = BodyCatalog.FindBodyIndex("ScavLunar4Body");
        }

        //Super bootleg code.
        public static void FakeMithrixChatMessageServer(string token)
        {
            string fullMessage = "<color=#c6d5ff>";
            fullMessage += Language.GetString("BROTHER_BODY_NAME");
            fullMessage += ":";
            fullMessage += Language.GetString(token);
            fullMessage += "</color>";
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = fullMessage
            });
        }

        public EnemyCore()
        {
            CreatePrefab();
            CreateAI();
            SetUpSkills();
            RegisterStates();
            CreateSpawnCards();

            new Items.ShackledLamp().Init();
        }

        internal static void CreatePrefab()
        {
            wayfarerPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/CharacterBodies/GreaterWispBody"), "WayfarerBody", true);
            wayfarerPrefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

            UnityEngine.Object.Destroy(wayfarerPrefab.transform.Find("Model Base").gameObject);
            //UnityEngine.Object.Destroy(wayfarerPrefab.transform.Find("CameraPivot").gameObject);
            //UnityEngine.Object.Destroy(wayfarerPrefab.transform.Find("AimOrigin").gameObject);

            GameObject model = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("mdlWayfarer");

            //GameObject modelBase = wayfarerPrefab.transform.Find("Model Base?").gameObject;
            GameObject modelBase = new GameObject("ModelBase");
            modelBase.transform.parent = wayfarerPrefab.transform;
            modelBase.transform.localPosition = new Vector3(0, 8.5f, 0);
            modelBase.transform.localRotation = Quaternion.identity;
            modelBase.transform.localScale = Vector3.one;

            Transform transform = model.transform;
            transform.parent = modelBase.transform;
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            CharacterBody body = wayfarerPrefab.GetComponent<CharacterBody>();
            body.name = "WayfarerBody";
            body.baseNameToken = "WAYFARER_BODY_NAME";
            body.subtitleNameToken = "WAYFARER_SUBTITLE";
            body.isChampion = true;
            body.portraitIcon = Modules.Assets.mainAssetBundle.LoadAsset<Texture>("WayfarerIcon");
            body.baseMaxHealth = 2100;
            body.levelMaxHealth = 630;
            body.baseArmor = 20f;
            body.baseDamage = 10f;
            body.levelDamage = 5.4f;
            body.baseMoveSpeed = 8.5f;
            body.hullClassification = HullClassification.Golem;
            body.mainRootSpeed = 0;

            body.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(GenericCharacterMain));

            DeathRewards reward = wayfarerPrefab.GetComponent<DeathRewards>();
            reward.bossPickup = new SerializablePickupIndex() { pickupName = "ItemIndex.Lamp" };
            //reward.goldReward
            //reward.expReward

            CameraTargetParams camera = wayfarerPrefab.AddOrGetComponent<CameraTargetParams>();
            camera.cameraParams = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/TitanBody").GetComponent<CameraTargetParams>().cameraParams;

            ModelLocator modelLocator = wayfarerPrefab.GetComponent<ModelLocator>();
            modelLocator.modelTransform = transform;
            modelLocator.modelBaseTransform = modelBase.transform;
            modelLocator.autoUpdateModelTransform = true;

            CharacterModel charModel = model.AddComponent<CharacterModel>();
            charModel.body = body;
            charModel.baseRendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = model.GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = model.GetComponentInChildren<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("Head").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("Head").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("LanternL").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("LanternL").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("LanternR").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("LanternR").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("ChainL1").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("ChainL1").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("ChainL2").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("ChainL2").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("ChainR1").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("ChainR1").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("ChainR2").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("ChainR2").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("LNeckChain").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("LNeckChain").GetComponent<SkinnedMeshRenderer>()
                },
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = childLocator.FindChild("RNeckChain").GetComponentInChildren<SkinnedMeshRenderer>().material,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false,
                    renderer = childLocator.FindChild("RNeckChain").GetComponent<SkinnedMeshRenderer>()
                },
            };
            charModel.autoPopulateLightInfos = true;
            charModel.temporaryOverlays = new List<TemporaryOverlay>();

            TeamComponent team = wayfarerPrefab.AddOrGetComponent<TeamComponent>();
            team.body = body;

            CharacterDeathBehavior deathBehavior = wayfarerPrefab.GetComponent<CharacterDeathBehavior>();
            deathBehavior.deathStateMachine = wayfarerPrefab.GetComponent<EntityStateMachine>();
            deathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));

            Rigidbody rigidbody = wayfarerPrefab.GetComponent<Rigidbody>();
            rigidbody.freezeRotation = true;

            RigidbodyDirection rigidbodyDirection = wayfarerPrefab.GetComponent<RigidbodyDirection>();
            //rigidbodyDirection.animator = animator;
            rigidbodyDirection.freezeXRotation = true;
            rigidbodyDirection.freezeYRotation = false;
            rigidbodyDirection.freezeZRotation = true;
            //rigidbodyDirection.inputBank = wayfarerPrefab.GetComponent<InputBankTest>();
            //rigidbodyDirection.rigid = rigidbody;

            UnityEngine.Object.Destroy(wayfarerPrefab.GetComponent<SphereCollider>());
            CapsuleCollider collider = wayfarerPrefab.AddComponent<CapsuleCollider>();
            //collider.transform.localPosition = model.transform.Find("Collider").localPosition;
            //collider.transform.localPosition = modelBase.transform.localPosition;
            collider.center = new Vector3(0, 8.35f, 0);
            collider.radius = 4.24f;
            collider.height = 16.52f;
            collider.direction = 1;

            HealthComponent health = wayfarerPrefab.GetComponent<HealthComponent>();

            HurtBoxGroup hurtBoxGroup = model.AddComponent<HurtBoxGroup>();
            hurtBoxGroup.bullseyeCount = 1;

            var hurtboxes = new List<HurtBox>();
            CapsuleCollider[] hurtcolliders = model.GetComponentsInChildren<CapsuleCollider>();
            for (int i = 0; i < hurtcolliders.Length; i++)
            {
                HurtBox hurtbox = hurtcolliders[i].gameObject.AddOrGetComponent<HurtBox>();
                hurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                hurtbox.healthComponent = health;
                if (i == 0)
                    hurtbox.isBullseye = true;
                hurtbox.hurtBoxGroup = hurtBoxGroup;
                hurtboxes.Add(hurtbox);
            }
            hurtBoxGroup.hurtBoxes = hurtboxes.ToArray();
            hurtBoxGroup.mainHurtBox = hurtcolliders[0].GetComponent<HurtBox>();

            wayfarerBuffWardPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/AffixHauntedWard").InstantiateClone("WayfarerBuffWard", true);
            NetworkedBodyAttachment a = wayfarerBuffWardPrefab.AddOrGetComponent<NetworkedBodyAttachment>();
            a.shouldParentToAttachedBody = true;
            BuffWard ward = wayfarerBuffWardPrefab.GetComponent<BuffWard>();
            ward.buffDuration = 8.0f;
            ward.buffDef = RoR2Content.Buffs.WarCryBuff;
            ward.Networkradius = States.Wayfarer.CreateBuffWard.radius + body.radius;

            On.RoR2.CharacterBody.AddBuff_BuffDef += CharacterBody_AddBuff_BuffDef;
            On.RoR2.Projectile.HookProjectileImpact.FixedUpdate += HookProjectileImpact_FixedUpdate;

            LanguageAPI.Add("WAYFARER_BODY_NAME", "Wayfarer");
            LanguageAPI.Add("WAYFARER_SUBTITLE", "Usher of Light");

            //TODO: migrate wayfarer prefab code to prefabcore while consuming unhealthy amounts of liquor
            PrefabCore.bodyList.Add(wayfarerPrefab);
        }

        private static void CharacterBody_AddBuff_BuffDef(On.RoR2.CharacterBody.orig_AddBuff_BuffDef orig, CharacterBody self, BuffDef buffDef)
        {
            if (self.bodyIndex == BodyCatalog.FindBodyIndex(wayfarerPrefab) && buffDef == RoR2Content.Buffs.WarCryBuff)
                return;
            orig(self, buffDef);
        }

        private static void HookProjectileImpact_FixedUpdate(On.RoR2.Projectile.HookProjectileImpact.orig_FixedUpdate orig, RoR2.Projectile.HookProjectileImpact self)
        {
            if (self.hookState == RoR2.Projectile.HookProjectileImpact.HookState.Reel && 
                self.projectileController.Networkowner?.GetComponent<CharacterBody>()?.bodyIndex == BodyCatalog.FindBodyIndex("WayfarerBody"))
            { 
                UnityEngine.Object.Destroy(self);
            }
            else
                orig(self);
        }

        internal static void CreateAI()
        {
            masterPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/CharacterMasters/AncientWispMaster"), "WayfarerMaster", true);
            CharacterMaster master = masterPrefab.AddOrGetComponent<CharacterMaster>();
            master.bodyPrefab = wayfarerPrefab;

            Modules.Prefabs.masterPrefabs.Add(masterPrefab);

            BaseAI ai = masterPrefab.GetComponent<BaseAI>();
            //ai.drawAIPath = true;
            ai.fullVision = true;
            ai.enemyAttentionDuration = 6000;
            //ai.minDistanceFromEnemy = 0;
            //ai.navigationType = BaseAI.NavigationType.Nodegraph;

            foreach (AISkillDriver driver in master.GetComponentsInChildren<AISkillDriver>())
                UnityEngine.Object.Destroy(driver);

            AISkillDriver skill1 = master.AddComponent<AISkillDriver>();
            skill1.activationRequiresTargetLoS = false;
            skill1.aimType = AISkillDriver.AimType.AtMoveTarget;
            skill1.maxDistance = 50;
            skill1.minDistance = 0;
            skill1.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            skill1.requireSkillReady = true;
            skill1.selectionRequiresTargetLoS = true;
            skill1.skillSlot = SkillSlot.Primary;
            //skill1.driverUpdateTimerOverride

            AISkillDriver skill2 = master.AddComponent<AISkillDriver>();
            skill2.maxDistance = 100f;
            skill2.minDistance = 0;
            skill2.maxUserHealthFraction = 0.5f;
            skill2.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            skill2.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            skill2.requireSkillReady = true;
            skill2.skillSlot = SkillSlot.Secondary;

            AISkillDriver skill3 = master.AddComponent<AISkillDriver>();
            skill3.maxDistance = States.Wayfarer.CreateBuffWard.radius + 5;
            skill3.minDistance = 0;
            skill3.movementType = AISkillDriver.MovementType.Stop;
            skill3.moveTargetType = AISkillDriver.TargetType.NearestFriendlyInSkillRange;
            skill3.requireSkillReady = true;
            skill3.skillSlot = SkillSlot.Utility;

            AISkillDriver chase = master.AddComponent<AISkillDriver>();
            chase.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            chase.minDistance = 0;
            chase.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            chase.skillSlot = SkillSlot.None;
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in wayfarerPrefab.GetComponentsInChildren<GenericSkill>())
                UnityEngine.Object.DestroyImmediate(sk);

            SetUpPrimary();
            SetUpSecondary();
            SetUpUtility();
        }

        //primary - melee
        private void SetUpPrimary()
        {
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(States.Wayfarer.FireChains));
            def.activationStateMachineName = "Weapon";
            def.baseMaxStock = 1;
            def.baseRechargeInterval = 3f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Any;
            def.isCombatSkill = true;
            def.mustKeyPress = false;
            def.cancelSprintingOnActivation = false;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;
            //def.icon = LegacyResourcesAPI.Load<Sprite>("notanicon");

            Modules.Skills.skillDefs.Add(def);

            var skillLocator = wayfarerPrefab.GetComponent<SkillLocator>();
            skillLocator.primary = wayfarerPrefab.AddComponent<GenericSkill>();

            SkillFamily primaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            primaryFamily.variants = new SkillFamily.Variant[1];
            Modules.Skills.skillFamilies.Add(primaryFamily);
            skillLocator.primary.SetFieldValue("_skillFamily", primaryFamily);
            primaryFamily = skillLocator.primary.skillFamily;
            primaryFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false)
            };
        }

        //secondary - invis
        private void SetUpSecondary()
        {
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(States.Wayfarer.CloakState));
            def.activationStateMachineName = "Weapon";
            def.baseMaxStock = 1;
            def.baseRechargeInterval = 16f;
            def.beginSkillCooldownOnSkillEnd = true;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Skill;
            def.isCombatSkill = false;
            def.mustKeyPress = false;
            def.cancelSprintingOnActivation = true;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;

            Modules.Skills.skillDefs.Add(def);

            var skillLocator = wayfarerPrefab.GetComponent<SkillLocator>();
            skillLocator.secondary = wayfarerPrefab.AddComponent<GenericSkill>();

            SkillFamily secondaryFamily = ScriptableObject.CreateInstance<SkillFamily>();
            secondaryFamily.variants = new SkillFamily.Variant[1];
            Modules.Skills.skillFamilies.Add(secondaryFamily);
            skillLocator.secondary.SetFieldValue("_skillFamily", secondaryFamily);
            secondaryFamily = skillLocator.secondary.skillFamily;
            secondaryFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false)
            };
        }

        //utility - buff ward
        private void SetUpUtility()
        {
            SkillDef def = ScriptableObject.CreateInstance<SkillDef>();
            def.activationState = new SerializableEntityStateType(typeof(States.Wayfarer.CreateBuffWard));
            def.activationStateMachineName = "Weapon";
            def.baseMaxStock = 1;
            def.baseRechargeInterval = 9f;
            def.beginSkillCooldownOnSkillEnd = false;
            def.canceledFromSprinting = false;
            def.fullRestockOnAssign = true;
            def.interruptPriority = InterruptPriority.Any;
            def.isCombatSkill = true;
            def.mustKeyPress = false;
            def.cancelSprintingOnActivation = false;
            def.rechargeStock = 1;
            def.requiredStock = 1;
            def.stockToConsume = 1;

            Modules.Skills.skillDefs.Add(def);

            var skillLocator = wayfarerPrefab.GetComponent<SkillLocator>();
            skillLocator.utility = wayfarerPrefab.AddComponent<GenericSkill>();

            SkillFamily utilityFamily = ScriptableObject.CreateInstance<SkillFamily>();
            utilityFamily.variants = new SkillFamily.Variant[1];
            Modules.Skills.skillFamilies.Add(utilityFamily);
            skillLocator.utility.SetFieldValue("_skillFamily", utilityFamily);
            utilityFamily = skillLocator.utility.skillFamily;
            utilityFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = def,
                viewableNode = new ViewablesCatalog.Node(def.skillNameToken, false)
            };
        }

        internal static void RegisterStates()
        {
        }

        internal static void CreateSpawnCards()
        {
            CharacterSpawnCard wfCard = ScriptableObject.CreateInstance<CharacterSpawnCard>();
            wfCard.name = "WayfarerCSC";
            wfCard.prefab = masterPrefab;
            wfCard.directorCreditCost = 600;
            wfCard.hullSize = HullClassification.Golem;
            wfCard.nodeGraphType = RoR2.Navigation.MapNodeGroup.GraphType.Ground;
            wfCard.occupyPosition = false;

            DirectorCard wfDirCard = new DirectorCard();
            wfDirCard.selectionWeight = 1;
            wfDirCard.spawnCard = wfCard;
            wfDirCard.spawnDistance = DirectorCore.MonsterSpawnDistance.Standard;

            DirectorCardHolder wfHolder = new DirectorCardHolder();
            wfHolder.Card = wfDirCard;
            wfHolder.InteractableCategory = default;
            wfHolder.MonsterCategory = MonsterCategory.Champions;

            /*MonsterActions += delegate (List<DirectorCardHolder> list, StageInfo stage)
            {
                var currStg = stage.stage;
                bool canSpawn = (currStg == DirectorAPI.Stage.RallypointDelta || currStg == DirectorAPI.Stage.ScorchedAcres
                || currStg == DirectorAPI.Stage.AbyssalDepths || currStg == DirectorAPI.Stage.SirensCall || stage.CustomStageName == "rootjungle"
                || currStg == DirectorAPI.Stage.SkyMeadow
                || currStg == DirectorAPI.Stage.VoidCell || currStg == DirectorAPI.Stage.ArtifactReliquary);
                bool canSpawnLoop = Run.instance.loopClearCount > 1 && (currStg == DirectorAPI.Stage.DistantRoost);
                if ((canSpawn || canSpawnLoop) && !list.Contains(wfHolder))
                    list.Add(wfHolder);
            };*/
        }
    }
}
