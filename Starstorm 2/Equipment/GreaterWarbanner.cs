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
        private static GameObject warbannerObj;

        public override string NameInternal => "SS2U_GreaterWarbanner";
        public override string Name => "Greater Warbanner";
        public override string Pickup => "Place a greater warbanner which grants healing, critical chance, and cooldown reduction.";
        public override string Description => "Place a greater warbanner which strengthens all allies within <style=cIsDamage>25m</style>. Raises <style=cIsDamage>critical chance</style> by <style=cIsDamage>25%</style>. Every second, <style=cIsUtility>reduces skill cooldowns by 0.5s</style> and <style=cIsHealing>heals</style> for <style=cIsHealing>2.5%</style> of your <style=cIsHealing>health</style>. Only <style=cIsUtility>1</style> banner may be active at a time.";
        public override string Lore => "<style=cMono>Recording from the Terran Museum of Tactics and Warfare.</style>\n\nSome say that one of the most important resources on the battlefield is morale. The functionality of a unit is exponentially increased depending on the mood of the troops within it. This can be raised or lowered through various means. For example, the quality of their lodging or food, news they've heard from their home, or even spending time recreationally.\n\nHowever, one tried and true method of raising the morale of a unit is the usage of symbolism. Many armies would wear colors and designs symbolizing their beliefs and their strength. Many units would carry flags in order to keep their morale high in combat.\n\nThe usage of this method has also seen variations. While a flag bearer for a unit will increase morale, it has been observed that flags unique to higher ranking military will further increase the unit's morale. Should a Colonel or General's flag appear on the battlefield, the troops will be encouraged by the power they display, and in turn increase their own fighting prowess.\n";
        public override float Cooldown => 60;
        public override string PickupIconPath => "GreaterWarbanner_Icon";
        public override string PickupModelPath => "MDLGreaterWarbanner";

        private static GameObject CreateWarbanner() {

            GameObject bannerObj = LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/WarbannerWard").InstantiateClone("GreaterWarbannerWard", true);
            bannerObj.GetComponent<BuffWard>().buffDef = BuffCore.greaterBannerBuff;
            bannerObj.AddComponent<GreaterWarbannerComponent>();

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

            //Modules.Assets.networkedObjectPrefabs.Add(bannerObj); //Apparently R2API auto handles this if you InstantiateClone and RegisterNetwork = True?

            return bannerObj;
        }

        public override void RegisterHooks()
        {
            warbannerObj = CreateWarbanner();
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            if (!equip.characterBody) return false;

            GreaterWarbannerBodyTracker tracker = equip.characterBody.AddOrGetComponent<GreaterWarbannerBodyTracker>();
            if (tracker.banner != null) NetworkServer.Destroy(tracker.banner);

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(warbannerObj, equip.characterBody.transform.position, Quaternion.identity);
            gameObject.GetComponent<TeamFilter>().teamIndex = equip.characterBody.teamComponent.teamIndex;
            gameObject.GetComponent<BuffWard>().radius = 25;

            tracker.banner = gameObject;

            NetworkServer.Spawn(gameObject);
            return true;
        }
    }

    public class GreaterWarbannerBodyTracker : MonoBehaviour
    {
        public GameObject banner;
    }

    [RequireComponent(typeof(BuffWard))]
    public class GreaterWarbannerComponent : MonoBehaviour
    {
        private float stopwatch;
        private BuffWard ward;

        public float cdr = 0.5f;
        public float healFraction = 0.025f;
        public float procInterval = 1f;

        private void Awake()
        {
            ward = base.GetComponent<BuffWard>();
            stopwatch = 0f;
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;

            stopwatch += Time.fixedDeltaTime;
            if (stopwatch >= procInterval)
            {
                stopwatch -= procInterval;
                Proc();
            }
        }

        private void Proc()
        {
            if (!ward) return;

            float radiusSqr = ward.calculatedRadius * ward.calculatedRadius;
            Vector3 position = base.transform.position;
            if (ward.invertTeamFilter)
            {
                for (TeamIndex teamIndex = TeamIndex.Neutral; teamIndex < TeamIndex.Count; teamIndex += 1)
                {
                    if (teamIndex != ward.teamFilter.teamIndex)
                    {
                        ProcTeam(TeamComponent.GetTeamMembers(teamIndex), radiusSqr, position);
                    }
                }
                return;
            }
            ProcTeam(TeamComponent.GetTeamMembers(ward.teamFilter.teamIndex), radiusSqr, position);
        }

        private void ProcTeam(System.Collections.ObjectModel.ReadOnlyCollection<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            foreach (TeamComponent teamComponent in recipients)
            {
                Vector3 vector = teamComponent.transform.position - currentPosition;
                if (ward.shape == BuffWard.BuffWardShape.VerticalTube)
                {
                    vector.y = 0f;
                }
                if (vector.sqrMagnitude <= radiusSqr)
                {
                    CharacterBody body = teamComponent.GetComponent<CharacterBody>();
                    if (body && (!ward.requireGrounded || !body.characterMotor || body.characterMotor.isGrounded))
                    {
                        if (body.healthComponent) body.healthComponent.HealFraction(healFraction, default);
                        if (body.skillLocator) body.skillLocator.DeductCooldownFromAllSkillsServer(cdr);
                    }
                }
            }
        }
    }
}
