using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nucleator.Special
{

    public class BuffSelf : BaseState
    {
        public static float buffDuration = 6f;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound("SS2UNucleatorSkill4a", base.gameObject);
            if (NetworkServer.active)
            {
                SetBuffs(GetBuffDuration());
            }

            if (base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public virtual float GetBuffDuration()
        {
            return buffDuration;
        }

        public void SetBuffs(float duration)
        {
            if (!base.characterBody) return;

            base.characterBody.ClearTimedBuffs(BuffCore.nucleatorSpecialBuff);

            float buffTimeAccumulated = 0f;
            while (duration > 0f)
            {
                float toSubtract = Mathf.Min(1f, duration);
                duration -= toSubtract;
                buffTimeAccumulated += toSubtract;

                base.characterBody.AddTimedBuff(BuffCore.nucleatorSpecialBuff, buffTimeAccumulated);
            }
        }
    }
}
