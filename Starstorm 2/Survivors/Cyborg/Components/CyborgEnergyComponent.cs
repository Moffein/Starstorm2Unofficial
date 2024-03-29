using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgEnergyComponent : NetworkBehaviour
    {
        public static float delayBeforeShieldRecharge = 0.5f;
        public static float energyDepletedFraction = 0.2f; //If energy is fully depleted, lock skills until this threshold is reached.

        public float energyRechargeDelayStopwatch;
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
                    remainingEnergyFraction += Time.fixedDeltaTime / GetRechargeTime();
                    if (remainingEnergyFraction >= energyDepletedFraction) energyDepleted = false;

                    remainingEnergyFraction = Mathf.Min(remainingEnergyFraction, GetMaxEnergyFraction());
                }
                else
                {
                    energyRechargeDelayStopwatch -= Time.fixedDeltaTime;
                }
            }
        }

        public void ApplyAmmoPack()
        {
            AddEnergyFraction(1f);
            energyDepleted = false;
        }

        public void ResetEnergy()
        {
            remainingEnergyFraction = GetMaxEnergyFraction();
            energyDepleted = false;
            energyRechargeDelayStopwatch = 0f;
        }

        public void AddEnergyFraction(float fraction)
        {
            remainingEnergyFraction = Mathf.Min(remainingEnergyFraction + fraction, GetMaxEnergyFraction());
        }

        public void ConsumeEnergy(float fraction)
        {
            remainingEnergyFraction -= fraction;
            if (remainingEnergyFraction <= 0f)
            {
                remainingEnergyFraction = 0f;
                energyDepleted = true;
                energyRechargeDelayStopwatch = 0f;
            }
            energyRechargeDelayStopwatch = CyborgEnergyComponent.delayBeforeShieldRecharge;
        }

        public float GetMaxEnergyFraction()
        {
            float fraction = 1f;

            if (!CyborgCore.useEnergyRework.Value)
            {
                if (skillLocator && skillLocator.secondary) fraction += skillLocator.secondary.bonusStockFromBody;
                return fraction;
            }

            if (skillLocator)
            {
                int totalStocks = 0;
                if (skillLocator.secondary) totalStocks += skillLocator.secondary.maxStock;
                if (skillLocator.utility) totalStocks += skillLocator.utility.maxStock;
                if (skillLocator.special) totalStocks += skillLocator.special.maxStock;

                fraction = Mathf.Max(1f, totalStocks / 9f); //was /3f but +33% charge from backup mags was too much
            }
            return fraction;
        }

        private float GetRechargeTime()
        {
            if (!CyborgCore.useEnergyRework.Value)
            {
                if (skillLocator && skillLocator.secondary) return skillLocator.secondary.CalculateFinalRechargeInterval();
                return 6f;
            }

            float secondaryTime = 5f;
            float utilityTime = 5f;
            float specialTime = 5f;
            if (skillLocator)
            {
                if (skillLocator.secondary) secondaryTime = skillLocator.secondary.CalculateFinalRechargeInterval();
                if (skillLocator.utility) utilityTime = skillLocator.utility.CalculateFinalRechargeInterval();
                if (skillLocator.special) specialTime = skillLocator.special.CalculateFinalRechargeInterval();
            }
            return secondaryTime + utilityTime + specialTime;
        }

        public void ResetCharge()
        {
            rifleChargeFraction = 0f;
            riflePerfectCharge = false;
        }

        [Server]
        public void RestoreEnergyServer(float reductionFraction)
        {
            if (!NetworkServer.active) return;
            if (skillLocator)
            {
                RpcRestoreEnergy(reductionFraction);
            }
        }

        [ClientRpc]
        private void RpcRestoreEnergy(float reductionFraction)
        {
            AddEnergyFraction(reductionFraction);
        }
    }
}
