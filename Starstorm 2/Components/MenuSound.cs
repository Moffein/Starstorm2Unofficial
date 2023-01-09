using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Components
{
    public class MenuSound : MonoBehaviour
    {
        public string soundString;
        private uint playID;

        private void OnEnable()
        {
            if (this.soundString != "") this.PlaySound();
        }

        private void PlaySound()
        {
            this.playID = Util.PlaySound(this.soundString, base.gameObject);
        }

        private void OnDestroy()
        {
            if (this.playID != 0) AkSoundEngine.StopPlayingID(this.playID);
        }
    }
}