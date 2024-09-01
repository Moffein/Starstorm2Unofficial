using EntityStates.SS2UStates.Common;
using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class Headbutt : BaseMeleeAttack
    {
        public static GameObject swingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Lemurian/LemurianBiteTrail.prefab").WaitForCompletion();
        //public static GameObject hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniImpactVFX.prefab").WaitForCompletion();    //Why does this play sounds?

        public static new float damageCoefficient = 5f;
        public static float momentumStartPercent = 0.3f;
        public static float momentumFadePercent = 0.6f;
        public static float momentumEndPercent = 0.8f;
        public static float forwardSpeed = 36f;
        public static float baseStateDuration = 1f;   //was 0.7f
        public static float knockbackForce = 2700f;
        public static float yForce = 1200f;

        private Vector3 attackDirection;
        private Vector3 attackDirectionFlat;
        private float momentumStartTime;
        private float momentumFadeTime;
        private float momentumEndTime;

        private bool startedGrounded;
        private float lastUpdateTime;

        public override void OnEnter()
        {
            lastUpdateTime = Time.time;
            this.bonusForce = Vector3.zero;
            this.attackRecoil = 0f;

            this.damageType = DamageType.Stun1s;
            this.hitHopVelocity = 0f;
            this.scaleHitHopWithAttackSpeed = false;
            this.hitStopDuration = 0.1f;
            this.hitSoundString = "Play_acrid_m1_hit";
            this.swingSoundString = "Play_acrid_m1_bigSlash";
            this.hitboxName = "HeadbuttHitbox";
            base.damageCoefficient = Headbutt.damageCoefficient;
            this.procCoefficient = 1f;
            this.baseDuration = Headbutt.baseStateDuration;
            this.attackStartTime = 0.3f;
            this.attackEndTime = 0.8f;
            this.pushForce = 0f;
            this.forceForwardVelocity = false;
            this.swingEffectPrefab = Headbutt.swingEffect;
            this.hitEffectPrefab = null;
            this.muzzleString = "MuzzleHeadbutt";

            momentumStartTime = this.baseDuration * momentumStartPercent;
            momentumFadeTime = this.baseDuration * momentumFadePercent;
            momentumEndTime = this.baseDuration * momentumEndPercent;

            base.OnEnter();

            startedGrounded = base.isGrounded;

            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }
            if(this.attack != null)
            {
                this.attack.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ScaleForceToMass);
                this.attack.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ResetVictimForce);

                Ray aimRay = base.GetAimRay();
                attackDirection = aimRay.direction;

                attackDirectionFlat = attackDirection;
                attackDirectionFlat.y = 0;
                attackDirectionFlat.Normalize();

                this.attack.forceVector = attackDirectionFlat * Headbutt.knockbackForce;
                this.attack.forceVector.y += Headbutt.yForce;

                this.attack.forceVector = attackDirection * Headbutt.knockbackForce;
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            if (!this.hasFired && this.stopwatch < momentumStartTime)
            {
                Ray aimRay = base.GetAimRay();
                attackDirection = aimRay.direction;

                attackDirectionFlat = attackDirection;
                attackDirectionFlat.y = 0;
                attackDirectionFlat.Normalize();

                if (this.attack != null)
                {
                    this.attack.forceVector = attackDirectionFlat * Headbutt.knockbackForce;
                    this.attack.forceVector.y += Headbutt.yForce;
                }

                this.startedSkillStationary = base.inputBank && base.inputBank.moveVector == Vector3.zero;
                if (base.inputBank && !this.startedSkillStationary)
                {
                    Vector2 moveDirectionFlat = new Vector2(base.inputBank.moveVector.x, base.inputBank.moveVector.z);
                    Vector2 forwardDirectionFlat = new Vector2(aimRay.direction.x, aimRay.direction.z);

                    float angle = Vector2.Angle(moveDirectionFlat, forwardDirectionFlat);

                    if (angle >= 50f)
                    {
                        this.startedSkillStationary = true;
                    }
                }
            }

            if (base.isAuthority)
            {

                if (!this.startedSkillStationary && !this.inHitPause && this.stopwatch >= this.momentumStartTime && this.stopwatch <= this.momentumEndTime)
                {
                    float evaluatedForwardSpeed = Headbutt.forwardSpeed * deltaTime;
                    if (this.stopwatch >= this.momentumFadeTime)
                    {
                        evaluatedForwardSpeed *= Mathf.Lerp(1f, 0f, (this.stopwatch - this.momentumFadeTime)/(this.momentumEndTime - this.momentumFadeTime));
                    }
                    if (base.characterMotor && evaluatedForwardSpeed > 0f)
                    {
                        if (base.characterDirection)
                        {
                            base.characterDirection.forward = attackDirection;
                        }
                        Vector3 evaluatedForwardVector = attackDirection * evaluatedForwardSpeed;

                        base.characterMotor.AddDisplacement(new Vector3(evaluatedForwardVector.x, evaluatedForwardVector.y, evaluatedForwardVector.z));

                        if (!startedGrounded && base.characterMotor.velocity.y < 0f) base.characterMotor.velocity.y = 0f;
                    }
                    else
                    {
                        //Use this to cancel lunge
                        startedSkillStationary = true;
                    }
                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Additive", "Secondary", "Secondary.playbackRate", this.baseDuration);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
