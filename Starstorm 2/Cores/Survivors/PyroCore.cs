using System.Collections.Generic;
using R2API;
using RoR2;
using UnityEngine;

//TODO: bring in moffein's pyro kit
//TODO: update pyro to use prefab builder stuff like other survivors

namespace Starstorm2.Cores
{
    public class PyroCore
    {
        public Color charColor = new Color(0.69f, 0.44f, 0.49f);

        public static GameObject pyroPrefab;
        public static GameObject doppelganger;

        public PyroCore() => Setup();

        private void Setup()
        {
            pyroPrefab = PrefabCore.pyroPrefab;
            //pyroPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(PyroMain));

            ItemDisplays.PyroItemDisplays.RegisterDisplays();
            CreateDoppelganger();
        }

        public static void CreateDoppelganger()
        {
            doppelganger = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/CommandoMonsterMaster"), "PyroMonsterMaster", true);
            doppelganger.GetComponent<CharacterMaster>().bodyPrefab = pyroPrefab;

            Modules.Prefabs.masterPrefabs.Add(doppelganger);
        }
    }
}