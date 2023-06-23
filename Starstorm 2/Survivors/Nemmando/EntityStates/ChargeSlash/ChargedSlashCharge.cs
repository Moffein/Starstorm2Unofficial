using EntityStates;
using EntityStates.SS2UStates.Common;
using RoR2;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Survivors.Nemmando.Components;
using UnityEngine;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargedSlashCharge : BaseCustomSkillState
    {
        public static float baseChargeDuration = 1.75f;

        private float chargeDuration;
        private bool finishedCharge;
        private ChildLocator childLocator;
        private Animator animator;
        private Transform modelBaseTransform;
        private uint chargePlayID;
        private ParticleSystem swordVFX;
        private NemmandoController nemmandoController;
        private bool zoomin;
        private Material swordMat;
        private float minEmission;
        private GameObject chargeEffectInstance;
        private Transform areaIndicator;

        public CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        private CharacterCameraParamsData decisiveCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1f, //how far up should the camera go?
            idealLocalCameraPos = zoomCameraPosition,
            wallCushion = 0.1f
        };
        public static Vector3 zoomCameraPosition = new Vector3(0f, 0f, -5.3f); // how far back should the camera go?

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = ChargedSlashCharge.baseChargeDuration / this.attackSpeedStat;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();
            this.nemmandoController = base.GetComponent<NemmandoController>();
            this.zoomin = false;
            base.characterBody.hideCrosshair = true;
            if (this.nemmandoController) this.nemmandoController.chargingDecisiveStrike = true;

            this.minEmission = this.effectComponent.defaultSwordEmission;

            this.swordVFX = this.childLocator.FindChild(this.effectComponent.chargeEffectString).GetComponent<ParticleSystem>();

            var main = this.swordVFX.main;
            main.startLifetime = this.chargeDuration;

            main = this.swordVFX.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startLifetime = this.chargeDuration;

            main = this.swordVFX.transform.GetChild(1).GetComponent<ParticleSystem>().main;
            main.startDelay = this.chargeDuration;

            this.swordVFX.Play();
            this.chargePlayID = Util.PlayAttackSpeedSound("SS2UNemmandoDecisiveStrikeCharge", base.gameObject, this.attackSpeedStat);
            base.PlayAnimation("FullBody, Override", "DecisiveStrikeCharge", "DecisiveStrike.playbackRate", this.chargeDuration);

            if (cameraTargetParams)
            {
                CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = decisiveCameraParams,
                    priority = 0f
                };
                camOverrideHandle = cameraTargetParams.AddParamsOverride(request, chargeDuration);
            }

            this.swordMat = base.GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

            if (base.GetTeam() == TeamIndex.Monster)
            {
                this.chargeEffectInstance = GameObject.Instantiate(new EntityStates.ImpBossMonster.BlinkState().blinkDestinationPrefab, base.gameObject.transform);
                this.chargeEffectInstance.transform.position = base.characterBody.corePosition;
                this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;
                this.areaIndicator = this.chargeEffectInstance.transform.Find("Particles").Find("AreaIndicator");

                this.chargeEffectInstance.GetComponentInChildren<PostProcessDuration>().maxDuration = this.chargeDuration;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            float charge = this.CalcCharge();

            this.swordMat.SetFloat("_EmPower", Util.Remap(charge, 0, 1, this.minEmission, ChargedSlashAttack.maxEmission));

            if (this.areaIndicator) this.areaIndicator.localScale = Vector3.one * Util.Remap(charge, 0f, 1f, ChargedSlashAttack.minRadius, ChargedSlashAttack.maxRadius);

            if (charge >= 0.6f && !this.zoomin)
            {
                this.zoomin = true;
            }

            if (charge >= 1f && !this.finishedCharge)
            {
                this.finishedCharge = true;

                AkSoundEngine.StopPlayingID(this.chargePlayID);
                Util.PlaySound("SS2UNemmandoDecisiveStrikeReady", base.gameObject);
            }

            bool keyDown = base.IsKeyDownAuthority();
            if (base.GetTeam() == TeamIndex.Monster) keyDown = true;

            if (base.isAuthority && (base.fixedAge >= 1.25f * this.chargeDuration || !keyDown && base.fixedAge >= 0.1f))
            {
                ChargedSlashEntry nextState = new ChargedSlashEntry();
                nextState.charge = charge;
                nextState.camOverrideHandle = this.camOverrideHandle;
                this.outer.SetNextState(nextState);
            }
        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.chargeDuration);
        }

        public override void OnExit()
        {
            base.OnExit();
            this.swordVFX.gameObject.SetActive(false);
            this.swordVFX.gameObject.SetActive(true);

            if (this.nemmandoController) this.nemmandoController.chargingDecisiveStrike = false;
            if (this.chargeEffectInstance) EntityState.Destroy(this.chargeEffectInstance);

            if (!this.outer.destroying) base.PlayAnimation("FullBody, Override", "BufferEmpty");

            AkSoundEngine.StopPlayingID(this.chargePlayID);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}