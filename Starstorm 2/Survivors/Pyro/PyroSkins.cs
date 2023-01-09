using R2API;
using R2API.Utils;
using RoR2;
using Starstorm2Unofficial.Cores;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Pyro
{
    public static class PyroSkins
    {
        public static Mesh meshPyro;

        public static void LoadMeshes(AssetBundle assetBundle)
        {
            meshPyro = assetBundle.LoadAsset<Mesh>("meshpyro");   //there's meshchirr and chirrmesh. meshchirr seems to be broken
        }

        private static SkinDef.MeshReplacement[] CreateMeshReplacements(CharacterModel.RendererInfo[] rendererInfos, Mesh bodyMesh)
        {
            return SkinsCore.CreateMeshReplacements(rendererInfos, bodyMesh);
        }

        public static void RegisterSkins()
        {
            LoadMeshes(Modules.Assets.mainAssetBundle);

            GameObject bodyPrefab = PyroCore.bodyPrefab;

            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = Reflection.GetFieldValue<SkinnedMeshRenderer>(characterModel, "mainSkinnedMeshRenderer");

            List<SkinDef> skinDefs = new List<SkinDef>();
            
            #region DefaultSkin

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            LanguageAPI.Add("PYRO_DEFAULT_SKIN_NAME", "Default");
            SkinDef defaultSkin = SkinsCore.CreateSkinDef("PYRO_DEFAULT_SKIN_NAME",
                                                          LoadoutAPI.CreateSkinIcon(new Color32(247, 247, 181, 255), new Color32(75, 76, 54, 255), new Color32(56, 44, 39, 255), new Color32(117, 115, 78, 255)),
                                                          defaultRenderers,
                                                          mainRenderer,
                                                          model,
                                                          null);

            defaultSkin.meshReplacements = CreateMeshReplacements(defaultRenderers,
                                                                  meshPyro);

            skinDefs.Add(defaultSkin);
            #endregion

            skinController.skins = skinDefs.ToArray();
        }
    }
}
