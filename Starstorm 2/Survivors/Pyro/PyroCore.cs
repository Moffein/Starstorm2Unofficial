using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using UnityEngine;
using R2API;
using Starstorm2.Cores;
using System.Runtime.CompilerServices;
using EntityStates;
using RoR2.Skills;
using UnityEngine.AddressableAssets;
using Starstorm2.Survivors.Pyro.Components;
using EntityStates.SS2UStates.Pyro;
using RoR2.UI;
using Starstorm2.Survivors.Pyro.Components.Crosshair;

namespace Starstorm2.Survivors.Pyro
{
    public class PyroCore
    {
        public static GameObject bodyPrefab;
        public static BodyIndex bodyIndex;

        public PyroCore() => Setup();

        private void Setup()
        {
            bodyPrefab = CreatePyroPrefab();
            R2API.ItemAPI.DoNotAutoIDRSFor(bodyPrefab);

            LanguageAPI.Add("SS2UPYRO_NAME", "Pyro");
            LanguageAPI.Add("SS2UPYRO_SUBTITLE", "Pest Control");
            LanguageAPI.Add("SS2UPYRO_OUTRO_FLAVOR", "..and so he left, in a blaze of glory.");
            LanguageAPI.Add("SS2UPYRO_OUTRO_FAILURE", "..and so he vanished, with nothing but ashes left behind.");
            LanguageAPI.Add("SS2UPYRO_DESCRIPTION", "");

            RegisterStates();
            SetUpSkills();
            PyroSkins.RegisterSkins();
            //CreateDoppelganger();

            Modules.Prefabs.RegisterNewSurvivor(bodyPrefab, PrefabCore.CreateDisplayPrefab("PyroDisplay", bodyPrefab), Color.red, "SS2UPYRO", 40.3f);
            RoR2.RoR2Application.onLoad += SetBodyIndex;

            if (StarstormPlugin.emoteAPILoaded) EmoteAPICompat();
        }

        private void RegisterStates()
        {
            Modules.States.AddState(typeof(FireFlamethrower));
        }

