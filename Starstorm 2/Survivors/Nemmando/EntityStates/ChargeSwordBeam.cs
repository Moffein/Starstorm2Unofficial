using EntityStates;
using EntityStates.SS2UStates.Common;
using RoR2;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargeSwordBeam : BaseCustomSkillState
    {
        public static float baseChargeDuration = 1f;

        public static float maxEmission;
        public static float minEmission;

        public static GameObject crosshairOverridePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/MageCrosshair");

        private Material swordMat;
        private float chargeDuration;
        private ChildLocator childLocator;
        private Animator animator;
        private Transform modelBaseTransform;
        private ParticleSystem swordVFX;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private uint chargePlayID;
        private float maximumEmission;
        private float minimumEmission;

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = ChargeSwordBeam.baseChargeDuration / this.attackSpeedStat;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();

            this.minimumEmission = this.effectComponent.defaultSwordEmission;
            this.maximumEmission = 50f + this.minimumEmission;

            if (ChargeSwordBeam.crosshairOverridePrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairOverridePrefab, CrosshairUtils.OverridePriority.PrioritySkill);
            }

            this.swordVFX = FindModelChild("SwordLightning").GetComponent<ParticleSystem>();
            this.swordVFX.Play();

            bool moving = this.animator.GetBool("isMoving");
            bool grounded = this.animator.GetBool("isGrounded");

            base.PlayCrossfade("UpperBody, Override", "Secondary1", "Secondary.rate", this.chargeDuration, 0.05f);

            this.swordMat = base.GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

            this.chargePlayID = Util.PlaySound("NemmandoChargeBeam2", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float charge = this.CalcCharge();

            base.characterBody.SetSpreadBloom(Util.Remap(charge, 0f, 1f, 0f, 3f), true);

            base.StartAimMode();

            if (this.swordMat)
            {
                this.swordMat.SetFloat("_EmPower", Util.Remap(charge, 0, 1, this.minimumEmission, this.maximumEmission));
                this.swordMat.SetColor("_EmColor", Color.white);
            }

            if (base.isAuthority && (charge >= 1f || (!base.IsKeyDownAuthority() && base.fixedAge >= 0.1f)))
            {
                FireSwordBeam nextState = new FireSwordBeam();
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
            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            AkSoundEngine.StopPlayingID(this.chargePlayID);
            this.swordVFX.Stop();
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}