using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial
{
    internal static class ModCompat
    {
        public static void Initialize()
        {
            RiskyMod.InitCompat();
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
    }
}
