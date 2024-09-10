using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargedSlashEntry : BaseSkillState
    {
        public float charge;
        public static float maxRecoil = 5f;
        public static float minRecoil = 0.4f;
        public static float initialMaxSpeedCoefficient = 23f;
        public static float initialMinSpeedCoefficient = 2f;
        public static float minDuration = 0.2f;
        public static float maxDuration = 0.25f;

        private float speedCoefficient;
        private float recoil;
        private float duration;

        private float dashSpeed;
        private Vector3 forwardDirection;
        private ChildLocator childLocator;
        private ParticleSystem dashEffect;

        private float lastUpdateTime;

        public CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        private CharacterCameraParamsData decisiveCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 2f, //how far up should the camera go?
            idealLocalCameraPos = zoomCameraPosition,
            wallCushion = 0.1f
        };
        public static Vector3 zoomCameraPosition = new Vector3(0f, 0f, -14f); // how far back should the camera go?

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            base.characterBody.isSprinting = true;

            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle, .25f);
                CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = decisiveCameraParams,
                    priority = 0f
                };
                camOverrideHandle = cameraTargetParams.AddParamsOverride(request, duration);
            }

            this.duration = Util.Remap(this.charge, 0f, 1f, ChargedSlashEntry.minDuration, ChargedSlashEntry.maxDuration);
            this.speedCoefficient = Util.Remap(this.charge, 0f, 1f, ChargedSlashEntry.initialMinSpeedCoefficient, ChargedSlashEntry.initialMaxSpeedCoefficient);
            this.recoil = Util.Remap(this.charge, 0f, 1f, ChargedSlashEntry.minRecoil, ChargedSlashEntry.maxRecoil);

            if (base.GetTeam() == TeamIndex.Monster) this.speedCoefficient = 0f;

            this.childLocator = base.GetModelChildLocator();

            this.forwardDirection = base.GetAimRay().direction;

            this.RecalculateSpeed();

            if (base.characterMotor)
            {
                base.characterMotor.velocity = Vector3.zero;
            }

            base.PlayAnimation("FullBody, Override", "DecisiveStrikeDash");

            this.dashEffect = this.childLocator.FindChild("DashEffect").GetComponent<ParticleSystem>();
            if (this.dashEffect) this.dashEffect.Play();

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                CharacterModel cm = modelTransform.GetComponent<CharacterModel>();
                if (cm)
                {
                    TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay.duration = 1.5f * this.duration;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matDoppelganger");
                    temporaryOverlay.AddToCharacterModel(cm);
                    temporaryOverlay.Start();
                }
            }
        }

        private void RecalculateSpeed()
        {
            float moveSpeed = Starstorm2Unofficial.Modules.Config.NemmandoDecisiveMoveSpeedScaling.Value ? this.moveSpeedStat : 10.15f;
            this.dashSpeed = (4 + 0.25f * moveSpeed) * this.speedCoefficient;
        }

        public override void OnExit()
        {
            if (base.characterMotor)
            {
                base.characterMotor.disableAirControlUntilCollision = false;
                base.characterMotor.velocity = Vector3.zero;
            }

            if (!this.outer.destroying) base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (cameraTargetParams) cameraTargetParams.RemoveParamsOverride(camOverrideHandle, .25f);


            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            base.characterBody.isSprinting = true;

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                ChargedSlashAttack nextState = new ChargedSlashAttack();
                nextState.charge = charge;
                //nextState.camOverrideHandle = camOverrideHandle;
                this.outer.SetNextState(nextState);
                return;
            }

            this.RecalculateSpeed();

            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    base.characterMotor.velocity = Vector3.zero;

                    Vector3 moveVector = this.forwardDirection * this.dashSpeed;
                    float distance = Mathf.Max(Vector3.Dot(moveVector, this.forwardDirection), 0f);
                    moveVector = this.forwardDirection * distance;
                    base.characterMotor.rootMotion += moveVector * deltaTime;
                }

                if (base.characterDirection) base.characterDirection.forward = this.forwardDirection;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}