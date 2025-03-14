﻿using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Pyro.Components
{
    public class HeatController : NetworkBehaviour
    {
        public static float baseHeatDecayDuration = 20f;
        public static float heatDecayDelay = 1f; //how long before heat starts draining

        public bool pauseDecay;
        private float heatPercent;
        private float heatDecayStopwatch;
        private CharacterBody characterBody;

        private void Awake()
        {
            characterBody = base.GetComponent<CharacterBody>();
            heatPercent = 0f;
            heatDecayStopwatch = 0f;
            pauseDecay = false;
        }
        private void FixedUpdate()
        {
            //Chat.AddMessage("Heat: " + heatPercent);
            if (heatPercent > 0f)
            {
                if (heatDecayStopwatch < heatDecayDelay)
                {
                    this.heatDecayStopwatch += Time.fixedDeltaTime;
                }
                else if (!this.pauseDecay)
                {
                    heatPercent -= Time.fixedDeltaTime / HeatController.baseHeatDecayDuration;
                    if (heatPercent < 0f)
                    {
                        heatPercent = 0f;
                    }
                }
            }
        }

        public bool IsHighHeat()
        {
            return heatPercent > 0.75f ? true : false;
        }

        public float GetHeatPercent()
        {
            return heatPercent;
        }

        [ClientRpc]
        public void RpcAddHeatServer(float heat)
        {
            AddHeatAuthority(heat);
        }

        public void AddHeatAuthority(float heat)
        {
            heatPercent += heat;
            if (heatPercent > 1f)
            {
                this.heatPercent = 1f;
            }
            this.heatDecayStopwatch = 0f;
        }

        public void ConsumeHeat(float heat, int stocks)
        {
            float heatMult = 1f / (Mathf.Max(1f, 0.5f + 0.5f * stocks));
            heatPercent -= heat * heatMult;
            if (heatPercent <= 0f)
            {
                this.heatPercent = 0f;
            }
            this.heatDecayStopwatch = 0f;
        }

        public void ConsumeHeat(float heat)
        {
            ConsumeHeat(heat, 1);
        }
    }
}
