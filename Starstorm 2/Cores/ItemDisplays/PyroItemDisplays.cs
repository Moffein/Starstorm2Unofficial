using UnityEngine;
using RoR2;
using System.Collections.Generic;

//TODO: anniversary update item displays

namespace Starstorm2.Cores.ItemDisplays
{
    public static class PyroItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = PrefabCore.pyroPrefab;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            #region Display Rules
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0, 0.0027f, 0.0012f), new Vector3(330, 0, 0), new Vector3(0.0036f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "ShoulderL", new Vector3(0.00f, 0.00f, -0.00f), new Vector3(13.00001f, 110, 210f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Gun", new Vector3(-0.0015f, 0.0025f, 0.00f), new Vector3(0, 0, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Behemoth", "DisplayBehemoth", "Gun", new Vector3(-0.001f, 0.0035f, 0f), new Vector3(295, 90, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(-0.003f, 0.00065f, 0f), new Vector3(2.1344340f, 9.99999f, 20f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "CalfR", new Vector3(0, 0.0037f, -0.001f), new Vector3(75, 0, 0), new Vector3(0.001f, 0.0012f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.0007f, 0.0035f, -0.0027f), new Vector3(0f, 180f, 56f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0f, 0.0025f, 0.0008f), new Vector3(330, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "ShoulderR", new Vector3(-0.0008f, 0f, 0.0005f), new Vector3(0f, 340f, 90f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(0, 0.0035f, 0.0005f), new Vector3(310, 0, 0), new Vector3(0.006f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Muzzle", new Vector3(-0f, -0.0005f, -0.0f), new Vector3(0, 0, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(0, 0.003f, -0.003f), new Vector3(270, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ElbowR", new Vector3(0.00f, 0.0007f, 0.0003f), new Vector3(90, 0, 2), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Clover", "DisplayClover", "Gun", new Vector3(0, 0.00220f, 0), new Vector3(0f, 0f, 300f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "ElbowL", new Vector3(0f, 0f, -0.0005f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(0, 0.0035f, -0.0003f), new Vector3(345f, 0f, 0f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(0.001f, -0.00f, 0), new Vector3(0f, 70f, 270f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "ElbowR", new Vector3(-0.0008f, 0.0005f, 0), new Vector3(0, 330, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Muzzle", new Vector3(0.0f, 0, -0.00028f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Muzzle", new Vector3(0.0f, 0, -0.0003f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(0.00f, -0.0009f, -0.001f), new Vector3(0, 0, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Axe", new Vector3(0, 0.006f, 0), new Vector3(270, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "ThighR", new Vector3(0.00f, 0.003f, -0.001f), new Vector3(90, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "ShoulderR", new Vector3(0, 0.001f, 0.00f), new Vector3(0, 180, 180), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Pelvis", new Vector3(-0.002f, 0.00f, 0.001f), new Vector3(310, 30, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ExecuteLowHealthElite", "DisplayGuillotine", "Axe", new Vector3(0.00f, 0.007f, -0.0002f), new Vector3(0, 270, 270), new Vector3(0.01f, 0.015f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "ThighL", new Vector3(0.00f, 0.0023f, -0.0015f), new Vector3(80, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(0.001f, 0.0023f, -0.0003f), new Vector3(0, 90, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Chest", new Vector3(0.0f, 0.002f, 0.0023f), new Vector3(0, 20, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "Pelvis", new Vector3(0.0025f, 0.0006f, 0), new Vector3(70, 250, 5), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(0, 0.0023f, -0.0006f), new Vector3(45, 80, 0), new Vector3(0.006f, 0.008f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(-0.0013f, 0.0035f, -0.003f), new Vector3(0, 0, 4), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "Pelvis", new Vector3(-0.0023f, 0.001f, 0.001f), new Vector3(80, 20, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "ThighR", new Vector3(-0.0005f, 0.002f, -0.0015f), new Vector3(90f, 230f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Chest", new Vector3(-0.002f, 0.003f, -0.0023f), new Vector3(270, 0, 10), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Pelvis", new Vector3(0.0026f, -0.001f, -0.001f), new Vector3(0, 0, 260), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(0.00f, 0.002f, 0.0018f), new Vector3(0, 0, 0), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Pelvis", new Vector3(-0.003f, 0.0007f, -0.00f), new Vector3(0, 20, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Pelvis", new Vector3(0.0028f, 0.001f, -0.001f), new Vector3(85, 180, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SecondarySkillMagazine", "DisplayDoubleMag", "Gun", new Vector3(0.00f, 0, 0.00008f), new Vector3(15, 270, 0), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Pelvis", new Vector3(-0.0026f, 0.002f, -0.0014f), new Vector3(345, 0, 11), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.0015f, 0.003f, 0.002f), new Vector3(20, 90, 80), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "ShoulderR", new Vector3(0.0f, 0.0015f, -0.001f), new Vector3(270, 20, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Pelvis", new Vector3(0.0016f, 0.008f, 0.006f), new Vector3(0, 90, 180), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.00f, 0.003f, -0.003f), new Vector3(0, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0, 0.002f, 0), new Vector3(345, 0, 0), new Vector3(0.003f, 0.004f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "Head", new Vector3(0.0012f, 0.002f, -0.001f), new Vector3(90, 100, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(0, 0f, 0.002f), new Vector3(0f, 90f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.001f, 0.002f, -0.0005f), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.0006f, 0.002f, 0.002f), new Vector3(0, 0, 20), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "ShoulderL", new Vector3(0.0004f, 0.0015f, 0), new Vector3(0, 90, 180), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.001f, 0.0023f, 0.0017f), new Vector3(90, 0, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Chest", new Vector3(0.002f, 0.0f, 0.0006f), new Vector3(0, 70, 90), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "CalfR", new Vector3(0f, 0.002f, 0.0021f), new Vector3(0, 270, 280), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "ShoulderL", new Vector3(-0.0005f, 0, 0), new Vector3(270, 0, 0), new Vector3(0.008f, 0.006f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "Head", new Vector3(0, 0.004f, 0.001f), new Vector3(90, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Head", new Vector3(-0.0005f, 0.0004f, -0.00f), new Vector3(310, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Pelvis", new Vector3(0.003f, 0.0005f, 0.001f), new Vector3(0, 0, 270), new Vector3(0.01f, 0.01f, 0.0f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(0.0018f, 0.0043f, -0.002f), new Vector3(290, 180, 180), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Chest", new Vector3(0, 0.002f, 0.002f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "HandR", new Vector3(-0.001f, 0.0005f, -0.00f), new Vector3(300, 0, 270), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Pelvis", new Vector3(-0.0025f, 0.0005f, 0.002f), new Vector3(20, 315, 180), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "CalfL", new Vector3(0, 0.002f, -0.001f), new Vector3(0, 90, 45), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Pelvis", new Vector3(0.0032f, 0.0025f, -0.001f), new Vector3(0, 0, 160), new Vector3(0.006f, 0.006f, 0.006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "ElbowR", new Vector3(-0.00f, 0.001f, 0.001f), new Vector3(90, 180, 0), new Vector3(0.003f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "ShoulderR", new Vector3(-0.00f, 0.001f, 0.00f), new Vector3(280, 180, 0), new Vector3(0.002f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Chest", new Vector3(0, 0.002f, -0.002f), new Vector3(270, 0, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Pelvis", new Vector3(-0.0026f, 0, 0), new Vector3(90, 90, 0), new Vector3(0.012f, 0.012f, 0.012f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullCrown", "Head", new Vector3(0, 0.0024f, -0.0003f), new Vector3(320, 0, 0), new Vector3(0.005f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Pelvis", new Vector3(0, 0, 0.0023f), new Vector3(0, 0, 180), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Chest", new Vector3(0f, 0.0026f, 0.0026f), new Vector3(330f, 0f, 0f), new Vector3(0.0016f, 0.0016f, 0.0016f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, "DisplayToothMeshLarge", "Chest", new Vector3(-0.002f, 0.0055f, -0.001f), new Vector3(340f, 0f, 0f), new Vector3(0.07f, 0.07f, 0.07f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Muzzle", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Muzzle", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "Pelvis", new Vector3(0f, 0f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "ShoulderL", new Vector3(0.001f, -0.00f, -0.001f), new Vector3(0, 35, 250), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Chest", new Vector3(0f, 0.0035f, -0.0025f), new Vector3(15f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FireballsOnHit", "DisplayFireballsOnHit", "Gun", new Vector3(-0.003f, 0.003f, 0f), new Vector3(330f, 270f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "ThighL", new Vector3(0.0006f, 0.002f, -0.001f), new Vector3(0f, 45f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.0005f, 0.0024f, 0.0025f), new Vector3(0f, 0f, 45f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "ThighR", new Vector3(-0.00085f, 0.0033f, 0f), new Vector3(30f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("RandomDamageZone", "DisplayRandomDamageZone", "Gun", new Vector3(-0.002f, 0.0015f, 0.000f), new Vector3(270f, 270f, 270f), new Vector3(0.0004f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0.002f, 0.003f, 0f), new Vector3(70f, 180f, 190f), new Vector3(0.01f, 0.01f, 0.01f)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "ThighR", new Vector3(-0.000f, 0.0003f, -0.00f), new Vector3(0f, 0f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(-0.001f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0f, 0.0042f, 0.0005f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Head", new Vector3(0f, 0.0032f, 0.00f), new Vector3(0f, 90f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0f, 0.0012f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Pelvis", new Vector3(-0.0013f, -0.00f, 0.002f), new Vector3(0f, 345f, 195f), new Vector3(0.006f, 0.006f, 0.006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, ItemDisplayCore.capacitorPrefab, "Chest", new Vector3(-0.001f, 0f, -0.004f), new Vector3(0f, 0f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Pelvis", new Vector3(0.0015f, 0.0002f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Pelvis", new Vector3(-0.0016f, 0.0018f, -0.0016f), new Vector3(0f, 30f, 180f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Head", new Vector3(0f, 0.0022f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.019f, 0.018f, 0.012f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0f, 0.0034f, -0.003f), new Vector3(0f, 90f, 15f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Pelvis", new Vector3(0.002f, 0.0008f, -0.0014f), new Vector3(85f, 180f, 0f), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(0f, 0.0015f, -0.0025f), new Vector3(0f, 180f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Pelvis", new Vector3(0.0015f, -0.001f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Pelvis", new Vector3(-0.0024f, 0.0005f, 0.001f), new Vector3(0f, 30f, 170f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Chest", new Vector3(0.0015f, 0.0035f, 0f), new Vector3(280f, 0f, 15f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Pelvis", new Vector3(0.000f, 0.00f, -0.002f), new Vector3(0f, 0f, 180f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", new Vector3(0f, 0.004f, -0.0028f), new Vector3(30f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(0f, 0.0018f, 0.002f), new Vector3(15f, 0f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));

            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(-0.00f, 0.013f, -0.008f), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", new Vector3(0.007f, 0.02f, -0.0f), new Vector3(0f, 0, 0), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", new Vector3(-0.002f, 0.02f, -0.01f), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", new Vector3(0.003f, 0.022f, -0.003f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", new Vector3(0.003f, 0.022f, -0.003f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(0, 0.002f, -0.002f), new Vector3(0, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(0.003f, 0.0058f, 0), new Vector3(20, 90, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Chest", new Vector3(0.0017f, 0.003f, 0), new Vector3(0, 0, 330), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(0, 0.002f, -0.0033f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", new Vector3(0, 0.015f, -0.015f), new Vector3(0, 90, 0), new Vector3(0.2f, 0.2f, 0.2f)));

            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FallBoots,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfL",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.0028f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfR",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 0),
                            localScale = new Vector3(0.002f, 0.002f, 0.0028f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        public static void RegisterModdedDisplays()
        {
            /*
            #region Aetherium
            if (ItemDisplayCore.aetheriumInstalled)
            {
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_ACCURSED_POTION", ItemDisplayCore.LoadAetheriumDisplay("AccursedPotion"), "ThighL", new Vector3(-0.002f, 0, 0), new Vector3(0, 0, 180), new Vector3(0.01f, 0.01f, 0.01f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_VOID_HEART", ItemDisplayCore.LoadAetheriumDisplay("VoidHeart"), "Chest", new Vector3(0, 0.002f, 0), new Vector3(0, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_SHARK_TEETH", ItemDisplayCore.LoadAetheriumDisplay("SharkTeeth"), "CalfL", new Vector3(-0.001f, 0.003f, 0), new Vector3(0, 0, 335), new Vector3(0.005f, 0.005f, 0.005f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_BLOOD_SOAKED_SHIELD", ItemDisplayCore.LoadAetheriumDisplay("BloodSoakedShield"), "ElbowL", new Vector3(0.0012f, 0.002f, 0), new Vector3(0, 90, 0), new Vector3(0.003f, 0.003f, 0.003f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_FEATHERED_PLUME", ItemDisplayCore.LoadAetheriumDisplay("FeatheredPlume"), "Head", new Vector3(0, 0.002f, 0), new Vector3(335, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_SHIELDING_CORE", ItemDisplayCore.LoadAetheriumDisplay("ShieldingCore"), "Chest", new Vector3(0, 0.002f, -0.003f), new Vector3(0, 180, 0), new Vector3(0.002f, 0.002f, 0.002f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_UNSTABLE_DESIGN", ItemDisplayCore.LoadAetheriumDisplay("UnstableDesign"), "Chest", new Vector3(0, 0, -0.0025f), new Vector3(0, 45, 0), new Vector3(0.01f, 0.01f, 0.01f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_WEIGHTED_ANKLET", ItemDisplayCore.LoadAetheriumDisplay("WeightedAnklet"), "CalfR", new Vector3(0, 0.001f, 0), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_BLASTER_SWORD", ItemDisplayCore.LoadAetheriumDisplay("BlasterSword"), "Pelvis", new Vector3(-0.005f, 0.004f), new Vector3(0, 0, 240), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_WITCHES_RING", ItemDisplayCore.LoadAetheriumDisplay("WitchesRing"), "HandL", new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));

                itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule("ITEM_ALIEN_MAGNET", ItemDisplayCore.LoadAetheriumDisplay("AlienMagnet"), new Vector3(0.015f, 0.016f, -0.014f), new Vector3(0, 0, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));
                itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule("ITEM_INSPIRING_DRONE", ItemDisplayCore.LoadAetheriumDisplay("InspiringDrone"), new Vector3(-0.014f, 0.014f, 0), new Vector3(0, 0, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));

                itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule("EQUIPMENT_JAR_OF_RESHAPING", ItemDisplayCore.LoadAetheriumDisplay("JarOfReshaping"), new Vector3(0.01f, 0.02f, 0), new Vector3(270, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            }
            #endregion
            #region SupplyDrop
            if (ItemDisplayCore.supplyDropInstalled)
            {
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPElectroPlankton", ItemDisplayCore.LoadSupplyDropDisplay("ElectroPlankton"), "Chest", new Vector3(0, 0, -0.003f), new Vector3(0, 0, 90), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPHardenedBoneFragments", ItemDisplayCore.LoadSupplyDropDisplay("HardenedBoneFragments"), "Chest", new Vector3(-0.002f, 0.0035f, 0), new Vector3(0, 270, 0), new Vector3(0.015f, 0.015f, 0.015f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPQSGen", ItemDisplayCore.LoadSupplyDropDisplay("QSGen"), "ElbowL", new Vector3(0, 0.002f, 0), new Vector3(0, 0, 270), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPSalvagedWires", ItemDisplayCore.LoadSupplyDropDisplay("SalvagedWires"), "ThighR", new Vector3(-0.0025f, 0.002f, 0.0015f), new Vector3(90, 90, 0), new Vector3(0.006f, 0.006f, 0.006f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPShellPlating", ItemDisplayCore.LoadSupplyDropDisplay("ShellPlating"), "Pelvis", new Vector3(0, 0, -0.0032f), new Vector3(0, 180, 180), new Vector3(0.0025f, 0.0025f, 0.0025f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPPlagueHat", ItemDisplayCore.LoadSupplyDropDisplay("PlagueHat"), "Head", new Vector3(0, 0.004f, 0f), new Vector3(0, 180, 0), new Vector3(0.0025f, 0.0025f, 0.0025f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPPlagueMask", ItemDisplayCore.LoadSupplyDropDisplay("PlagueMask"), "Head", new Vector3(0, 0.0025f, 0.003f), new Vector3(0, 180, 0), new Vector3(0.0025f, 0.0025f, 0.0025f)));

                itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule("SUPPDRPBloodBook", ItemDisplayCore.LoadSupplyDropDisplay("BloodBook"), new Vector3(-0.015f, 0.02f, 0.008f), new Vector3(0, 0, 0), new Vector3(0.08f, 0.08f, 0.08f)));
            }
            #endregion

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            */
        }
    }
}