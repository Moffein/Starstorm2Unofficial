using R2API;
using R2API.Utils;
using RoR2;
using Starstorm2.Cores;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2.Survivors.Chirr
{
    public static class ChirrSkins
    {
        public static Mesh meshChirr;

        public static void LoadMeshes(AssetBundle assetBundle)
        {
            meshChirr = assetBundle.LoadAsset<Mesh>("chirrmesh");   //there's meshchirr and chirrmesh. meshchirr seems to be broken
        }

        private static SkinDef.MeshReplacement[] CreateMeshReplacements(CharacterModel.RendererInfo[] rendererInfos, Mesh bodyMesh)
        {
            return SkinsCore.CreateMeshReplacements(rendererInfos, bodyMesh);
        }

        public static void RegisterSkins()
        {
            LoadMeshes(Modules.Assets.mainAssetBundle);

            GameObject bodyPrefab = ChirrCore.chirrPrefab;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            List<SkinDef> skinDefs = new List<SkinDef>();
            
            #region DefaultSkin

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            LanguageAPI.Add("CHIRR_DEFAULT_SKIN_NAME", "Default");
            SkinDef defaultSkin = SkinsCore.CreateSkinDef("CHIRR_DEFAULT_SKIN_NAME",
                                                          LoadoutAPI.CreateSkinIcon(new Color32(255, 255, 255, 255), new Color32(76, 116, 114, 255), new Color32(83, 118, 99, 255), new Color32(120, 147, 90, 255)),
                                                          defaultRenderers,
                                                          mainRenderer,
                                                          model,
                                                          null);

            defaultSkin.meshReplacements = CreateMeshReplacements(defaultRenderers,
                                                                  meshChirr);

            skinDefs.Add(defaultSkin);
            #endregion

            skinController.skins = skinDefs.ToArray();
        }
    }
}
