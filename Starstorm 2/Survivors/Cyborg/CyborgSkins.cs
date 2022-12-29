using System;
using UnityEngine;
using R2API;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using Starstorm2.Cores;
using Starstorm2.Modules;

namespace Starstorm2.Survivors.Cyborg
{
    public static class CyborgSkins 
    {
        public static Mesh meshCyborg;
        public static Mesh meshCyborgSteam;
        public static Mesh meshCyborgRock;


        public static void LoadMeshes(AssetBundle CyborgAssetBundle)
        {
            meshCyborg = CyborgAssetBundle.LoadAsset<Mesh>("meshCyborg");
            meshCyborgSteam = CyborgAssetBundle.LoadAsset<Mesh>("meshCyborgSteam");
            meshCyborgRock = CyborgAssetBundle.LoadAsset<Mesh>("meshCyborgRock");
        }

        private static SkinDef.MeshReplacement[] CreateMeshReplacements(CharacterModel.RendererInfo[] rendererInfos, Mesh bodyMesh) {
           
            return SkinsCore.CreateMeshReplacements(rendererInfos, bodyMesh);
        }

        public static void RegisterSkins()
        {
            LoadMeshes(Modules.Assets.mainAssetBundle);

            GameObject bodyPrefab = CyborgCore.cybPrefab;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            List<SkinDef> skinDefs = new List<SkinDef>();

            #region DefaultSkin

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            LanguageAPI.Add("CYBORG_DEFAULT_SKIN_NAME", "Default");
            SkinDef defaultSkin = SkinsCore.CreateSkinDef("CYBORG_DEFAULT_SKIN_NAME",
                                                          Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgprimary"),
                                                          defaultRenderers,
                                                          mainRenderer,
                                                          model, 
                                                          null);

            defaultSkin.meshReplacements = CreateMeshReplacements(defaultRenderers, 
                                                                  meshCyborg);

            skinDefs.Add(defaultSkin);
            #endregion

            #region MasterySkin

            LanguageAPI.Add("ACHIEVEMENT_SS2UCYBORGCLEARGAMEMONSOON_NAME", "CYBORG: Mastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UCYBORGCLEARGAMEMONSOON_DESCRIPTION", "As CYBORG, beat the game or obliterate on Monsoon.");

            UnlockableDef masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UCyborg.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UCYBORGCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary");
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);

            CharacterModel.RendererInfo[] masteryRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(masteryRendererInfos, 0);

            string materialName = "matSteamborg";
            masteryRendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial(materialName, 1, new Color(0.839f, 0.812f, 0.812f), 0);
            //masteryRendererInfos[1].defaultMaterial = AssetsCore.CreateMaterial("matCyborg", 1, Color.white, 0);
            //masteryRendererInfos[2].defaultMaterial = AssetsCore.CreateMaterial("matCyborg", 1, Color.white, 0);

            LanguageAPI.Add("CYBORG_MASTERY_SKIN_NAME", "Cybersteam");
            SkinDef masterySkin = SkinsCore.CreateSkinDef("CYBORG_MASTERY_SKIN_NAME",
                                                          Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgsecondary"),
                                                          masteryRendererInfos, 
                                                          mainRenderer, 
                                                          model,
                                                          Modules.Config.ForceUnlockSkins.Value ? null : masterySkinUnlockableDef);

            masterySkin.meshReplacements = CreateMeshReplacements(masteryRendererInfos, 
                                                                  meshCyborgSteam);

            skinDefs.Add(masterySkin);
            #endregion

            #region GrandMasterySkin

            LanguageAPI.Add("ACHIEVEMENT_SS2UCYBORGCLEARGAMETYPHOON_NAME", "CYBORG: Grandmastery");
            LanguageAPI.Add("ACHIEVEMENT_SS2UCYBORGCLEARGAMETYPHOON_DESCRIPTION", "As CYBORG, beat the game or obliterate on Typhoon.");

            UnlockableDef grandmasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandmasterySkinUnlockableDef.cachedName = "Skins.SS2UCyborg.GrandMastery";
            grandmasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UCYBORGCLEARGAMETYPHOON_NAME";
            grandmasterySkinUnlockableDef.achievementIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial");
            Unlockables.unlockableDefs.Add(grandmasterySkinUnlockableDef);

            CharacterModel.RendererInfo[] grandMasteryRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(grandMasteryRendererInfos, 0);

            materialName = "matRockborg";
            grandMasteryRendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial(materialName, 1, Color.white, 0);
            //masteryRendererInfos[1].defaultMaterial = AssetsCore.CreateMaterial("matCyborg", 1, Color.white, 0);
            //masteryRendererInfos[2].defaultMaterial = AssetsCore.CreateMaterial("matCyborg", 1, Color.white, 0);

            LanguageAPI.Add("CYBORG_GRANDMASTERY_SKIN_NAME", "Metamorphic");
            SkinDef grandMasterySkin = SkinsCore.CreateSkinDef("CYBORG_GRANDMASTERY_SKIN_NAME",
                                                          Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("cyborgspecial"),
                                                          grandMasteryRendererInfos,
                                                          mainRenderer,
                                                          model,
                                                          Modules.Config.ForceUnlockSkins.Value ? null : grandmasterySkinUnlockableDef);

            grandMasterySkin.meshReplacements = CreateMeshReplacements(grandMasteryRendererInfos,
                                                                  meshCyborgRock);
            skinDefs.Add(grandMasterySkin);
            #endregion GrandMasterySkin

            skinController.skins = skinDefs.ToArray();
        }
    }
}
