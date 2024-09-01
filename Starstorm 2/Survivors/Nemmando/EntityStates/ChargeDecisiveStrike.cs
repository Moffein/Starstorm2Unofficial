using EntityStates;
using EntityStates.Captain.Weapon;
using EntityStates.SS2UStates.Common;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargeDecisiveStrike : BaseCustomSkillState
    {
        public static float baseMaxChargeTime = 1F;
        private static GameObject chargeupVfxPrefab = ChargeCaptainShotgun.chargeupVfxPrefab;
        public static float damageCoef = 2.5f;
        public static float procCoef = 1.0f;

        private float speedCoef;
        private float initialSpeedCoefficient = 2.5f;
        private float finalSpeedCoefficient = 7.5f;
        private float buttonReleaseTime = 0f;
        private float minChargeTime = 0.35f;
        private bool buttonReleased = false;
        public float baseDashTime = 0.35f;
        private bool chargeAnimationPlayed = false;
        private bool jumpAnimationPlayed = false;
        private OverlapAttack attack;
        private float massThresholdForKnockback = 300f;

        private List<HurtBox> victimsStruck = new List<HurtBox>();
        private Vector3 dashDirection;
        private Animator animator;

        public float charge;
        public float lastCharge;
        public float maxChargeTime;

        private float lastUpdateTime;

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            this.animator = base.GetModelAnimator();
            this.maxChargeTime = ChargeDecisiveStrike.baseMaxChargeTime / this.attackSpeedStat;

            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "SwordHitboxLarge");
            }

            this.attack = new OverlapAttack();
            this.attack.attacker = base.gameObject;
            this.attack.inflictor = base.gameObject;
            this.attack.teamIndex = base.GetTeam();
            this.attack.damage = 0;
            this.attack.hitBoxGroup = hitBoxGroup;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            base.characterBody.SetAimTimer(1f);

            if(base.isAuthority)
            {
                if ((base.fixedAge >= this.maxChargeTime || !base.inputBank || !base.inputBank.skill2.down))
                {
                    if (!this.buttonReleased)
                    {
                        if (!jumpAnimationPlayed && base.fixedAge >= this.minChargeTime)
                        {
                            base.PlayAnimation("FullBody, Override", "Secondary3", "Secondary.rate", 0.05f);
                            jumpAnimationPlayed = true;
                        }

                        this.buttonReleased = true;
                        this.buttonReleaseTime = base.fixedAge;
                        this.dashDirection = base.GetAimRay().direction;
                        RecalculateDashSpeed();
                    }
                }
                else
                {
                    this.lastCharge = this.charge;
                    this.charge = base.fixedAge / this.maxChargeTime;
                    if (!chargeAnimationPlayed && base.fixedAge >= this.minChargeTime)
                    {
                        base.PlayAnimation("UpperBody, Override", "Secondary1", "Secondary.rate", 0.05f);
                        chargeAnimationPlayed = true;
                    }
                }

                if (this.buttonReleased)
                {
                    if (this.charge > minChargeTime)
                    {
                        if (this.fixedAge <= this.buttonReleaseTime + this.baseDashTime)
                        {
                            if (base.characterMotor && base.characterDirection)
                            {
                                if (this.attack.Fire(this.victimsStruck))
                                {
                                    Util.PlaySound(EntityStates.Merc.Assaulter.beginSoundString, base.gameObject);
                                    this.outer.SetNextState(new FireDecisiveStrike
                                    {
                                        charge = this.charge
                                    });
                                    return;
                                }

                                base.characterMotor.velocity = Vector3.zero;
                                base.characterMotor.rootMotion += this.dashDirection * (this.characterBody.baseMoveSpeed * this.speedCoef * deltaTime);
                                base.characterBody.isSprinting = true;
                            }
                        }
                        else
                        {
                            FireDecisiveStrike fireDecisiveStrike = new FireDecisiveStrike();
                            fireDecisiveStrike.charge = this.charge;

                            this.outer.SetNextState(fireDecisiveStrike);
                            return;
                        }
                    }
                    else
                    {
                        bool @bool = this.animator.GetBool("isMoving");
                        bool bool2 = this.animator.GetBool("isGrounded");
                        if (!@bool && bool2)
                        {
                            base.PlayCrossfade("FullBody, Override", "Special", "Secondary.rate", minChargeTime / this.attackSpeedStat, 0.05f);
                        }
                        else
                        {
                            base.PlayCrossfade("UpperBody, Override", "Special", "Secondary.rate", minChargeTime / this.attackSpeedStat, 0.05f);
                        }
                        Util.PlaySound(EntityStates.Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);

                        if (base.isAuthority)
                        {
                            var aimRay = this.GetAimRay();
                            new BulletAttack
                            {
                                owner = base.gameObject,
                                weapon = base.gameObject,
                                origin = aimRay.origin,
                                aimVector = aimRay.direction.normalized,
                                minSpread = 0f,
                                maxSpread = 0f,
                                bulletCount = 1,
                                procCoefficient = procCoef,
                                damage = this.damageStat * damageCoef,
                                force = 0f,
                                falloffModel = BulletAttack.FalloffModel.None,
                                tracerEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.tracerEffectPrefab,
                                muzzleName = "AimOrigin",
                                hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                                isCrit = this.RollCrit(),
                                HitEffectNormal = false,
                                radius = 1f,
                                damageType = DamageType.Generic,
                                stopperMask = LayerIndex.world.mask,
                            }.Fire();
                        }
                        this.outer.SetNextStateToMain();
                        return;
                    }
                }
            }
        }
        
        private void RecalculateDashSpeed()
        {
            this.speedCoef = Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, this.charge);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

