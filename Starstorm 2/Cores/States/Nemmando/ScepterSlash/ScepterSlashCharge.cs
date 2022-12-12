using EntityStates;
using RoR2;
using Starstorm2.Components;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando
{
    public class ScepterSlashCharge : BaseSkillState
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

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = ScepterSlashCharge.baseChargeDuration;// / this.attackSpeedStat;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();
            this.nemmandoController = base.GetComponent<NemmandoController>();
            this.zoomin = false;
            base.characterBody.hideCrosshair = true;
            if (this.nemmandoController) this.nemmandoController.chargingDecisiveStrike = true;

            if (base.characterBody.skinIndex == 2) this.minEmission = 70f;
            else this.minEmission = 0f;

            this.swordVFX = this.childLocator.FindChild("SwordChargeEffect").GetComponent<ParticleSystem>();

            var main = this.swordVFX.main;
            main.startLifetime = this.chargeDuration;

            main = this.swordVFX.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startLifetime = this.chargeDuration;

            main = this.swordVFX.transform.GetChild(1).GetComponent<ParticleSystem>().main;
            main.startDelay = this.chargeDuration;

            this.swordVFX.Play();
            this.chargePlayID = Util.PlayAttackSpeedSound("NemmandoDecisiveStrikeCharge", base.gameObject, this.attackSpeedStat);
            base.PlayAnimation("FullBody, Override", "DecisiveStrikeCharge", "DecisiveStrike.playbackRate", this.chargeDuration);

            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.OverTheShoulder;

            this.swordMat = base.GetModelTransform().GetComponent<ModelSkinController>().skins[base.characterBody.skinIndex].rendererInfos[1].defaultMaterial;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterMotor.velocity = Vector3.zero;
            float charge = this.CalcCharge();

            this.swordMat.SetFloat("_EmPower", Util.Remap(charge, 0, 1, this.minEmission, ScepterSlashAttack.swordEmission));

            if (charge >= 0.6f && !this.zoomin)
            {
                this.zoomin = true;
                if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }

            if (charge >= 1f && !this.finishedCharge)
            {
                this.finishedCharge = true;

                AkSoundEngine.StopPlayingID(this.chargePlayID);
                Util.PlaySound("NemmandoDecisiveStrikeReady", base.gameObject);

                if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
                if (this.nemmandoController) this.nemmandoController.CoverScreen();
            }

            if (base.isAuthority && (base.fixedAge >= 1.25f * this.chargeDuration || !base.IsKeyDownAuthority() && base.fixedAge >= 0.1f))
            {
                ScepterSlashEntry nextState = new ScepterSlashEntry();
                nextState.charge = charge;
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

            base.PlayAnimation("Gesture, Override", "BufferEmpty");

            AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (base.cameraTargetParams)
            {
                base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Aura;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}