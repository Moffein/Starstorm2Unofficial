using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components
{
    public class NucleatorNetworkComponent : NetworkBehaviour
    {
        private HealthComponent healthComponent;
        private void Awake()
        {
            healthComponent = base.GetComponent<HealthComponent>();
        }

        public void HealFractionAuthority(float healFraction)
        {
            if (this.hasAuthority) CmdHealFraction(healFraction);
        }

        [Command]
        private void CmdHealFraction(float healFraction)
        {
            if (!NetworkServer.active || !healthComponent) return;

            healthComponent.HealFraction(healFraction, default);
        }
    }
}
