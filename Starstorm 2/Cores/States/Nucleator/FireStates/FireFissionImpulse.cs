using EntityStates;
using EntityStates.Huntress;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nucleator
{
    class FireFissionImpulse : BaseSkillState
    {
        public static float minChargeForceCoef = 1F;
        public static float maxChargeForceCoef = 5F;
        public static float maxOverchargeDistanceCoef = 9F;
        private static float damageCoef = 5.5f;

        public float charge;

        private GameObject resource = Resources.Load<GameObject>("prefabs/effects/impacteffects/Hitspark");
        private float duration;
        private float currentDuration;
        private float speedCoef;
        private Vector3 fissionVector;
        private Vector3 previousPosition;
        private OverlapAttack overlapAttack;

        private readonly float minChargeSpeedCoef = 2f;
        private readonly float maxChargeSpeedCoef = 4f;
        private readonly float maxOverchargeSpeedCoef = 8f;


        public HurtBoxGroup hurtboxGroup;
        public CharacterModel characterModel;


        public FireFissionImpulse(HurtBoxGroup hurtboxGroupIn, CharacterModel characterModelIn, float chargeIn) : base()
        {
            this.fissionVector = Vector3.zero;
            this.duration = 0.15f;
            this.charge = chargeIn;
            this.hurtboxGroup = hurtboxGroupIn;
            this.characterModel = characterModelIn;           
        }

        public override void OnEnter()
        {
            base.OnEnter();
            CalculateSpeed();

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.characterMotor.Motor.ForceUnground();
            base.characterMotor.velocity = Vector3.zero;

            if (this.characterModel)
            {
                //this.characterModel.invisibilityCount++;
            }

            var modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                this.overlapAttack = base.InitMeleeOverlap(FireFissionImpulse.damageCoef, null, modelTransform, "Assaulter");
                this.overlapAttack.damageType = DamageType.Generic;
                this.overlapAttack.teamIndex = base.GetTeam();
                //this.overlapAttack.pushAwayForce 
            }

            base.PlayAnimation("FullBody, Override", "UtilityRelease", "Utility.playbackRate", this.duration);

            Util.PlaySound(EntityStates.Croco.Leap.landingSound.eventName, base.gameObject);
            EffectManager.SpawnEffect(Resources.Load<GameObject>("CrocoLeapExplosion"), new EffectData
            {
                origin = base.characterBody.footPosition,
                scale = 10f
            }, true);
        }

        public override void OnExit()
        {
            if (this.characterModel)
            {
                //this.characterModel.invisibilityCount--;
            }
            if (this.hurtboxGroup)
            {
                HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
                int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
                hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
            }
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.currentDuration += Time.fixedDeltaTime;
            if (base.characterMotor && base.characterDirection)
            {
                base.characterMotor.velocity = Vector3.zero;
                base.characterMotor.rootMotion += base.inputBank.aimDirection * (this.moveSpeedStat * this.speedCoef * Time.fixedDeltaTime);
                base.characterBody.isSprinting = true;
            }
            bool flag = this.overlapAttack.Fire(null);
            if (this.currentDuration >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void CalculateSpeed()
        {
            float chargeCoef;
            float speedCoef;
            float overchargeThreshold = NucleatorSkillStateBase.overchargeThreshold;

            if (this.charge < overchargeThreshold)
            {
                chargeCoef = this.charge / overchargeThreshold;
                speedCoef = chargeCoef * (maxChargeSpeedCoef - minChargeSpeedCoef) + minChargeSpeedCoef;
                this.speedCoef = this.moveSpeedStat * speedCoef;
            }
            else
            {
                chargeCoef = (this.charge - overchargeThreshold) / (1 - overchargeThreshold);
                speedCoef = chargeCoef * (maxOverchargeSpeedCoef - maxChargeSpeedCoef) + maxChargeSpeedCoef;
                this.speedCoef = this.moveSpeedStat * speedCoef;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}