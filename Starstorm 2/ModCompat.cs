using RoR2.Skills;
using R2API;
using RoR2;
using Starstorm2Unofficial.Cores.NemesisInvasion;
using Starstorm2Unofficial.Cores.NemesisInvasion.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Runtime.CompilerServices;
using Survariants;

namespace Starstorm2Unofficial
{
    internal static class ModCompat
    {
        public static void Initialize()
        {
            RiskyMod.InitCompat();
            SS2OCompat.InitCompat();
            SurvariantsCompat.InitCompat();
        }

        public static class RiskyMod
        {
            public static bool pluginLoaded = false;

            public static ItemIndex AllyMarkerItem, AllyScalingItem, AllyRegenItem, AllyAllowVoidDeathItem, AllyAllowOverheatDeathItem, AllyResistAoEItem;

            public static void InitCompat()
            {
                pluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskyLives.RiskyMod");
                RoR2Application.onLoad += OnLoadActions;
            }

            private static void OnLoadActions()
            {
                AllyMarkerItem = ItemCatalog.FindItemIndex("RiskyModAllyMarkerItem");
                AllyScalingItem = ItemCatalog.FindItemIndex("RiskyModAllyScalingItem");
                AllyRegenItem = ItemCatalog.FindItemIndex("RiskyModAllyRegenItem");
                AllyAllowVoidDeathItem = ItemCatalog.FindItemIndex("RiskyModAllyAllowVoidDeathItem");
                AllyAllowOverheatDeathItem = ItemCatalog.FindItemIndex("RiskyModAllyAllowOverheatDeathItem");
                AllyResistAoEItem = ItemCatalog.FindItemIndex("RiskyModAllyResistAoEItem");
            }
        }

        public static class SS2OCompat
        {
            public static bool pluginLoaded = false;
            public static bool autoConfig = true;

            public static bool enableNemMercInvasion = true;
            public static bool enableNemCommandoInvasion = true;

            private static BodyIndex NemMercIndex;
            private static BodyIndex NemCommandoIndex;

            public static bool ShouldLoadAutoconfigContent()
            {
                return !pluginLoaded || !autoConfig;
            }

            public static void InitCompat()
            {
                pluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2")
                    || BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamSS2")||
                     BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm");
                Debug.Log("Starstorm 2 Official Loaded: " + pluginLoaded);

                RoR2Application.onLoad += OnLoadActions;
            }
            private static void OnLoadActions()
            {
                NemMercIndex = BodyCatalog.FindBodyIndex("NemMercBody");
                NemCommandoIndex = BodyCatalog.FindBodyIndex("NemCommandoBody");

                if (Modules.Config.EnableVoid.Value)
                {
                    NemMercInvasionCompat();
                    NemCommandoInvasionCompat();
                }
            }

            private static void NemMercInvasionCompat()
            {
                if (!enableNemMercInvasion || NemMercIndex == BodyIndex.None) return;
                GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemMercMonsterMaster");  //This is mislabeled as requiring BodyName instead of MasterName.
                if (!masterPrefab) return;
                Debug.Log("Starstorm 2 Unofficial: Adding SS2O NemMercenary to Nemesis invader list.");
                NemesisInvasionCore.AddNemesisBoss(masterPrefab, null, "Remuneration", true, true);
                NemesisInvasionCore.prioritizePlayersList.Add(NemMercIndex);

                NemesisInvasionCore.NemesisItemActions += NemMercStatModifier;
            }

            private static void NemMercStatModifier(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (sender.bodyIndex != NemMercIndex) return;

                float levelFactor = sender.level - 1f;
                args.baseHealthAdd += (3600f - sender.baseMaxHealth) + levelFactor * (1080f - sender.levelMaxHealth);
                args.baseRegenAdd -= sender.baseRegen + levelFactor * sender.levelRegen;
                args.baseDamageAdd -= (sender.baseDamage - 3f) + levelFactor * (sender.levelDamage - 0.6f);
            }

