using RoR2;
using UnityEngine;

namespace Starstorm2.Components
{
    class StormSoundComponent : MonoBehaviour
    {
        private uint playID;

        private void Awake()
        {
            // someone else can clean this up i cba LOL
            string soundString = "SS_RainAmbience";
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            if (sceneName == "frozenwall" || sceneName == "goolake" || sceneName == "dampcavesimple") soundString = "SS_WindAmbience";
            this.playID = Util.PlaySound(soundString, this.gameObject);
        }

        private void OnDisable()
        {
            AkSoundEngine.StopPlayingID(this.playID);
        }

        private void OnDestroy()
        {
            AkSoundEngine.StopPlayingID(this.playID);
        }
    }
}