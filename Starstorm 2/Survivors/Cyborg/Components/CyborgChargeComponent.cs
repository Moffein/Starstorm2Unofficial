using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgChargeComponent : NetworkBehaviour
    {
        public static float baseMaxShieldDuration = 3f;
        public static float delayBeforeShieldRecharge = 1f;

        private float shieldRechargeDelayStopwatch;
        public bool shieldActive = false;
        public bool shieldDepleted = false;
        public float remainingShieldDuration = 2f;

        public float chargeFraction = 0f;
        public bool perfectCharge = false;
        public SkillLocator skillLocator;

        private void Awake()
        {
            skillLocator = base.GetComponent<SkillLocator>();
        }

        private void FixedUpdate()
        {
            if (!shieldActive)
            {
                if (shieldRechargeDelayStopwatch <= 0f)
                {
                    remainingShieldDuration += Time.fixedDeltaTime * CyborgChargeComponent.baseMaxShieldDuration / GetShieldRechargeTime();

                    if (remainingShieldDuration >= baseMaxShieldDuration)
                    {
                        shieldDepleted = false;
                    }

                    float maxShield = GetMaxShieldDuration();
                    if (remainingShieldDuration > maxShield)
                    {
                        remainingShieldDuration = maxShield;
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
            remainingShieldDuration = GetMaxShieldDuration();
            shieldDepleted = false;
        }

        public void ConsumeShield(float duration)
        {
            remainingShieldDuration -= duration;
            if (remainingShieldDuration <= 0f)
            {
                remainingShieldDuration = 0f;
                shieldDepleted = true;
            }
            shieldRechargeDelayStopwatch = CyborgChargeComponent.delayBeforeShieldRecharge;
        }

        public float GetMaxShieldDuration()
        {
            float duration = CyborgChargeComponent.baseMaxShieldDuration;
            if (skillLocator && skillLocator.secondary && skillLocator.secondary.skillDef == CyborgCore.defenseMatrixDef)
            {
                int extraStocks = Mathf.Max(0, skillLocator.secondary.maxStock - 1);
                duration += CyborgChargeComponent.baseMaxShieldDuration * extraStocks;
            }
            return duration;
        }

        private float GetShieldRechargeTime()
        {
            if (skillLocator && skillLocator.secondary && skillLocator.secondary.skillDef == CyborgCore.defenseMatrixDef)
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
