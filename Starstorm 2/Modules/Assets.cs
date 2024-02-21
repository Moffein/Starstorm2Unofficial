using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using System.Collections.Generic;
using RoR2.UI;
using RoR2.Projectile;
using Starstorm2Unofficial.Cores.NemesisInvasion;

namespace Starstorm2Unofficial.Modules
{
    internal static class Assets
    {
        // the assetbundle to load assets from
        internal static AssetBundle mainAssetBundle;

        // lists of assets to add to contentpack
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();
        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<GameObject> networkedObjectPrefabs = new List<GameObject>();

        // cache these and use to create our own materials
        internal static Shader hotpoo = LegacyResourcesAPI.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;
        private static string[] assetNames = new string[0];

        internal static GameObject nemmandoCameraCover;
        internal static Material pureBlackMaterial;

        internal static GameObject needlerPrefab;

        internal static GameObject sigilFX;
        internal static GameObject jetBootsFX;
        internal static GameObject lightJetBootsFX;

        #region Materials
        internal static Material matBlueLightningLong;
        internal static Material matJellyfishLightningLarge;
        internal static Material matMageMatrixDirectionalLightning;
        //internal static Material matMoonbatteryCrippleRadius;
        internal static Material matClaySwing;
        internal static Material matDistortion;
        internal static Material matMercSwipe;
        internal static Material matMercSwipeRed;
        internal static Material matLunarGolem;
        #endregion

        #region Executioner
        internal static GameObject exeGunTracer;
        internal static GameObject exeIonBurstTracer;
        internal static GameObject exeIonEffect;
        internal static GameObject exeAxeSlamEffect;
        internal static GameObject exeIonOrb;
        internal static GameObject exeIonSuperOrb;

        internal static NetworkSoundEventDef exeChargeGainSoundDef;
        internal static NetworkSoundEventDef exeChargeMaxSoundDef;
        internal static NetworkSoundEventDef exeSuperchargeSoundDef;
        #endregion

        #region Nemmando
        internal static GameObject nemmandoBossFX;
        internal static GameObject nemSwingFX;
        internal static GameObject nemImpactFX;
        internal static GameObject nemPreImpactFX;
        internal static GameObject nemChargedSlashFX;
        internal static GameObject nemChargedSlashStartFX;
        internal static GameObject nemChargedSlashFXBlue;
        internal static GameObject nemChargedSlashStartFXBlue;
        internal static GameObject nemScepterImpactFX;
        internal static GameObject nemDashEffect;
        internal static GameObject nemSaberSwingFX;
        #endregion

        internal static GameObject mercSwingFX;

        internal static NetworkSoundEventDef nemImpactSoundDef;

        internal static void Initialize()
        {
            LoadAssetBundle();
            PopulateAssets();
        }

        internal static void LoadAssetBundle()
        {
            if (mainAssetBundle == null)
            {
                mainAssetBundle = AssetBundle.LoadFromFile(Files.GetPathToFile("Assets", "assetstorm"));
            }

            assetNames = mainAssetBundle.GetAllAssetNames();
        }

        internal static void PopulateAssets()
        {
            #region Materials
            matBlueLightningLong = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/LightningStrikeOrbEffect").transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material);
            matJellyfishLightningLarge = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/VagrantCannonExplosion").transform.Find("Lightning, Radial").GetComponent<ParticleSystemRenderer>().material);
            matMageMatrixDirectionalLightning = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXLightningMage").transform.Find("Matrix, Directional").GetComponent<ParticleSystemRenderer>().material);
            //matMoonbatteryCrippleRadius = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MoonBatteryCrippleWard").transform.Find("Indicator").Find("RadiusScaler").Find("ClearAreaIndicator").GetComponent<MeshRenderer>().material);
            matClaySwing = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/ImpSwipeEffect").GetComponentInChildren<ParticleSystemRenderer>().material);
            matDistortion = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/LoaderGroundSlam").transform.Find("Sphere, Distortion").GetComponent<ParticleSystemRenderer>().material);
            matMercSwipe = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EvisProjectile").GetComponent<ProjectileController>().ghostPrefab.transform.Find("Base").GetComponent<ParticleSystemRenderer>().material);
            matMercSwipeRed = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/EvisProjectile").GetComponent<ProjectileController>().ghostPrefab.transform.Find("Base").GetComponent<ParticleSystemRenderer>().material);
            matMercSwipeRed.SetColor("_TintColor", Color.red);
            matLunarGolem = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/LunarGolemBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial);
            #endregion

