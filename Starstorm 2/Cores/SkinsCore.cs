
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using Starstorm2Unofficial.Cores.Skins;
using Starstorm2Unofficial.Modules;
using System.Collections;

namespace Starstorm2Unofficial.Cores
{
    public class SkinsCore 
    {
        public SkinsCore() {

            BodyCatalog.availability.onAvailable += Availability_onAvailable;
        }

        private void Availability_onAvailable()
        {
            try
            {
                VanillaSurvivorSkins.RegisterVanillaSurvivorSkins();
            }
            catch (Exception ex)
            {
                LogCore.LogError("error registering vanilla survivor skins\n" + ex);
            }
        }

        public static SkinDef.MeshReplacement[] CreateMeshReplacements(CharacterModel.RendererInfo[] rendererInfos, params Mesh[] orderedMeshes) {
            List<SkinDef.MeshReplacement> replacements = new List<SkinDef.MeshReplacement>();

            for (int i = 0; i < orderedMeshes.Length; i++) {
                if (orderedMeshes[i] == null)
                    continue;

                replacements.Add(new SkinDef.MeshReplacement {
                    mesh = orderedMeshes[i],
                    renderer = rendererInfos[i].renderer
                });
            }
            return replacements.ToArray();
        }

        /// <summary>
        /// create an array of all gameobjects that are activated/deactivated by skins, then for each skin pass in the specific objects that will be active
        /// </summary>
        /// <param name="allObjects">array of all gameobjects that are activated/deactivated by skins</param>
        /// <param name="activatedObjects">specific objects that will be active</param>
        /// <returns></returns>
        public static SkinDef.GameObjectActivation[] createGameObjectActivations(GameObject[] allObjects, params GameObject[] activatedObjects) {

            List<SkinDef.GameObjectActivation> GameObjectActivations = new List<SkinDef.GameObjectActivation>();

            for (int i = 0; i < allObjects.Length; i++) {

                bool activate = activatedObjects.Contains(allObjects[i]);

                GameObjectActivations.Add(new SkinDef.GameObjectActivation {
                    gameObject = allObjects[i],
                    shouldActivate = activate
                });
            }

            return GameObjectActivations.ToArray();
        }
    }
}
