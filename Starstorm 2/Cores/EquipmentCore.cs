using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;

namespace Starstorm2.Cores
{
    class EquipmentCore
    {
        public static EquipmentCore instance;

        public List<SS2Equipment> equipment = new List<SS2Equipment>();
        public List<EquipmentDef> equipmentDefs = new List<EquipmentDef>();

        //protected internal EquipmentIndex greaterWarbannerIndex;
        //protected internal EquipmentIndex cloakIndex;
        //protected internal EquipmentIndex canisterIndex;

        private const string modPrefix = "@Starstorm2:";
        private const string prefabPath = modPrefix + "Assets/AssetBundle/Items/";

        //private GameObject greaterWarbanner;

        public EquipmentCore()
        {
            //LogCore.LogInfo("Initializing Core: " + base.ToString());
            instance = this;

            //do/did we use this for anything?
            /*
            var prefab = Resources.Load<GameObject>("Prefabs/PickupModels/PickupMystery");
            var rule = new ItemDisplayRule
            {
                ruleType = ItemDisplayRuleType.ParentedPrefab,
                followerPrefab = prefab,
                childName = "Chest",
                localScale = new Vector3(0f, 0, 0f),
                localAngles = new Vector3(0f, 0f, 0f),
                localPos = new Vector3(0, 0f, 0f)
            };
            string pickUpIconPath = "Textures/MiscIcons/texMysteryIcon";
            string pickUpModelPath = "Prefabs/PickupModels/PickupMystery";
            */
        }

        public void InitEquipment()
        {
            foreach (var equip in equipment)
            {
                equip.Init();
                equipmentDefs.Add(equip.equipDef);
            }
        }
    }
}
