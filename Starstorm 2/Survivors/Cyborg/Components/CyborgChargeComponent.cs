using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgChargeComponent : NetworkBehaviour
    {
        public static float delayBeforeShieldRecharge = 0.5f;

        private float shieldRechargeDelayStopwatch;
        public bool shieldActive = false;
        public bool shieldDepleted = false;
        public float remainingShieldFraction = 1f;

        public bool showTriShotCrosshair = false;

        public float chargeFraction = 0f;
        public bool perfectCharge = false;
        public SkillLocator skillLocator;

        //Jank to make FireTriShot alternate without a SteppedSkillDef
        public int armToFireFrom = 0;

        private void Awake()
        {
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void FixedUpdate()
        {
            if (!shieldActive)
            {
                if (shieldRechargeDelayStopwatch <= 0f || shieldDepleted)
                {
                    remainingShieldFraction += Time.fixedDeltaTime / GetShieldRechargeTime();

                    if (remainingShieldFraction >= 1f)
                    {
                        shieldDepleted = false;
                    }

                    float maxShield = GetMaxShieldDuration();
                    if (remainingShieldFraction > maxShield)
                    {
                        remainingShieldFraction = maxShield;
                    }
                }
                else
                {
                    shieldRechargeDelayStopwatch -= Time.fixedDeltaTime;
                }
            }
        }

        public void RefreshShield()
        {
            remainingShieldFraction = GetMaxShieldDuration();
            shieldDepleted = false;
        }

        public void ConsumeShield(float fraction)
        {
            remainingShieldFraction -= fraction;
            if (remainingShieldFraction <= 0f)
            {
                remainingShieldFraction = 0f;
                shieldDepleted = true;
            }
            shieldRechargeDelayStopwatch = CyborgChargeComponent.delayBeforeShieldRecharge;
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
            chargeFraction = 0f;
            perfectCharge = false;
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
