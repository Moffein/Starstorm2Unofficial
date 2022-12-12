using RoR2;
using System;
using UnityEngine;

namespace Starstorm2.Components
{
    public class NemesisMusicComponent : MonoBehaviour
    {
        public string musicString;

        private HealthComponent healthComponent;
        private uint playID;
        private bool isPlaying;

        private void Start()
        {
            this.healthComponent = this.GetComponent<HealthComponent>();

            this.playID = Util.PlaySound(this.musicString, this.gameObject);
            Modules.Music.musicSources++;
            this.isPlaying = true;
            this.InvokeRepeating("CheckAlive", 0.25f, 0.25f);

            CharacterBody characterBody = this.GetComponent<CharacterBody>();
            if (characterBody)
            {
                characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            }
        }

        private void CheckAlive()
        {
            if (this.healthComponent && !this.healthComponent.alive)
            {
                this.StopMusic();
            }
        }

        private void OnDestroy()
        {
            this.StopMusic();
        }

        private void OnDisable()
        {
            this.StopMusic();
        }

        private void StopMusic()
        {
            if (this.isPlaying)
            {
                this.isPlaying = false;
                AkSoundEngine.StopPlayingID(this.playID);
                Modules.Music.musicSources--;
            }
        }
    }
}