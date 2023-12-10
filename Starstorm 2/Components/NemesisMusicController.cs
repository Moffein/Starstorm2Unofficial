using UnityEngine;
using UnityEngine.Networking;
using RoR2;

namespace Starstorm2Unofficial.Components
{
    public class NemesisMusicController : NetworkBehaviour
    {
        private bool isPlaying = false;
        private uint playID;

        public void StartMusicServer(string soundName)
        {
            if (!NetworkServer.active) return;
            RpcStartMusic(soundName);
        }

        public void StopMusicServer()
        {
            if (!NetworkServer.active) return;
            RpcStopMusic();
        }

        [ClientRpc]
        private void RpcStartMusic(string soundName)
        {
            StopMusic();
            isPlaying = true;
            Modules.Music.musicSources++;
            playID = Util.PlaySound(soundName, base.gameObject);
        }

        [ClientRpc]
        private void RpcStopMusic()
        {
            StopMusic();
        }

        private void StopMusic()
        {
            if (!isPlaying) return;
            isPlaying = false;
            Modules.Music.musicSources--;
            AkSoundEngine.StopPlayingID(this.playID);
        }

        private void OnDestroy()
        {
            StopMusic();
        }
    }
}
