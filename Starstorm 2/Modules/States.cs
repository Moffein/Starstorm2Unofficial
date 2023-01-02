using System.Collections.Generic;
using System;
using EntityStates.SS2UStates.Common.Emotes;
using EntityStates.SS2UStates.Common;

namespace Starstorm2.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void Initialize()
        {
            AddState(typeof(BaseEmote));
            AddState(typeof(RestEmote));
            AddState(typeof(TauntEmote));

            AddState(typeof(NemesisSpawnState));

            AddState(typeof(BaseCustomMainState));
            AddState(typeof(BaseCustomSkillState));
            AddState(typeof(BaseMeleeAttack));

            AddState(typeof(Cores.States.Nucleator.ApplyRadionuclideSurge));
            AddState(typeof(Cores.States.Nucleator.ChargeFissionImpulse));
            AddState(typeof(Cores.States.Nucleator.ChargeIrradiate));
            AddState(typeof(Cores.States.Nucleator.ChargeQuarantine));
            AddState(typeof(Cores.States.Nucleator.FireFissionImpulse));
            AddState(typeof(Cores.States.Nucleator.FireIrradiate));
            AddState(typeof(Cores.States.Nucleator.FireQuarantine));
            AddState(typeof(Cores.States.Nucleator.NucleatorSkillStateBase));
        }

        internal static void AddState(Type t)
        {
            entityStates.Add(t);
        }
    }
}