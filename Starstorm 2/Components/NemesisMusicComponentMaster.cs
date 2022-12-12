using RoR2;
using System;
using UnityEngine;

namespace Starstorm2.Components
{
    public class NemesisMusicComponentMaster : MonoBehaviour
    {
        public string musicString;
        private CharacterMaster master;
        private bool addedMusic;

        private void Awake()
        {
            this.master = this.GetComponent<CharacterMaster>();
            this.addedMusic = false;
        }

        private void FixedUpdate()
        {
            if (!this.addedMusic)
            {
                if (this.master && this.master.GetBodyObject())
                {
                    this.addedMusic = true;
                    this.master.GetBodyObject().AddComponent<NemesisMusicComponent>().musicString = this.musicString;
                    Destroy(this);
                }
            }
        }
    }
}