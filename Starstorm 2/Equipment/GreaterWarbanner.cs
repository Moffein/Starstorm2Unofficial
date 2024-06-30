using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using Starstorm2Unofficial.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Equipment
{
    class GreaterWarbanner : SS2Equipment<GreaterWarbanner>
    {
        private static GameObject warbannerObj;

        public override string NameInternal => "SS2U_GreaterWarbanner";
        public override float Cooldown => 60;
        public override string PickupIconPath => "GreaterWarbanner_Icon";
        public override string PickupModelPath => "MDLGreaterWarbanner";

        public static NetworkSoundEventDef networkSound;

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
            Assets.ConvertAllRenderersToHopooShader(greaterWarbannerProp);

            greaterWarbannerProp.transform.parent = banner.transform;
            greaterWarbannerProp.transform.localPosition = new Vector3(0, 3f, 0);
            greaterWarbannerProp.transform.localScale = new Vector3(6.3f, 6.0f, 6.0f);
            greaterWarbannerProp.transform.localRotation = Quaternion.identity;

            //Modules.Assets.networkedObjectPrefabs.Add(bannerObj); //Apparently R2API auto handles this if you InstantiateClone and RegisterNetwork = True?

            return bannerObj;
        }

        public override void RegisterHooks()
        {
            networkSound = Modules.Assets.CreateNetworkSoundEventDef("SS2UGreaterWarbanner");
            warbannerObj = CreateWarbanner();
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            if (!equip.characterBody || !equip.characterBody.masterObject) return false;

            GreaterWarbannerBodyTracker tracker = equip.characterBody.masterObject.AddOrGetComponent<GreaterWarbannerBodyTracker>();
            if (tracker.banner != null) NetworkServer.Destroy(tracker.banner);

            GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(warbannerObj, equip.characterBody.transform.position, Quaternion.identity);
            gameObject.GetComponent<TeamFilter>().teamIndex = equip.characterBody.teamComponent.teamIndex;
            gameObject.GetComponent<BuffWard>().radius = 25;

            tracker.banner = gameObject;

            EffectManager.SimpleSoundEffect(networkSound.index, equip.characterBody.transform.position, true);

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
