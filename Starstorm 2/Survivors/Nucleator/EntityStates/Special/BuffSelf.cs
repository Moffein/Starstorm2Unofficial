using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nucleator.Special
{

    public class BuffSelf : BaseState
    {
        public static float baseBuffDuration = 6f;
        protected float buffDurationRemaining;

        public override void OnEnter()
        {
            base.OnEnter();

            SetBuffDuration();
            if (NetworkServer.active) SetBuffsServer();
        }

        public virtual void SetBuffDuration()
        {
            buffDurationRemaining = BuffSelf.baseBuffDuration;
            Util.PlaySound("SS2UNucleatorSkill4a", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            buffDurationRemaining -= Time.fixedDeltaTime;

            if (NetworkServer.active) SetBuffsServer();

            if (base.isAuthority && buffDurationRemaining <= 0f) this.outer.SetNextStateToMain();
        }

        public override void OnExit()
        {
            buffDurationRemaining = 0f;
            if (NetworkServer.active) SetBuffsServer();
            base.OnExit();
        }

        public void SetBuffsServer()
        {
            if (!base.characterBody || !NetworkServer.active) return;

            int currentBuffs = base.characterBody.GetBuffCount(BuffCore.nucleatorSpecialBuff);
            int desiredBuffs = Mathf.CeilToInt(buffDurationRemaining);

            if (desiredBuffs < currentBuffs)
            {
                int toSubtract = currentBuffs - desiredBuffs;
                for (int i = 0; i < toSubtract; i++)
                {
                    base.characterBody.RemoveBuff(BuffCore.nucleatorSpecialBuff);
                }
            }
            else if (desiredBuffs > currentBuffs)
            {
                int toAdd = desiredBuffs - currentBuffs;
                for (int i = 0; i < toAdd; i++)
                {
                    base.characterBody.AddBuff(BuffCore.nucleatorSpecialBuff);
                }
            }
        }
    }
}
