using BepInEx;
using BepInEx.Configuration;
using R2API.Utils;
using Starstorm2.Cores;
using Starstorm2.Cores.Equipment;
using Starstorm2.Cores.Items;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2
{
    [BepInDependency("com.bepis.r2api", BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency("com.niwith.DropInMultiplayer", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
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
        "RecalculateStatsAPI"
    })]

    public class Starstorm : BaseUnityPlugin
    {
        internal const string guid = "com.ChirrLover.Starstorm2Unofficial";
        internal const string modName = "Starstorm 2 Unofficial";
        internal const string version = "0.4.0";

        public static Starstorm instance;

        public static bool scrollableLobbyInstalled; // putting this here because lazy, move it if you want

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
            instance = this;
            LogCore.logger = Logger;
            StaticValues.InitValues();

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ScrollableLobbyUI")) scrollableLobbyInstalled = true;

            Initialize();

            CommandHelper.AddToConsoleWhenReady();

            new Modules.ContentPacks().Initialize();
        }

        private void InitializeSurvivors()
        {
            //if (Modules.Config.EnableExecutioner.Value)
            new Modules.Survivors.Executioner().Initialize();
            new Modules.Survivors.Nemmando().Initialize();

            if (Modules.Config.ss_test.Value)
            {
                cyborgCore = new CyborgCore();
                chirrCore = new ChirrCore();
                new Modules.Survivors.Nucleator().Initialize();
            }
            //
            //if (EnableNucleator.Value) nucleatorCore = new NucleatorCore();
            //
            //if (EnablePyro.Value) pyroCore = new PyroCore();
        }

        private void Initialize()
        {
            //MAKE SURE DAMAGETYPES INITIALIZE FIRST
            damageTypeCore = new DamageTypeCore();

            Modules.Assets.Initialize();
            Modules.CameraParams.Initialize();
            Modules.States.Initialize();
            Modules.Buffs.Initialize();
            Modules.Config.Initialize();
            Modules.Projectiles.Initialize();
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
            if (false)//(Modules.Config.EnableEquipment.Value)
            {
                equipmentCore = new EquipmentCore();
                AddEquipmentIfEnabled(new CloakingHeadband(), EquipmentCore.instance.equipment);
                AddEquipmentIfEnabled(new GreaterWarbanner(), EquipmentCore.instance.equipment);
                AddEquipmentIfEnabled(new PressurizedCanister(), EquipmentCore.instance.equipment);
                EquipmentCore.instance.InitEquipment();
            }
            //enemyCore = new EnemyCore();
            if (false)//(Modules.Config.EnableItems.Value)
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
            }
            if (Modules.Config.EnableTyphoon.Value)
            {
                typhoonCore = new TyphoonCore();
            }
            if (false)//(Modules.Config.EnableVoid.Value)
            {
                voidCore = new VoidCore();
            }
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
    }
}