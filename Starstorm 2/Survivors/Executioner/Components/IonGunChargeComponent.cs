using RoR2;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Executioner.Components
{
    public class IonGunChargeComponent : NetworkBehaviour
    {
        public SkillLocator skillLocator;

        [ClientRpc]
        public void RpcAddIonCharge()
        {
            if (this.hasAuthority)
            {
                if (!skillLocator) skillLocator = this.gameObject.GetComponent<SkillLocator>();
                GenericSkill ionGunSkill = skillLocator?.secondary;
                if (ionGunSkill && ionGunSkill.stock < ionGunSkill.maxStock)
                    ionGunSkill.AddOneStock();
            }
        }
    }
}
