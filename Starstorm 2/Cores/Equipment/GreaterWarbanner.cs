using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Equipment
{
    class GreaterWarbanner : SS2Equipment<GreaterWarbanner>
    {
        private static GameObject warbannerObj = CreateWarbanner();
        private GameObject activeBanner;

        public override string NameInternal => "GreaterWarbanner";
        public override string Name => "Greater Warbanner";
        public override string Pickup => "Place a greater warbanner which boosts regen, critical hit chance, and lowers cooldowns.";
        public override string Description => "Place a greater warbanner which grants <style=cIsHealing>50% health regeneration</style>, <style=cIsDamage>20% critical hit chance</style>, and <style=cIsUtility>50% cooldown reduction</style>. Only one banner may be active at a time.";
        public override string Lore => "<style=cMono>Recording from the Terran Museum of Tactics and Warfare.</style>\n\nSome say that one of the most important resources on the battlefield is morale. The functionality of a unit is exponentially increased depending on the mood of the troops within it. This can be raised or lowered through various means. For example, the quality of their lodging or food, news they've heard from their home, or even spending time recreationally.\n\nHowever, one tried and true method of raising the morale of a unit is the usage of symbolism. Many armies would wear colors and designs symbolizing their beliefs and their strength. Many units would carry flags in order to keep their morale high in combat.\n\nThe usage of this method has also seen variations. While a flag bearer for a unit will increase morale, it has been observed that flags unique to higher ranking military will further increase the unit's morale. Should a Colonel or General's flag appear on the battlefield, the troops will be encouraged by the power they display, and in turn increase their own fighting prowess.\n";
        public override float Cooldown => 55;
        public override string PickupIconPath => "GreaterWarbanner_Icon";
        public override string PickupModelPath => "MDLGreaterWarbanner";

        private static GameObject CreateWarbanner() {

            GameObject bannerObj = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard").InstantiateClone("GreaterWarbannerWard", true);
            bannerObj.GetComponent<BuffWard>().buffDef = BuffCore.greaterBannerBuff;

            Transform sphereIndicator = bannerObj.transform.Find("Indicator/IndicatorSphere");
            Transform banner = bannerObj.transform.Find("mdlWarbanner");
            for (int i = 0; i < banner.childCount; i++) {
                banner.GetChild(i).gameObject.SetActive(false);
            }
            var indicatorMat = Resources.FindObjectsOfTypeAll<Material>().Where(mat => mat.name == "matWarcrySphereIndicator").FirstOrDefault();
            if (indicatorMat)
                sphereIndicator.GetComponent<MeshRenderer>().material = indicatorMat;

            GameObject greaterWarbannerProp = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MDLGreaterWarbannerWard").InstantiateClone("GreaterWarbannerProp", false);
            greaterWarbannerProp.GetComponent<Renderer>().material.shader = Modules.Assets.hotpoo;

            greaterWarbannerProp.transform.parent = banner.transform;
            greaterWarbannerProp.transform.localPosition = new Vector3(0, 3f, 0);
            greaterWarbannerProp.transform.localScale = new Vector3(6.3f, 6.0f, 6.0f);
            greaterWarbannerProp.transform.localRotation = Quaternion.identity;

            return bannerObj;
        }

        public override void RegisterHooks()
        {
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(warbannerObj, equip.characterBody.transform.position, Quaternion.identity);
            gameObject.GetComponent<TeamFilter>().teamIndex = equip.characterBody.teamComponent.teamIndex;
            gameObject.GetComponent<BuffWard>().radius = 25;
            if (activeBanner)
                NetworkServer.Destroy(activeBanner);
            activeBanner = gameObject;
            NetworkServer.Spawn(gameObject);
            return true;
        }
    }
}
