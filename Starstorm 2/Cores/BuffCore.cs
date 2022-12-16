using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using Starstorm2.Survivors.Chirr.Components;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

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

        public static BuffDef exeSuperchargedBuff;
        public static BuffDef nucleatorSpecialBuff;

        public static BuffDef chirrFriendBuff;
        public static BuffDef chirrSelfBuff;

        private GameObject greenChocPrefab;
        private GameObject greenChocEffect;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        public BuffCore()
        {
            RegisterBuffs();
            RegisterEffects();
            Hook();
        }


        /*

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
        }*/

        protected void RegisterBuffs()
        {
            //LogCore.LogInfo("Initializing Core: " + base.ToString());
            exeSuperchargedBuff = CreateBuffDef("ExecutionerSuperchargedBuff", false, false, false, new Color(72 / 255, 1, 1), LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffNullifiedIcon"));
            nucleatorSpecialBuff = CreateBuffDef("NucleatorSpecialBuff", false, false, false, Color.green, LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffOverheat"));

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

            chirrFriendBuff = CreateBuffDef("ChirrFriendBuff", false, false, false, new Color32(245, 123, 145, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("buffChirrSoulLink"));
            chirrSelfBuff = CreateBuffDef("ChirrSelfBuff", false, false, false, new Color32(245, 123, 145, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("buffChirrSoulLink"));

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
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.OnClientBuffsChanged += CharacterBody_OnClientBuffsChanged;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            //Prevent Infestors from infesting chirr friends
            IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                c.GotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                    );
                c.Emit(OpCodes.Ldloc_3);
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                {
                    return playerControlled || body.HasBuff(chirrFriendBuff) || body.HasBuff(chirrSelfBuff);
                });
            };
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.skillLocator)
            {
                if (self.HasBuff(greaterBannerBuff))
                {
                    if(self.skillLocator.primary) self.skillLocator.primary.cooldownScale *= 0.5f;
                    if (self.skillLocator.secondary) self.skillLocator.secondary.cooldownScale *= 0.5f;
                    if (self.skillLocator.utility) self.skillLocator.utility.cooldownScale *= 0.5f;
                    if (self.skillLocator.special) self.skillLocator.special.cooldownScale *= 0.5f;
                }
                if (self.HasBuff(chirrFriendBuff))
                {
                    if (self.skillLocator.primary) self.skillLocator.primary.cooldownScale *= 0.66f;
                    if (self.skillLocator.secondary) self.skillLocator.secondary.cooldownScale *= 0.66f;
                    if (self.skillLocator.utility) self.skillLocator.utility.cooldownScale *= 0.66f;
                    if (self.skillLocator.special) self.skillLocator.special.cooldownScale *= 0.66f;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                if (self.body.HasBuff(BuffCore.chirrSelfBuff) && !damageInfo.rejected && !(damageInfo.damageType.HasFlag(DamageType.BypassArmor) || damageInfo.damageType.HasFlag(DamageType.BypassBlock) || damageInfo.damageType.HasFlag(DamageType.BypassOneShotProtection)))
                {
                    ChirrFriendController friendController = self.GetComponent<ChirrFriendController>();
                    if (friendController && friendController.HasFriend())
                    {
                        float minionDamage = damageInfo.damage * 0.3f;
                        damageInfo.damage *= 0.7f;

                        DamageInfo minionDamageInfo = new DamageInfo
                        {
                            damage = minionDamage,
                            procCoefficient = 0f,
                            procChainMask = damageInfo.procChainMask,
                            position = damageInfo.position,
                            attacker = damageInfo.attacker,
                            inflictor = damageInfo.inflictor,
                            canRejectForce = true,
                            crit = damageInfo.crit,
                            damageColorIndex = damageInfo.damageColorIndex,
                            damageType = damageInfo.damageType,
                            dotIndex = damageInfo.dotIndex,
                            force = Vector3.zero
                        };
                        friendController.HurtFriend(minionDamageInfo);
                    }
                }
            }
            orig(self, damageInfo);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(greaterBannerBuff))
            {
                //args.cooldownMultAdd *= 0.5f; //handle in recalculatestats since I don't think this works
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

            if (sender.HasBuff(BuffCore.chirrFriendBuff))
            {
                args.damageMultAdd += 2f;
                args.healthMultAdd += 2f;
                //args.cooldownMultAdd *= 0.5f; //handle in recalculatestats since I don't think this works
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

        public static BuffDef CreateBuffDef(string name, bool canStack, bool isCooldown, bool isDebuff, Color color, Sprite iconSprite)
        {
            BuffDef bd = ScriptableObject.CreateInstance<BuffDef>();
            bd.name = name;
            bd.canStack = canStack;
            bd.isCooldown = isCooldown;
            bd.isDebuff = isDebuff;
            bd.buffColor = color;
            bd.iconSprite = iconSprite;
            buffDefs.Add(bd);

            (bd as UnityEngine.Object).name = bd.name;
            return bd;
        }
    }
}