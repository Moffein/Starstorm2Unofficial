using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using RoR2;
using Starstorm2.Cores;
using Starstorm2.Cores.Equipment;
using Starstorm2.Cores.Items;
using Starstorm2.Survivors.Chirr;
using Starstorm2.Survivors.Cyborg;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Starstorm2
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2-Nightly", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("HIFU.Inferno", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.niwith.DropInMultiplayer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ThinkInvisible.ClassicItems", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Kingpinush.KingKombatArena", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(guid, modName, version)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LoadoutAPI",
        "LanguageAPI",
        "DirectorAPI",
        "NetworkingAPI",
        "SoundAPI",
        "CommandHelper",
        "DamageAPI",
        "RecalculateStatsAPI",
        "ContentAddition"
    })]

    public class StarstormPlugin : BaseUnityPlugin
    {
        internal const string guid = "com.ChirrLover.Starstorm2Unofficial";
        internal const string modName = "Starstorm 2 Unofficial";
        internal const string version = "0.5.3";

        public static StarstormPlugin instance;

        public static bool scrollableLobbyInstalled = false; // putting this here because lazy, move it if you want
        public static bool infernoPluginLoaded = false;
        public static bool riskOfOptionsLoaded = false;
        public static bool scepterPluginLoaded = false;
        public static bool classicItemsLoaded = false;
        public static bool kingArenaLoaded = false;

        public static bool kingArenaActive = false;

        LogCore logCore;
        PrefabCore prefabCore;
        BuffCore buffCore;
        ItemCore itemCore;
        EquipmentCore equipmentCore;
        DoTCore dotCore;
        TyphoonCore typhoonCore;
        //EtherealCore etherealCore;
        EventsCore eventsCore;
        DamageTypeCore damageTypeCore;
        public static VoidCore voidCore;
        ItemDisplayCore itemDisplayCore;
        SkinsCore skinsCore;

        CyborgCore cyborgCore;
        //NucleatorCore nucleatorCore;
        ChirrCore chirrCore;
        //PyroCore pyroCore;

        //EnemyCore enemyCore;

        public void Awake()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2-Nightly") || BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2"))
            {
                return;
            }
            scepterPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            classicItemsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ThinkInvisible.ClassicItems");
            kingArenaLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Kingpinush.KingKombatArena");

            if (kingArenaLoaded)
            {
                Stage.onStageStartGlobal += SetArena;
            }

            instance = this;
            LogCore.logger = Logger;
            StaticValues.InitValues();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ScrollableLobbyUI")) scrollableLobbyInstalled = true;

            Initialize();

            CommandHelper.AddToConsoleWhenReady();

            new Modules.ContentPacks().Initialize();
        }


        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena(Stage obj)
        {
            StarstormPlugin.kingArenaActive = NS_KingKombatArena.KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }

        private void InitializeSurvivors()
        {
            //if (Modules.Config.EnableExecutioner.Value)
            new Survivors.Executioner.ExecutionerCore().Initialize();

            new Survivors.Nemmando.NemmandoCore().Initialize();

            cyborgCore = new CyborgCore();

            chirrCore = new ChirrCore();

            if (false)//(Modules.Config.ss_test.Value)
            {
                new Modules.Survivors.Nucleator().Initialize();
            }
            //
            //if (EnableNucleator.Value) nucleatorCore = new NucleatorCore();
            //
            //if (EnablePyro.Value) pyroCore = new PyroCore();
        }

        private void Initialize()
        {
            infernoPluginLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("HIFU.Inferno");
            riskOfOptionsLoaded = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

            //MAKE SURE DAMAGETYPES INITIALIZE FIRST
            damageTypeCore = new DamageTypeCore();

            Modules.Assets.Initialize();
            Modules.CameraParams.Initialize();
            Modules.States.Initialize();
            Modules.Config.Initialize();
            Modules.Music.Initialize();
            Cores.Unlockables.VanillaSurvivorUnlockables.RegisterUnlockables();

            logCore = new LogCore(Logger);
            prefabCore = new PrefabCore();
            buffCore = new BuffCore();
            dotCore = new DoTCore();

            //Modules.ItemDisplays.PopulateDisplays();
            itemDisplayCore = new ItemDisplayCore();

            skinsCore = new SkinsCore();

            //if (EnableNemmando.Value) Cores.Unlockables.NemmandoUnlockables.RegisterUnlockables();
            //if (EnableExecutioner.Value) Cores.Unlockables.ExecutionerUnlockables.RegisterUnlockables();

            InitializeSurvivors();

            Modules.Effects.Initialize();

            //equipment must be loaded before items so hottest sauce interacts correctly with ss2 equips
            /*if (Modules.Config.EnableEquipment.Value)
            {
                equipmentCore = new EquipmentCore();
                AddEquipmentIfEnabled(new CloakingHeadband(), EquipmentCore.instance.equipment);
                AddEquipmentIfEnabled(new GreaterWarbanner(), EquipmentCore.instance.equipment);
                AddEquipmentIfEnabled(new PressurizedCanister(), EquipmentCore.instance.equipment);
                EquipmentCore.instance.InitEquipment();
            }*/

            //enemyCore = new EnemyCore();
            RoR2.RoR2Application.onLoad += EnemyCore.StoreBodyIndexes;

            /*if (Modules.Config.EnableItems.Value)
            {
                itemCore = new ItemCore();
                AddItemIfEnabled(new DormantFungus(), ItemCore.instance.items);
                AddItemIfEnabled(new DetritiveTrematode(), ItemCore.instance.items);
                AddItemIfEnabled(new Diary(), ItemCore.instance.items);
                AddItemIfEnabled(new MoltenCoin(), ItemCore.instance.items);
                AddItemIfEnabled(new Malice(), ItemCore.instance.items);
                AddItemIfEnabled(new Fork(), ItemCore.instance.items);
                AddItemIfEnabled(new CoffeeBag(), ItemCore.instance.items);
                AddItemIfEnabled(new BrokenBloodTester(), ItemCore.instance.items);
                AddItemIfEnabled(new HottestSauce(), ItemCore.instance.items);
                AddItemIfEnabled(new StrangeCan(), ItemCore.instance.items);
                AddItemIfEnabled(new PrototypeJetBoots(), ItemCore.instance.items);
                AddItemIfEnabled(new WatchMetronome(), ItemCore.instance.items);
                AddItemIfEnabled(new HuntersSigil(), ItemCore.instance.items);
                AddItemIfEnabled(new NkotasHeritage(), ItemCore.instance.items);
                AddItemIfEnabled(new ErraticGadget(), ItemCore.instance.items);
                AddItemIfEnabled(new GreenChocolate(), ItemCore.instance.items);
                AddItemIfEnabled(new DroidHead(), ItemCore.instance.items);
                AddItemIfEnabled(new RelicOfMass(), ItemCore.instance.items);
                AddItemIfEnabled(new StirringSoul(), ItemCore.instance.items);
                //AddItemIfEnabled(new BabyToys(), ItemCore.instance.items);
                ItemCore.instance.InitItems();
            }*/
            if (Modules.Config.EnableTyphoon.Value)
            {
                typhoonCore = new TyphoonCore();
            }
            /*if (Modules.Config.EnableVoid.Value)
            {
                voidCore = new VoidCore();
            }*/
            if (Modules.Config.EnableEvents.Value)
            {
                eventsCore = new EventsCore();
            }
        }

        private void AddItemIfEnabled(SS2Item item, List<SS2Item> list)
        {
            var enabled = Config.Bind<bool>("Starstorm 2 :: Items", $"Enable {item.NameInternal}", true);
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