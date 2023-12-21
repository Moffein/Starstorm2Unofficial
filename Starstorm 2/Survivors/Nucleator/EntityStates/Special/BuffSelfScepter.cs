using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nucleator.Special
{
    public class BuffSelfScepter : BuffSelf
    {
        public static new float baseBuffDuration = 12f;


        public override void SetBuffDuration()
        {
            buffDurationRemaining = BuffSelfScepter.baseBuffDuration;
            Util.PlaySound("SS2UNucleatorSkill4c", base.gameObject);
        }
    }
}
