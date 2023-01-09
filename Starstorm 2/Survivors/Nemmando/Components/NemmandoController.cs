using RoR2;
using RoR2.Projectile;
using Starstorm2Unofficial.Cores;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Nemmando.Components
{
    public class NemmandoController : MonoBehaviour
    {
        public static GameObject gunCrosshairOverridePrefab = Utils.LoadCrosshair("SimpleDot");

        public bool chargingDecisiveStrike = false;
        public bool rolling = false;
        private bool gunHolstered;
        private Transform gun;

        public GameObject cameraCoverInstance;
        
        private CharacterBody characterBody;
        private CharacterModel model;
        private ChildLocator childLocator;

        public static GameObject jetEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoDashJets.prefab").WaitForCompletion();
        private GameObject leftJetInstance;
        private GameObject rightJetInstance;

        private void Awake()
        {
            this.characterBody = this.gameObject.GetComponent<CharacterBody>();
            this.childLocator = this.gameObject.GetComponentInChildren<ChildLocator>();
            this.model = this.GetComponentInChildren<CharacterModel>();

            Invoke("HolsterGun", 0.5f);
        }

        private void LateUpdate()
        {
            // i hate to do it like this but changing the bone name doesn't work for some reason?
            if (this.gunHolstered)
            {
                this.gun.localPosition = new Vector3(-0.002f, -0.0015f, 0);
                this.gun.localRotation = Quaternion.Euler(0, 90, 0);
            }
        }

        private void HolsterGun()
        {
            if (this.characterBody)
            {
                if (this.characterBody.skillLocator.secondary.skillDef== Starstorm2Unofficial.Survivors.Nemmando.NemmandoCore.secondaryDistantGash && this.characterBody.skillLocator.special == Starstorm2Unofficial.Survivors.Nemmando.NemmandoCore.specialDecisiveStrike)
                {
                    this.gunHolstered = true;
                    this.gun = this.childLocator.FindChild("Gun");
                    this.gun.parent = this.childLocator.FindChild("Pelvis");

                    //this.characterBody._defaultCrosshairPrefab = Utils.LoadCrosshair("SimpleDot");
                }
            }
        }

        //Still doesn't work. Just going to not use this.
        public void ActivateThrusters()
        {
            //this.childLocator.FindChild("JetMuzzleL").gameObject.SetActive(true);
            //this.childLocator.FindChild("JetMuzzleR").gameObject.SetActive(true);
            Transform leftJet = this.childLocator.FindChild("JetMuzzleL");
            Transform rightJet = this.childLocator.FindChild("JetMuzzleR");
            if (leftJet && !leftJetInstance)
            {
                leftJetInstance = UnityEngine.Object.Instantiate<GameObject>(NemmandoController.jetEffect, leftJet);
            }
            if (rightJet && !rightJetInstance)
            {
                rightJetInstance = UnityEngine.Object.Instantiate<GameObject>(NemmandoController.jetEffect, rightJet);
            }
        }

        //Still doesn't work. Just going to not use this.
        public void DeactivateThrusters()
        {
            //this.childLocator.FindChild("JetMuzzleL").gameObject.SetActive(false);
            //this.childLocator.FindChild("JetMuzzleR").gameObject.SetActive(false);
            if (leftJetInstance)
            {
                Destroy(leftJetInstance);
                leftJetInstance = null;
            }
            if (rightJetInstance)
            {
                Destroy(leftJetInstance);
                leftJetInstance = null;
            }
        }

        public void CoverScreen()
        {
            if (this.cameraCoverInstance) return;

            Transform cameraTransform = this.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController.transform;
            this.cameraCoverInstance = GameObject.Instantiate(Modules.Assets.nemmandoCameraCover);

            this.cameraCoverInstance.transform.parent = cameraTransform;
            this.cameraCoverInstance.transform.localPosition = Vector3.zero;
            this.cameraCoverInstance.transform.localRotation = Quaternion.Euler(0, 0, 0);

            this.ApplyOverlays();
        }

        public void UncoverScreen()
        {
            if (this.cameraCoverInstance) GameObject.Destroy(this.cameraCoverInstance);
        }

        private void ApplyOverlays()
        {
            // this is by far the worst way to do this. i promise i will clean it up

            foreach (CharacterModel i in FindObjectsOfType<CharacterModel>())
            {
                if (i)
                {
                    TemporaryOverlay temporaryOverlay = i.gameObject.AddComponent<TemporaryOverlay>();
                    temporaryOverlay.duration = 5f * (EntityStates.SS2UStates.Nemmando.ChargedSlashAttack.baseDuration / this.characterBody.attackSpeed);
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 1f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = Modules.Assets.pureBlackMaterial;
                    temporaryOverlay.AddToCharacerModel(i.transform.GetComponent<CharacterModel>());
                }
            }
        }
    }
}