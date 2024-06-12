using BepInEx.Configuration;
using RiskOfOptions;
using Starstorm2Unofficial.Modules.Achievements;
using Starstorm2Unofficial.Survivors.Cyborg;
using Starstorm2Unofficial.Survivors.Cyborg.Components.Crosshair;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Starstorm2Unofficial.Modules
{
    internal static class Config
    {
        #region Config
        internal static ConfigEntry<bool> ss_test;
        internal static ConfigEntry<bool> cursed;

        internal static ConfigEntry<KeyboardShortcut> RestKeybind;
        internal static ConfigEntry<KeyboardShortcut> TauntKeybind;

        internal static ConfigEntry<bool> EnableGrandMasteryCommando;
        internal static ConfigEntry<bool> EnableGrandMasteryToolbot;

        internal static ConfigEntry<bool> EnableExecutioner;
        internal static ConfigEntry<bool> EnableNemmando;
        internal static ConfigEntry<bool> EnableCyborg;
        internal static ConfigEntry<bool> EnableChirr;
        internal static ConfigEntry<bool> EnablePyro;
        internal static ConfigEntry<bool> EnableNucleator;

        internal static ConfigEntry<bool> EnableItems;
        internal static List<ConfigEntry<bool>> ItemToggles;
        internal static ConfigEntry<bool> EnableEquipment;
        internal static ConfigEntry<bool> EnableFunnyCanister;

        internal static ConfigEntry<bool> ChirrEgoFullHeadReplacement;

        internal static ConfigEntry<bool> NemmandoDecisiveMoveSpeedScaling;
        internal static ConfigEntry<bool> NemmandoSecondaryAlwaysFullCharge;

        internal static ConfigEntry<bool> EnableTyphoon;
        internal static ConfigEntry<bool> TyphoonIncreaseSpawnCap;
        internal static ConfigEntry<bool> EnableEvents;
        internal static ConfigEntry<int> stormCreditCost;
        internal static ConfigEntry<bool> enableOptimizedStormVisuals;
        internal static ConfigEntry<bool> disableStormVisuals;

        internal static ConfigEntry<bool> ForceUnlockSkins;
        internal static ConfigEntry<bool> EnableUnlockAll;

        internal static ConfigEntry<bool> EnableVoid;
        #endregion

        internal static void Initialize()
        {

            ModCompat.SurvariantsCompat.useVariants = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Compatibility",
                            "Survariants",
                            true,
                            "Enable Survariants compat. Nemesis characters will be variants of their original character, and duplicated SS2 Official characters will be variants of those characters if they are enabled.").Value;

            EnableUnlockAll = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Unlock All",
                            "Unlock Gameplay",
                            false,
                            "Automatically unlock all survivors and alt skills.");

            ForceUnlockSkins = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Unlock All",
                            "Unlock Skins",
                            false,
                            "Automatically unlock all skins.");

            AchievementHider.enabled = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Unlock All",
                            "Hide Disabled Achievements",
                            true,
                            "Achievements for config-disabled content will not show up in the logbook (might still show up in other places).").Value;

            cursed =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Cursed",
                            "Enabled",
                            false,
                            "Enables Starstorm 2's lesser serious features, featuring content ranging from skins that slightly clash with lore to high quality shitposts - here be dragons!.");
            EnableItems =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Items",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's items. Set to false to disable all of Starstorm 2's items.");
            EnableEquipment =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Equipment",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's equipment. Set to false to disable all of Starstorm 2's equipment.");

            EnableExecutioner = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                            "Executioner",
                            true,
                            "Enable this survivor.");

            EnableNemmando = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                             "Nemesis Commando",
                             true,
                             "Enable this survivor.");

            EnableCyborg = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                             "Cyborg",
                             true,
                             "Enable this survivor.");

            EnableChirr = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                             "Chirr",
                             true,
                             "Enable this survivor.");

            EnablePyro = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                             "Pyro",
                             true,
                             "Enable this survivor.");

            EnableNucleator = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors",
                             "Nucleator",
                             true,
                             "Enable this survivor.");

            EnableGrandMasteryCommando = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Skins",
                             "Commando: Grand Mastery",
                             true,
                             "Enable this skin.");
            
            EnableGrandMasteryToolbot = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Skins",
                              "MUL-T: Grand Mastery",
                              true,
                              "Enable this skin.");

            EnableVoid =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Enabled",
                            true,
                            "Enables gameplay changes related to the Void Fields hidden realm. Set to false to make Void Fields behave as it does in vanilla RoR2. Note that some unlocks will be unavailable if this is disabled.");

            Cores.NemesisInvasion.NemesisInvasionCore.scaleHPWithPlayercount = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Scale HP with Playercount",
                            true,
                            "Nemesis Bosses gain extra HP per connected player.").Value;

            Cores.NemesisInvasion.NemesisInvasionCore.hpMult = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - HP Multiplier",
                            1f,
                            "Multiplies Nemesis Boss HP. Stacks multiplicatively with Playercount HP Scaling. Must be > 1").Value;

            Cores.NemesisInvasion.NemesisInvasionCore.damageMult = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Damage Multiplier",
                            1f,
                            "Multiplies Nemesis Boss damage. Must be > 1").Value;

            Cores.NemesisInvasion.NemesisInvasionCore.speedMult = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Speed Multiplier",
                            1f,
                            "Multiplies Nemesis Boss movement speed. Must be > 1").Value;

            Cores.NemesisInvasion.NemesisInvasionCore.moveSpeedCap = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Speed Cap",
                            0f,
                            "Maximum raw movement speed value for Nemesis Bosses. Set to 0 to disable this check.").Value;

            Cores.NemesisInvasion.NemesisInvasionCore.bonusArmor = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Bonus Armor",
                            0f,
                            "Additional armor that Nemesis Bosses recieve.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.useVoidTeam = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Use Void Team",
                            false,
                            "Invaders are a part of the void team.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.requireFullVoid = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Require Void Fields Completion",
                            true,
                            "Invasion only starts if Void Fields was successfully cleared. If false, you only need to enter Void Fields to trigger the invasion.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.useAIBlacklist = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion  - Blacklist AIBlacklist items.",
                             true,
                             "Prevents invaders from getting items with this tag.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.useMithrixBlacklist = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion - Item Blacklists - Blacklist BrotherBlacklist (Mithrix) items.",
                             true,
                             "Prevents invaders from getting items with this tag.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.useEngiTurretBlacklist = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion - Item Blacklists - Blacklist CannotCopy (Engi Turret) items.",
                             true,
                             "Prevents invaders from getting items with this tag.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.useHealingBlacklist = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion - Item Blacklists - Blacklist Healing items.",
                             true,
                             "Prevents invaders from getting items with this tag.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.nemesisItemBlacklistString = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion - Additional Item Blacklist",
                             "IceRing, FireRing, ElementalRingVoid, FlatHealth, PersonalShield, ArmorPlate, MushroomVoid, Bear, BearVoid, ITEM_BLOODMASK, BleedOnHit, BleedOnHitVoid, BleedOnHitAndExplode, Missile, MissileVoid, PrimarySkillShuriken, ShockNearby, NovaOnHeal, Thorns, DroneWeapons, Icicle, ImmuneToDebuff, CaptainDefenseMatrix, ExtraLife, ExtraLifeVoid, ExplodeOnDeathVoid",
                             "List item codenames separated by commas (ex. Behemoth, ShockNearby, Clover). List of item codenames can be found by using the list_item console command from the DebugToolKit mod.").Value;

            Cores.NemesisInvasion.Components.NemesisInvasionManager.forceRemoveBlacklistedItems = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Item Blacklists",
                             "Nemesis Invasion - Item Blacklists - Force Remove Blacklisted Items",
                             true,
                             "Removes blacklisted items even if they were added from other sources (ex. EnemiesWithItems).").Value;

            ModCompat.SS2OCompat.enableNemCommandoInvasion = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Compatibility",
                             "SS2 Official Nemesis Commando",
                             true,
                             "Add this survivor to the Nemesis Invasion event.").Value;

            ModCompat.SS2OCompat.enableNemMercInvasion = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes :: Compatibility",
                             "SS2 Official Nemesis Mercenary",
                             true,
                             "Add this survivor to the Nemesis Invasion event.").Value;

            Starstorm2Unofficial.Survivors.Chirr.Components.ChirrFriendController.allowBefriendNemesis = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Void Fields changes",
                            "Nemesis Invasion - Allow Chirr Befriend",
                            false,
                            "Allow Chirr to befriend Nemesis Invaders when she has Scepter. Might cause crashes?").Value;

            EnableTyphoon =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Typhoon",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's Typhoon difficulty. Set to false to prevent Typhoon from appearing in difficulty selection.");

            TyphoonIncreaseSpawnCap =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Typhoon",
                            "Increase Spawn Limit",
                            true,
                            "Increases the enemy spawn limit when Typhoon difficulty is selected. May cause bugs or performance issues. Disable to use the default spawn limit.");

            EnableEvents =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Events",
                            "Enabled",
                            true,
                            "Enables Starstorm 2's random events, including storms. Set to false to disable events.");
            stormCreditCost =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Events",
                            "Storm Credit Cost",
                            150,
                            "Credit cost to begin a storm. Increase this value to make storms less frequent.");
            enableOptimizedStormVisuals =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Events",
                            "Enable Optimized Storm Visuals",
                            false,
                            "Changes storm visuals like rain and snow to have far fewer particles to improve performance");
            disableStormVisuals =
                StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Events",
                            "Disable Storm Visuals",
                            false,
                            "Removes storm particle effects entirely if they're still hurting your computer. Doesn't disable environment fog/filter");

            ChirrEgoFullHeadReplacement = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Chirr",
                             "Egocentrism full head replacement.",
                             false,
                             "Egocentrism replaces Chirr's head (looks a bit jank).");
            Starstorm2Unofficial.Survivors.Chirr.Components.ChirrFriendController.minionPingRetarget = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Chirr",
                             "Minion Ping Targeting",
                             true,
                             "Befriended minions attack enemies you ping.").Value;

            NemmandoDecisiveMoveSpeedScaling = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Nemesis Commando",
                             "Decisive Strike Move Speed Scaling",
                             true,
                             "Decisive Strike's dash distance scales with move speed. (Client-Side)");

            NemmandoSecondaryAlwaysFullCharge = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Nemesis Commando",
                             "Distant Gash Always Fully Charges",
                             false,
                             "Distant Gash fully charges without holding down the button. (Client-Side)");

            EntityStates.SS2UStates.Nucleator.Utility.FireLeap.leapAirControl = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Nucleator",
                             "Utility Air Control",
                             false,
                             "Utility gains extra air control at higher movement speeds (this causes momentum to be preserved worse). (Client-Side)");
            EntityStates.SS2UStates.Nucleator.Utility.ChargeLeap.stationaryLeap = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Nucleator",
                             "Utility Stops Movement",
                             false,
                             "Charging the Utility makes you stand still. (Client-Side)");

            CyborgCore.useEnergyRework = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "Enable Energy Core Passive (Client-Side)",
                             true,
                             "Cyborg skills use a single Energy Pool instead of having cooldowns. Set this to false to get the old Cyborg kit.");

            CyborgCrosshairChargeController.useSimpleEnergyBar = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "UI - Simple Energy Meter",
                             false,
                             "Use a simplified Energy Meter. Ignores other Energy Meter options.");

            //How to even add this to riskofoptions? Just need a simple float input field.
            CyborgCrosshairChargeController.energyBarScale = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "UI - Energy Meter Scale",
                             0.6f,
                             "Size of Energy Meter.");
            CyborgCrosshairChargeController.fontSize = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "UI - Energy Meter Font Size",
                             24f,
                             "Size of Energy Meter number text.");
            CyborgCrosshairChargeController.energyBarXPos = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "UI - Energy Meter X Position",
                            0f,
                             "Horizontal position of Energy Meter.");
            CyborgCrosshairChargeController.energyBarYPos = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Survivors :: Cyborg",
                             "UI - Energy Meter Y Position",
                            -160f,
                             "Vertical position of Energy Meter.");


            //survivors
            //EnableExecutioner = CharacterEnableConfig("Executioner");
            //EnableNemmando = CharacterEnableConfig("Nemmando", "Nemesis Commando");
            //EnableCyborg = CharacterEnableConfig("Cyborg");
            //EnableChirr = CharacterEnableConfig("Chirr");
            //EnablePyro = CharacterEnableConfig("Pyro");

            //EnableEnemies = Config.Bind("Starstorm 2 :: Enemies", "Enabled", true, "Enables Starstorm 2's enemies. Set to false to disable all enemies added by Starstorm 2.");

            //emotes
            RestKeybind = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Keybinds", "Rest Emote", new KeyboardShortcut(KeyCode.Alpha1), "Keybind used for the Rest emote.");

            TauntKeybind = StarstormPlugin.instance.Config.Bind("Starstorm 2 :: Keybinds", "Taunt Emote", new KeyboardShortcut(KeyCode.Alpha2), "Keybind used for the Taunt emote.");

            if (StarstormPlugin.riskOfOptionsLoaded)
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
            return StarstormPlugin.instance.Config.Bind<bool>(new ConfigDefinition("Starstorm 2 :: " + characterName, "Enabled"), true, new ConfigDescription("Enables Starstorm 2's " + fullName + " survivor. Set to false to disable Starstorm 2's " + fullName + " survivor."));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void RiskOfOptionsCompat()
        {
            ModSettingsManager.SetModIcon(Assets.mainAssetBundle.LoadAsset<Sprite>("modIcon.png"));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(RestKeybind));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.KeyBindOption(TauntKeybind));

            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(NemmandoSecondaryAlwaysFullCharge));
            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(NemmandoDecisiveMoveSpeedScaling));

            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(EntityStates.SS2UStates.Nucleator.Utility.FireLeap.leapAirControl));

            ModSettingsManager.AddOption(new RiskOfOptions.Options.CheckBoxOption(CyborgCrosshairChargeController.useSimpleEnergyBar));
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