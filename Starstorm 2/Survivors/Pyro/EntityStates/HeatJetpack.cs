using RoR2;
using Starstorm2.Survivors.Pyro.Components;
using UnityEngine;


namespace EntityStates.SS2UStates.Pyro
{
    public class HeatJetpack : BaseState
    {
        public static float minDuration = 0.5f;
        public static float heatConsumptionPerSecond = 0.5f;
        public static float maxSpeedCoefficient = 3f;
        public static float minSpeedCoefficient = 1.5f;

        public static GameObject trailPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/missileexplosionvfx");
        public static float trailFrequency = 5f;
        private float trailStopwatch;
        private float trailTime;
        private HeatController heatController;

        public override void OnEnter()
        {
            base.OnEnter();
            trailStopwatch = 0f;
            trailTime = 1f / HeatJetpack.trailFrequency;
            heatController = base.GetComponent<HeatController>();
            Util.PlaySound("Play_mage_R_start", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            trailStopwatch += Time.fixedDeltaTime;
            if (this.trailStopwatch >= trailTime)
            {
                EffectManager.SpawnEffect(trailPrefab, new EffectData
                {
                    origin = base.transform.position
                }, false);
                this.trailStopwatch -= trailTime;
            }

            bool outOfHeat = false;
            float desiredSpeed = this.moveSpeedStat;
            if (heatController)
            {
                desiredSpeed *= Mathf.Lerp(HeatJetpack.minSpeedCoefficient, HeatJetpack.maxSpeedCoefficient, heatController.GetHeatPercent());

                int stocks = 1;
                if (base.skillLocator && base.skillLocator.utility) stocks = base.skillLocator.utility.maxStock;
                heatController.ConsumeHeat(HeatJetpack.heatConsumptionPerSecond * Time.fixedDeltaTime, stocks);
                outOfHeat = heatController.GetHeatPercent() <= 0f;
            }

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                if (base.characterMotor)
                {
                    if (base.characterMotor.isGrounded && base.characterMotor.Motor) base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.rootMotion += Time.fixedDeltaTime * desiredSpeed * aimRay.direction;
                }

                bool isKeyPressed = base.inputBank && base.inputBank.skill3.down;
                if (outOfHeat || (base.fixedAge >= HeatJetpack.minDuration && !isKeyPressed))
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            Util.PlaySound("Play_mage_R_end", base.gameObject);
            if (base.isAuthority && base.characterMotor && !base.characterMotor.isGrounded)
            {
                float desiredSpeed = this.moveSpeedStat;
                if (heatController)
                {
                    desiredSpeed *= Mathf.Lerp(HeatJetpack.minSpeedCoefficient, HeatJetpack.maxSpeedCoefficient, heatController.GetHeatPercent());
                }
                base.characterMotor.velocity = base.GetAimRay().direction * desiredSpeed;
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
