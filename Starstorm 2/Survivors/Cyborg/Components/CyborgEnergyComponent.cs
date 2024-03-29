using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgEnergyComponent : NetworkBehaviour
    {
        public static float delayBeforeShieldRecharge = 0.5f;

        private float energyRechargeDelayStopwatch;
        public int energySkillsActive = 0;
        public bool energyDepleted = false;
        public float remainingEnergyFraction = 1f;

        //Specific to Charge Rifle
        public float rifleChargeFraction = 0f;
        public bool riflePerfectCharge = false;
        public SkillLocator skillLocator;

        //Specific to TriShot
        public bool showTriShotCrosshair = false;
        public int armToFireFrom = 0; //Jank to make FireTriShot alternate arms without a SteppedSkillDef

        private void Awake()
        {
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void FixedUpdate()
        {
            if (energySkillsActive <= 0)
            {
                if (energyRechargeDelayStopwatch <= 0f || energyDepleted)
                {
                    remainingEnergyFraction += Time.fixedDeltaTime / GetShieldRechargeTime();

                    if (remainingEnergyFraction >= 1f)
                    {
                        energyDepleted = false;
                    }

                    float maxShield = GetMaxShieldDuration();
                    if (remainingEnergyFraction > maxShield)
                    {
                        remainingEnergyFraction = maxShield;
                    }
                }
                else
                {
                    energyRechargeDelayStopwatch -= Time.fixedDeltaTime;
                }
            }
        }

        public void RefreshShield()
        {
            remainingEnergyFraction = GetMaxShieldDuration();
            energyDepleted = false;
        }

        public void ConsumeShield(float fraction)
        {
            remainingEnergyFraction -= fraction;
            if (remainingEnergyFraction <= 0f)
            {
                remainingEnergyFraction = 0f;
                energyDepleted = true;
            }
            energyRechargeDelayStopwatch = CyborgEnergyComponent.delayBeforeShieldRecharge;
        }

        public float GetMaxShieldDuration()
        {
            float fraction = 1f;
            if (skillLocator && skillLocator.secondary)
            {
                int extraStocks = Mathf.Max(0, skillLocator.secondary.maxStock - 1);
                fraction += extraStocks;
            }
            return fraction;
        }

        private float GetShieldRechargeTime()
        {
            if (skillLocator && skillLocator.secondary)
            {
                return skillLocator.secondary.CalculateFinalRechargeInterval();
            }
            return 6f;
        }

        public void ResetCharge()
        {
            rifleChargeFraction = 0f;
            riflePerfectCharge = false;
        }

        [Server]
        public void RestoreNonDefenseMatrixCooldownsServer(float reductionAmount)
        {
            if (!NetworkServer.active) return;
            if (skillLocator)
            {
                RpcRestoreNonDefenseMatrixCooldowns(reductionAmount);
            }
        }

        [ClientRpc]
        private void RpcRestoreNonDefenseMatrixCooldowns(float reductionAmount)
        {
            if (this.hasAuthority && skillLocator)
            {
                foreach (GenericSkill gs in skillLocator.allSkills)
                {
                    if (gs.skillDef != CyborgCore.defenseMatrixDef)
                    {
                        gs.RunRecharge(reductionAmount);
                    }
                }
            }
        }
    }
}
