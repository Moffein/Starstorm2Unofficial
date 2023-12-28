using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace EntityStates.SS2UStates.Nucleator.Utility
{
    public class FireLeap : BaseState
    {
        public static float minimumDuration;
        public static float upwardVelocity;
        public static float forwardVelocity;
        public static string leapSoundString = "SS2UNucleatorSkill3";

        public float charge;
        private bool detonateNextframe;
        private float previousAirControl;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();

            isCrit = base.RollCrit();
            detonateNextframe = false;
            previousAirControl = base.characterMotor.airControl;
        }
    }
}
