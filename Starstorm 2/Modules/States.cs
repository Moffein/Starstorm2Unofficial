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
            AddSkill(typeof(BaseEmote));
            AddSkill(typeof(RestEmote));
            AddSkill(typeof(TauntEmote));

            AddSkill(typeof(NemesisSpawnState));

            AddSkill(typeof(BaseCustomMainState));
            AddSkill(typeof(BaseCustomSkillState));

            AddSkill(typeof(Cores.States.Nucleator.ApplyRadionuclideSurge));
            AddSkill(typeof(Cores.States.Nucleator.ChargeFissionImpulse));
            AddSkill(typeof(Cores.States.Nucleator.ChargeIrradiate));
            AddSkill(typeof(Cores.States.Nucleator.ChargeQuarantine));
            AddSkill(typeof(Cores.States.Nucleator.FireFissionImpulse));
            AddSkill(typeof(Cores.States.Nucleator.FireIrradiate));
            AddSkill(typeof(Cores.States.Nucleator.FireQuarantine));
            AddSkill(typeof(Cores.States.Nucleator.NucleatorSkillStateBase));
        }

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }
    }
}