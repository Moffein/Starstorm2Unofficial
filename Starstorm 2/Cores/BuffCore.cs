using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2.Cores
{
    public class BuffCore
    {

        public static BuffDef detritiveBuff;
        public static BuffDef greaterBannerBuff;
        public static BuffDef strangeCanPoisonBuff;
        public static BuffDef sigilBuff;
        public static BuffDef greenChocBuff;
        public static BuffDef watchMetronomeBuff;
        public static BuffDef fearDebuff;
        public static BuffDef gougeBuff;
        public static BuffDef awarenessBuff;

        public static BuffDef exeSuperchargedBuff;
        public static BuffDef exeAssistBuff;
        public static BuffDef nucleatorSpecialBuff;

        private GameObject greenChocPrefab;
        private GameObject greenChocEffect;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        public BuffCore()
        {
            RegisterBuffs();
            RegisterEffects();
            Hook();
        }

        protected void RegisterBuffs()
        {
            //LogCore.LogInfo("Initializing Core: " + base.ToString());
            exeAssistBuff = AddNewBuff("ExecutionerAssistBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffPowerIcon"), Color.white, false, false);
            exeSuperchargedBuff = AddNewBuff("ExecutionerSuperchargedBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffNullifiedIcon"), new Color(72 / 255, 1, 1), false, false);
            nucleatorSpecialBuff = AddNewBuff("NucleatorSpecialBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffOverheat"), Color.green, false, false);

            detritiveBuff = ScriptableObject.CreateInstance<BuffDef>();
            detritiveBuff.buffColor = Color.white;
            //detritiveBuff.buffIndex = BuffIndex.Count;
            detritiveBuff.canStack = false;
            detritiveBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Trematodes");
            detritiveBuff.isDebuff = false;
            detritiveBuff.name = "Infested";
            buffDefs.Add(detritiveBuff);

            greaterBannerBuff = ScriptableObject.CreateInstance<BuffDef>();
            //buffIndex = BuffIndex.Count,
            greaterBannerBuff.canStack = false;
            greaterBannerBuff.iconSprite = LegacyResourcesAPI.Load<Sprite>("textures/bufficons/texBuffWarbannerIcon");
            greaterBannerBuff.name = "GreaterWarbanner";
            greaterBannerBuff.buffColor = new Color(0.8392157f, 0.4882353f, 0.22745098f);
            buffDefs.Add(greaterBannerBuff);

            strangeCanPoisonBuff = ScriptableObject.CreateInstance<BuffDef>();
            //buffIndex = BuffIndex.Count,
            strangeCanPoisonBuff.canStack = false;
            strangeCanPoisonBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_StrangeCan");
            strangeCanPoisonBuff.name = "StrangeCanPoison";
            strangeCanPoisonBuff.isDebuff = true;
            strangeCanPoisonBuff.buffColor = Color.green;
            buffDefs.Add(strangeCanPoisonBuff);

            sigilBuff = ScriptableObject.CreateInstance<BuffDef>();
            sigilBuff.canStack = false;
            sigilBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Sigil");
            sigilBuff.name = "SigilCritDefBoost";
            sigilBuff.isDebuff = false;
            sigilBuff.buffColor = Color.white;
            buffDefs.Add(sigilBuff);

            greenChocBuff = ScriptableObject.CreateInstance<BuffDef>();
            greenChocBuff.canStack = true;
            greenChocBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Chocolate");
            greenChocBuff.name = "GreenChocAttackBoost";
            greenChocBuff.isDebuff = false;
            greenChocBuff.buffColor = Color.white;
            buffDefs.Add(greenChocBuff);

            watchMetronomeBuff = ScriptableObject.CreateInstance<BuffDef>();
            watchMetronomeBuff.canStack = true;
            watchMetronomeBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_WatchMetronome");
            watchMetronomeBuff.name = "WatchMetronome";
            watchMetronomeBuff.isDebuff = false;
            watchMetronomeBuff.buffColor = Color.cyan;
            buffDefs.Add(watchMetronomeBuff);

            #region Executioner
            fearDebuff = ScriptableObject.CreateInstance<BuffDef>();
            fearDebuff.canStack = false;
            fearDebuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("surprised-skull");
            fearDebuff.name = "FearDebuff";
            fearDebuff.isDebuff = true;
            fearDebuff.buffColor = Color.white;
            buffDefs.Add(fearDebuff);
            #endregion

            #region Nemmando
            gougeBuff = ScriptableObject.CreateInstance<BuffDef>();
            gougeBuff.canStack = true;
            gougeBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_NemesisBleed");
            gougeBuff.name = "Gouge";
            gougeBuff.isDebuff = true;
            gougeBuff.buffColor = Color.red;
            buffDefs.Add(gougeBuff);

            awarenessBuff = ScriptableObject.CreateInstance<BuffDef>();
            awarenessBuff.canStack = true;
            awarenessBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Awareness");
            awarenessBuff.name = "Awareness";
            awarenessBuff.isDebuff = false;
            awarenessBuff.buffColor = Color.green;
            buffDefs.Add(awarenessBuff);
            #endregion
        }

        private void RegisterEffects()
        {
            //TODO: register in content pack?
            greenChocPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/WarCryEffect"), "GreenChocEffect", true);
            greenChocPrefab.AddComponent<NetworkIdentity>();
            var particles = greenChocPrefab.GetComponentInChildren<ParticleSystem>();
            if (particles)
            {
                var mainmod = particles.main;
                //this doesn't do anything :(
                mainmod.startColor = new Color(0.58f, 0.90f, 0.66f);
            }
        }

        private void Hook()
        {
            On.RoR2.CharacterBody.OnClientBuffsChanged += CharacterBody_OnClientBuffsChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(greaterBannerBuff))
            {
                args.cooldownMultAdd *= 0.5f;
                args.critAdd += 20f;
                args.regenMultAdd += 0.5f;
            }

            int chocStack = sender.GetBuffCount(greenChocBuff);
            if (chocStack > 0)
            {
                args.critAdd += 20f * chocStack;
                args.damageMultAdd += 0.5f * chocStack;
            }

            if (sender.HasBuff(BuffCore.fearDebuff))
            {
                args.moveSpeedReductionMultAdd += 0.5f;
            }
        }

        private void CharacterBody_OnClientBuffsChanged(On.RoR2.CharacterBody.orig_OnClientBuffsChanged orig, CharacterBody self)
        {
            orig(self);
            if (self.HasBuff(greenChocBuff) && !greenChocEffect)
            {
                Transform transform = self.mainHurtBox ? self.mainHurtBox.transform : self.transform;
                if (transform)
                {
                    /*
                    EffectData effectData = new EffectData();
                    effectData.color = new Color(0.58f, 0.90f, 0.66f);
                    effectData.start = transform.position;
                    EffectManager.SpawnEffect(greenChocPrefab, effectData, true);
                    */
                    greenChocEffect = Object.Instantiate(greenChocPrefab, transform.position, Quaternion.identity, transform);
                }
            }
            if (!self.HasBuff(greenChocBuff) && greenChocEffect)
            {
                Object.Destroy(greenChocEffect);
            }
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}