            private static void NemCommandoInvasionCompat()
            {
                //TODO: SKILL OVERRIDES
                if (!enableNemCommandoInvasion || NemCommandoIndex == BodyIndex.None) return;
                GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemCommandoMonsterMaster");  //This is mislabeled as requiring BodyName instead of MasterName.
                if (!masterPrefab) return;
                Debug.Log("Starstorm 2 Unofficial: Adding SS2O NemCommando to Nemesis invader list.");
                NemesisInvasionCore.AddNemesisBoss(masterPrefab, null, "StirringSoul", true, true);
                NemesisInvasionCore.prioritizePlayersList.Add(NemCommandoIndex);

                NemesisInvasionCore.NemesisItemActions += NemCommandoStatModifier;

                NemesisItemBehavior.NemStartActions += NemCommandoStart;
            }

            private static void NemCommandoStart(NemesisItemBehavior self, CharacterBody body)
            {
                if (body.bodyIndex != NemCommandoIndex || !body.skillLocator || !body.skillLocator.secondary) return;

                SkillDef targetSkillDef = null;

                foreach (SkillFamily.Variant v in body.skillLocator.secondary.skillFamily.variants)
                {
                    if (v.skillDef && v.skillDef.skillName.Equals("NemCommandoSwordBeam"))
                    {
                        targetSkillDef = v.skillDef;
                        break;
                    }
                }

                if (targetSkillDef)
                {
                    body.skillLocator.secondary.SetSkillOverride(self, targetSkillDef, GenericSkill.SkillOverridePriority.Replacement);
                }
            }

            private static void NemCommandoStatModifier(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (sender.bodyIndex != NemCommandoIndex) return;

                float levelFactor = sender.level - 1f;
                args.baseHealthAdd += (3600f - sender.baseMaxHealth) + levelFactor * (1080f - sender.levelMaxHealth);
                args.baseRegenAdd -= sender.baseRegen + levelFactor * sender.levelRegen;
                args.baseDamageAdd -= (sender.baseDamage - 3f) + levelFactor * (sender.levelDamage - 0.6f);
            }
        }

        public static class SurvariantsCompat
        {
            public static bool pluginLoaded = false;

            public static bool useVariants = true;

            public static void InitCompat()
            {
                pluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("pseudopulse.Survariants");
            }

            public static void SetVariant(SurvivorDef variantDef, string baseBodyName)
            {
                if (!pluginLoaded) return;
                GameObject bodyPrefab = BodyCatalog.FindBodyPrefab(baseBodyName);
                SurvivorDef baseSurvivorDef = SurvivorCatalog.FindSurvivorDefFromBody(bodyPrefab);

                if (!baseSurvivorDef)
                {
                    Debug.LogWarning("SS2U: Survariants Compat: Could not find SurvivorDef for " + baseBodyName);
                    return;
                }

                SetVariant(variantDef, baseSurvivorDef);
            }

            public static void SetVariant(SurvivorDef variantDef, SurvivorDef baseSurvivorDef)
            {
                if (pluginLoaded && useVariants && variantDef && baseSurvivorDef) SetVariantInternal(variantDef, baseSurvivorDef);
            }

            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static void SetVariantInternal(SurvivorDef variantDef, SurvivorDef baseSurvivorDef)
            {
                SurvivorVariantDef variant = ScriptableObject.CreateInstance<SurvivorVariantDef>();
                (variant as ScriptableObject).name = variantDef.cachedName;
                variant.DisplayName = variantDef.displayNameToken;
                variant.VariantSurvivor = variantDef;
                variant.TargetSurvivor = baseSurvivorDef;
                variant.RequiredUnlock = variantDef.unlockableDef;
                variant.Description = "Starstorm 2 Unofficial";

                variantDef.hidden = true;
                SurvivorVariantCatalog.AddSurvivorVariant(variant);
            }
        }
    }
}
