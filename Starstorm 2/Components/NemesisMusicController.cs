using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace Starstorm2Unofficial.Components
{
    public class NemesisMusicController : NetworkBehaviour
    {
        public string soundName = "Play_SS2U_NemesisTheme";
        private bool isPlaying = false;
        private uint playID;

        public void StartMusicServer()
        {
            if (!NetworkServer.active) return;
            RpcStartMusic();
        }

        public void StopMusicServer()
        {
            if (!NetworkServer.active) return;
            RpcStopMusic();
        }

        [ClientRpc]
        private void RpcStartMusic()
        {
            StartMusic();
        }

        [ClientRpc]
        private void RpcStopMusic()
        {
            StopMusic();
        }

        private void StartMusic()
        {
            StopMusic();
            isPlaying = true;
            Modules.Music.musicSources++;
            
            if (ModCompat.SS2OCompat.pluginLoaded && soundName == "Play_SS2U_NemesisTheme")
            {
                soundName = "Play_SS2_Music_System";
            }

            playID = Util.PlaySound(soundName, base.gameObject);
        }

        private void StopMusic()
        {
            if (!isPlaying) return;
            isPlaying = false;
            Modules.Music.musicSources--;
            AkSoundEngine.StopPlayingID(playID);
        }

        private void OnDestroy()
        {
            StopMusic();
        }
    }
}
