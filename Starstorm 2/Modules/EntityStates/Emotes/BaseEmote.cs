using EntityStates;
using RoR2;
using Starstorm2.Modules;
using UnityEngine;
using static RoR2.CameraTargetParams;

namespace EntityStates.SS2UStates.Common.Emotes
{
    public class BaseEmote : BaseState
    {
        public string soundString;
        public string animString;
        public float duration;
        public float animDuration;
        public bool normalizeModel;

        private uint activePlayID;
        private Animator animator;
        private ChildLocator childLocator;

        private static CharacterCameraParamsData emoteCameraParams = new CharacterCameraParamsData()
        {
            maxPitch = 70,
            minPitch = -70,
            pivotVerticalOffset = 1f,
            idealLocalCameraPos = new Vector3(0, 0.0f, -7.9f),
            wallCushion = 0.1f,
        };
        private CameraParamsOverrideHandle camOverrideHandle;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.childLocator = base.GetModelChildLocator();

            base.characterBody.hideCrosshair = true;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = false;
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 0);
            this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 0);

            if (this.animDuration == 0 && this.duration != 0) this.animDuration = this.duration;

            base.PlayAnimation("FullBody, Override", this.animString, "Emote.playbackRate", this.animDuration);

            this.activePlayID = Util.PlaySound(soundString, base.gameObject);

            if (this.normalizeModel)
            {
                if (base.modelLocator)
                {
                    base.modelLocator.normalizeToFloor = true;
                }
            }

            CameraParamsOverrideRequest request = new CameraParamsOverrideRequest
            {
                cameraParamsData = emoteCameraParams,
                priority = 0,
            };

            camOverrideHandle = base.cameraTargetParams.AddParamsOverride(request, 0.5f);
        }

        public override void OnExit()
        {
            base.OnExit();

            base.characterBody.hideCrosshair = false;

            if (base.GetAimAnimator()) base.GetAimAnimator().enabled = true;
            if (this.animator)
            {
                this.animator.SetLayerWeight(animator.GetLayerIndex("AimPitch"), 1);
                this.animator.SetLayerWeight(animator.GetLayerIndex("AimYaw"), 1);
            }

            if (this.normalizeModel)
            {
                if (base.modelLocator)
                {
                    base.modelLocator.normalizeToFloor = false;
                }
            }

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (this.activePlayID != 0) AkSoundEngine.StopPlayingID(this.activePlayID);

            base.cameraTargetParams.RemoveParamsOverride(camOverrideHandle, 0.5f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            bool flag = false;

            if (base.characterMotor)
            {
                if (!base.characterMotor.isGrounded) flag = true;
                if (base.characterMotor.velocity != Vector3.zero) flag = true;
            }

            if (base.inputBank)
            {
                if (base.inputBank.skill1.down) flag = true;
                if (base.inputBank.skill2.down) flag = true;
                if (base.inputBank.skill3.down) flag = true;
                if (base.inputBank.skill4.down) flag = true;
                if (base.inputBank.jump.down) flag = true;

                if (base.inputBank.moveVector != Vector3.zero) flag = true;
            }

            //emote cancels
            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Starstorm2.Modules.Config.restKeybind))
                {
                    this.outer.SetInterruptState(new RestEmote(), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Starstorm2.Modules.Config.tauntKeybind))
                {
                    this.outer.SetInterruptState(new TauntEmote(), InterruptPriority.Any);
                    return;
                }
            }

            if (this.duration > 0 && base.fixedAge >= this.duration) flag = true;

            if (flag)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}
