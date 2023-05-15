using UnityEngine;
using RoR2;
using System.Collections.Generic;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Executioner
{
    internal class ExecutionerItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = ExecutionerCore.instance.bodyPrefab;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = ExecutionerCore.instance.itemDisplayRules;

            #region Display Rules
            #region RoR2Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0, 0.0027f, 0.0012f), new Vector3(330, 0, 0), new Vector3(0.0036f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "ShoulderL", new Vector3(0.00f, 0.00f, -0.00f), new Vector3(13.00001f, 110, 210f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Gun", new Vector3(-0.0015f, 0.0025f, 0.00f), new Vector3(0, 0, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Gun", new Vector3(-0.001f, 0.0035f, 0f), new Vector3(295, 90, 0), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(-0.003f, 0.00065f, 0f), new Vector3(2.1344340f, 9.99999f, 20f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "CalfR", new Vector3(0, 0.0037f, -0.001f), new Vector3(75, 0, 0), new Vector3(0.001f, 0.0012f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.0007f, 0.0035f, -0.0027f), new Vector3(0f, 180f, 56f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0F, 0.00233F, 0.00078F), new Vector3(340.8874F, 0F, 0F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "ShoulderR", new Vector3(-0.0008f, 0f, 0.0005f), new Vector3(0f, 340f, 90f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(0, 0.0035f, 0.0005f), new Vector3(310, 0, 0), new Vector3(0.006f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Muzzle", new Vector3(-0f, -0.0005f, -0.0f), new Vector3(0, 0, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(0, 0.003f, -0.003f), new Vector3(270, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ElbowR", new Vector3(0.00f, 0.0007f, 0.0003f), new Vector3(90, 0, 2), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Gun", new Vector3(0, 0.00220f, 0), new Vector3(0f, 0f, 300f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "ElbowL", new Vector3(0f, 0f, -0.0005f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(0, 0.0035f, -0.0003f), new Vector3(345f, 0f, 0f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(0.001f, -0.00f, 0), new Vector3(0f, 70f, 270f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "ElbowR", new Vector3(-0.0008f, 0.0005f, 0), new Vector3(0, 330, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Muzzle", new Vector3(0.0f, 0, -0.00028f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Muzzle", new Vector3(0.0f, 0, -0.0003f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            //itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(0.00f, -0.0009f, -0.001f), new Vector3(0, 0, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Axe", new Vector3(0, 0.006f, 0), new Vector3(270, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "ThighR", new Vector3(0.00f, 0.003f, -0.001f), new Vector3(90, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "ShoulderR", new Vector3(0, 0.001f, 0.00f), new Vector3(0, 180, 180), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Pelvis", new Vector3(-0.002f, 0.00f, 0.001f), new Vector3(310, 30, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "Axe", new Vector3(0.00f, 0.007f, -0.0002f), new Vector3(0, 270, 270), new Vector3(0.01f, 0.015f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "ThighL", new Vector3(0.00f, 0.0023f, -0.0015f), new Vector3(80, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(0.001f, 0.0023f, -0.0003f), new Vector3(0, 90, 0), new Vector3(0.008f, 0.008f, 0.008f)));
            //https://discord.com/channels/753709254598328400/755273415719387146/793188879557328916
            //https://discord.com/channels/753709254598328400/757459787117101096/785685039177793547
            //https://discord.com/channels/753709254598328400/757459787117101096/785641674977706034
            //this could've been done once but now must be done twice
            //:damnation:
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
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Gun", new Vector3(0.00f, 0, 0.00008f), new Vector3(15, 270, 0), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Pelvis", new Vector3(-0.0026f, 0.002f, -0.0014f), new Vector3(345, 0, 11), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.0015f, 0.003f, 0.002f), new Vector3(20, 90, 80), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "ShoulderR", new Vector3(0.0f, 0.0015f, -0.001f), new Vector3(270, 20, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Pelvis", new Vector3(0.0016f, 0.008f, 0.006f), new Vector3(0, 90, 180), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.00f, 0.003f, -0.003f), new Vector3(0, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0, 0.002f, 0), new Vector3(345, 0, 0), new Vector3(0.003f, 0.004f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "Head", new Vector3(0.0012f, 0.002f, -0.001f), new Vector3(90, 100, 0), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(0, 0f, 0.002f), new Vector3(0f, 90f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.001f, 0.002f, -0.0005f), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            //at around this point i scrolled down to check how many more items i had to do then lay my head on my desk at the realization
            //remember when i mentioned multiple times to wait for classic model
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.0006f, 0.002f, 0.002f), new Vector3(0, 0, 20), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "ShoulderL", new Vector3(0.0004f, 0.0015f, 0), new Vector3(0, 90, 180), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.001f, 0.0023f, 0.0017f), new Vector3(90, 0, 0), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Chest", new Vector3(0.002f, 0.0f, 0.0006f), new Vector3(0, 70, 90), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "CalfR", new Vector3(0f, 0.002f, 0.0021f), new Vector3(0, 270, 280), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "ShoulderL", new Vector3(-0.0005f, 0, 0), new Vector3(270, 0, 0), new Vector3(0.008f, 0.006f, 0.003f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("LunarPrimaryReplacement", "DisplayBirdEye", "Head", new Vector3(0, 0.004f, 0.001f), new Vector3(90, 180, 0), new Vector3(0.003f, 0.003f, 0.003f)));
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
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Gun", new Vector3(-0.003f, 0.003f, 0f), new Vector3(330f, 270f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "ThighL", new Vector3(0.0006f, 0.002f, -0.001f), new Vector3(0f, 45f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.0005f, 0.0024f, 0.0025f), new Vector3(0f, 0f, 45f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "ThighR", new Vector3(-0.00085f, 0.0033f, 0f), new Vector3(30f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "Gun", new Vector3(-0.002f, 0.0015f, 0.000f), new Vector3(270f, 270f, 270f), new Vector3(0.0004f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0.002f, 0.003f, 0f), new Vector3(70f, 180f, 190f), new Vector3(0.01f, 0.01f, 0.01f)));
            //set up thing to make this work (needs mirror that mirrors z - rotation)
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "ThighR", new Vector3(-0.000f, 0.0003f, -0.00f), new Vector3(0f, 0f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(-0.001f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0f, 0.0042f, 0.0005f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Gun", new Vector3(0f, 0.0032f, 0.00f), new Vector3(0f, 90f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0f, 0.0012f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Pelvis", new Vector3(-0.0013f, -0.00f, 0.002f), new Vector3(0f, 345f, 195f), new Vector3(0.006f, 0.006f, 0.006f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmLeft", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmLeftVoidSurvivor", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmRight,Bandit2", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmRight,Croco", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Lightning, "DisplayLightningArmRight", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [LIGHTNING] = Choose one, They use LimbMatchers
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.Lightning,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmLeft"),
                            childName = "Head",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmLeftVoidSurvivor"),
                            childName = "Head",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmRight,Bandit2"),
                            childName = "Head",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmRight,Croco"),
                            childName = "Head",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmRight"),
                            childName = "Head",
                            localPos = Vector3.zero,
                            localAngles = Vector3.zero,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });//no clue how to do this rob........ :(meru
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
            //equipmentRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Lightning", "???", "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //I have no clue what the model name is for the Capacitator, and both the Miner / Enforcer gits do some weird fucky shit.

            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(-0.00684F, 0.013F, -0.008F), new Vector3(90F, 0F, 0F), new Vector3(1F, 1F, 1F)));
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
localPos = new Vector3(0F, 0.0044F, 0F),
localAngles = new Vector3(0F, 180F, 180F),
localScale = new Vector3(0.002F, 0.002F, 0.0028F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfR",
localPos = new Vector3(0F, 0.00441F, 0F),
localAngles = new Vector3(0F, 180F, 180F),
localScale = new Vector3(0.002F, 0.002F, 0.0028F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });

            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.LunarPrimaryReplacement,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBirdEye"),
                            childName = "Head",
                            localPos = new Vector3(0, 0.004f, 0.001f),
                            localAngles = new Vector3(90, 180, 0),
                            localScale = new Vector3(0.003f, 0.003f, 0.003f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Modules.Assets.needlerPrefab,
                            childName = "Gun",
                            localPos = new Vector3(-0.001f, 0.001f, 0f),
                            localAngles = new Vector3(270, 0, 0),
                            localScale = new Vector3(0.02f, 0.02f, 0.02f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion RoR2Content
            #region DLC1Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "ShoulderR", new Vector3(-0.0008F, 0F, 0.0005F), new Vector3(0F, 340F, 90F), new Vector3(0.0008F, 0.0008F, 0.0008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "Gun", new Vector3(0F, 0.0022F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "Chest", new Vector3(-0.00141F, 0.00091F, -0.00212F), new Vector3(1.71386F, 357.6067F, 44.94984F), new Vector3(0.00092F, 0.00092F, 0.00092F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "Chest", new Vector3(-0.00134F, 0.00202F, -0.00144F), new Vector3(55.5871F, 45.15545F, 52.31505F), new Vector3(0.00231F, 0.00231F, 0.00231F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "Muzzle", new Vector3(0F, -0.0005F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.004F, 0.004F, 0.004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "Head", new Vector3(0F, 0.0027F, 0.0012F), new Vector3(330F, 0F, 0F), new Vector3(0.0036F, 0.004F, 0.004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "Chest", new Vector3(0.0015F, 0.003F, 0.002F), new Vector3(20.00001F, 90F, 80F), new Vector3(0.01F, 0.01F, 0.01F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "Pelvis", new Vector3(0.0016F, 0.008F, 0.006F), new Vector3(0F, 90F, 180F), new Vector3(0.008F, 0.008F, 0.008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "Chest", new Vector3(-0.003F, 0.00065F, 0F), new Vector3(2.13443F, 9.99999F, 20F), new Vector3(0.0008F, 0.0008F, 0.0008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "Chest", new Vector3(-0.0007F, 0.00342F, -0.00271F), new Vector3(0F, 180F, 56F), new Vector3(0.008F, 0.008F, 0.008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "Chest", new Vector3(0F, 0.003F, -0.003F), new Vector3(0F, 180F, 0F), new Vector3(0.003F, 0.003F, 0.003F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "ThighL", new Vector3(0F, 0.0023F, -0.0015F), new Vector3(16.48722F, 160.779F, 182.1724F), new Vector3(0.002F, 0.002F, 0.002F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "Pelvis", new Vector3(-0.0025F, 0.0005F, 0.002F), new Vector3(20.00001F, 315F, 180F), new Vector3(0.0008F, 0.0008F, 0.0008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "HandR", new Vector3(0F, 0F, -0.00024F), new Vector3(270F, 0F, 0F), new Vector3(0.00695F, 0.00695F, 0.00695F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "Chest", new Vector3(0F, 0.002F, 0.00218F), new Vector3(353.448F, 0.16994F, 0.10158F), new Vector3(0.00218F, 0.00218F, 0.00218F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "Chest", new Vector3(0.00133F, 0.00116F, -0.00285F), new Vector3(0.15432F, 2.31891F, 332.6128F), new Vector3(0.00088F, 0.00088F, 0.00088F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "ElbowR", new Vector3(0.00018F, -0.00039F, 0.00011F), new Vector3(344.4545F, 84.28957F, 166.0611F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "Pelvis", new Vector3(-0.00089F, 0.00065F, -0.00151F), new Vector3(0F, 194.6028F, 180F), new Vector3(0.00129F, 0.00129F, 0.00129F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "HandL", new Vector3(0.00047F, 0.00085F, -0.00004F), new Vector3(0F, 274.3412F, 0F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", new Vector3(0.00439F, 0.01899F, 0.00147F), new Vector3(85.58855F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "Pelvis", new Vector3(-0.00193F, 0.00028F, -0.00036F), new Vector3(354.6176F, 156.8903F, 195.675F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "ShoulderL", new Vector3(0F, 0F, 0F), new Vector3(5.71015F, 28.78572F, 237.3158F), new Vector3(0.00816F, 0.00816F, 0.00816F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "ShoulderR", new Vector3(0F, 0F, 0F), new Vector3(357.3668F, 97.95754F, 197.8274F), new Vector3(0.00816F, 0.00816F, 0.00816F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "Pelvis", new Vector3(-0.00001F, 0.00015F, 0.00187F), new Vector3(284.6354F, 349.0458F, 189.2882F), new Vector3(0.00304F, 0.00304F, 0.00304F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "Muzzle", new Vector3(0F, 0F, -0.00009F), new Vector3(0F, 0F, 0F), new Vector3(0.003F, 0.003F, 0.003F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.LunarSun, "DisplaySunHead", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.LunarSun, "DisplaySunHeadNeck", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [LUNARSUN] = Keep both.
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.LunarSun,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHead"),
                            childName = "Head",
localPos = new Vector3(0F, 0.0023F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01113F, 0.01113F, 0.01113F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "Head",
localPos = new Vector3(-0.00014F, 0.00039F, 0.00044F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01765F, 0.01765F, 0.01765F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            //Not an item display
            ////itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeapons, 
            // Unless you're a drone, you don't need these.
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsBoost, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsDisplay1, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.DroneWeaponsDisplay2, 
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", new Vector3(0.00579F, 0.02212F, 0F), new Vector3(343.9205F, 0F, 0.32262F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "Chest", new Vector3(0F, 0.00125F, -0.00261F), new Vector3(351.6874F, 0F, 0F), new Vector3(0.0016F, 0.0016F, 0.0016F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "Pelvis", new Vector3(-0.00076F, 0.00089F, -0.00171F), new Vector3(3.14155F, 93.79848F, 172.1136F), new Vector3(0.00169F, 0.00169F, 0.00169F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.VendingMachine, "DisplayVendingMachine", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.VendingMachine, "DisplayVendingMachine2", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [VENDINGMACHINE] = Choose one
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Equipment.VendingMachine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayVendingMachine2"),
                            childName = "Head",
localPos = new Vector3(-0.00117F, 0.00148F, 0.00158F),
localAngles = new Vector3(356.6433F, 148.7356F, 325.9175F),
localScale = new Vector3(0.00112F, 0.00112F, 0.00112F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            //IDRS NOTE: [BOSSHUNTER] = Keep both. DisplayBlunderbuss is the hat, and Dis 
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Equipment.BossHunter,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBlunderbuss"),
                            childName = "Head",
localPos = new Vector3(0.0048F, 0.00251F, -0.0008F),
localAngles = new Vector3(72.28313F, 0F, 0F),
localScale = new Vector3(1, 1, 1),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "Head",
localPos = new Vector3(0F, 0.00432F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.00751F, 0.00751F, 0.00751F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "Head", new Vector3(0F, 0.00432F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.00751F, 0.00751F, 0.00751F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "Chest", new Vector3(0.00129F, 0.00166F, 0.00197F), new Vector3(0F, 189.0614F, 0F), new Vector3(0.00155F, 0.00155F, 0.00155F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "Chest", new Vector3(0.00171F, 0.00095F, 0.00103F), new Vector3(48.88351F, 141.3087F, 113.1223F), new Vector3(0.005F, 0.005F, 0.005F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "Head", new Vector3(0F, 0.00223F, 0.00087F), new Vector3(65.59724F, 0F, 0F), new Vector3(0.00159F, 0.00159F, 0.00159F)));
            //from claymen git
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "Head", new Vector3(0F, 0.00343F, 0.00077F), new Vector3(0F, 0F, 0F), new Vector3(0.005F, 0.005F, 0.005F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "Axe", new Vector3(0.00011F, 0.00604F, 0.00026F), new Vector3(276.3906F, 73.82772F, 107.6121F), new Vector3(0.00692F, 0.00495F, 0.00629F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "Gun", new Vector3(0.00025F, 0.00164F, 0.00012F), new Vector3(0F, 355.8867F, 0F), new Vector3(0.00351F, 0.00381F, 0.00505F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", new Vector3(-0.00519F, 0.02043F, -0.00345F), new Vector3(-0.00001F, 180F, 180F), new Vector3(0.34151F, 0.34151F, 0.34151F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "Chest", new Vector3(0F, -0.00133F, 0.00171F), new Vector3(0F, 0F, 0F), new Vector3(0.00036F, 0.00036F, 0.00036F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "Pelvis", new Vector3(0.00162F, -0.00021F, 0.00148F), new Vector3(26.26909F, 8.96337F, 181.8389F), new Vector3(0.00138F, 0.00138F, 0.00138F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "Pelvis", new Vector3(-0.00228F, 0.00021F, -0.00096F), new Vector3(0F, 0F, 227.5042F), new Vector3(0.00043F, 0.00043F, 0.00043F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "Pelvis", new Vector3(0F, 0.00054F, 0.00217F), new Vector3(358.3619F, 180F, 180F), new Vector3(0.01F, 0.01F, 0.01F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "ThighL", new Vector3(-0.00196F, -0.0002F, -0.0006F), new Vector3(18.29921F, 325.5469F, 181.0322F), new Vector3(0.00159F, 0.00159F, 0.00159F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "Gun", new Vector3(-0.0006F, 0.0024F, -0.00002F), new Vector3(0.50947F, 1.13161F, 335.7652F), new Vector3(0.00058F, 0.00058F, 0.00058F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoid", "Chest", new Vector3(0F, 0.002F, 0.002F), new Vector3(0F, 0F, 0F), new Vector3(0.003F, 0.003F, 0.003F)));
            #endregion
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar,Eye", "Head", new Vector3(0F, 0.00232F, 0.00275F), new Vector3(0F, 0F, 0F), new Vector3(0.00241F, 0.00241F, 0.00241F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "Gun", new Vector3(-0.00282F, 0.00242F, -0.00002F), new Vector3(82.19386F, 139.1523F, 47.9679F), new Vector3(0.012F, 0.012F, 0.012F)));
            #endregion


            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            //itemDisplayRuleSet.GenerateRuntimeValues();

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            
        }


    }
}
