using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using Starstorm2Unofficial.Components.Projectiles;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Cores.Equipment;
using Starstorm2Unofficial.Cores.Items;
using Starstorm2Unofficial.Cores.NemesisInvasion;
using Starstorm2Unofficial.Modules;
using Starstorm2Unofficial.Modules.Achievements;
using Starstorm2Unofficial.SharedHooks;
using Starstorm2Unofficial.Survivors.Chirr;
using Starstorm2Unofficial.Survivors.Cyborg;
using Starstorm2Unofficial.Survivors.Pyro;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static Starstorm2Unofficial.ModCompat;

[assembly: HG.Reflection.SearchableAttribute.OptInAttribute]
namespace Starstorm2Unofficial
{
    [BepInDependency("pseudopulse.Survariants", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.BlightedElites", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamSS2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.niwith.DropInMultiplayer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.weliveinasociety.CustomEmotesAPI", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskyLives.RiskyMod", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Moffein.AdditiveExecutes", BepInDependency.DependencyFlags.SoftDependency)]

    [BepInDependency("com.Moffein.DefenseMatrixManager", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.R2API.PluginGUID)]
    [BepInDependency(R2API.DifficultyAPI.PluginGUID)]
    [BepInDependency(R2API.DotAPI.PluginGUID)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID)]
    [BepInDependency(R2API.LoadoutAPI.PluginGUID)]
    [BepInDependency(R2API.DirectorAPI.PluginGUID)]
    [BepInDependency(R2API.DamageAPI.PluginGUID)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID)]
    [BepInDependency(R2API.ItemAPI.PluginGUID)]
    [BepInDependency(R2API.ExecuteAPI.PluginGUID)]

    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(guid, modName, version)]

    public class StarstormPlugin : BaseUnityPlugin
    {
        internal const string guid = "com.ChirrLover.Starstorm2Unofficial";
        internal const string modName = "Starstorm 2 Unofficial";
        internal const string version = "0.22.9";

        public static StarstormPlugin instance;

        //These should be moved to ModCompat
        public static bool scrollableLobbyInstalled = false; // putting this here because lazy, move it if you want
        public static bool infernoPluginLoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool scepterPluginLoaded = false;
        public static bool classicItemsLoaded = false;
        public static bool kingArenaLoaded = false;
        public static bool emoteAPILoaded = false;
        public static bool blightedElitesLoaded = false;
        public static bool additiveExecutesLoaded = false;

        public static bool kingArenaActive = false;

        LogCore logCore;
        PrefabCore prefabCore;
        BuffCore buffCore;
        ItemCore itemCore;
        EquipmentCore equipmentCore;
        DoTCore dotCore;
        TyphoonCore typhoonCore;
        EventsCore eventsCore;
        DamageTypeCore damageTypeCore;
        public static NemesisInvasionCore nemesisInvasionCore;
        ItemDisplayCore itemDisplayCore;
        SkinsCore skinsCore;

        EnemyCore enemyCore;

        public void Awake()
        {
            Modules.Files.PluginInfo = Info;
            LanguageTokens.RegisterLanguageTokens();
            scepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            classicItemsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            kingArenaLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");
            emoteAPILoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.weliveinasociety.CustomEmotesAPI");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
            blightedElitesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.BlightedElites");
            additiveExecutesLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Moffein.AdditiveExecutes");
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ScrollableLobbyUI")) scrollableLobbyInstalled = true;
            ModCompat.Initialize();
            Modules.SoundBanks.Init();

            if (kingArenaLoaded)
            {
                Stage.onStageStartGlobal += SetArena;
            }

            instance = this;
            LogCore.logger = Logger;
            StaticValues.InitValues();


            Initialize();

            new Modules.ContentPacks().Initialize();

            //Figure out where to place this later.
            ShootableProjectileComponent.AddHooks();
            IgnoreSprintCrosshair.Init();
            AchievementHider.Init();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena(Stage obj)
        {
            StarstormPlugin.kingArenaActive = NS_KingKombatArena.KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }

        private void InitializeSurvivors()
        {
            if (Modules.Config.EnableExecutioner.Value) new Survivors.Executioner.ExecutionerCore().Initialize();
            if (Modules.Config.EnableNemmando.Value) new Survivors.Nemmando.NemmandoCore().Initialize();
            if (Modules.Config.EnableCyborg.Value) new CyborgCore();
            if (Modules.Config.EnableChirr.Value) new ChirrCore();
            if (Modules.Config.EnablePyro.Value) new PyroCore();
            if (Modules.Config.EnableNucleator.Value) new Survivors.Nucleator.NucleatorCore().Initialize();
        }

        private void Initialize()
        {
            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");

            //MAKE SURE DAMAGETYPES INITIALIZE FIRST
            damageTypeCore = new DamageTypeCore();

            Modules.Assets.Initialize();
            Modules.States.Initialize();
            Modules.Config.Initialize();

            SS2OCompat.InitCompat();

            Modules.Music.Initialize();
            Cores.Unlockables.VanillaSurvivorUnlockables.RegisterUnlockables();

            logCore = new LogCore(Logger);
            prefabCore = new PrefabCore();
            buffCore = new BuffCore();
            dotCore = new DoTCore();

            //Modules.ItemDisplays.PopulateDisplays();
            itemDisplayCore = new ItemDisplayCore();

            skinsCore = new SkinsCore();

            InitializeSurvivors();
            //enemyCore = new EnemyCore();
            RoR2.RoR2Application.onLoad += EnemyCore.StoreBodyIndexes;

            Modules.Effects.Initialize();

            //equipment must be loaded before items so hottest sauce interacts correctly with ss2 equips
            if (Modules.Config.EnableEquipment.Value)
            {
                equipmentCore = new EquipmentCore();
                if (ModCompat.SS2OCompat.ShouldLoadAutoconfigContent())
                {
                    AddEquipmentIfEnabled(new CloakingHeadband(), EquipmentCore.instance.equipment);
                    AddEquipmentIfEnabled(new GreaterWarbanner(), EquipmentCore.instance.equipment);
                }
                else
                {
                    Debug.LogWarning("SS2U: Disabling autoconfig equipment.");
                }
                //AddEquipmentIfEnabled(new PressurizedCanister(), EquipmentCore.instance.equipment);   //fuck this equipment in particular
                EquipmentCore.instance.InitEquipment();
            }

            if (Modules.Config.EnableItems.Value)
            {
                itemCore = new ItemCore();

                if (ModCompat.SS2OCompat.ShouldLoadAutoconfigContent())
                {
                    AddItemIfEnabled(new Fork(), ItemCore.instance.items);
                    AddItemIfEnabled(new MoltenCoin(), ItemCore.instance.items);
                    AddItemIfEnabled(new DetritiveTrematode(), ItemCore.instance.items);
                    AddItemIfEnabled(new CoffeeBag(), ItemCore.instance.items, false);
                    AddItemIfEnabled(new WatchMetronome(), ItemCore.instance.items);
                    AddItemIfEnabled(new DormantFungus(), ItemCore.instance.items);
                    AddItemIfEnabled(new DroidHead(), ItemCore.instance.items);
                    AddItemIfEnabled(new GreenChocolate(), ItemCore.instance.items);
                    AddItemIfEnabled(new NkotasHeritage(), ItemCore.instance.items, false);
                    AddItemIfEnabled(new RelicOfMass(), ItemCore.instance.items);
                    AddItemIfEnabled(new StirringSoul(), ItemCore.instance.items);

                    AddItemIfEnabled(new Diary(), ItemCore.instance.items);

                    AddItemIfEnabled(new StrangeCan(), ItemCore.instance.items);

                    AddItemIfEnabled(new ErraticGadget(), ItemCore.instance.items, false);
                }
                else
                {
                    Debug.LogWarning("SS2U: Disabling autoconfig items.");
                }

                //AddItemIfEnabled(new Malice(), ItemCore.instance.items);
                //AddItemIfEnabled(new BrokenBloodTester(), ItemCore.instance.items);
                //AddItemIfEnabled(new HottestSauce(), ItemCore.instance.items);
                //AddItemIfEnabled(new PrototypeJetBoots(), ItemCore.instance.items);
                //AddItemIfEnabled(new HuntersSigil(), ItemCore.instance.items);
                //AddItemIfEnabled(new BabyToys(), ItemCore.instance.items);
                ItemCore.instance.InitItems();
            }
            if (Modules.Config.EnableTyphoon.Value)
            {
                if (ModCompat.SS2OCompat.ShouldLoadAutoconfigContent())
                {
                    typhoonCore = new TyphoonCore();
                }
                else
                {
                    Debug.LogWarning("SS2U: Disabling Typhoon due to autoconfig.");
                }
            }
            if (Modules.Config.EnableVoid.Value)
            {
                if (ModCompat.SS2OCompat.ShouldLoadAutoconfigContent())
                {
                    nemesisInvasionCore = new NemesisInvasionCore();
                }
                else
                {
                    Debug.LogWarning("SS2U: Disabling Void due to autoconfig.");
                }
            }
            if (Modules.Config.EnableEvents.Value)
            {
                if (ModCompat.SS2OCompat.ShouldLoadAutoconfigContent())
                {
                    eventsCore = new EventsCore();
                }
                else
                {
                    Debug.LogWarning("SS2U: Disabling Storms due to autoconfig.");
                }
            }

            RoR2.Stage.onStageStartGlobal += BazaarChecker.Stage_onStageStartGlobal;
            RecalculateStatsAPI.GetStatCoefficients += GetStatCoefficients.RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GlobalEventManager.ProcessHitEnemy += SharedHooks.OnHitEnemy.GlobalEventManager_OnHitEnemy;
            On.RoR2.GlobalEventManager.OnCharacterDeath += SharedHooks.OnCharacterDeathGlobal.GlobalEventManager_OnCharacterDeath;
        }

        private void AddItemIfEnabled(SS2Item item, List<SS2Item> list, bool enableByDefault = true)
        {
            var enabled = Config.Bind<bool>("Starstorm 2 :: Items", $"Enable {item.NameInternal}", enableByDefault);
            if (enabled.Value)
            {
                list.Add(item);
            }
        }

        private void AddEquipmentIfEnabled(SS2Equipment eqp, List<SS2Equipment> list)
        {
            var enabled = Config.Bind<bool>("Starstorm 2 :: Equipment", $"Enable {eqp.NameInternal}", true);
            if (enabled.Value)
            {
                list.Add(eqp);
            }
        }

        public static DifficultyDef GetInfernoDef()
        {
            if (infernoPluginLoaded) return GetInfernoDefInternal();
            return null;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static DifficultyDef GetInfernoDefInternal()
        {
            return Inferno.Main.InfernoDiffDef;
        }
    }
}