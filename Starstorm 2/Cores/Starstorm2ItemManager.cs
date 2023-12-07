using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores
{
    public class Starstorm2ItemManager : NetworkBehaviour
    {
        public HealthComponent health;
        public CharacterBody body;

        public void Awake()
        {
            health = gameObject.GetComponent<HealthComponent>();
            body = gameObject.GetComponent<CharacterBody>();
        }

        #region Dungus
        public void HealFractionAuthority(float frac)
        {
            if (this.hasAuthority)
            {
                CmdHealFractionInternal(frac);
            }
        }

        [Server]
        private void HealFractionInternal(float frac)
        {
            if (!NetworkServer.active || !health) return;
            health.HealFraction(frac, default);
        }

        [Command]
        private void CmdHealFractionInternal(float frac)
        {
            HealFractionInternal(frac);
        }
        #endregion

        #region watch metronome
        public void SetMetronomeBuffsAuthority(int count)
        {
            if (this.hasAuthority)
            {
                CmdSetMetronomeBuffs(count);
            }
        }

        [Command]
        private void CmdSetMetronomeBuffs(int count)
        {
            if (!body || !NetworkServer.active) return;
            int buffCount = body.GetBuffCount(BuffCore.watchMetronomeBuff.buffIndex);

            if (buffCount > count)
            {
                do
                {
                   body.RemoveBuff(BuffCore.watchMetronomeBuff);
                   buffCount--;
                } while (buffCount > count);
            }
            else if (buffCount < count)
            {
                do
                {
                    body.AddBuff(BuffCore.watchMetronomeBuff);
                    buffCount++;
                } while (buffCount < count);
            }
        }

        public void ClearMetronomeBuffsAuthority()
        {
            if (this.hasAuthority)
            {
                CmdClearMetronomeBuffs();
            }
        }

        [Command]
        private void CmdClearMetronomeBuffs()
        {
            if (!body) return;
            ClearMetronomeBuffsServer();
        }

        [Server]
        private void ClearMetronomeBuffsServer()
        {
            if (!NetworkServer.active) return;
            int buffCount = body.GetBuffCount(BuffCore.watchMetronomeBuff.buffIndex);

            for (int i = 0; i < buffCount; i++)
            {
                body.RemoveBuff(BuffCore.watchMetronomeBuff);
            }
        }
        #endregion


        private void OnDestroy()
        {
            if (NetworkServer.active)
            {
                ClearMetronomeBuffsServer();
            }
        }
    }
}
