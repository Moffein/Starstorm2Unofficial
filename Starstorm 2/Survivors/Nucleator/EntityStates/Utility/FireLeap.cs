using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using UnityEngine.AddressableAssets;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class FireLeap : BaseCharacterMain
    {
        public static float minimumDuration = 0.3f;
        public static float upwardVelocity = 7f;
        public static float forwardVelocity = 3f;
        public static float aimVelocity = 4f;   //4f
        public static float airControl = 0.15f;
        public static float minimumY = 0.05f;

        public static GameObject blastEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderGroundSlam.prefab").WaitForCompletion();
        public static float blastRadius = 12f;
        public static float minDamageCoefficient = 6f;
        public static float maxDamageCoefficient = 6f;
        public static float blastForce = 3000f;

        public static string soundLoopStartEvent = "Play_acrid_shift_fly_loop";
        public static string soundLoopStopEvent = "Stop_acrid_shift_fly_loop";

        public static string leapSoundString = "SS2UNucleatorSkill3";

        public float charge;
        private float previousAirControl;
        private bool isCrit;
        private bool detonateNextFrame;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(soundLoopStartEvent, base.gameObject);
            base.PlayAnimation("FullBody, Override", "UtilityRelease");

            previousAirControl = base.characterMotor.airControl;
            detonateNextFrame = false;
            isCrit = base.RollCrit();

            if (base.characterBody)
            {
                base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                if (base.isAuthority)
                {
                    base.characterBody.isSprinting = true;
                    base.characterBody.RecalculateStats();  //Get sprint bonus
                    this.moveSpeedStat = base.characterBody.moveSpeed;

                    float moveSpeedCoeff = base.characterBody.moveSpeed / (base.characterBody.baseMoveSpeed * (!base.characterBody.isSprinting ? 1f : base.characterBody.sprintingSpeedMultiplier));
                    moveSpeedCoeff = Mathf.Min(moveSpeedCoeff, 3f);
                    base.characterMotor.airControl *= moveSpeedCoeff;
                }

                if (NetworkServer.active)
                {
                    base.characterBody.AddBuff(RoR2Content.Buffs.ArmorBoost);
                }
            }

            if (base.isAuthority)
            {
                Vector3 direction = base.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, minimumY);
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
                    if (Starstorm2Unofficial.SneedUtils.IsEnemyInSphere(blastRadius, base.transform.position, base.GetTeam(), false)) detonateNextFrame = true;

                    base.characterMotor.moveDirection = base.inputBank.moveVector;
                    if (base.fixedAge >= minimumDuration && (base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround))
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

            if (base.characterBody)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
                }
            }

            Animator modelAnimator = base.GetModelAnimator();
            if (modelAnimator)
            {
                int layerIndex = modelAnimator.GetLayerIndex("Impact");
                if (layerIndex >= 0)
                {
                    modelAnimator.SetLayerWeight(layerIndex, 2f);
                    this.PlayAnimation("Impact", "LightImpact");
                }
            }

            base.OnExit();
        }

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

        protected virtual void DetonateAuthority()
        {
            if (!base.isAuthority) return;

            EffectManager.SpawnEffect(blastEffectPrefab, new EffectData
            {
                rotation = base.transform.rotation,
                origin = base.transform.position,
                scale = blastRadius
            }, true);

            float damageCoeff = Mathf.Lerp(minDamageCoefficient, maxDamageCoefficient, charge / BaseChargeState.overchargeFraction);

            new BlastAttack
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                baseDamage = base.damageStat * damageCoeff,
                baseForce = blastForce,
                bonusForce = Vector3.zero,
                canRejectForce = true,
                crit = this.isCrit,
                damageColorIndex = DamageColorIndex.Default,
                damageType = base.characterBody && base.characterBody.HasBuff(BuffCore.nucleatorSpecialBuff) ? DamageType.PoisonOnHit : DamageType.Generic,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = base.gameObject,
                losType = BlastAttack.LoSType.None,
                position = base.transform.position,
                procChainMask = default,
                procCoefficient = 1f,
                radius = blastRadius,
                teamIndex = base.GetTeam()
            }.Fire();
        }
    }
}

/*
    [Info   : Unity Log] EntityStates.Croco.BaseLeap
    [Info   : Unity Log] blastDamageCoefficient - 3.2
    [Info   : Unity Log] blastForce - 800
    [Info   : Unity Log] blastBonusForce - 0 800 0
    [Info   : Unity Log] blastImpactEffectPrefab - 
    [Info   : Unity Log] blastEffectPrefab - 
    [Info   : Unity Log] fistEffectPrefab - CrocoFistEffect (UnityEngine.GameObject)
    [Info   : Unity Log] minimumDuration - 0.3
    [Info   : Unity Log] blastRadius - 10
    [Info   : Unity Log] blastProcCoefficient - 1
    [Info   : Unity Log] leapSoundString - Play_acrid_shift_jump
    [Info   : Unity Log] projectilePrefab - RiskyMod_CrocoLeapAcid (UnityEngine.GameObject)
    [Info   : Unity Log] airControl - 0.15
    [Info   : Unity Log] aimVelocity - 4
    [Info   : Unity Log] upwardVelocity - 7
    [Info   : Unity Log] forwardVelocity - 3
    [Info   : Unity Log] minimumY - 0.05
    [Info   : Unity Log] minYVelocityForAnim - 30
    [Info   : Unity Log] maxYVelocityForAnim - -30
    [Info   : Unity Log] knockbackForce - 1600
    [Info   : Unity Log] soundLoopStartEvent - Play_acrid_shift_fly_loop
    [Info   : Unity Log] soundLoopStopEvent - Stop_acrid_shift_fly_loop
    [Info   : Unity Log] landingSound - nseAcridShiftLand (RoR2.NetworkSoundEventDef)
 */