            needlerPrefab = CreateItemDisplay("DisplayNeedler", "matNeedler", Color.white, 3f);

            sigilFX = mainAssetBundle.LoadAsset<GameObject>("SigilEffect");
            sigilFX.AddComponent<Cores.Items.SigilSoundComponent>();
            sigilFX.AddComponent<NetworkIdentity>();

            PrefabAPI.RegisterNetworkPrefab(sigilFX);

            NemesisItemBehavior.effectPrefab = mainAssetBundle.LoadAsset<GameObject>("NemmandoBossEffect").InstantiateClone("GenericNemesisEffect", false);

            //LoadExecutionerEffects();
            //LoadNemmandoEffects();

            #region JetBoots
            jetBootsFX = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXCommandoGrenade"), "JetBootsEffect");

            RemoveEffect(jetBootsFX.transform.Find("ScaledHitsparks 1"));
            RemoveEffect(jetBootsFX.transform.Find("UnscaledHitsparks 1"));
            RemoveEffect(jetBootsFX.transform.Find("Nova Sphere (1)"));
            //RemoveEffect(lightJetBootsFX.transform.Find("Point Light"));

            jetBootsFX.GetComponent<ShakeEmitter>().radius *= 0.5f;
            jetBootsFX.GetComponent<EffectComponent>().soundName = "SS2UJetBootsExplosion";
            jetBootsFX.AddComponent<NetworkIdentity>();
            var jetBootsDef = new EffectDef(jetBootsFX);
            effectDefs.Add(jetBootsDef);
            #endregion

            #region JetBoots(Light)
            lightJetBootsFX = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXCommandoGrenade"), "JetBootsLightEffect");

            RemoveEffect(lightJetBootsFX.transform.Find("ScaledHitsparks 1"));
            RemoveEffect(lightJetBootsFX.transform.Find("UnscaledHitsparks 1"));
            RemoveEffect(lightJetBootsFX.transform.Find("Nova Sphere (1)"));
            RemoveEffect(lightJetBootsFX.transform.Find("ScaledSmoke, Billboard"));
            RemoveEffect(lightJetBootsFX.transform.Find("Unscaled Smoke, Billboard"));
            RemoveEffect(lightJetBootsFX.transform.Find("Physics Sparks"));
            RemoveEffect(lightJetBootsFX.transform.Find("Flash, Soft Glow"));
            RemoveEffect(lightJetBootsFX.transform.Find("Unscaled Flames"));
            RemoveEffect(lightJetBootsFX.transform.Find("Dash, Bright"));
            //RemoveEffect(lightJetBootsFX.transform.Find("Point Light"));

            lightJetBootsFX.GetComponent<ShakeEmitter>().radius = 0f;
            lightJetBootsFX.GetComponent<EffectComponent>().soundName = "SS2UJetBootsExplosion";
            lightJetBootsFX.AddComponent<NetworkIdentity>();
            var lightJetBootsDef = new EffectDef(lightJetBootsFX);
            effectDefs.Add(lightJetBootsDef);
            #endregion
        }

        private static void RemoveEffect(Transform transformToKill)
        {
            if (transformToKill) StarstormPlugin.DestroyImmediate(transformToKill.gameObject);
        }

        internal static void LoadExecutionerEffects()
        {
            exeChargeGainSoundDef = CreateNetworkSoundEventDef("SS2UExecutionerGainCharge");
            exeChargeMaxSoundDef = CreateNetworkSoundEventDef("SS2UExecutionerMaxCharge");
            exeSuperchargeSoundDef = CreateNetworkSoundEventDef("SS2UExecutionerSupercharge");

            exeGunTracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault").InstantiateClone("TracerExecutionerDefault", true);

            if (!exeGunTracer.GetComponent<EffectComponent>()) exeGunTracer.AddComponent<EffectComponent>();
            if (!exeGunTracer.GetComponent<VFXAttributes>()) exeGunTracer.AddComponent<VFXAttributes>();
            if (!exeGunTracer.GetComponent<NetworkIdentity>()) exeGunTracer.AddComponent<NetworkIdentity>();

            Tracer tracer = exeGunTracer.GetComponent<Tracer>();
            tracer.speed = 300;
            tracer.length = 16;

            foreach (LineRenderer lineRend in exeGunTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (lineRend)
                {
                    lineRend.widthMultiplier = 0.4f;
                }
            }

            AddEffect(exeGunTracer);

            exeIonBurstTracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("TracerExecutionerIonBurst", true);

            if (!exeIonBurstTracer.GetComponent<EffectComponent>()) exeIonBurstTracer.AddComponent<EffectComponent>();
            if (!exeIonBurstTracer.GetComponent<VFXAttributes>()) exeIonBurstTracer.AddComponent<VFXAttributes>();
            if (!exeIonBurstTracer.GetComponent<NetworkIdentity>()) exeIonBurstTracer.AddComponent<NetworkIdentity>();

            Tracer tracer2 = exeIonBurstTracer.GetComponent<Tracer>();
            tracer2.speed = 240;
            tracer2.length = 21;

            foreach (LineRenderer i in exeIonBurstTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    i.widthMultiplier *= 2f;
                }
            }

