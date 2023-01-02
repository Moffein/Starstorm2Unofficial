using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgChargeComponent : NetworkBehaviour
    {
        public float chargeFraction = 0f;
        public bool perfectCharge = false;
        private SkillLocator skillLocator;

        public void Awake()
        {
            skillLocator = base.GetComponent<SkillLocator>();
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
