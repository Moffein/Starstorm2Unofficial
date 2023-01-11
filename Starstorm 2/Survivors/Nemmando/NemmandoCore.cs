using BepInEx.Configuration;
using EntityStates;
using EntityStates.SS2UStates.Nemmando;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Projectile;
using RoR2.Skills;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Cores.NemesisInvasion;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Survivors;
using Starstorm2Unofficial.Survivors.Nemmando.Components;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Nemmando
{
    internal class NemmandoCore : SurvivorBase
    {
        public static BodyIndex bodyIndex;

        internal override string bodyName { get; set; } = "SS2UNemmando";
        internal override string modelName { get; set; } = "mdlNemmando";
        internal override string displayName { get; set; } = "NemmandoDisplay";

        internal override GameObject bodyPrefab { get; set; }
        internal override GameObject displayPrefab { get; set; }

        internal static GameObject bossMasterPrefab { get; set; }
        internal GameObject bossBodyPrefab { get; set; }

        internal override float sortPosition { get; set; } = 1.001f;

        internal override StarstormBodyInfo bodyInfo { get; set; } = new StarstormBodyInfo
        {
            armor = 20f,
            bodyName = "SS2UNemmandoBody",
            bodyNameToken = "SS2UNEMMANDO_NAME",
            bodyColor = new Color(1.0f, 0.20f, 0.20f),
            characterPortrait = Modules.Assets.LoadCharacterIcon("Nemmando"),
            crosshair = Modules.Assets.LoadCrosshair("Standard"),
            damage = 12f,
            healthGrowth = 33f,
            healthRegen = 2.5f,
            jumpCount = 1,
            maxHealth = 110f,
            subtitleNameToken = "SS2UNEMMANDO_SUBTITLE",
            menuSoundString = "SS2UNemmandoCSS"
        };

        internal static Material nemmandoMat = Modules.Assets.CreateMaterial("matNemmando");
        internal static Material swordMat = Modules.Assets.CreateMaterial("matNemmando");
        internal override int mainRendererIndex { get; set; } = 0;

        internal override CustomRendererInfo[] customRendererInfos { get; set; } = new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = nemmandoMat
                },
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = swordMat,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "GunModel",
                    material = nemmandoMat,
                    ignoreOverlays = true
                },
                new CustomRendererInfo
                {
                    childName = "CoatModel",
                    material = Modules.Assets.CreateMaterial("matVergil", 1f)
                }};

        internal override Type characterMainState { get; set; } = typeof(NemmandoMain);

        // item display stuffs
        internal override ItemDisplayRuleSet itemDisplayRuleSet { get; set; }
        internal override List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules { get; set; }

        internal override UnlockableDef characterUnlockableDef { get; set; } = CreateUnlockableDef();
        public static UnlockableDef survivorUnlock;
        public static UnlockableDef killSelfUnlockableDef;

        private static void CreateKillSelfUnlockable()
        {
            if (!NemmandoCore.killSelfUnlockableDef)
            {
                NemmandoCore.killSelfUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
                NemmandoCore.killSelfUnlockableDef.cachedName = "Skins.SS2UNemmando.Commando";
                NemmandoCore.killSelfUnlockableDef.nameToken = "ACHIEVEMENT_SS2UNEMMANDOKILLSELF_NAME";
                NemmandoCore.killSelfUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSingleTapUnlockIcon");

                SkinDef sd = Addressables.LoadAssetAsync<SkinDef>("RoR2/Base/Commando/skinCommandoDefault.asset").WaitForCompletion();
                if (sd && sd.icon) NemmandoCore.killSelfUnlockableDef.achievementIcon = sd.icon;

                Modules.Unlockables.unlockableDefs.Add(NemmandoCore.killSelfUnlockableDef);
            }
        }

        private static UnlockableDef CreateUnlockableDef()
        {
            if (!survivorUnlock)
            {
                survivorUnlock = ScriptableObject.CreateInstance<UnlockableDef>();
                survivorUnlock.cachedName = "Characters.SS2UNemmando";
                survivorUnlock.nameToken = "ACHIEVEMENT_SS2UNEMMANDOUNLOCK_NAME";
                survivorUnlock.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoIconUnlock");

                Modules.Unlockables.unlockableDefs.Add(survivorUnlock);
            }

            if (Modules.Config.EnableUnlockAll.Value || !Modules.Config.EnableVoid.Value) return null;

            return survivorUnlock;
        }

        private static UnlockableDef masterySkinUnlockableDef;
        private static UnlockableDef grandMasterySkinUnlockableDef;

        private static UnlockableDef singleTapUnlockableDef;    //todo: reimplement
        private static UnlockableDef decisiveStrikeUnlockableDef;   //todo: reimplement

        // ugh
        internal static SkillDef decisiveStrikeSkillDef;
        internal static SkillDef scepterDecisiveStrikeSkillDef;
        internal static SkillDef scepterSubmissionSkillDef;

        //Used by NemmandoController. Todo: organize and rename
        public static SkillDef secondaryDistantGash;
        public static SkillDef specialDecisiveStrike;
        public static SkillDef specialSubmission;

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UNemmandoBody");

            BodyIndex monsterBodyIndex = BodyCatalog.FindBodyIndex("SS2UNemmandoMonsterBody");
            if (monsterBodyIndex != BodyIndex.None)
            {
                NemesisInvasionCore.prioritizePlayersList.Add(monsterBodyIndex);
            }
        }
        internal override void InitializeCharacter()
        {
            base.InitializeCharacter();
            R2API.ItemAPI.DoNotAutoIDRSFor(bodyPrefab);

            SetupSwordProjectile();
            SetupLaserTracer();

            RoR2.RoR2Application.onLoad += SetBodyIndex;
            Modules.Assets.LoadNemmandoEffects();

            bodyPrefab.AddComponent<Components.NemmandoController>();
            bodyPrefab.GetComponent<EntityStateMachine>().initialStateType = new SerializableEntityStateType(typeof(EntityStates.SS2UStates.Common.NemmandoSpawnState));
            bodyPrefab.AddComponent<CustomEffectComponent>();

            bodyPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<Components.SS2CharacterAnimationEvents>();

            bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("GunReloadEffect").GetChild(0).GetComponent<ParticleSystemRenderer>().material = Modules.Assets.CreateMaterial("matAmmo");

            InitializeBoss();
            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SS2UNemmandoBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animNemmandoEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }

        private void SetupSwordProjectile()
        {

            GameObject swordBeam = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FMJ"), "SS2UNemmandoSwordBeam", true);
            swordBeam.transform.localScale = new Vector3(4.5f, 2.5f, 2.5f);

            GameObject swordBeamGhost = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EvisProjectile").GetComponent<ProjectileController>().ghostPrefab, "SS2UNemmandoSwordBeamGhost", false);
            foreach (ParticleSystemRenderer i in swordBeamGhost.GetComponentsInChildren<ParticleSystemRenderer>())
            {
                if (i)
                {
                    Material mat = UnityEngine.Object.Instantiate<Material>(i.material);
                    mat.SetColor("_TintColor", Color.red);
                    i.material = mat;
                }
            }
            foreach (Light i in swordBeamGhost.GetComponentsInChildren<Light>())
            {
                if (i)
                {
                    i.color = Color.red;
                }
            }

            swordBeam.GetComponent<ProjectileController>().ghostPrefab = swordBeamGhost;
            swordBeam.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;

            DamageAPI.ModdedDamageTypeHolderComponent moddedDamage = swordBeam.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            moddedDamage.Add(Cores.DamageTypeCore.ModdedDamageTypes.GougeOnHit);

            StarstormPlugin.Destroy(swordBeam.transform.Find("SweetSpotBehavior").gameObject);

            Modules.Prefabs.projectilePrefabs.Add(swordBeam);

            FireSwordBeam.projectilePrefab = swordBeam;
        }

        private void SetupLaserTracer()
        {
            GameObject laserTracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("SS2UNemmandoLaserTracer", true);

            foreach (LineRenderer i in laserTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    Material material = UnityEngine.Object.Instantiate<Material>(i.material);
                    material.SetColor("_TintColor", Color.red);
                    i.material = material;
                    i.startColor = new Color(0.8f, 0.19f, 0.19f);
                    i.endColor = new Color(0.8f, 0.19f, 0.19f);
                }
            }

            Modules.Assets.AddEffect(laserTracer);
            ChargeBarrageFire.tracerEffectPrefab = laserTracer;
            ScepterBarrageFire.tracerEffectPrefab = laserTracer;
        }

        private void InitializeBoss()
        {
            bossBodyPrefab = PrefabAPI.InstantiateClone(bodyPrefab, "SS2UNemmandoMonsterBody", true);

            var body = bossBodyPrefab.GetComponent<CharacterBody>();
            body.baseMaxHealth = 3200f;
            body.levelMaxHealth =  960f;
            body.baseRegen = 0;
            body.levelRegen = 0;
            body.baseDamage = 3f;
            body.levelDamage = 0.6f;
            body.isChampion = true;

            R2API.ItemAPI.DoNotAutoIDRSFor(bossBodyPrefab);

            //does this even work?
            if (body.mainHurtBox)
            {
                (body.mainHurtBox.collider as CapsuleCollider).radius *= 1.5f;
            }

            ModelLocator ml = bossBodyPrefab.GetComponent<ModelLocator>();
            if (ml && ml.modelTransform)
            {
                ml.modelTransform.localScale *= 2f;
            }

            bossMasterPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/MercMonsterMaster"), "SS2UNemmandoBossMaster", true);
            bossMasterPrefab.GetComponent<CharacterMaster>().bodyPrefab = bossBodyPrefab;

            GameObject bossEffect = GameObject.Instantiate(Modules.Assets.nemmandoBossFX);
            bossEffect.transform.parent = bossBodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("Chest");
            bossEffect.transform.localPosition = Vector3.zero;
            bossEffect.transform.GetChild(0).localPosition = Vector3.zero;
            bossEffect.transform.localScale = Vector3.one * 2f;
            bossEffect.transform.GetChild(0).localScale = Vector3.one * 2f;

            bossBodyPrefab.AddComponent<Components.NemmandoUnlockComponent>();
            bossBodyPrefab.AddComponent<Components.NemmandoSpecialSwapComponent>();
            bossBodyPrefab.AddComponent<Starstorm2Unofficial.Components.NemesisBossComponent>();

            Modules.Prefabs.bodyPrefabs.Add(bossBodyPrefab);
            Modules.Prefabs.masterPrefabs.Add(bossMasterPrefab);

            Modules.Prefabs.RemoveAISkillDrivers(bossMasterPrefab);
            Modules.Prefabs.AddAISkillDriver(bossMasterPrefab, "Special", SkillSlot.Special, null,
                  true, false,
                  Mathf.NegativeInfinity, Mathf.Infinity,
                  Mathf.NegativeInfinity, Mathf.Infinity,
                  0f, 30f,
                  true, false, false, -1,
                  AISkillDriver.TargetType.CurrentEnemy,
                  true, false, false,
                  AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                  AISkillDriver.AimType.AtCurrentEnemy,
                  true,
                  false,
                  false,
                  AISkillDriver.ButtonPressType.Hold,
                  3f,
                  false,
                  true,
                  null);

            Modules.Prefabs.AddAISkillDriver(bossMasterPrefab, "Secondary", SkillSlot.Secondary, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 10f, 40f,
                 true, false, true, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 true, true, true,
                 AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                 AISkillDriver.AimType.AtCurrentEnemy,
                 false,
                 true,
                 false,
                 AISkillDriver.ButtonPressType.Hold,
                 1f,
                 false,
                 false,
                 null);

            Modules.Prefabs.AddAISkillDriver(bossMasterPrefab, "Roll", SkillSlot.Utility, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                15f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                0.5f,
                false,
                true,
                null);

            Modules.Prefabs.AddAISkillDriver(bossMasterPrefab, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 12f,
                true, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1f,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(bossMasterPrefab, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);

            NemesisInvasionCore.AddNemesisBoss(bossMasterPrefab, null, "ShinyPearl", true, true); //Replace if Stirring Soul ever gets fixed
        }

        internal override void InitializeUnlockables()
        {
            masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UNemmando.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UNEMMANDOCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinMaster");
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);

            grandMasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandMasterySkinUnlockableDef.cachedName = "Skins.SS2UNemmando.GrandMastery";
            grandMasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UNEMMANDOCLEARGAMETYPHOON_NAME";
            grandMasterySkinUnlockableDef.achievementIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinGrandMaster");
            Unlockables.unlockableDefs.Add(grandMasterySkinUnlockableDef);

            //Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoIconUnlock");
            //Assets.mainAssetBundle.LoadAsset<Sprite>("texSingleTapUnlockIcon");
            //Assets.mainAssetBundle.LoadAsset<Sprite>("texDecisiveStrike");
        }

        internal override void InitializeDoppelganger()
        {
            GameObject masterPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "SS2UNemmandoMonsterMaster", true);
            masterPrefab.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;
            Modules.Prefabs.masterPrefabs.Add(masterPrefab);

            Modules.Prefabs.RemoveAISkillDrivers(masterPrefab);
            Modules.Prefabs.AddAISkillDriver(masterPrefab, "Special", SkillSlot.Special, null,
                  true, false,
                  Mathf.NegativeInfinity, Mathf.Infinity,
                  Mathf.NegativeInfinity, Mathf.Infinity,
                  0f, 30f,
                  true, false, false, -1,
                  AISkillDriver.TargetType.CurrentEnemy,
                  true, false, false,
                  AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                  AISkillDriver.AimType.AtCurrentEnemy,
                  true,
                  false,
                  false,
                  AISkillDriver.ButtonPressType.Hold,
                  3f,
                  false,
                  true,
                  null);

            Modules.Prefabs.AddAISkillDriver(masterPrefab, "Secondary", SkillSlot.Secondary, null,
                 true, false,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 Mathf.NegativeInfinity, Mathf.Infinity,
                 10f, 40f,
                 true, false, true, -1,
                 AISkillDriver.TargetType.CurrentEnemy,
                 true, true, true,
                 AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                 AISkillDriver.AimType.AtCurrentEnemy,
                 false,
                 true,
                 false,
                 AISkillDriver.ButtonPressType.Hold,
                 1f,
                 false,
                 false,
                 null);

            Modules.Prefabs.AddAISkillDriver(masterPrefab, "Roll", SkillSlot.Utility, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                15f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                0.5f,
                false,
                true,
                null);

            Modules.Prefabs.AddAISkillDriver(masterPrefab, "Primary", SkillSlot.Primary, null,
                true, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, 12f,
                true, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                true, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Hold,
                -1f,
                false,
                false,
                null);

            Modules.Prefabs.AddAISkillDriver(masterPrefab, "Chase", SkillSlot.None, null,
                false, false,
                Mathf.NegativeInfinity, Mathf.Infinity,
                Mathf.NegativeInfinity, Mathf.Infinity,
                0f, Mathf.Infinity,
                false, false, false, -1,
                AISkillDriver.TargetType.CurrentEnemy,
                false, false, false,
                AISkillDriver.MovementType.ChaseMoveTarget, 1f,
                AISkillDriver.AimType.AtCurrentEnemy,
                false,
                true,
                false,
                AISkillDriver.ButtonPressType.Abstain,
                -1,
                false,
                false,
                null);
        }

        internal override void InitializeHitboxes()
        {
            ChildLocator childLocator = bodyPrefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SwordHitbox");

            hitboxTransform = childLocator.FindChild("SwordHitboxLarge");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SwordHitboxLarge");

            hitboxTransform = childLocator.FindChild("SwordHitboxWide");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SwordHitboxWide");
        }

        internal override void InitializeSkills()
        {
            RegisterStates();

            Modules.Skills.CreateSkillFamilies(bodyPrefab);

            #region Primary
            SkillDef m1SkillDef = Modules.Skills.CreatePrimarySkillDef(new SerializableEntityStateType(typeof(BladeOfCessation2)), "Weapon", "SS2UNEMMANDO_PRIMARY_BLADE_NAME", "SS2UNEMMANDO_PRIMARY_BLADE_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBladeOfCessation"), true);
            m1SkillDef.keywordTokens = new string[] { "KEYWORD_SS2U_GOUGE", "KEYWORD_AGILE" };

            Modules.Skills.AddPrimarySkill(bodyPrefab, m1SkillDef);
            #endregion

            #region Secondary
            SkillDef gashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SECONDARY_CONCUSSION_NAME",
                skillNameToken = "SS2UNEMMANDO_SECONDARY_CONCUSSION_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SECONDARY_CONCUSSION_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPhaseCharge"),
                activationState = new SerializableEntityStateType(typeof(ChargeSwordBeam)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SS2U_GOUGE" }
            });
            secondaryDistantGash = gashSkillDef;

            SkillDef gunSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SECONDARY_SHOOT_NAME",
                skillNameToken = "SS2UNEMMANDO_SECONDARY_SHOOT_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SECONDARY_SHOOT_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSubmission"),
                activationState = new SerializableEntityStateType(typeof(ShootGun)),
                activationStateMachineName = "Slide",
                baseMaxStock = 6,
                baseRechargeInterval = 2f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 6,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] {}
            });

            Modules.Skills.AddSecondarySkill(bodyPrefab, gashSkillDef);
            Modules.Skills.AddSecondarySkill(bodyPrefab, gunSkillDef);//, singleTapUnlockableDef
            #endregion

            #region Utility
            SkillDef dashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_UTILITY_DODGE_NAME",
                skillNameToken = "SS2UNEMMANDO_UTILITY_DODGE_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_UTILITY_DODGE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texTacticalRoll"),
                activationState = new SerializableEntityStateType(typeof(DodgeState)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Modules.Skills.AddUtilitySkills(bodyPrefab, dashSkillDef);
            #endregion

            #region Special
            SkillDef submissionSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SPECIAL_SUBMISSION_NAME",
                skillNameToken = "SS2UNEMMANDO_SPECIAL_SUBMISSION_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SPECIAL_SUBMISSION_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSubmission"),
                activationState = new SerializableEntityStateType(typeof(ChargeBarrageCharge)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });
            specialSubmission = submissionSkillDef;

            decisiveStrikeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SPECIAL_EPIC_NAME",
                skillNameToken = "SS2UNEMMANDO_SPECIAL_EPIC_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SPECIAL_EPIC_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDecisiveStrike"),
                activationState = new SerializableEntityStateType(typeof(ChargedSlashCharge)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SS2U_GOUGE" }
            });
            specialDecisiveStrike = decisiveStrikeSkillDef;

            Modules.Skills.AddSpecialSkill(bodyPrefab, submissionSkillDef);
            Modules.Skills.AddSpecialSkill(bodyPrefab, decisiveStrikeSkillDef);//, decisiveStrikeUnlockableDef
            #endregion

            SetupScepterSkills();
            if (StarstormPlugin.scepterPluginLoaded)
            {
                ScepterSetup();
            }
            if (StarstormPlugin.classicItemsLoaded)
            {
                ClassicScepterSetup();
            }
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(EntityStates.SS2UStates.Common.NemmandoSpawnState));
            Modules.States.AddState(typeof(NemmandoMain));
            Modules.States.AddState(typeof(BladeOfCessation2));
            Modules.States.AddState(typeof(ChargeSwordBeam));
            Modules.States.AddState(typeof(DodgeState));
            Modules.States.AddState(typeof(Submission));
            Modules.States.AddState(typeof(ChargedSlashCharge));
            Modules.States.AddState(typeof(ScepterSlashCharge));
            Modules.States.AddState(typeof(ScepterSlashEntry));
            Modules.States.AddState(typeof(ScepterSlashAttack));
            Modules.States.AddState(typeof(ScepterBarrageCharge));
            Modules.States.AddState(typeof(ScepterBarrageFire));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ScepterSetup()
        {

            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterSubmissionSkillDef, bodyInfo.bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(scepterDecisiveStrikeSkillDef, bodyInfo.bodyName, SkillSlot.Special, 1);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void ClassicScepterSetup()
        {
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(scepterSubmissionSkillDef, bodyInfo.bodyName, SkillSlot.Special, specialSubmission);
            ThinkInvisible.ClassicItems.Scepter.instance.RegisterScepterSkill(scepterDecisiveStrikeSkillDef, bodyInfo.bodyName, SkillSlot.Special, specialDecisiveStrike);
        }

        private static void SetupScepterSkills()
        {
            scepterSubmissionSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SPECIAL_SCEPSUBMISSION_NAME",
                skillNameToken = "SS2UNEMMANDO_SPECIAL_SCEPSUBMISSION_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SPECIAL_SCEPSUBMISSION_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSubmissionScepter"),
                activationState = new SerializableEntityStateType(typeof(ScepterBarrageCharge)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            scepterDecisiveStrikeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = "SS2UNEMMANDO_SPECIAL_SCEPEPIC_NAME",
                skillNameToken = "SS2UNEMMANDO_SPECIAL_SCEPEPIC_NAME",
                skillDescriptionToken = "SS2UNEMMANDO_SPECIAL_SCEPEPIC_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDecisiveStrikeScepter"),
                activationState = new SerializableEntityStateType(typeof(ScepterSlashCharge)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[] { "KEYWORD_SS2U_GOUGE" }
            });
        }

        internal override void RegisterTokens()
        {
            LanguageAPI.Add("SS2UNEMMANDO_NAME", "Nemesis Commando");
            LanguageAPI.Add("SS2UNEMMANDO_SUBTITLE", "Lone Echo");
            LanguageAPI.Add("SS2UNEMMANDO_DESCRIPTION", "The Nemesis Commando's origins are unknown, but something is clear: he's no longer who he once was.\r\n\r\n" +
                "< ! > Blade of Cessation is risky but can build up a lot of damage by stacking Gouge.\r\n\r\n" +
                "< ! > Distant Gash can be used to safely apply stacks of Gouge to groups of enemies - take advantage of its piercing!\r\n\r\n" +
                "< ! > Proper balance of melee and ranged combat is key, and Tactical Roll grants high armor to help enter and exit battles.\r\n\r\n" +
                "< ! > Submission deals massive damage when fully charged.\r\n\r\n");
            LanguageAPI.Add("SS2UNEMMANDO_OUTRO_FLAVOR", "..and so he left, newfound sense of humanity within.");
            LanguageAPI.Add("SS2UNEMMANDO_OUTRO_FAILURE", "..and so he returned, concluding his endless struggle.");
            LanguageAPI.Add("SS2UNEMMANDO_LORE", "He lays to rest by a fire, setting the weapons he carries within an arm's reach. Checking his surroundings, hoping the cave he hid in would be enough shelter. Some would say, it isn't wise to be overly cautious. He would say, <color=#FFCCED>they haven't seen enough.</color>\n\n" +
                "The service pistol he got when he enlisted. Well used, dirtied, and loved. It's brought him through thick and thin, no matter which mission he took on. Some would say, only a fool would trust their weapon. He would say, <color=#FFCCED>your weapon is all you can trust.</color>\n\n" +
                "The blade of a fallen brother of war. Picked from what he could scavenge of the last of humanity he's seen. Some would say, it keeps their spirits united. He would say, <color=#FFCCED>it's to keep their spirits from joining for as long as possible.</color>\n\n" +
                "The loads of cargo that landed with him. Each item picked from a chest, keeping him alive in one way or another. Dozens of shipments to anyone but him. Some would say, they aren't his items to take. He would say, <color=#FFCCED>if he doesn't take them, then no one will.</color>\n\n" +
                "The glow of a violet light coming from deeper within the cave. Calming, warming... beckoning. Welcoming him deeper and deeper, promising safety. Some would say, what sounds too good to be true probably is. He would say, <color=#FFCCED>Faust was a wise man.</color>");

            LanguageAPI.Add("SS2UNEMMANDO_DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add("SS2UNEMMANDO_MASTERY_SKIN_NAME", "Minuano");
            LanguageAPI.Add("SS2UNEMMANDO_CLASSIC_SKIN_NAME", "Classic");
            LanguageAPI.Add("SS2UNEMMANDO_COMMANDO_SKIN_NAME", "Commando");
            LanguageAPI.Add("SS2UNEMMANDO_VERGIL_SKIN_NAME", "Motivator");

            float gougeDuration = 2f;
            string gougeDamage = $"<style=cIsDamage>{gougeDuration * DamageTypeCore.gougeDamageCoefficient * 100}%</style> base damage";
            string bladeDamage = $"<style=cIsDamage>{BladeOfCessation2.damageCoefficient * 100}% damage</style>";

            LanguageAPI.Add("KEYWORD_SS2U_GOUGE", $"<style=cKeywordName>Gouge</style><style=cSub>Deal {gougeDamage} over {gougeDuration}s. <i>Capable of triggering item effects.</i>");

            LanguageAPI.Add("SS2UNEMMANDO_PRIMARY_BLADE_NAME", "Blade of Cessation");
            LanguageAPI.Add("SS2UNEMMANDO_PRIMARY_BLADE_DESCRIPTION", $@"<style=cIsHealth>Gouging</style>. <style=cIsUtility>Agile.</style> Slice enemies for {bladeDamage}.");

            LanguageAPI.Add("SS2UNEMMANDO_SECONDARY_CONCUSSION_NAME", "Distant Gash");

            string chargeDamage = $"<style=cIsDamage>{FireSwordBeam.minDamageCoeffficient * 100}%-{FireSwordBeam.maxDamageCoefficient * 100}% damage</style>";
            LanguageAPI.Add("SS2UNEMMANDO_SECONDARY_CONCUSSION_DESCRIPTION", $"<style=cIsHealth>Gouging</style>. Charge a piercing sword beam for {chargeDamage}.");

            LanguageAPI.Add("SS2UNEMMANDO_SECONDARY_SHOOT_NAME", "Single Tap");

            chargeDamage = $"<style=cIsDamage>{ShootGun.damageCoefficient * 100}% damage</style>";
            LanguageAPI.Add("SS2UNEMMANDO_SECONDARY_SHOOT_DESCRIPTION", $"Fire your gun for {chargeDamage}.");

            LanguageAPI.Add("SS2UNEMMANDO_UTILITY_DODGE_NAME", "Tactical Roll");
            LanguageAPI.Add("SS2UNEMMANDO_UTILITY_DODGE_DESCRIPTION", "<style=cIsUtility>Roll</style> a short distance and gain <style=cIsUtility>100 armor</style>.");

            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SUBMISSION_NAME", "Submission");
            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SUBMISSION_DESCRIPTION", $"Charge up a barrage of <style=cIsDamage>5 shotgun blasts</style> that deal <style=cIsDamage>4x{ChargeBarrageFire.damageCoefficient * 100}% damage</style> each. The number of shots increases with attack speed.");

            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_EPIC_NAME", "Decisive Strike");
            string chargedSlashDamage = $"<style=cIsDamage>{ChargedSlashAttack.maxHits}x{ChargedSlashAttack.maxDamageCoefficient * 100}% damage</style>";
            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_EPIC_DESCRIPTION", $"<style=cIsHealth>Gouging</style>. Charge up a devastating barrage of <style=cIsDamage>slashes</style> that deals up to {chargedSlashDamage}.");

            string desc = $"Charge up a barrage of <style=cIsDamage>5 shotgun blasts</style> that deal <style=cIsDamage>4x{ChargeBarrageFire.damageCoefficient * 100}% damage</style> each. The number of shots increases with attack speed.";
            desc += Helpers.ScepterDescription($"Fire lasers for {100f * ScepterBarrageFire.laserDamageCoefficient}% damage.");

            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SCEPSUBMISSION_NAME", "Subjunction");
            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SCEPSUBMISSION_DESCRIPTION", desc);

            chargedSlashDamage = $"<style=cIsDamage>{ChargedSlashAttack.maxHits}x{ChargedSlashAttack.maxDamageCoefficient * 100}% damage</style>";
            desc = $"<style=cIsHealth>Gouging</style>. Charge up a devastating barrage of <style=cIsDamage>slashes</style> that deals up to {chargedSlashDamage}.";
            desc += Helpers.ScepterDescription("Briefly disappear and then unleash all your strikes at once. Hit count increases the more attack speed you have.");

            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SCEPEPIC_NAME", "Judgement Cut End");
            LanguageAPI.Add("SS2UNEMMANDO_SPECIAL_SCEPEPIC_DESCRIPTION", desc);

            // todo: make a base class for mastery achievements and simply inherit from it for each character 
            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOCLEARGAMEMONSOON_NAME", "Nemesis Commando: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOCLEARGAMEMONSOON_DESCRIPTION", "As Nemesis Commando, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOCLEARGAMETYPHOON_NAME", "Nemesis Commando: Grand Mastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOCLEARGAMETYPHOON_DESCRIPTION", "As Nemesis Commando, beat the game or obliterate on Typhoon.");

            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOKILLSELF_NAME", "Nemesis Commando: Nemesis Nemesis");
            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOKILLSELF_DESCRIPTION", "As Nemesis Commando, defeat Commando's vestige.");

            LanguageAPI.Add("SS2UNEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_NAME", "Nemesis Commando: Zandatsu");
            LanguageAPI.Add("SS2UNEMMANDO_EPICUNLOCKABLE_ACHIEVEMENT_DESC", "As Nemesis Commando, inflict 50 stacks of Gouge on one enemy.");
            LanguageAPI.Add("SS2UNEMMANDO_EPICUNLOCKABLE_UNLOCKABLE_NAME", "Nemesis Commando: Zandatsu");

            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOUNLOCK_NAME", "???");
            LanguageAPI.Add("ACHIEVEMENT_SS2UNEMMANDOUNLOCK_DESCRIPTION", "Defeat Commando's vestige.");
        }

        #region Hooks
        internal override void Hook()
        {
            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;
            On.RoR2.HealthComponent.Suicide += HealthComponent_Suicide;
            On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;
        }

        private void PlayRuinAnimation(On.EntityStates.GlobalSkills.LunarDetonator.Detonate.orig_OnEnter orig, EntityStates.GlobalSkills.LunarDetonator.Detonate self)
        {
            orig(self);

            if (self.characterBody.bodyIndex == NemmandoCore.bodyIndex)
            {
                self.PlayAnimation("Gesture, Override", "CastRuin");
                self.StartAimMode(self.duration + 0.5f);

                EffectManager.SimpleMuzzleFlash(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashLunarNeedle"), self.gameObject, "Head", false);
            }
        }

        private void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            if (other.gameObject)
            {
                CharacterBody body = other.GetComponent<CharacterBody>();
                if (body)
                {
                    if (body.bodyIndex == NemmandoCore.bodyIndex)
                    {
                        var teamComponent = body.teamComponent;
                        if (teamComponent)
                        {
                            if (teamComponent.teamIndex != TeamIndex.Player)
                            {
                                teamComponent.teamIndex = TeamIndex.Player;
                                orig(self, other);
                                teamComponent.teamIndex = TeamIndex.Monster;
                                return;
                            }
                        }
                    }
                }
            }
            orig(self, other);
        }

        private void HealthComponent_Suicide(On.RoR2.HealthComponent.orig_Suicide orig, HealthComponent self, GameObject killerOverride, GameObject inflictorOverride, DamageType damageType)
        {

            if (damageType == DamageType.VoidDeath)
            {
                if (self.body.bodyIndex == NemmandoCore.bodyIndex)
                {
                    if (self.body.teamComponent.teamIndex != TeamIndex.Player)
                    {
                        ChatMessage.SendColored("He laughs in the face of the void.", new Color(0.149f, 0.0039f, 0.2117f));
                        return;
                    }
                }
            }
            orig(self, killerOverride, inflictorOverride, damageType);
        }
        #endregion

        internal override void InitializeSkins()
        {
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            GameObject coat = childLocator.FindChild("Coat").gameObject;

            SkinDef.GameObjectActivation[] defaultActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = coat,
                    shouldActivate = false
                }
            };

            SkinDef.GameObjectActivation[] coatActivations = new SkinDef.GameObjectActivation[]
            {
                new SkinDef.GameObjectActivation
                {
                    gameObject = coat,
                    shouldActivate = true
                }
            };

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmando"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoSword"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoGun"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            defaultSkin.gameObjectActivations = defaultActivations;

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            Material masteryMat = Modules.Assets.CreateMaterial("matNemmandoMGS", 1f);
            Material masterySword = Modules.Assets.CreateMaterial("matNemmandoMGS", 0f);
            Material masteryMatNoEmission = Modules.Assets.CreateMaterial("matNemmandoMGS", 0f);
            CharacterModel.RendererInfo[] masteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                masteryMat,
                masterySword,
                masteryMatNoEmission
            });

            SkinDef masterySkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_MASTERY_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinMaster"),
                masteryRendererInfos,
                mainRenderer,
                model,
                Modules.Config.ForceUnlockSkins.Value ? null : masterySkinUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoMGS"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoSwordMGS"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoClassicGun"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            masterySkin.gameObjectActivations = defaultActivations;

            skins.Add(masterySkin);
            #endregion

            #region GrandMasterySkin
            CharacterModel.RendererInfo[] grandMasteryRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                Modules.Assets.CreateMaterial("matNemmandoClassic", 1, Color.white, 1),
                Modules.Assets.CreateMaterial("matNemmando", 70, Color.white),
                Modules.Assets.CreateMaterial("matNemmandoClassic", 0, Color.white)
            });

            SkinDef grandMasterySkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_CLASSIC_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSkinGrandMaster"),
                grandMasteryRendererInfos,
                mainRenderer,
                model,
                Modules.Config.ForceUnlockSkins.Value ? null : grandMasterySkinUnlockableDef);

            grandMasterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoClassic"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoSword"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoClassicGun"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            grandMasterySkin.gameObjectActivations = defaultActivations;

            skins.Add(grandMasterySkin);
            #endregion

            #region CommandoSkin
            CharacterModel.RendererInfo[] commandoRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
            {
                Modules.Assets.CreateMaterial("matNemmandoAlt", 1f),
                Modules.Assets.CreateMaterial("matNemmandoAlt", 1f),
                Modules.Assets.commandoMat
            });

            CreateKillSelfUnlockable();

            SkinDef commandoSkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_COMMANDO_SKIN_NAME",
                 Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoCommandoSkin"),
                commandoRendererInfos,
                mainRenderer,
                model,
                (Modules.Config.EnableVoid.Value && !Modules.Config.ForceUnlockSkins.Value) ? NemmandoCore.killSelfUnlockableDef : null);

            commandoSkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmando"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoSword"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoGun"),
                    renderer = defaultRenderers[2].renderer
                }
            };

            //Causes console errors
            /*commandoSkin.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[]
                {
                    new SkinDef.ProjectileGhostReplacement
                    {
                        projectilePrefab = FireSwordBeam.projectilePrefab,
                        projectileGhostReplacementPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/EvisProjectileGhost")
                    }
                };*/

            commandoSkin.gameObjectActivations = defaultActivations;

            skins.Add(commandoSkin);
            #endregion

            #region Cursed
            if (Modules.Config.cursed.Value)
            {
                #region CommandoJokeSkin
                Material mercSwordMat = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[1].defaultMaterial;
                CharacterModel.RendererInfo[] commandoJokeRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
                {
                Modules.Assets.commandoMat,
                mercSwordMat,
                Modules.Assets.commandoMat
                });

                SkinDef commandoJokeSkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_COMMANDO_SKIN_NAME",
                    LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<ModelSkinController>().skins[0].icon,
                    commandoJokeRendererInfos,
                    mainRenderer,
                    model,
                    (Modules.Config.EnableVoid.Value && !Modules.Config.ForceUnlockSkins.Value) ? NemmandoCore.killSelfUnlockableDef : null);

                commandoJokeSkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshCommandoMesh"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshMercSword"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoGun"),
                    renderer = defaultRenderers[2].renderer
                }
                };

                commandoJokeSkin.gameObjectActivations = defaultActivations;

                skins.Add(commandoJokeSkin);
                #endregion

                #region VergilSkin
                CharacterModel.RendererInfo[] vergilRendererInfos = SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matVergil", 3f),
                    Modules.Assets.CreateMaterial("matYamato", 5f),
                    Modules.Assets.commandoMat
                });

                SkinDef vergilSkin = Modules.Skins.CreateSkinDef("SS2UNEMMANDO_VERGIL_SKIN_NAME",
                    Assets.mainAssetBundle.LoadAsset<Sprite>("texVergilSkin"),
                    vergilRendererInfos,
                    mainRenderer,
                    model,
                    decisiveStrikeUnlockableDef);

                vergilSkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshVergil"),
                    renderer = defaultRenderers[0].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshYamato"),
                    renderer = defaultRenderers[1].renderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshNemmandoGun"),
                    renderer = defaultRenderers[2].renderer
                }
                };

                vergilSkin.gameObjectActivations = coatActivations;

                //Causes console errors
                /*vergilSkin.projectileGhostReplacements = new SkinDef.ProjectileGhostReplacement[]
                {
                    new SkinDef.ProjectileGhostReplacement
                    {
                        projectilePrefab = FireSwordBeam.projectilePrefab,
                        projectileGhostReplacementPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/ProjectileGhosts/EvisProjectileGhost")
                    }
                };*/

                skins.Add(vergilSkin);
                #endregion
            }
            #endregion

            skinController.skins = skins.ToArray();
        }

        internal override void SetItemDisplays()
        {
            instance.itemDisplayRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            #region Display Rules
            //i leave the rest to you, swuff. godspeed.
            // great work~
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0, 0.0026f, 0.0018f), new Vector3(0, 0, 0), new Vector3(0.0038f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "Chest", new Vector3(-0.002f, 0.003f, 0.001f), new Vector3(37.00008f, 340f, 2.6726040f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Sword", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Gun", new Vector3(-0.003f, 0.002f, -0.001f), new Vector3(270f, 68.01749f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(0.002f, .008f, 0f), new Vector3(0f, 9.999999f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "CalfR", new Vector3(0f, 0.003f, -0.0006f), new Vector3(70f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.0008f)));
            //nemmando needs his calves rigged so i can attach the hoof to it, it looks freaky on his foot.
            // calves are rigged!
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.0007f, 0.0035f, -0.0025f), new Vector3(0f, 180f, 56f), new Vector3(0.007f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0f, 0.0025f, 0.0008f), new Vector3(330f, 0f, 0f), new Vector3(0.008f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "Chest", new Vector3(0.002f, 0.005f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.006f, 0.006f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Chest", new Vector3(-0.0012f, -0.0005f, 0.0023f), new Vector3(300f, 180f, 80.00002f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(0f, 0.0008f, -0.0018f), new Vector3(0f, 180f, 90f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ThighL", new Vector3(0.0005f, 0.002f, 0f), new Vector3(40.00003f, 14f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Sword", new Vector3(0f, 0.0025f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "ArmR", new Vector3(0f, 0f, -0.0005f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(340f, 0f, 0f), new Vector3(0.013f, 0.013f, 0.013f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(0.001f, 0f, 0f), new Vector3(70.00011f, 270f, 200f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "ArmL", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Sword", new Vector3(-0.0002f, 0.005f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Sword", new Vector3(-0.0002f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //hard light afterburner SHIT DON'T WORK NEED NEW HELPER AAAAAAAA
            // i did it manually, at the end, same for headstomp
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(0f, -0.0009f, -0.001f), new Vector3(0f, 0f, 0f), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(70.00005f, 250f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "ThighR", new Vector3(0f, 0.003f, 0.0008f), new Vector3(90f, 186.0004f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(0f, 0.005f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "Sword", new Vector3(-0.0005f, 0.0005f, -0.0003f), new Vector3(270f, 255f, 0f), new Vector3(0.0002f, 0.0002f, 0.0002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Pelvis", new Vector3(-0.002f, 0f, 0.0015f), new Vector3(30.00001f, 2700f, 1800f), new Vector3(0.002f, 0.002f, 0.002f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //same as hard light afterburner
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "ThighR", new Vector3(-0.0015f, 0f, 0f), new Vector3(90f, 90f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "Chest", new Vector3(-0.0015f, 0.001f, 0.0014f), new Vector3(0f, 90f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(0.001f, 0.003f, 0), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            //might need this also? in miner's code there's two... will find out i suppose
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Pelvis", new Vector3(0.0008f, 0f, 0.0016f), new Vector3(0f, 30f, 180f), new Vector3(0.008f, 0.008f, 0.008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "ThighL", new Vector3(0.0014f, 0.002f, 0f), new Vector3(90f, 270f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(-0.0007f, 0.0034f, 0f), new Vector3(330f, 45f, 9.8585290f), new Vector3(0.005f, 0.008f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(0f, 0.005f, -0.0023f), new Vector3(0f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "ThighR", new Vector3(-0.0013f, 0.004f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.006f, 0.006f, 0.006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "ThighL", new Vector3(0.0008f, 0.0024f, -0.0017f), new Vector3(90f, 230f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Pelvis", new Vector3(0.0015f, 0f, 0.0012f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Chest", new Vector3(0.001f, 0.002f, 0.002f), new Vector3(270f, 161f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(0.0006f, 0.0023f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Chest", new Vector3(0.0006f, 0.001f, 0.002f), new Vector3(-2.134434f, 20f, 15f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Pelvis", new Vector3(-0.0015f, 0f, 0.0012f), new Vector3(85f, 180f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Gun", new Vector3(-0.00017f, -0.0004f, -0.001f), new Vector3(0f, 240f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(0f, 0f, 11f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.0018f, 0.003f, 0.0016f), new Vector3(0f, 0f, 90f), new Vector3(0.016f, 0.016f, 0.016f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "Pelvis", new Vector3(0f, -0.0006f, 0.0015f), new Vector3(270f, 345f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Pelvis", new Vector3(-0.0008f, -0.0038f, 0f), new Vector3(0f, 180f, 2.5761611f), new Vector3(0.004f, 0.004f, 0.004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(0.0007f, 0.005f, -0.0024f), new Vector3(0f, 180f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0f, 0.0018f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "Head", new Vector3(-0.0008f, 0.0018f, -0.0017f), new Vector3(90f, 180f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(-0.0005f, -0.001f, 0.0017f), new Vector3(0f, 90f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.001f, 0.003f, 0), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            //again, this might need the duplication thing, i've no fucking clue LOL rob save me
            // i did rejuv rack for you, the mirrored display rule works. just do the display rule for one and it'll do the other one automatically. easy as fuck
            // should work for transcendence, nkuhanas, elite horns and whatever the fuck else uses two
            //thanks man
            //ily
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.0004f, 0.002f, 0.002f), new Vector3(0f, 0f, 20f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "ShoulderL", new Vector3(0.0013f, 0f, 0f), new Vector3(15f, 90f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.0006f, 0.0023f, 0.0023f), new Vector3(90f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "ShoulderL", new Vector3(0.0015f, 0.0007f, 0f), new Vector3(-1.024528f, 90f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "Head", new Vector3(0f, 0.0035f, -0.001f), new Vector3(0f, 270f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "ArmL", new Vector3(0.0003f, 0f, 0f), new Vector3(270f, 160f, 0f), new Vector3(0.005f, 0.005f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.01f, 0.013f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Chest", new Vector3(0f, 0f, -0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Pelvis", new Vector3(0.0015f, 0.001f, 0f), new Vector3(0f, 180f, 270f), new Vector3(0.01f, 0.01f, 0.01f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(0.001f, 0.0052f, -0.001f), new Vector3(270f, 0f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Chest", new Vector3(0.0f, 0.0025f, 0.0025f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "HandR", new Vector3(0f, 0.0005f, 0.0002f), new Vector3(90f, 180f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Pelvis", new Vector3(0.0015f, -0.0004f, -0.0014f), new Vector3(0f, 0f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "Sword", new Vector3(-0.0006f, 0.012f, -0.0005f), new Vector3(90f, 345f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(0f, 0f, 180f), new Vector3(0.0025f, 0.0025f, 0.0025f)));
            //grow up wicked ring.... nope! you're not even in the game!
            // get fucked!
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "ThighL", new Vector3(0.0003f, 0.002f, 0.0009f), new Vector3(270f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "Chest", new Vector3(-0.002f, 0.003f, 0.0008f), new Vector3(0f, 180f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Sword", new Vector3(0f, 0.0023f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            //what the fuck is this item? i'm going to kill someone at hopoo holy shit name stuff better
            //trasncendence are you kidding me... jesus christ... i feel like an idiot
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Sword", new Vector3(0.0004f, 0.01f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullCrown", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.006f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Pelvis", new Vector3(0.0015f, -0.0013f, 0.0017f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Sword", new Vector3(0f, 0.008f, 0.001f), new Vector3(50.000001f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.001f, 0.005f, -0.0023f), new Vector3(15f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, "DisplayToothMeshLarge", "Head", new Vector3(0.0006f, 0.004f, -0.0008f), new Vector3(1.7075470f, 13f, 65.00001f), new Vector3(0.03f, 0.03f, 0.03f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Sword", new Vector3(0f, 0.013f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Sword", new Vector3(0f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "ThighR", new Vector3(-0.0013f, 0.001f, 0f), new Vector3(0f, 270f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "Head", new Vector3(0f, 0.0043f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            //leaving off at line 1953 for now - laser turbine, whatever that is. copy pasting code is boring as fuck. hopoo, i kneel.
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Chest", new Vector3(0f, 0.0035f, -0.0025f), new Vector3(15f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Sword", new Vector3(-0.0007f, 0.015f, 0f), new Vector3(275f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "ThighL", new Vector3(0.0017f, 0.002f, 0.0008f), new Vector3(0f, 45f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.0005f, 0.0024f, 0.0025f), new Vector3(0f, 0f, 45f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "ThighR", new Vector3(-0.00085f, 0.0033f, 0f), new Vector3(30f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "HandR", new Vector3(0f, 0.0017f, 0.0009f), new Vector3(18f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0.002f, 0.004f, 0f), new Vector3(70f, 180f, 180f), new Vector3(0.01f, 0.01f, 0.01f)));

            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "Chest", new Vector3(-0.0003f, 0.0007f, -0.0016f), new Vector3(0f, 35f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(-0.001f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Head", new Vector3(0f, 0.003f, 0.003f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0f, 0.0045f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Pelvis", new Vector3(-0.0013f, -0.001f, 0.0015f), new Vector3(0f, 345f, 195f), new Vector3(0.006f, 0.006f, 0.006f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, ItemDisplayCore.capacitorPrefab, "ShoulderR", new Vector3(0f, 0f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            //no clue how to do this rob........ :(meru
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Pelvis", new Vector3(0.002f, 0f, -0.0015f), new Vector3(0f, 0f, 180f), new Vector3(0.0004f, 0.0004f, 0.0004f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Pelvis", new Vector3(-0.0012f, 0.001f, -0.0015f), new Vector3(0f, 30f, 180f), new Vector3(0.005f, 0.005f, 0.005f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Head", new Vector3(0f, 0.0022f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.019f, 0.015f, 0.012f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0f, 0.0034f, -0.003f), new Vector3(0f, 90f, 15f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Pelvis", new Vector3(0.0018f, 0f, -0.001f), new Vector3(85f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(0f, 0.0015f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Pelvis", new Vector3(0.0015f, -0.001f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Pelvis", new Vector3(-0.002f, -0.0008f, 0.001f), new Vector3(0f, 30f, 170f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Chest", new Vector3(0.0015f, 0.0045f, 0f), new Vector3(280f, 0f, 15f), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Sword", new Vector3(-0.0002f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", new Vector3(0f, 0.004f, -0.0028f), new Vector3(30f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(0f, 0.002f, 0.002f), new Vector3(15f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));

            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(-0.004f, 0, -0.01f), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", new Vector3(0.007f, 0, -0.01f), new Vector3(270f, 0, 0), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", new Vector3(0.007f, -0.005f, -0.005f), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", new Vector3(0.003f, 0f, -0.013f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", new Vector3(0.003f, 0f, -0.013f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));

            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(0, 0.0028f, 0), new Vector3(0, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(0.0028f, 0.006f, 0), new Vector3(15f, 90f, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Chest", new Vector3(-0.0017f, 0.004f, 0), new Vector3(0, 0, 10f), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(0, 0.003f, -0.002f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            instance.itemDisplayRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", new Vector3(0, -0, 0), new Vector3(0, 0, 0), new Vector3(0.2f, 0.2f, 0.2f)));
            //equipmentRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Lightning", capacitorPrefab, "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //I have no clue what the model name is for the Capacitator, and both the Miner / Enforcer gits do some weird fucky shit.

            //just lazy to write helpers for these two use cases
            instance.itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfL",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfR",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion

            itemDisplayRuleSet.keyAssetRuleGroups = instance.itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[2].defaultMaterial = materials[2];

            return newRendererInfos;
        }
    }
}