            AddEffect(exeIonBurstTracer);

            exeIonOrb = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "ExecutionerIonOrbEffect", true);
            if (!exeIonOrb.GetComponent<NetworkIdentity>()) exeIonOrb.AddComponent<NetworkIdentity>();
            StarstormPlugin.DestroyImmediate(exeIonOrb.GetComponent<AkEvent>());

            //Material titanPredictionEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/TitanPreFistProjectile").transform.Find("TeamAreaIndicator, GroundOnly").GetComponent<TeamAreaIndicator>().teamMaterialPairs[0].sharedMaterial;
            //Material globMat = new EntityStates.TitanMonster.FireMegaLaser().laserPrefab.transform.Find("End").Find("EndEffect").Find("Particles").Find("Glob").GetComponent<ParticleSystemRenderer>().material;

            exeIonOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>().widthMultiplier = 0.5f;
            exeIonOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>().material = matBlueLightningLong;
            exeIonOrb.transform.Find("VFX").gameObject.SetActive(false);//.Find("Core").gameObject.SetActive(false);
            //var main = exeIonOrb.transform.Find("VFX").Find("PulseGlow").gameObject.GetComponent<ParticleSystem>().main;
            //main.startColor = Color.blue;
            exeIonOrb.transform.Find("VFX").localScale = Vector3.one * 3f;

            AddEffect(exeIonOrb);

            exeIonSuperOrb = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "ExecutionerIonSuperOrbEffect", true);
            if (!exeIonSuperOrb.GetComponent<NetworkIdentity>()) exeIonSuperOrb.AddComponent<NetworkIdentity>();
            StarstormPlugin.DestroyImmediate(exeIonSuperOrb.GetComponent<AkEvent>());

            exeIonSuperOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>().widthMultiplier = 1.5f;
            exeIonSuperOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>().material = matBlueLightningLong;
            exeIonSuperOrb.transform.Find("VFX").gameObject.SetActive(false);//.Find("Core").gameObject.SetActive(false);
            //var main = exeIonOrb.transform.Find("VFX").Find("PulseGlow").gameObject.GetComponent<ParticleSystem>().main;
            //main.startColor = Color.blue;
            exeIonSuperOrb.transform.Find("VFX").localScale = Vector3.one * 3f;

            AddEffect(exeIonSuperOrb);

            exeIonEffect = LoadEffect("IonEffect", true);
            exeAxeSlamEffect = LoadEffect("AxeSlamEffect", "", false, false);
        }

        internal static void LoadNemmandoEffects()
        {
            nemImpactSoundDef = CreateNetworkSoundEventDef("SS2UNemmandoSaberHit");

            nemmandoCameraCover = mainAssetBundle.LoadAsset<GameObject>("NemmandoCameraCover");
            pureBlackMaterial = mainAssetBundle.LoadAsset<Material>("matPureBlack");

            nemmandoBossFX = mainAssetBundle.LoadAsset<GameObject>("NemmandoBossEffect");
            nemSwingFX = LoadEffect("NemmandoSwingEffect", "", true);
            nemImpactFX = LoadEffect("ImpactNemmandoSlash", "", false);
            nemPreImpactFX = LoadEffect("PreImpactScepterStrike", "", false);
            nemScepterImpactFX = LoadEffect("ImpactScepterStrike", "", false);
            nemSaberSwingFX = LoadEffect("NemmandoSwingEffectSaber", "", true);
            mercSwingFX = LoadEffect("NemmandoSwingEffectMerc", "", true);
            nemChargedSlashFX = LoadEffect("NemmandoChargeSlashEffect", "", true);
            nemChargedSlashStartFX = LoadEffect("NemmandoChargeSlashStartEffect", "", true);
            nemChargedSlashFXBlue = LoadEffect("NemmandoChargeSlashEffectBlue", "", true);
            nemChargedSlashStartFXBlue = LoadEffect("NemmandoChargeSlashStartEffectBlue", "", true);

            nemSwingFX.GetComponent<ParticleSystemRenderer>().material = matMercSwipeRed;
            //nemSwingFX.transform.Find("SwingTrail").GetComponent<ParticleSystemRenderer>().material = matMercSwipeRed;
            //nemSwingFX.transform.Find("Distortion").GetComponent<ParticleSystemRenderer>().material = matDistortion;

            nemSaberSwingFX.GetComponent<ParticleSystemRenderer>().material = matMercSwipeRed;
            nemSaberSwingFX.transform.Find("Distortion").GetComponent<ParticleSystemRenderer>().material = matDistortion;

            mercSwingFX.GetComponent<ParticleSystemRenderer>().material = matMercSwipe;
            mercSwingFX.transform.Find("Distortion").GetComponent<ParticleSystemRenderer>().material = matDistortion;

            nemChargedSlashStartFX.transform.Find("Round").GetComponent<ParticleSystemRenderer>().material = matDistortion;
            nemChargedSlashStartFXBlue.transform.Find("Round").GetComponent<ParticleSystemRenderer>().material = matDistortion;

            nemDashEffect = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "NemmandoDashOrbEffect", true);
            if (!nemDashEffect.GetComponent<NetworkIdentity>()) nemDashEffect.AddComponent<NetworkIdentity>();
            StarstormPlugin.DestroyImmediate(nemDashEffect.GetComponent<AkEvent>());
            StarstormPlugin.DestroyImmediate(nemDashEffect.transform.Find("TrailParent").gameObject);
            StarstormPlugin.DestroyImmediate(nemDashEffect.transform.Find("VFX").gameObject);

            GameObject dashEffect = UnityEngine.Object.Instantiate<GameObject>(mainAssetBundle.LoadAsset<GameObject>("NemmandoDashEffect"));
            dashEffect.transform.parent = nemDashEffect.transform;
            dashEffect.transform.localRotation = Quaternion.identity;
            dashEffect.transform.localPosition = Vector3.zero;

            AddEffect(nemDashEffect);
        }

        private static GameObject CreateItemDisplay(string prefabName, string matName, Color emColor, float emPower)
        {
            GameObject displayPrefab = mainAssetBundle.LoadAsset<GameObject>(prefabName);
            Material itemMat = CreateMaterial(matName, emPower, emColor);
            MeshRenderer renderer = displayPrefab.GetComponent<MeshRenderer>();

            renderer.material = itemMat;
            displayPrefab.AddComponent<ItemDisplay>().rendererInfos = new CharacterModel.RendererInfo[]
            {
                new CharacterModel.RendererInfo
                {
                    defaultMaterial = itemMat,
                    renderer = renderer,
                    ignoreOverlays = false,
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On
                }
            };

            return displayPrefab;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            if (LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName) == null) return null;

            GameObject newTracer = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName), newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            newTracer.GetComponent<Tracer>().speed = 250f;
            newTracer.GetComponent<Tracer>().length = 50f;

            AddEffect(newTracer);

            return newTracer;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;
            networkSoundEventDef.name = "nse" + eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            if (!objectToConvert) return;

            foreach (MeshRenderer i in objectToConvert.GetComponentsInChildren<MeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }

            foreach (SkinnedMeshRenderer i in objectToConvert.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            if (LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair") == null) return LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/StandardCrosshair");
            return LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform, true);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform, bool applyScale)
        {
            bool assetExists = false;
            for (int i = 0; i < assetNames.Length; i++)
            {
                if (assetNames[i].Contains(resourceName.ToLower()))
                {
                    assetExists = true;
                    i = assetNames.Length;
                }
            }

            if (!assetExists)
            {
                Debug.LogError("Failed to load effect: " + resourceName + " because it does not exist in the AssetBundle");
                return null;
            }

            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = applyScale;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddEffect(newEffect, soundName);

            return newEffect;
        }

        internal static void AddEffect(GameObject effectPrefab)
        {
            AddEffect(effectPrefab, "");
        }

        private static void AddEffect(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            if (!effectPrefab.GetComponent<NetworkIdentity>()) effectPrefab.AddComponent<NetworkIdentity>();

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat)
            {
                Debug.LogError("Failed to load material: " + materialName + " - Check to see that the name in your Unity project matches the one in this code");
                return commandoMat;
            }

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}