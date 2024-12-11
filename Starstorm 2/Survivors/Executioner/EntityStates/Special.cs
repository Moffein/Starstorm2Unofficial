﻿using R2API;
using Rewired.Demos;
using RoR2;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Executioner.Components;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.SS2UStates.Executioner
{
    public class ExecutionerAxe : BaseSkillState
    {
        public static float baseDuration = 0.85f;    //was 0.85
        public static float velocityMultiplier = 0.6f;    //was 0.6

        private float duration;
        private Animator animator;
        private ExecutionerController exeController;

        private float lastUpdateTime;

        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        public static CharacterCameraParamsData slamCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1f, //how far up should the camera go?
            idealLocalCameraPos = new Vector3(0f, 0f, -17.5f),
            wallCushion = 0.1f
        };

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            this.animator = base.GetModelAnimator();
            this.exeController = base.GetComponent<ExecutionerController>();
            this.duration = ExecutionerAxe.baseDuration;// / this.attackSpeedStat;
            base.PlayAnimation("FullBody, Override", "Special1", "Special.playbackRate", 0.9f * this.duration);

            Util.PlaySound("SS2UExecutionerSpecialCast", base.gameObject);

            if (this.exeController) this.exeController.PlayAxeSpawnEffect();

            if (base.isAuthority)
            {
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.jumpCount = base.characterBody.maxJumpCount;
                base.characterMotor.velocity *= 0.5f;

                EntityStateMachine dashMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Slide");
                if (dashMachine.state.GetType() == typeof(ExecutionerDash))
                {
                    dashMachine.SetNextStateToMain();
                }
            }

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = slamCameraParams,
                priority = 0f
            };
            camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0.15f);

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                CharacterModel cm = modelTransform.GetComponent<CharacterModel>();
                if (cm)
                {
                    TemporaryOverlayInstance temporaryOverlay = TemporaryOverlayManager.AddOverlay(cm.gameObject);
                    temporaryOverlay.duration = this.duration * 0.5f;
                    temporaryOverlay.animateShaderAlpha = true;
                    temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                    temporaryOverlay.destroyComponentOnEnd = true;
                    temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                    temporaryOverlay.AddToCharacterModel(cm);
                    temporaryOverlay.Start();
                }
            }
        }

        public override void OnExit()
        {
            if (!this.outer.destroying) base.PlayAnimation("FullBody, Override", "BufferEmpty");
            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle, .4f);
            }

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            float moveSpeed = 11f;//Mathf.Clamp(0f, 11f, 0.5f * this.moveSpeedStat); //Don't scale with movement speed or else you launch yourself into the skybox.
            base.characterMotor.rootMotion += velocityMultiplier * Vector3.up * (moveSpeed * Mage.FlyUpState.speedCoefficientCurve.Evaluate(base.fixedAge / this.duration) * deltaTime);
            base.characterMotor.velocity.y = 0f;

            base.characterMotor.moveDirection *= 2f;

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                SetNextState();
                return;
            }
        }

        public virtual void SetNextState()
        {
            this.outer.SetNextState(new ExecutionerAxeSlam());
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }

    public class ExecutionerAxeSlam : BaseSkillState
    {
        public static float damageCoefficient = 10f;
        public static float procCoefficient = 1.0f;
        //shorter value if axe slam should be finite
        //public static float baseDuration = 0.4f;
        public static float baseDuration = 10f;
        public static float dropAttackRadius = 4f;
        public static float slamRadius = 14f;
        public static float rechargePerKill = 1.0f;

        private GameObject slamEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/VagrantCannonExplosion");
        private EffectData slamEffectData;
        private Animator animator;
        private bool crit;

        public float radiusInternal;
        public float damageCoefficientInternal;

        private CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        public static CharacterCameraParamsData slamCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1f, //how far up should the camera go?
            idealLocalCameraPos = new Vector3(0f, 0f, -17.5f),
            wallCushion = 0.1f
        };

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();

            base.PlayAnimation("FullBody, Override", "Special2", "Special.playbackRate", 0.15f);

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

            if (base.isAuthority)
            {
                base.characterMotor.velocity.y = -120f;
                base.characterMotor.rootMotion.y -= 0.5f;
                base.characterMotor.velocity.x = 0f;
                base.characterMotor.velocity.z = 0f;
                this.crit = RollCrit();
            }

            CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
            {
                cameraParamsData = slamCameraParams,
                priority = 0f
            };
            camOverrideHandle = cameraTargetParams.AddParamsOverride(request, 0.15f);

            damageCoefficientInternal = ExecutionerAxeSlam.damageCoefficient;
            radiusInternal = ExecutionerAxeSlam.slamRadius;
            OverrideStats();
        }

        public virtual void OverrideStats() { }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (base.characterMotor.velocity.y > -200f) base.characterMotor.velocity.y = -200f;

                bool hitEnemy = base.characterBody ? IsEnemyInSphere(5f, base.characterBody.footPosition, base.GetTeam(), true) : false;

                if (base.fixedAge >= baseDuration || base.characterMotor.isGrounded || hitEnemy)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            slamEffectData = new EffectData()
            {
                color = new Color32(255, 255, 255, 255),
                scale = slamRadius,
                origin = base.characterBody.footPosition
            };

            float recoil = 8f;
            base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);

            base.characterMotor.velocity *= 0.1f;
            base.SmallHop(base.characterMotor, 10f);

            base.PlayAnimation("FullBody, Override", "Special4", "Special.playbackRate", 0.4f);
            Util.PlayAttackSpeedSound("SS2UExecutionerSpecialImpact", base.gameObject, this.attackSpeedStat);
            //Util.PlayAttackSpeedSound(Engi.EngiWeapon.FireGrenades.attackSoundString, base.gameObject, this.attackSpeedStat);
            EffectManager.SpawnEffect(slamEffect, slamEffectData, true);
            EffectManager.SpawnEffect(Starstorm2Unofficial.Modules.Assets.exeAxeSlamEffect, slamEffectData, true);

            base.characterBody.bodyFlags -= CharacterBody.BodyFlags.IgnoreFallDamage;

            if (base.isAuthority)
            {
                Vector3 atkCenter = base.characterBody.footPosition;
                BlastAttack blast = new BlastAttack()
                {
                    radius = slamRadius,
                    procCoefficient = procCoefficient,
                    position = atkCenter,
                    attacker = base.gameObject,
                    teamIndex = base.teamComponent.teamIndex,
                    crit = this.crit,
                    baseDamage = (base.characterBody.damage * ExecutionerAxeSlam.damageCoefficient),
                    damageColorIndex = DamageColorIndex.Default,
                    falloffModel = BlastAttack.FalloffModel.None,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    damageType = DamageType.Generic,
                    bonusForce = Vector3.down * 4000f
                };
                blast.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.SlayerExceptItActuallyWorks);
                blast.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.AntiFlyingForce);
                blast.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ResetVictimForce);
                blast = ModifyBlastAttack(blast);
                blast.damageType.damageSource = DamageSource.Special;
                blast.Fire();

                base.characterMotor.velocity.y = 0f;
            }

            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle, .4f);
            }

            base.OnExit();
        }

        public virtual BlastAttack ModifyBlastAttack(BlastAttack blast)
        {
            return blast;
        }

        private bool IsEnemyInSphere(float radius, Vector3 position, TeamIndex team, bool airborneOnly = false)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (!airborneOnly ||
                                (healthComponent.body.isFlying ||
                                (healthComponent.body.characterMotor && !healthComponent.body.characterMotor.isGrounded)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}