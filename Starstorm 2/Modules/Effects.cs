using UnityEngine;
using RoR2;
using System.Collections.Generic;
using System;

namespace Starstorm2Unofficial.Modules
{
    internal static class Effects
    {
        private static List<SkinEffectData> skinList;

        internal static SkinEffectData[] skinInfos;

        internal static void Initialize()
        {
            skinList = new List<SkinEffectData>();
            #region Executioner
            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UEXECUTIONER_DEFAULT_SKIN_NAME",
                shootSound = "ExecutionerPrimary",
                ionShootSound = "ExecutionerSecondary",
                bodyName = "SS2UEXECUTIONER_NAME"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UEXECUTIONER_MASTERY_SKIN_NAME",
                shootSound = "ExecutionerPrimary",
                ionShootSound = "ExecutionerSecondary"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UEXECUTIONER_KNIGHT_SKIN_NAME",
                shootSound = "ExecutionerPrimary",
                ionShootSound = "ExecutionerSecondary"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UEXECUTIONER_WASTELANDER_SKIN_NAME",
                shootSound = "WastelanderPrimary",
                ionShootSound = "WastelanderSecondary"
            });
            #endregion
            #region Nemmando
            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UNEMMANDO_DEFAULT_SKIN_NAME",
                impactEffect = Assets.nemImpactFX,
                impactSoundDef = Assets.nemImpactSoundDef,
                defaultSwordEmission = 0f,
                chargeEffectString = "SwordChargeEffect",
                chargeAttackEffect = Assets.nemChargedSlashStartFX,
                chargeAttackLoopEffect = Assets.nemChargedSlashFX,
                swingEffect = Assets.nemSaberSwingFX,
                swingSound = "NemmandoSwingSaber",
                shootSound = "NemmandoShoot",
                bodyName = "SS2UNEMMANDO_NAME"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UNEMMANDO_MASTERY_SKIN_NAME",
                impactEffect = Assets.nemImpactFX,
                impactSoundDef = Assets.nemImpactSoundDef,
                defaultSwordEmission = 0f,
                chargeEffectString = "SwordChargeEffect",
                chargeAttackEffect = Assets.nemChargedSlashStartFX,
                chargeAttackLoopEffect = Assets.nemChargedSlashFX,
                swingEffect = Assets.nemSaberSwingFX,
                swingSound = "NemmandoSwingSaber",
                shootSound = "NemmandoShootClassic",
                hasSheath = true
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UNEMMANDO_CLASSIC_SKIN_NAME",
                impactEffect = Assets.nemImpactFX,
                impactSoundDef = Assets.nemImpactSoundDef,
                defaultSwordEmission = 1f,
                chargeEffectString = "SwordChargeEffect",
                chargeAttackEffect = Assets.nemChargedSlashStartFX,
                chargeAttackLoopEffect = Assets.nemChargedSlashFX,
                swingEffect = Assets.nemSaberSwingFX,
                swingSound = "NemmandoSwingSaber",
                shootSound = "NemmandoShootClassic"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UNEMMANDO_COMMANDO_SKIN_NAME",
                impactEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXSlashMerc"),
                impactSoundDef = LegacyResourcesAPI.Load<NetworkSoundEventDef>("NetworkSoundEventDefs/nseMercSwordImpact"),
                defaultSwordEmission = 1f,
                chargeEffectString = "SwordChargeEffectBlue",
                chargeAttackEffect = Assets.nemChargedSlashStartFXBlue,
                chargeAttackLoopEffect = Assets.nemChargedSlashFXBlue,
                swingEffect = Assets.mercSwingFX,
                swingSound = "Play_merc_sword_swing",
                shootSound = "Play_commando_M1"
            });

            skinList.Add(new SkinEffectData
            {
                skinNameToken = "SS2UNEMMANDO_VERGIL_SKIN_NAME",
                impactEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniImpactVFXSlashMerc"),
                impactSoundDef = Assets.nemImpactSoundDef,
                defaultSwordEmission = 5f,
                chargeEffectString = "SwordChargeEffectBlue",
                chargeAttackEffect = Assets.nemChargedSlashStartFXBlue,
                chargeAttackLoopEffect = Assets.nemChargedSlashFXBlue,
                hasSheath = true,
                swingEffect = Assets.mercSwingFX,
                swingSound = "NemmandoSwingSaber",
                shootSound = "NemmandoShoot"
            });
            #endregion

            skinInfos = skinList.ToArray();
        }

        // for external mods to add their own skin data
        public static void AddSkinData(SkinEffectData newSkinData)
        {
            Array.Resize(ref skinInfos, skinInfos.Length + 1);
            skinInfos[skinInfos.Length - 1] = newSkinData;
        }

        public static SkinEffectData GetSkinData(string skinName, string bodyName)
        {
            for (int i = 0; i < skinInfos.Length; i++)
            {
                if (skinInfos[i].skinNameToken == skinName)
                {
                    return skinInfos[i];
                }
            }

            for (int i = 0; i < skinInfos.Length; i++)
            {
                if (skinInfos[i].bodyName == bodyName)
                {
                    return skinInfos[i];
                }
            }

            return skinInfos[0];
        }
    }

    public class CustomEffectComponent : MonoBehaviour
    {
        public GameObject swingEffect;
        public GameObject impactEffect;
        public float defaultSwordEmission;
        public string chargeEffectString;
        public GameObject chargeAttackEffect;
        public GameObject chargeAttackLoopEffect;

        public string swingSound;
        public NetworkSoundEventDef impactSoundDef;

        public string shootSound;
        public string ionShootSound;

        public bool hasSheath;

        private CharacterBody body;
        private CharacterModel model;

        private void Awake()
        {
            this.body = this.GetComponent<CharacterBody>();
            this.model = this.GetComponentInChildren<CharacterModel>();
            this.Invoke("GetEffectData", 0.5f);
        }

        private void GetEffectData()
        {
            SkinEffectData skinData = Effects.GetSkinData(this.model.GetComponent<ModelSkinController>().skins[this.body.skinIndex].nameToken, this.body.baseNameToken);

            this.swingEffect = skinData.swingEffect;
            this.impactEffect = skinData.impactEffect;
            this.swingSound = skinData.swingSound;
            this.impactSoundDef = skinData.impactSoundDef;
            this.shootSound = skinData.shootSound;
            this.ionShootSound = skinData.ionShootSound;
            this.hasSheath = skinData.hasSheath;
            this.defaultSwordEmission = skinData.defaultSwordEmission;
            this.chargeAttackEffect = skinData.chargeAttackEffect;
            this.chargeAttackLoopEffect = skinData.chargeAttackLoopEffect;
            this.chargeEffectString = skinData.chargeEffectString;
        }

        public void PlaySwingEffect(string muzzleString)
        {
            EffectManager.SimpleMuzzleFlash(this.swingEffect, this.gameObject, muzzleString, false);
        }

        public void PlayShootSound(bool scaled)
        {
            if (scaled) Util.PlayAttackSpeedSound(this.swingSound, this.gameObject, this.body.attackSpeed);
            else Util.PlaySound(this.swingSound, this.gameObject);
        }
    }

    public struct SkinEffectData
    {
        public string skinNameToken;

        public GameObject swingEffect;
        public GameObject impactEffect;
        public float defaultSwordEmission;
        public string chargeEffectString;
        public GameObject chargeAttackEffect;
        public GameObject chargeAttackLoopEffect;

        public string swingSound;
        public NetworkSoundEventDef impactSoundDef;

        public string shootSound;
        public string ionShootSound;

        public bool hasSheath;

        public string bodyName;
    }
}