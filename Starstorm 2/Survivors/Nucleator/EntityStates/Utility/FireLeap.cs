using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.AddressableAssets;
using Starstorm2Unofficial.Cores;
using R2API;
using BepInEx.Configuration;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class FireLeap : BaseCharacterMain
    {
        public static ConfigEntry<bool> leapAirControl;
        public static float minimumDuration = 0.3f;
        public static float upwardVelocity = 7f;
        public static float forwardVelocity = 3f;
        public static float aimVelocity = 3f;
        public static float airControl = 0.15f;
        public static float minimumY = 0.05f; //Determines whether leap should be able to be aimed downwards.
        public static float maxExitYVelocity = 24f; //Prevent yourself from being launched into instadeath fall damage.

        public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderGroundSlam.prefab").WaitForCompletion();
        public static float blastForce = 3000f;

        public static string soundLoopStartEvent = "Play_acrid_shift_fly_loop";
        public static string soundLoopStopEvent = "Stop_acrid_shift_fly_loop";

        public static string leapSoundString = "SS2UNucleatorSkill3";

        public float charge;
        private float previousAirControl;
        protected bool isCrit;
        private bool detonateNextFrame;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(soundLoopStartEvent, base.gameObject);
            base.PlayAnimation("Body", "UtilityRelease");

            previousAirControl = base.characterMotor.airControl;
            detonateNextFrame = false;
            isCrit = base.RollCrit();
            this.damageStat *= this.attackSpeedStat;

            base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            if (base.isAuthority)
            {
                base.characterBody.isSprinting = true;
                base.characterBody.RecalculateStats();  //Get sprint bonus
                this.moveSpeedStat = base.characterBody.moveSpeed;

                if (!leapAirControl.Value)
                {
                    base.characterMotor.airControl = 0.15f;
                }
                else
                {
                    float moveSpeedCoeff = base.characterBody.moveSpeed / (base.characterBody.baseMoveSpeed * (!base.characterBody.isSprinting ? 1f : base.characterBody.sprintingSpeedMultiplier));
                    moveSpeedCoeff = Mathf.Min(moveSpeedCoeff, 3f);
                    base.characterMotor.airControl *= moveSpeedCoeff;
                }
            }

            if (NetworkServer.active)
            {
                base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
            }

            if (base.isAuthority)
            {
                Vector3 direction = base.GetAimRay().direction;

                if (base.characterMotor.isGrounded)
                {
                    direction.y = Mathf.Max(direction.y, minimumY);
                }

                Vector3 a = direction.normalized * aimVelocity * this.moveSpeedStat * CalculateChargeMultiplier();
                Vector3 b = Vector3.up * upwardVelocity;
                Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * forwardVelocity;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = a + b + b2;
                base.characterMotor.onMovementHit += this.OnMovementHit;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    bool passedMinDuration = base.fixedAge >= minimumDuration;
                    if (passedMinDuration && Starstorm2Unofficial.SneedUtils.IsEnemyInSphere(4f, base.transform.position, base.GetTeam(), true)) detonateNextFrame = true;

                    base.characterMotor.moveDirection = base.inputBank.moveVector;
                    bool hitGround = base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround;
                    if (passedMinDuration && (this.detonateNextFrame || hitGround))
                    {
                        DetonateAuthority();
                        this.outer.SetNextStateToMain();
                    }
                }
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(soundLoopStopEvent, base.gameObject);
            base.characterMotor.airControl = this.previousAirControl;
            if (base.isAuthority)
            {
                base.characterMotor.onMovementHit -= this.OnMovementHit;
            }

            base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
            if (NetworkServer.active)
            {
                base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
            }

            base.OnExit();
            if (base.modelAnimator) base.PlayAnimation("FullBody, Override", "UtilityLanding", "Utility.playbackRate", 0.5f);
        }

        public override void UpdateAnimationParameters() { }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
        protected virtual float CalculateChargeMultiplier()
        {
            float mult = Mathf.Lerp(1f, 1.5f, this.charge / BaseChargeState.overchargeFraction);
            return mult;
        }
        private void OnMovementHit(ref CharacterMotor.MovementHitInfo movementHitInfo)
        {
            this.detonateNextFrame = true;
        }

        protected virtual float CalcDamageCoefficient()
        {
            return Mathf.Lerp(4f, 8f, this.charge / BaseChargeState.overchargeFraction);
        }

        protected virtual float GetBlastRadius()
        {
            return 12f;
        }

        protected virtual void DetonateAuthority()
        {
            if (!base.isAuthority) return;

            float blastRadius = GetBlastRadius();
            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                rotation = base.transform.rotation,
                origin = base.transform.position,
                scale = blastRadius
            }, true);

            BlastAttack ba = new BlastAttack
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                baseDamage = base.damageStat * CalcDamageCoefficient(),
                baseForce = blastForce,
                bonusForce = Vector3.zero,
                canRejectForce = true,
                crit = this.isCrit,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Stun1s,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                losType = BlastAttack.LoSType.None,
                position = base.transform.position,
                procChainMask = default,
                procCoefficient = 1f,
                radius = blastRadius,
                teamIndex = base.GetTeam()
            };
            if (base.characterBody && base.characterBody.HasBuff(Starstorm2Unofficial.Cores.BuffCore.nucleatorSpecialBuff))
            {
                ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.NucleatorRadiationOnHit);
            }
            ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.AntiFlyingForce);
            ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ResetVictimForce);
            ba.Fire();

            //Prevents Nucleator from flying off and dying to fall damage
            if (base.characterMotor)
            {
                if (base.characterMotor.velocity.y < 0f)
                    base.characterMotor.velocity.y = 0f;
                else if (base.characterMotor.velocity.y > maxExitYVelocity)
                    base.characterMotor.velocity.y = maxExitYVelocity;

                base.characterMotor.velocity.x = 0f;
                base.characterMotor.velocity.z = 0f;
            }
        }
    }
}