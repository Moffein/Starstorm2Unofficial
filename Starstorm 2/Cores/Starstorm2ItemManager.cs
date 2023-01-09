using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores
{
    public class Starstorm2ItemManager : NetworkBehaviour
    {
        public HealthComponent health;
        [SyncVar]
        public float metroCharge;

        public void Awake()
        {
            health = gameObject.GetComponent<HealthComponent>();
        }

        #region Dungus
        public void HealFractionAuthority(float frac)
        {
            if (NetworkServer.active)
            {
                HealFractionInternal(frac);
                return;
            }
            CmdHealFractionInternal(frac);
        }

        [Server]
        private void HealFractionInternal(float frac)
        {
            if (!health)
            {
                health = gameObject.GetComponent<HealthComponent>();
                LogCore.LogError(gameObject.name + "'s health component was null!");
            }
            health.HealFraction(frac, default);
        }

        [Command]
        private void CmdHealFractionInternal(float frac)
        {
            HealFractionInternal(frac);
        }
        #endregion
        #region Diary
        public void AddExperienceAuthority(uint experience)
        {
            if (NetworkServer.active)
            {
                AddExperienceInternal(experience);
                return;
            }
            CmdAddExperienceInternal(experience);
        }

        [Server]
        private void AddExperienceInternal(uint experience)
        {
            TeamManager.instance.GiveTeamExperience(TeamIndex.Player, experience);
        }

        [Command]
        private void CmdAddExperienceInternal(uint experience)
        {
            AddExperienceInternal(experience);
        }
        #endregion
        #region watch metronome
        public void SetMetronomeChargeAuthority(float value)
        {
            if (NetworkServer.active)
            {
                SetMetronomeChargeInternal(value);
                return;
            }
            CmdSetMetronomeChargeInternal(value);
        }
        [Server]
        private void SetMetronomeChargeInternal(float value)
        {
            metroCharge = value;
        }
        [Command]
        private void CmdSetMetronomeChargeInternal(float value)
        {
            SetMetronomeChargeInternal(value);
        }
        #endregion
    }
}
