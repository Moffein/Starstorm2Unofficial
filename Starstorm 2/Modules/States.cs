using System.Collections.Generic;
using System;
using EntityStates.Executioner;
using Starstorm2.Cores.States.Emotes;
using Starstorm2.Cores.States.Executioner;

namespace Starstorm2.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        internal static void Initialize()
        {
            AddSkill(typeof(ExecutionerMain));
            AddSkill(typeof(ExecutionerPistol));
            AddSkill(typeof(ExecutionerBurstPistol));
            AddSkill(typeof(ExecutionerTaser));
            AddSkill(typeof(ExecutionerIonGun));
            AddSkill(typeof(ExecutionerDash));
            AddSkill(typeof(ExecutionerAxe));
            AddSkill(typeof(ExecutionerAxeSlam));
            AddSkill(typeof(BaseEmote));
            AddSkill(typeof(RestEmote));
            AddSkill(typeof(TauntEmote));

            AddSkill(typeof(Cores.States.Generic.NemesisSpawnState));
            AddSkill(typeof(Cores.States.Generic.NemmandoSpawnState));
            AddSkill(typeof(Cores.States.Nemmando.NemmandoMain));
            AddSkill(typeof(Cores.States.Nemmando.BladeOfCessation2));
            AddSkill(typeof(Cores.States.Nemmando.ChargeSwordBeam));
            AddSkill(typeof(Cores.States.Nemmando.DodgeState));
            AddSkill(typeof(Cores.States.Nemmando.Submission));
            AddSkill(typeof(Cores.States.Nemmando.ChargedSlashCharge));
            AddSkill(typeof(Cores.States.Nemmando.ScepterSlashCharge));
            AddSkill(typeof(Cores.States.Nemmando.ScepterSlashEntry));
            AddSkill(typeof(Cores.States.Nemmando.ScepterSlashAttack));
            AddSkill(typeof(Cores.States.Nemmando.ScepterBarrage.ScepterBarrageCharge));
            AddSkill(typeof(Cores.States.Nemmando.ScepterBarrage.ScepterBarrageFire));

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