using UnityEngine;
using UnityEngine.Networking;
using RoR2;

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

        public static float maxChargeVelocityMultiplier = 1.5f;

        public static string leapSoundString = "SS2UNucleatorSkill3";

        public float charge;
        private float previousAirControl;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("FullBody, Override", "UtilityRelease");
            isCrit = base.RollCrit();
            previousAirControl = base.characterMotor.airControl;

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
                Vector3 a = direction.normalized * aimVelocity * this.moveSpeedStat * Mathf.Lerp(1f, FireLeap.maxChargeVelocityMultiplier, this.charge / BaseChargeState.overchargeFraction);
                Vector3 b = Vector3.up * upwardVelocity;
                Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * forwardVelocity;
                base.characterMotor.Motor.ForceUnground();
                base.characterMotor.velocity = a + b + b2;
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority)
            {
                if (base.characterMotor)
                {
                    base.characterMotor.moveDirection = base.inputBank.moveVector;
                    if (base.fixedAge >= minimumDuration && (base.characterMotor.Motor.GroundingStatus.IsStableOnGround && !base.characterMotor.Motor.LastGroundingStatus.IsStableOnGround))
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
                if (NetworkServer.active)
                {
                    base.characterBody.RemoveBuff(RoR2Content.Buffs.ArmorBoost);
                }
            }

            base.characterMotor.airControl = this.previousAirControl;

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