        private void SetUpSkills()
        {
            foreach (GenericSkill sk in bodyPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(sk);
            }

            SkillDef squawkDef = Addressables.LoadAssetAsync<SkillDef>("RoR2/Base/Heretic/HereticDefaultAbility.asset").WaitForCompletion();
            SkillFamily.Variant squawkVariant =  Utils.RegisterSkillVariant(squawkDef);
            SkillLocator skillLocator = bodyPrefab.GetComponent<SkillLocator>();
            SetUpPrimaries(skillLocator);
            skillLocator.secondary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { squawkVariant });
            skillLocator.utility = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { squawkVariant });
            skillLocator.special = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { squawkVariant });
        }
        private void SetUpPrimaries(SkillLocator skillLocator)
        {
            LanguageAPI.Add("SS2UPYRO_PRIMARY_NAME", "Scorch");
            LanguageAPI.Add("SS2UPYRO_PRIMARY_DESCRIPTION", $"Build up <color=#D78326>heat</color> and burn enemies for <style=cIsDamage>375% damage per second</style>. <style=cIsDamage>Ignites</style> at <color=#D78326>high heat</color>.");

            SkillDef primaryDef1 = ScriptableObject.CreateInstance<SkillDef>();
            primaryDef1.activationState = new SerializableEntityStateType(typeof(FireFlamethrower));
            primaryDef1.activationStateMachineName = "Weapon";
            primaryDef1.skillName = "SS2UPYRO_PRIMARY_NAME";
            primaryDef1.skillNameToken = "SS2UPYRO_PRIMARY_NAME";
            primaryDef1.skillDescriptionToken = "SS2UPYRO_PRIMARY_DESCRIPTION";
            primaryDef1.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("pyroSkill1");
            primaryDef1.baseMaxStock = 1;
            primaryDef1.baseRechargeInterval = 0f;
            primaryDef1.beginSkillCooldownOnSkillEnd = false;
            primaryDef1.canceledFromSprinting = false;
            primaryDef1.fullRestockOnAssign = true;
            primaryDef1.interruptPriority = EntityStates.InterruptPriority.Any;
            primaryDef1.isCombatSkill = true;
            primaryDef1.mustKeyPress = false;
            primaryDef1.cancelSprintingOnActivation = true;
            primaryDef1.rechargeStock = 1;
            primaryDef1.requiredStock = 1;
            primaryDef1.stockToConsume = 1;
            Modules.Skills.FixSkillName(primaryDef1);
            Modules.Skills.skillDefs.Add(primaryDef1);
            SkillFamily.Variant primaryVariant1 = Utils.RegisterSkillVariant(primaryDef1);

            skillLocator.primary = Utils.RegisterSkillsToFamily(bodyPrefab, new SkillFamily.Variant[] { primaryVariant1});
        }

        private void SetBodyIndex()
        {
            bodyIndex = BodyCatalog.FindBodyIndex("SS2UPyroBody");
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private void EmoteAPICompat()
        {
            On.RoR2.SurvivorCatalog.Init += (orig) =>
            {
                orig();

                foreach (var item in SurvivorCatalog.allSurvivorDefs)
                {
                    if (item.bodyPrefab.name == "SS2UPyroBody")
                    {
                        var skele = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("animPyroEmote.prefab");

                        EmotesAPI.CustomEmotesAPI.ImportArmature(item.bodyPrefab, skele);
                        skele.GetComponentInChildren<BoneMapper>().scale = 1.5f;
                        break;
                    }
                }
            };
        }
        internal static GameObject CreatePyroPrefab()
        {
            GameObject crosshair = BuildCrosshair();//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion();

            GameObject pyroPrefab = PrefabCore.CreatePrefab("SS2UPyroBody", "mdlPyro", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "SS2UPyroBody",
                bodyNameToken = "SS2UPYRO_NAME",
                characterPortrait = Modules.Assets.mainAssetBundle.LoadAsset<Texture2D>("pyroicon"),
                bodyColor = new Color32(215, 131, 38, 255),
                crosshair = crosshair,
                damage = 12f,
                healthGrowth = 33f,
                healthRegen = 1f,
                jumpCount = 1,
                maxHealth = 110f,
                subtitleNameToken = "SS2UPYRO_SUBTITLE",
                podPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                acceleration = 80f
            });

            pyroPrefab.AddComponent<HeatController>();
            pyroPrefab.AddComponent<FlamethrowerController>();

            PrefabCore.SetupCharacterModel(pyroPrefab, new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matPyro", 1f, new Color(0.839f, 0.812f, 0.812f))
                }
            }, 0);

            //pyroPrefab.GetComponent<EntityStateMachine>().mainStateType = new SerializableEntityStateType(typeof(PyroMain));

            /*bool hadSlide = true;
            EntityStateMachine jetpackStateMachine = EntityStateMachine.FindByCustomName(pyroPrefab, "Slide");
            if (!jetpackStateMachine)
            {
                hadSlide = false;
                jetpackStateMachine = pyroPrefab.AddComponent<EntityStateMachine>();
            }
            jetpackStateMachine.customName = "Jetpack";
            jetpackStateMachine.initialStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            jetpackStateMachine.mainStateType = new SerializableEntityStateType(typeof(EntityStates.Idle));
            NetworkStateMachine nsm = pyroPrefab.GetComponent<NetworkStateMachine>();
            nsm.stateMachines = nsm.stateMachines.Append(jetpackStateMachine).ToArray();

            //This makes the Jetpack get shut off when frozen
            if (!hadSlide)
            {
                SetStateOnHurt ssoh = pyroPrefab.GetComponent<SetStateOnHurt>();
                ssoh.idleStateMachine.Append(jetpackStateMachine);
            }*/

            return pyroPrefab;
        }

        private static GameObject BuildCrosshair()
        {
            GameObject crosshairPrefab = Modules.Assets.mainAssetBundle.LoadAsset<UnityEngine.GameObject>("crosshairPyro.prefab").InstantiateClone("SS2UPyroCrosshair", false);
            crosshairPrefab.AddComponent<HudElement>();

            CrosshairController cc = crosshairPrefab.AddComponent<CrosshairController>();
            cc.maxSpreadAngle = 0f;

            crosshairPrefab.AddComponent<PyroCrosshairController>();

            return crosshairPrefab;
        }
    }
}
