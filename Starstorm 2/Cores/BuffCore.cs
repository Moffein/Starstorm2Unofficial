using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using Starstorm2Unofficial.Cores.NemesisInvasion;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace Starstorm2Unofficial.Cores
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
        public static BuffDef nucleatorSpecialDebuff;

        public static BuffDef chirrFriendBuff;
        public static BuffDef chirrSelfBuff;
        public static BuffDef chirrFriendDistractBuff;

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
            exeSuperchargedBuff = CreateBuffDef("SS2UExecutionerSuperchargedBuff", false, false, false, new Color(72 / 255, 1, 1), LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffNullifiedIcon"));
            nucleatorSpecialBuff = CreateBuffDef("SS2UNucleatorSpecialBuff", true, false, false, new Color32(224, 217, 67, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("radiation_debuff"));
            nucleatorSpecialDebuff = CreateBuffDef("SS2UNucleatorSpecialDebuff", false, false, false, new Color32(224, 217, 67, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("radiation_debuff"));

            detritiveBuff = ScriptableObject.CreateInstance<BuffDef>();
            detritiveBuff.buffColor = Color.white;
            //detritiveBuff.buffIndex = BuffIndex.Count;
            detritiveBuff.canStack = true;
            detritiveBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Trematodes");
            detritiveBuff.isDebuff = false;
            detritiveBuff.name = "SS2UInfested";
            buffDefs.Add(detritiveBuff);


            greaterBannerBuff = ScriptableObject.CreateInstance<BuffDef>();
            //buffIndex = BuffIndex.Count,
            greaterBannerBuff.canStack = false;
            greaterBannerBuff.iconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/WardOnLevel/texBuffWarbannerIcon.tif").WaitForCompletion();
            greaterBannerBuff.name = "SS2UGreaterWarbanner";
            greaterBannerBuff.buffColor = new Color(0.8392157f, 0.4882353f, 0.22745098f);
            buffDefs.Add(greaterBannerBuff);

            strangeCanPoisonBuff = ScriptableObject.CreateInstance<BuffDef>();
            //buffIndex = BuffIndex.Count,
            strangeCanPoisonBuff.canStack = false;
            strangeCanPoisonBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_StrangeCan");
            strangeCanPoisonBuff.name = "SS2UStrangeCanPoison";
            strangeCanPoisonBuff.isDebuff = false;
            strangeCanPoisonBuff.buffColor = new Color32(200, 233, 61, 255);
            buffDefs.Add(strangeCanPoisonBuff);

            sigilBuff = ScriptableObject.CreateInstance<BuffDef>();
            sigilBuff.canStack = false;
            sigilBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Sigil");
            sigilBuff.name = "SS2USigilCritDefBoost";
            sigilBuff.isDebuff = false;
            sigilBuff.buffColor = Color.white;
            buffDefs.Add(sigilBuff);

            greenChocBuff = ScriptableObject.CreateInstance<BuffDef>();
            greenChocBuff.canStack = true;
            greenChocBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_Chocolate");
            greenChocBuff.name = "SS2UGreenChocAttackBoost";
            greenChocBuff.isDebuff = false;
            greenChocBuff.buffColor = Color.white;
            buffDefs.Add(greenChocBuff);

            watchMetronomeBuff = ScriptableObject.CreateInstance<BuffDef>();
            watchMetronomeBuff.canStack = true;
            watchMetronomeBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_WatchMetronome");
            watchMetronomeBuff.name = "SS2UWatchMetronome";
            watchMetronomeBuff.isDebuff = false;
            watchMetronomeBuff.buffColor = new Color(0.376f, 0.843f, 0.898f, 1f);
            buffDefs.Add(watchMetronomeBuff);

            chirrFriendBuff = CreateBuffDef("SS2UChirrFriendBuff", false, false, false, new Color32(245, 123, 145, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("buffChirrSoulLink"));
            chirrSelfBuff = CreateBuffDef("SS2UChirrSelfBuff", false, false, false, new Color32(245, 123, 145, 255), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("buffChirrSoulLink"));
            chirrFriendDistractBuff = CreateBuffDef("SS2UChirrFriendDistractBuff", false, false, false, new Color(210f / 255f, 50f / 255f, 22f / 255f), Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/texBuffCloakIcon.tif").WaitForCompletion());

            #region Executioner
            fearDebuff = ScriptableObject.CreateInstance<BuffDef>();
            fearDebuff.canStack = false;
            fearDebuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("surprised-skull");
            fearDebuff.name = "SS2UFearDebuff";
            fearDebuff.isDebuff = true;
            fearDebuff.buffColor = Color.white;
            buffDefs.Add(fearDebuff);
            #endregion

            #region Nemmando
            gougeBuff = ScriptableObject.CreateInstance<BuffDef>();
            gougeBuff.canStack = true;
            gougeBuff.iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("Status_NemesisBleed");
            gougeBuff.name = "SS2UGouge";
            gougeBuff.isDebuff = true;
            gougeBuff.buffColor = Color.red;
            buffDefs.Add(gougeBuff);
            #endregion
        }

        private void RegisterEffects()
        {
            greenChocPrefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/WarCryEffect"), "SS2UGreenChocEffect", false);
            var particles = greenChocPrefab.GetComponentInChildren<ParticleSystem>();
            if (particles)
            {
                var mainmod = particles.main;
                //this doesn't do anything :(
                mainmod.startColor = new Color(0.58f, 0.90f, 0.66f);
            }
            Modules.Assets.AddEffect(greenChocPrefab);
        }

        private void Hook()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.CharacterBody.OnClientBuffsChanged += CharacterBody_OnClientBuffsChanged;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.EntityStates.BaseState.OnEnter += BaseState_OnEnter;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            //Prevent Infestors from infesting chirr friends
            IL.EntityStates.VoidInfestor.Infest.FixedUpdate += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(MoveType.After,
                     x => x.MatchCallvirt<CharacterBody>("get_isPlayerControlled")
                    ))
                {
                    c.Emit(OpCodes.Ldloc_3);
                    c.EmitDelegate<Func<bool, CharacterBody, bool>>((playerControlled, body) =>
                    {
                        return playerControlled || body.HasBuff(chirrFriendBuff) || body.HasBuff(chirrSelfBuff);
                    });
                }
                else
                {
                    Debug.LogError("Starstorm 2 Unofficial: Failed to set up Chirr anti-Void Infestor IL Hook.");
                }
            };

            //Steal vanilla overlay effects
            IL.RoR2.CharacterModel.UpdateOverlays += (il) =>
            {
                ILCursor c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "Weak")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || self.body.HasBuff(chirrFriendBuff) || self.body.HasBuff(nucleatorSpecialBuff);
                    });
                }
                else
                {
                    Debug.LogError("Starstorm 2 Unofficial: Failed to set up Weak overlay IL Hook.");
                }

                c = new ILCursor(il);
                if (c.TryGotoNext(
                     x => x.MatchLdsfld(typeof(RoR2Content.Buffs), "FullCrit")
                    ))
                {
                    c.Index += 2;
                    c.Emit(OpCodes.Ldarg_0);
                    c.EmitDelegate<Func<bool, CharacterModel, bool>>((hasBuff, self) =>
                    {
                        return hasBuff || (self.body.HasBuff(chirrFriendDistractBuff));
                    });
                }
                else
                {
                    Debug.LogError("Starstorm 2 Unofficial: Failed to set up FullCrit overlay IL Hook.");
                }
            };

            //Prevent befriended Gups/Geeps from splitting.
            On.EntityStates.Gup.BaseSplitDeath.OnEnter += (orig, self) =>
            {
                orig(self);
                if (self.characterBody && self.characterBody.HasBuff(BuffCore.chirrFriendBuff))
                {
                    if (NetworkServer.active)
                    {
                        self.spawnCount = 0;
                        self.DestroyBodyAsapServer();
                    }
                }
            };
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.skillLocator)
            {
                if (self.HasBuff(chirrFriendBuff))
                {
                    if (self.skillLocator.primary) self.skillLocator.primary.cooldownScale *= 0.66f;
                    if (self.skillLocator.secondary) self.skillLocator.secondary.cooldownScale *= 0.66f;
                    if (self.skillLocator.utility) self.skillLocator.utility.cooldownScale *= 0.66f;
                    if (self.skillLocator.special) self.skillLocator.special.cooldownScale *= 0.66f;

                    if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.OverheatImmune)) self.bodyFlags |= CharacterBody.BodyFlags.OverheatImmune;
                    if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToVoidDeath)) self.bodyFlags |= CharacterBody.BodyFlags.ImmuneToVoidDeath;
                    if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.ImmuneToExecutes)) self.bodyFlags |= CharacterBody.BodyFlags.ImmuneToExecutes;
                    if (!self.bodyFlags.HasFlag(CharacterBody.BodyFlags.IgnoreFallDamage)) self.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                //Chirr Friends dont take void fog damage
                bool isChirrFriend = self.body.HasBuff(BuffCore.chirrFriendBuff);
                if (isChirrFriend)
                {
                    if (!damageInfo.attacker && !damageInfo.inflictor
                        && damageInfo.damageColorIndex == DamageColorIndex.Void
                        && damageInfo.damageType == (DamageType.BypassArmor | DamageType.BypassBlock))
                    {
                        damageInfo.damage = 0f;
                        damageInfo.rejected = true;
                    }
                    if (!damageInfo.canRejectForce) damageInfo.force *= 0.1f;
                }

            }

            orig(self, damageInfo);
        }

        private void BaseState_OnEnter(On.EntityStates.BaseState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            orig(self);
            if (self.characterBody)
            {
                if (self.characterBody.HasBuff(BuffCore.chirrFriendBuff))
                {
                    if (ChirrFriendController.bodyDamageValueOverrides.TryGetValue(self.characterBody.bodyIndex, out float value))
                    {
                        self.damageStat *= value;
                    }
                    /*else if (Run.instance && Run.instance.ambientLevelFloor > self.characterBody.level)
                    {
                        self.damageStat *= (0.8f + 0.2f * Run.instance.ambientLevelFloor) / (0.8f + 0.2f * self.characterBody.level);
                    }*/

                    if (!self.characterBody.isElite)
                    {
                        self.damageStat *= 2f;
                    }
                    else
                    {
                        self.damageStat *= 3f;
                    }
                }
            }
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.HasBuff(greaterBannerBuff))
            {
                args.critAdd += 25f;
            }

            int chocStack = sender.GetBuffCount(greenChocBuff);
            if (chocStack > 0)
            {
                args.damageMultAdd += 0.5f * chocStack;
            }

            if (sender.HasBuff(BuffCore.fearDebuff))
            {
                args.moveSpeedReductionMultAdd += 0.5f;
            }

            if (sender.HasBuff(BuffCore.chirrFriendBuff))
            {
                if (!sender.isElite)
                {
                    args.healthMultAdd += 0.5f;
                }
                else
                {
                    args.healthMultAdd += 1f;
                }
            }

            if (sender.HasBuff(BuffCore.chirrFriendDistractBuff))
            {
                args.armorAdd += 100f;
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