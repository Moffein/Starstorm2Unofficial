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

namespace Starstorm2Unofficial
{
    internal static class ModCompat
    {
        public static void Initialize()
        {
            RiskyMod.InitCompat();
            SS2OCompat.InitCompat();
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

            public static bool enableNemMercInvasion = true;
            public static bool enableNemCommandoInvasion = true;

            private static BodyIndex NemMercIndex;
            private static BodyIndex NemCommandoIndex;

            public static void InitCompat()
            {
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
                NemesisInvasionCore.AddSS2ONemesisBoss(masterPrefab, null, "Remuneration", true);
                NemesisInvasionCore.prioritizePlayersList.Add(NemMercIndex);

                NemesisInvasionCore.NemesisItemActions += NemMercStatModifier;
            }

            private static void NemMercStatModifier(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (sender.bodyIndex != NemMercIndex) return;

                float levelFactor = sender.level - 1f;
                args.baseHealthAdd += 3490f + levelFactor * 1047f; //3600 - 110 + levelFactor x (1080 - 33)
                args.baseRegenAdd -= sender.baseRegen + levelFactor * sender.levelRegen;
                args.baseDamageAdd -= 9f + levelFactor * 1.8f;   //12 - 3 + levelFactor x (2.4 - 0.6)
            }

            private static void NemCommandoInvasionCompat()
            {
                //TODO: SKILL OVERRIDES
                if (!enableNemCommandoInvasion || NemCommandoIndex == BodyIndex.None) return;
                GameObject masterPrefab = MasterCatalog.FindMasterPrefab("NemCommandoMonsterMaster");  //This is mislabeled as requiring BodyName instead of MasterName.
                if (!masterPrefab) return;
                Debug.Log("Starstorm 2 Unofficial: Adding SS2O NemCommando to Nemesis invader list.");
                NemesisInvasionCore.AddSS2ONemesisBoss(masterPrefab, null, "StirringSoul", true);
                NemesisInvasionCore.prioritizePlayersList.Add(NemCommandoIndex);

                NemesisInvasionCore.NemesisItemActions += NemCommandoStatModifier;
            }

            private static void NemCommandoStatModifier(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
            {
                if (sender.bodyIndex != NemCommandoIndex) return;

                float levelFactor = sender.level - 1f;
                args.baseHealthAdd += 3490f + levelFactor * 1047f; //3600 - 110 + levelFactor x (1080 - 33)
                args.baseRegenAdd -= sender.baseRegen + levelFactor * sender.levelRegen;
                args.baseDamageAdd -= 9f + levelFactor * 1.8f;   //12 - 3 + levelFactor x (2.4 - 0.6)
            }
        }
    }
}
