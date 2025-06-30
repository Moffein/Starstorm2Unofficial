﻿using System;
using UnityEngine;
using R2API;
using RoR2;
using R2API.Utils;
using System.Collections.Generic;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Achievements;

namespace Starstorm2Unofficial.Survivors.Cyborg
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

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            List<SkinDef> skinDefs = new List<SkinDef>();

            #region DefaultSkin

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            SkinDef defaultSkin = Skins.CreateSkinDef("SS2UCYBORG_DEFAULT_SKIN_NAME",
                                                          LoadoutAPI.CreateSkinIcon(new Color32(234, 231, 212, 255), new Color32(33, 51, 49, 255), new Color32(32, 40, 53, 255), new Color32(56, 79, 77, 255)),
                                                          defaultRenderers,
                                                          mainRenderer,
                                                          model, 
                                                          null);
            defaultSkin.nameToken = "DEFAULT_SKIN";

            defaultSkin.meshReplacements = CreateMeshReplacements(defaultRenderers, 
                                                                  meshCyborg);

            skinDefs.Add(defaultSkin);
            #endregion

            #region MasterySkin
            Sprite masteryIcon = LoadoutAPI.CreateSkinIcon(new Color32(250, 243, 181, 255), new Color32(125, 92, 39, 255), new Color32(26, 17, 22, 255), new Color32(57, 33, 33, 255));
            UnlockableDef masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UCyborg.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UCYBORGCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = masteryIcon;
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Remove(masterySkinUnlockableDef.cachedName);

            CharacterModel.RendererInfo[] masteryRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(masteryRendererInfos, 0);

            masteryRendererInfos[0].defaultMaterial = Modules.Assets.LoadMaterialFromAssetBundle("matSteamborg");

            SkinDef masterySkin = Skins.CreateSkinDef("SS2UCYBORG_MASTERY_SKIN_NAME",
                                                          masteryIcon,
                                                          masteryRendererInfos,
                                                          mainRenderer,
                                                          model,
                                                          Modules.Config.ForceUnlockSkins.Value ? null : masterySkinUnlockableDef);

            masterySkin.meshReplacements = CreateMeshReplacements(masteryRendererInfos, 
                                                                  meshCyborgSteam);

            skinDefs.Add(masterySkin);
            #endregion

            #region GrandMasterySkin
            Sprite gmIcon = LoadoutAPI.CreateSkinIcon(new Color32(255, 134, 122, 255), new Color32(7, 14, 30, 255), new Color32(7, 12, 25, 255), new Color32(10, 22, 36, 255));
            UnlockableDef grandmasterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            grandmasterySkinUnlockableDef.cachedName = "Skins.SS2UCyborg.GrandMastery";
            grandmasterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UCYBORGCLEARGAMETYPHOON_NAME";
            grandmasterySkinUnlockableDef.achievementIcon = gmIcon;
            Unlockables.unlockableDefs.Add(grandmasterySkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Remove(grandmasterySkinUnlockableDef.cachedName);

            CharacterModel.RendererInfo[] grandMasteryRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(grandMasteryRendererInfos, 0);

            grandMasteryRendererInfos[0].defaultMaterial = Modules.Assets.LoadMaterialFromAssetBundle("matRockborg");

            SkinDef grandMasterySkin = Skins.CreateSkinDef("SS2UCYBORG_GRANDMASTERY_SKIN_NAME",
                                                          gmIcon,
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
