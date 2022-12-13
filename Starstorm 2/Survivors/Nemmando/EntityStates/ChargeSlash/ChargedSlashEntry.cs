using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Starstorm2States.Nemmando
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
        private Vector3 previousPosition;
        private ChildLocator childLocator;
        private ParticleSystem dashEffect;

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

            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity.y *= 0.1f;
                base.characterMotor.velocity = this.forwardDirection * this.dashSpeed;
            }

            base.PlayAnimation("FullBody, Override", "DecisiveStrikeDash");

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            Vector3 b = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
            this.previousPosition = base.transform.position - b;

            this.dashEffect = this.childLocator.FindChild("DashEffect").GetComponent<ParticleSystem>();
            if (this.dashEffect) this.dashEffect.Play();

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1.5f * this.duration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matDoppelganger");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
        }

        private void RecalculateSpeed()
        {
            this.dashSpeed = (4 + 0.25f * this.moveSpeedStat) * this.speedCoefficient;
        }

        public override void OnExit()
        {
            if (base.characterMotor) base.characterMotor.disableAirControlUntilCollision = false;

            base.characterMotor.velocity = Vector3.zero;

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.isSprinting = true;

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                ChargedSlashAttack nextState = new ChargedSlashAttack();
                nextState.charge = charge;
                nextState.camOverrideHandle = camOverrideHandle;
                this.outer.SetNextState(nextState);
                return;
            }

            this.RecalculateSpeed();

            if (base.isAuthority)
            {
                Vector3 normalized = (base.transform.position - this.previousPosition).normalized;

                if (base.characterDirection)
                {
                    if (normalized != Vector3.zero)
                    {
                        Vector3 vector = normalized * this.dashSpeed;
                        float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
                        vector = this.forwardDirection * d;
                        vector.y = base.characterMotor.velocity.y;
                        base.characterMotor.velocity = vector;
                    }

                    base.characterDirection.forward = this.forwardDirection;
                }

                this.previousPosition = base.transform.position;
            }
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.forwardDirection);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            forwardDirection = reader.ReadVector3();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}