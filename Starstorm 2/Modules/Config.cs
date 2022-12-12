using BepInEx.Configuration;
using RiskOfOptions;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Starstorm2.Modules
{
    internal static class Config
    {
        #region Config
        internal static ConfigEntry<bool> ss_test;
        internal static ConfigEntry<bool> cursed;

        internal static ConfigEntry<bool> EnableEnemies;

        internal static ConfigEntry<KeyCode> RestKeybind;
        internal static KeyCode restKeybind;
        internal static ConfigEntry<KeyCode> TauntKeybind;
        internal static KeyCode tauntKeybind;

        internal static ConfigEntry<bool> EnableItems;
        internal static List<ConfigEntry<bool>> ItemToggles;
        internal static ConfigEntry<bool> EnableEquipment;
        internal static ConfigEntry<bool> EnableFunnyCanister;

        internal static ConfigEntry<bool> EnableTyphoon;
        internal static ConfigEntry<bool> TyphoonIncreaseSpawnCap;
        internal static ConfigEntry<bool> EnableEthereal;
        internal static ConfigEntry<bool> EnableVoid;
        internal static ConfigEntry<bool> EnableElites;

        internal static ConfigEntry<bool> EnableEvents;
        internal static ConfigEntry<int> stormCreditCost;
        internal static ConfigEntry<bool> enableOptimizedStormVisuals;
        internal static ConfigEntry<bool> disableStormVisuals;

        internal static ConfigEntry<bool> EnableUnlockAll;
        #endregion

        internal static void Initialize()
        {
            ss_test =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Unfinished Content",
                            "Enabled",
                            false,
                            "Enables Starstorm 2's work-in-progress content. May be unstable so enable at your own risk.");
            cursed =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Cursed",
                            "Enabled",
                            false,
                            "Enables Starstorm 2's lesser serious features, featuring content ranging from skins that slightly clash with lore to high quality shitposts - here be dragons!.");
            /*EnableItems =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Items",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's items. Set to false to disable all of Starstorm 2's items.");
            EnableEquipment =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Equipment",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's equipment. Set to false to disable all of Starstorm 2's equipment.");
            EnableFunnyCanister =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Equipment",
                            "Pressurized Canister No Jump Control",
                            false,
                            "Set to true to disable jump control on Pressurized Canister - activating the equipment will apply constant upward force regardless of whether you hold the jump button. This may lead to Funny and Memorable (tm) moments, especially if you like picking up Gestures of the Drowned.");
            
            EnableVoid =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Enabled",
                            true,
                            "Enables gameplay changes related to the Void Fields hidden realm. Set to false to make Void Fields behave as it does in vanilla RoR2. Note that some unlocks will be unavailable if this is disabled.");
             */
            EnableTyphoon =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Typhoon",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's Typhoon difficulty. Set to false to prevent Typhoon from appearing in difficulty selection.");

            TyphoonIncreaseSpawnCap =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Typhoon",
                            "Increase Spawn Limit",
                            true,
                            "Increases the enemy spawn limit when Typhoon difficulty is selected. May cause bugs or performance issues. Disable to use the default spawn limit.");

            EnableEvents =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Events",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's random events, including storms. Set to false to disable events.");
            stormCreditCost =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Events",
                            "Storm Credit Cost",
                            150,
                            "Credit cost to begin a storm. Increase this value to make storms less frequent.");
            enableOptimizedStormVisuals =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Events",
                            "Enable Optimized Storm Visuals",
                            false,
                            "Changes storm visuals like rain and snow to have far fewer particles to improve performance");
            disableStormVisuals =
                Starstorm.instance.Config.Bind("Starstorm 2 :: Events",
                            "Disable Storm Visuals",
                            false,
                            "Removes storm particle effects entirely if they're still hurting your computer. Doesn't disable environment fog/filter");

            //survivors
            //EnableExecutioner = CharacterEnableConfig("Executioner");
            //EnableNemmando = CharacterEnableConfig("Nemmando", "Nemesis Commando");
            //EnableCyborg = CharacterEnableConfig("Cyborg");
            //EnableChirr = CharacterEnableConfig("Chirr");
            //EnablePyro = CharacterEnableConfig("Pyro");

            //EnableEnemies = Config.Bind("Starstorm 2 :: Enemies", "Enabled", true, "Enables Starstorm 2's enemies. Set to false to disable all enemies added by Starstorm 2.");

            //emotes
            RestKeybind = Starstorm.instance.Config.Bind("Starstorm 2 :: Keybinds", "Rest Emote", KeyCode.Alpha1, "Keybind used for the Rest emote.");
            restKeybind = RestKeybind.Value;// cache it for performance

            TauntKeybind = Starstorm.instance.Config.Bind("Starstorm 2 :: Keybinds", "Taunt Emote", KeyCode.Alpha2, "Keybind used for the Taunt emote.");
            tauntKeybind = TauntKeybind.Value;// cache it for performance

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                RiskOfOptionsCompat();
            }
        }
        // this helper automatically makes config entries for enabling/disabling survivors
        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return CharacterEnableConfig(characterName, characterName);
        }

        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName, string fullName)
        {
            return Starstorm.instance.Config.Bind<bool>(new ConfigDefinition("Starstorm 2 :: " + characterName, "Enabled"), true, new ConfigDescription("Enables Starstorm 2's " + fullName + " survivor. Set to false to disable Starstorm 2's " + fullName + " survivor."));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            //NEED TO CONVERT EMOTES TO USE KEYBOARDSHORTCUT AND GETKEYPRESSED
        }

        //Taken from https://github.com/ToastedOven/CustomEmotesAPI/blob/main/CustomEmotesAPI/CustomEmotesAPI/CustomEmotesAPI.cs
        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }
}