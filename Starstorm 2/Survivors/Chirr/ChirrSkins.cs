﻿using R2API;
using R2API.Utils;
using RoR2;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Achievements;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Chirr
{
    public static class ChirrSkins
    {
        public static Mesh meshChirr;
        public static Mesh meshChirrMaid;
        public static Mesh meshChirrMaidDress;

        public static SkinDef maidSkin;

        public static void LoadMeshes(AssetBundle assetBundle)
        {
            meshChirr = assetBundle.LoadAsset<Mesh>("meshChirr");
            meshChirrMaid = assetBundle.LoadAsset<Mesh>("meshChirrMaid");
            meshChirrMaidDress = assetBundle.LoadAsset<Mesh>("meshChirrMaidDress");
        }

        public static void RegisterSkins()
        {
            LoadMeshes(Modules.Assets.mainAssetBundle);

            GameObject bodyPrefab = ChirrCore.chirrPrefab;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            List<SkinDef> skinDefs = new List<SkinDef>();
            
            #region DefaultSkin
            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;
            SkinDef defaultSkin = Skins.CreateSkinDef("SS2UCHIRR_DEFAULT_SKIN_NAME",
                                                          LoadoutAPI.CreateSkinIcon(new Color32(255, 255, 255, 255), new Color32(76, 116, 114, 255), new Color32(83, 118, 99, 255), new Color32(120, 147, 90, 255)),
                                                          defaultRenderers,
                                                          mainRenderer,
                                                          model,
                                                          null);
            defaultSkin.nameToken = "DEFAULT_SKIN";

            defaultSkin.meshReplacements = SkinsCore.CreateMeshReplacements(defaultRenderers,
                                                                            meshChirr,
                                                                            null);

            defaultSkin.gameObjectActivations = new SkinDef.GameObjectActivation[] {
                new SkinDef.GameObjectActivation {
                    gameObject = childLocator.FindChildGameObject("ModelDress"),
                    shouldActivate = false,
                }
            };

            skinDefs.Add(defaultSkin);
            #endregion

            #region MaidSkin
            Sprite maidSkinIcon = LoadoutAPI.CreateSkinIcon(new Color32(255, 255, 255, 255), new Color32(139, 139, 139, 255), new Color32(0, 0, 0, 255), new Color32(120, 147, 90, 255));

            UnlockableDef masterySkinUnlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            masterySkinUnlockableDef.cachedName = "Skins.SS2UChirr.Mastery";
            masterySkinUnlockableDef.nameToken = "ACHIEVEMENT_SS2UCHIRRCLEARGAMEMONSOON_NAME";
            masterySkinUnlockableDef.achievementIcon = maidSkinIcon;
            Unlockables.unlockableDefs.Add(masterySkinUnlockableDef);
            AchievementHider.unlockableRewardIdentifiers.Add(masterySkinUnlockableDef.cachedName);

            CharacterModel.RendererInfo[] masteryRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(masteryRendererInfos, 0);

            masteryRendererInfos[0].defaultMaterial = Modules.Assets.CreateMaterial("matChirr", 0, new Color(1f, 1f, 1f), 1);
            masteryRendererInfos[1].defaultMaterial = Modules.Assets.CreateMaterial("matChirrMaidDress", 0, new Color(1f, 1f, 1f), 0);

            SkinDef masterySkin = Skins.CreateSkinDef("SS2UCHIRR_MASTERY_SKIN_NAME",
                                                          maidSkinIcon,
                                                          masteryRendererInfos,
                                                          mainRenderer,
                                                          model,
                                                          Modules.Config.ForceUnlockSkins.Value ? null : masterySkinUnlockableDef);

            masterySkin.meshReplacements = SkinsCore.CreateMeshReplacements(masteryRendererInfos,
                                                                            meshChirrMaid,
                                                                            meshChirrMaidDress);

            masterySkin.gameObjectActivations = new SkinDef.GameObjectActivation[] {
                    new SkinDef.GameObjectActivation {
                        gameObject = childLocator.FindChildGameObject("ModelDress"),
                        shouldActivate = true,
                    }
                };

            skinDefs.Add(masterySkin);

            ChirrSkins.maidSkin = masterySkin;
            #endregion

            skinController.skins = skinDefs.ToArray();
        }
    }
}
