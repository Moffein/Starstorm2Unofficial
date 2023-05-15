using UnityEngine;
using RoR2;
using System.Collections.Generic;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Nemmando
{
    internal class NemmandoItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = NemmandoCore.instance.bodyPrefab;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            #region Display Rules
            //i leave the rest to you, swuff. godspeed.
            // great work~
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0, 0.0026f, 0.0018f), new Vector3(0, 0, 0), new Vector3(0.0038f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "Chest", new Vector3(-0.002f, 0.003f, 0.001f), new Vector3(37.00008f, 340f, 2.6726040f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Sword", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Gun", new Vector3(-0.003f, 0.002f, -0.001f), new Vector3(270f, 68.01749f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(0.002f, .008f, 0f), new Vector3(0f, 9.999999f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "CalfR", new Vector3(0f, 0.003f, -0.0006f), new Vector3(70f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.0008f)));
            //nemmando needs his calves rigged so i can attach the hoof to it, it looks freaky on his foot.
            // calves are rigged!
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.0007f, 0.0035f, -0.0025f), new Vector3(0f, 180f, 56f), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0f, 0.0025f, 0.0008f), new Vector3(330f, 0f, 0f), new Vector3(0.008f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "Chest", new Vector3(0.002f, 0.005f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.006f, 0.006f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Chest", new Vector3(-0.0012f, -0.0005f, 0.0023f), new Vector3(300f, 180f, 80.00002f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(0f, 0.0008f, -0.0018f), new Vector3(0f, 180f, 90f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ThighL", new Vector3(0.0005f, 0.002f, 0f), new Vector3(40.00003f, 14f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Sword", new Vector3(0f, 0.0025f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "ArmR", new Vector3(0f, 0f, -0.0005f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(340f, 0f, 0f), new Vector3(0.013f, 0.013f, 0.013f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(0.001f, 0f, 0f), new Vector3(70.00011f, 270f, 200f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "ArmL", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 180f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Sword", new Vector3(-0.0002f, 0.005f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Sword", new Vector3(-0.0002f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //hard light afterburner SHIT DON'T WORK NEED NEW HELPER AAAAAAAA
            // i did it manually, at the end, same for headstomp
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(0f, -0.0009f, -0.001f), new Vector3(0f, 0f, 0f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(70.00005f, 250f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "ThighR", new Vector3(0f, 0.003f, 0.0008f), new Vector3(90f, 186.0004f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(0f, 0.005f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "Sword", new Vector3(-0.0005f, 0.0005f, -0.0003f), new Vector3(270f, 255f, 0f), new Vector3(0.0002f, 0.0002f, 0.0002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Pelvis", new Vector3(-0.002f, 0f, 0.0015f), new Vector3(30.00001f, 2700f, 1800f), new Vector3(0.002f, 0.002f, 0.002f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "FootR", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            //same as hard light afterburner
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "ThighR", new Vector3(-0.0015f, 0f, 0f), new Vector3(90f, 90f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "Chest", new Vector3(-0.0015f, 0.001f, 0.0014f), new Vector3(0f, 90f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(0.001f, 0.003f, 0), new Vector3(0, 90, 0), new Vector3(0.01f, 0.01f, 0.01f)));
            //might need this also? in miner's code there's two... will find out i suppose
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Pelvis", new Vector3(0.0008f, 0f, 0.0016f), new Vector3(0f, 30f, 180f), new Vector3(0.008f, 0.008f, 0.008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "ThighL", new Vector3(0.0014f, 0.002f, 0f), new Vector3(90f, 270f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(-0.0007f, 0.0034f, 0f), new Vector3(330f, 45f, 9.8585290f), new Vector3(0.005f, 0.008f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(0f, 0.005f, -0.0023f), new Vector3(0f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "ThighR", new Vector3(-0.0013f, 0.004f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.006f, 0.006f, 0.006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "ThighL", new Vector3(0.0008f, 0.0024f, -0.0017f), new Vector3(90f, 230f, 0f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Pelvis", new Vector3(0.0015f, 0f, 0.0012f), new Vector3(90f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Chest", new Vector3(0.001f, 0.002f, 0.002f), new Vector3(270f, 161f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(0.0006f, 0.0023f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Chest", new Vector3(0.0006f, 0.001f, 0.002f), new Vector3(-2.134434f, 20f, 15f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Pelvis", new Vector3(-0.0015f, 0f, 0.0012f), new Vector3(85f, 180f, 180f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Gun", new Vector3(-0.00017f, -0.0004f, -0.001f), new Vector3(0f, 240f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(0f, 0f, 11f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.0018f, 0.003f, 0.0016f), new Vector3(0f, 0f, 90f), new Vector3(0.016f, 0.016f, 0.016f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "Pelvis", new Vector3(0f, -0.0006f, 0.0015f), new Vector3(270f, 345f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Pelvis", new Vector3(-0.0008f, -0.0038f, 0f), new Vector3(0f, 180f, 2.5761611f), new Vector3(0.004f, 0.004f, 0.004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(0.0007f, 0.005f, -0.0024f), new Vector3(0f, 180f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0f, 0.0018f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "Head", new Vector3(-0.0008f, 0.0018f, -0.0017f), new Vector3(90f, 180f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(-0.0005f, -0.001f, 0.0017f), new Vector3(0f, 90f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.001f, 0.003f, 0), new Vector3(0, 90, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            //again, this might need the duplication thing, i've no fucking clue LOL rob save me
            // i did rejuv rack for you, the mirrored display rule works. just do the display rule for one and it'll do the other one automatically. easy as fuck
            // should work for transcendence, nkuhanas, elite horns and whatever the fuck else uses two
            //thanks man
            //ily
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.0004f, 0.002f, 0.002f), new Vector3(0f, 0f, 20f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "ShoulderL", new Vector3(0.0013f, 0f, 0f), new Vector3(15f, 90f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.0006f, 0.0023f, 0.0023f), new Vector3(90f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "ShoulderL", new Vector3(0.0015f, 0.0007f, 0f), new Vector3(-1.024528f, 90f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "Head", new Vector3(0f, 0.0035f, -0.001f), new Vector3(0f, 270f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "ArmL", new Vector3(0.0003f, 0f, 0f), new Vector3(270f, 160f, 0f), new Vector3(0.005f, 0.005f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.01f, 0.013f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Chest", new Vector3(0f, 0f, -0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Pelvis", new Vector3(0.0015f, 0.001f, 0f), new Vector3(0f, 180f, 270f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(0.001f, 0.0052f, -0.001f), new Vector3(270f, 0f, 0f), new Vector3(0.0006f, 0.0006f, 0.0006f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Chest", new Vector3(0.0f, 0.0025f, 0.0025f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "HandR", new Vector3(0f, 0.0005f, 0.0002f), new Vector3(90f, 180f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Pelvis", new Vector3(0.0015f, -0.0004f, -0.0014f), new Vector3(0f, 0f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "Sword", new Vector3(-0.0006f, 0.012f, -0.0005f), new Vector3(90f, 345f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Pelvis", new Vector3(0.0023f, 0f, 0f), new Vector3(0f, 0f, 180f), new Vector3(0.0025f, 0.0025f, 0.0025f)));
            //grow up wicked ring.... nope! you're not even in the game!
            // get fucked!
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "ThighL", new Vector3(0.0003f, 0.002f, 0.0009f), new Vector3(270f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "Chest", new Vector3(-0.002f, 0.003f, 0.0008f), new Vector3(0f, 180f, 90f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Sword", new Vector3(0f, 0.0023f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            //what the fuck is this item? i'm going to kill someone at hopoo holy shit name stuff better
            //trasncendence are you kidding me... jesus christ... i feel like an idiot
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Sword", new Vector3(0.0004f, 0.01f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullCrown", "Head", new Vector3(0f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.006f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Pelvis", new Vector3(0.0015f, -0.0013f, 0.0017f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Sword", new Vector3(0f, 0.008f, 0.001f), new Vector3(50.000001f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.001f, 0.005f, -0.0023f), new Vector3(15f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, "DisplayToothMeshLarge", "Head", new Vector3(0.0006f, 0.004f, -0.0008f), new Vector3(1.7075470f, 13f, 65.00001f), new Vector3(0.03f, 0.03f, 0.03f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Sword", new Vector3(0f, 0.013f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Sword", new Vector3(0f, 0.01f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "ThighR", new Vector3(-0.0013f, 0.001f, 0f), new Vector3(0f, 270f, 0f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "Head", new Vector3(0f, 0.0043f, 0f), new Vector3(0f, 90f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            //leaving off at line 1953 for now - laser turbine, whatever that is. copy pasting code is boring as fuck. hopoo, i kneel.
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Chest", new Vector3(0f, 0.0035f, -0.0025f), new Vector3(15f, 0f, 0f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Sword", new Vector3(-0.0007f, 0.015f, 0f), new Vector3(275f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "ThighL", new Vector3(0.0017f, 0.002f, 0.0008f), new Vector3(0f, 45f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.0005f, 0.0024f, 0.0025f), new Vector3(0f, 0f, 45f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "ThighR", new Vector3(-0.00085f, 0.0033f, 0f), new Vector3(30f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "HandR", new Vector3(0f, 0.0017f, 0.0009f), new Vector3(18f, 180f, 180f), new Vector3(0.0008f, 0.0008f, 0.0008f)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Chest", new Vector3(0.002f, 0.004f, 0f), new Vector3(70f, 180f, 180f), new Vector3(0.01f, 0.01f, 0.01f)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "Chest", new Vector3(-0.0003f, 0.0007f, -0.0016f), new Vector3(0f, 35f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(-0.001f, 0.002f, 0f), new Vector3(0f, 0f, 0f), new Vector3(-0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0f, 0.004f, 0f), new Vector3(90f, 180f, 0f), new Vector3(0.0003f, 0.0003f, 0.0003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0f, 0.003f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Head", new Vector3(0f, 0.003f, 0.003f), new Vector3(0f, 0f, 0f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0f, 0.0045f, 0f), new Vector3(270f, 0f, 0f), new Vector3(0.0005f, 0.0005f, 0.0005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Pelvis", new Vector3(-0.0013f, -0.001f, 0.0015f), new Vector3(0f, 345f, 195f), new Vector3(0.006f, 0.006f, 0.006f)));

            //TODO
            //RoR2Content.Equipment.Lightning doesn't match any of the childlocators so its crumpled and weird
            //no clue how to do this rob........ :(meru
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Pelvis", new Vector3(0.002f, 0f, -0.0015f), new Vector3(0f, 0f, 180f), new Vector3(0.0004f, 0.0004f, 0.0004f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Pelvis", new Vector3(-0.0012f, 0.001f, -0.0015f), new Vector3(0f, 30f, 180f), new Vector3(0.005f, 0.005f, 0.005f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Head", new Vector3(0f, 0.0022f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0.019f, 0.015f, 0.012f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0f, 0.0034f, -0.003f), new Vector3(0f, 90f, 15f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Pelvis", new Vector3(0.0018f, 0f, -0.001f), new Vector3(85f, 0f, 0f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(0f, 0.0015f, 0.002f), new Vector3(0f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Pelvis", new Vector3(0.0015f, -0.001f, 0.0014f), new Vector3(0f, 30f, 180f), new Vector3(0f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Pelvis", new Vector3(-0.002f, -0.0008f, 0.001f), new Vector3(0f, 30f, 170f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Chest", new Vector3(0.0015f, 0.0045f, 0f), new Vector3(280f, 0f, 15f), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Sword", new Vector3(-0.0002f, 0.004f, 0f), new Vector3(90f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", new Vector3(0f, 0.004f, -0.0028f), new Vector3(30f, 0f, 0f), new Vector3(0.0015f, 0.0015f, 0.0015f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(0f, 0.002f, 0.002f), new Vector3(15f, 0f, 0f), new Vector3(0.001f, 0.001f, 0.001f)));

            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(-0.004f, 0, -0.01f), new Vector3(0, 0, 0), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", new Vector3(0.007f, 0, -0.01f), new Vector3(270f, 0, 0), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", new Vector3(0.007f, -0.005f, -0.005f), new Vector3(0, 0, 0), new Vector3(0.1f, 0.1f, 0.1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", new Vector3(0.003f, 0f, -0.013f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", new Vector3(0.003f, 0f, -0.013f), new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(0, 0.0028f, 0), new Vector3(0, 0, 0), new Vector3(0.002f, 0.002f, 0.002f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(0.0028f, 0.006f, 0), new Vector3(15f, 90f, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Chest", new Vector3(-0.0017f, 0.004f, 0), new Vector3(0, 0, 10f), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(0, 0.003f, -0.002f), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", new Vector3(0, -0, 0), new Vector3(0, 0, 0), new Vector3(0.2f, 0.2f, 0.2f)));
            //equipmentRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Lightning", capacitorPrefab, "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //I have no clue what the model name is for the Capacitator, and both the Miner / Enforcer gits do some weird fucky shit.

            //just lazy to write helpers for these two use cases
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
                            localAngles = new Vector3(0, 0, 180),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "CalfR",
                            localPos = new Vector3(0, 0.004f, 0f),
                            localAngles = new Vector3(0, 0, 180),
                            localScale = new Vector3(0.002f, 0.002f, 0.002f),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "Chest", new Vector3(0.002F, 0.005F, 0F), new Vector3(0F, 180F, 0F), new Vector3(0.0008F, 0.0008F, 0.0008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "Sword", new Vector3(0F, 0.0025F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.005F, 0.005F, 0.005F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "Pelvis", new Vector3(-0.00126F, -0.00068F, -0.00115F), new Vector3(6.69249F, 180F, 163.5702F), new Vector3(0.00101F, 0.00101F, 0.00101F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "Pelvis", new Vector3(-0.00109F, -0.00112F, 0.00202F), new Vector3(10.59845F, 342.7056F, 176.7224F), new Vector3(0.001F, 0.001F, 0.001F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "Chest", new Vector3(-0.0012F, -0.0005F, 0.0023F), new Vector3(300.4234F, 186.2263F, 164.7107F), new Vector3(0.003F, 0.003F, 0.003F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "Head", new Vector3(0F, 0.0026F, 0.0018F), new Vector3(0F, 0F, 0F), new Vector3(0.0038F, 0.004F, 0.004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "Chest", new Vector3(0.0018F, 0.003F, 0.0016F), new Vector3(0F, 0F, 90F), new Vector3(0.016F, 0.016F, 0.016F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "Pelvis", new Vector3(-0.00064F, -0.00388F, 0F), new Vector3(0F, 180F, 0F), new Vector3(0.004F, 0.004F, 0.004F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "Chest", new Vector3(0.002F, 0.008F, 0F), new Vector3(0F, 10F, 0F), new Vector3(0.001F, 0.001F, 0.001F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "Chest", new Vector3(-0.0007F, 0.0035F, -0.0025F), new Vector3(0F, 180F, 56F), new Vector3(0.007F, 0.007F, 0.007F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "Chest", new Vector3(-0.00114F, 0.00445F, -0.00205F), new Vector3(345.0042F, 178.6128F, 0.359F), new Vector3(0.0015F, 0.0015F, 0.0015F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "Chest", new Vector3(-0.0008F, 0.001F, 0.0014F), new Vector3(0F, 0F, 90F), new Vector3(0.002F, 0.002F, 0.002F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "Pelvis", new Vector3(0.0015F, -0.0004F, -0.0014F), new Vector3(0F, 0F, 180F), new Vector3(0.0008F, 0.0008F, 0.0008F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "HandR", new Vector3(0.00026F, -0.00047F, 0.00037F), new Vector3(64.89851F, 22.99744F, 18.54056F), new Vector3(0.00596F, 0.00596F, 0.00596F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "Chest", new Vector3(0F, 0.00254F, 0.00228F), new Vector3(0F, 0F, 0F), new Vector3(0.00166F, 0.00166F, 0.00166F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "Chest", new Vector3(0F, 0.00376F, -0.00232F), new Vector3(3.02057F, 356.9576F, 314.7533F), new Vector3(0.00074F, 0.00074F, 0.00074F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "Pelvis", new Vector3(0F, 0.00003F, 0.0001F), new Vector3(6.93536F, 180F, 180F), new Vector3(0.00835F, 0.00502F, 0.00815F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "Pelvis", new Vector3(0.00095F, -0.00003F, 0.00086F), new Vector3(10.09583F, 180F, 180F), new Vector3(0.001F, 0.001F, 0.001F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "HandL", new Vector3(0F, 0.00078F, 0.00038F), new Vector3(0F, 0F, 0F), new Vector3(0.00234F, 0.00234F, 0.00234F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", new Vector3(0.00301F, -0.00211F, -0.00916F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "Pelvis", new Vector3(-0.00204F, -0.00045F, -0.00046F), new Vector3(0.19218F, 355.943F, 184.4624F), new Vector3(0.0043F, 0.0043F, 0.0043F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "ShoulderL", new Vector3(0.00114F, 0.0006F, 0.00005F), new Vector3(28.66669F, 333.8164F, 240.7946F), new Vector3(0.00583F, 0.00583F, 0.00583F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "ShoulderR", new Vector3(-0.00104F, 0.00046F, 0.00021F), new Vector3(349.0207F, 199.2222F, 190.1553F), new Vector3(0.00583F, 0.00583F, 0.00795F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "Pelvis", new Vector3(-0.00151F, -0.00063F, -0.00071F), new Vector3(286.5106F, 277.7527F, 137.3893F), new Vector3(0.00234F, 0.00234F, 0.00234F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "Sword", new Vector3(-0.0002F, 0.00445F, 0F), new Vector3(90F, 0F, 0F), new Vector3(0.002F, 0.002F, 0.002F)));
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
localPos = new Vector3(0F, 0.0022F, 0.00009F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01083F, 0.01083F, 0.01083F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "HeadCenter",
localPos = new Vector3(-0.00033F, 0.00073F, 0.00034F),
localAngles = new Vector3(351.1362F, 0F, 0F),
localScale = new Vector3(0.02015F, 0.02015F, 0.02015F),
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
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", new Vector3(0.0006F, -0.00002F, -0.01277F), new Vector3(270F, 0.32262F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "Chest", new Vector3(0F, -0.00017F, 0.00139F), new Vector3(0F, 0F, 0F), new Vector3(0.00054F, 0.00054F, 0.00054F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "Pelvis", new Vector3(-0.00107F, 0.00036F, -0.00142F), new Vector3(10.22982F, 87.78105F, 153.947F), new Vector3(0.00163F, 0.00163F, 0.00163F)));
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
localPos = new Vector3(-0.00196F, 0.002F, 0.00116F),
localAngles = new Vector3(350.9883F, 108.2437F, 0F),
localScale = new Vector3(0.00132F, 0.00132F, 0.00132F),
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
                            childName = "Gun",
localPos = new Vector3(-0.00001F, 0.00124F, 0F),
localAngles = new Vector3(89.44468F, 89.9985F, 204.323F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "Head",
localPos = new Vector3(0F, 0.00373F, -0.00096F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.01089F, 0.01089F, 0.01089F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "Head", new Vector3(0F, 0.00373F, -0.00096F), new Vector3(0F, 0F, 0F), new Vector3(0.01089F, 0.01089F, 0.01089F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "Sword", new Vector3(-0.00028F, 0.00631F, 0.00032F), new Vector3(87.2934F, 180F, 180F), new Vector3(0.00135F, 0.00135F, 0.00135F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "Chest", new Vector3(-0.00001F, 0.00216F, 0.00211F), new Vector3(71.89886F, 180F, 180F), new Vector3(0.01217F, 0.01217F, 0.01217F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "Head", new Vector3(0F, 0.00244F, 0.0015F), new Vector3(70.64544F, 0F, 0F), new Vector3(0.00172F, 0.00172F, 0.00172F)));
            //from claymen git
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "Head", new Vector3(0F, 0.00359F, 0.00057F), new Vector3(0F, 0F, 0F), new Vector3(0.00574F, 0.00574F, 0.00574F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "Sword", new Vector3(-0.00005F, 0.00008F, 0F), new Vector3(86.5349F, 261.7535F, 261.7387F), new Vector3(0.001F, 0.001F, 0.001F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "HandR", new Vector3(0.00141F, -0.0004F, 0.00049F), new Vector3(333.2811F, 351.5399F, 290.5176F), new Vector3(0.00498F, 0.00291F, 0.00619F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", new Vector3(-0.00835F, 0.0003F, -0.00748F), new Vector3(0F, 0F, 0F), new Vector3(0.34151F, 0.34151F, 0.34151F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "Chest", new Vector3(0F, 0.00038F, -0.00145F), new Vector3(0F, 0F, 0F), new Vector3(0.00037F, 0.00037F, 0.00037F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "Pelvis", new Vector3(0.00053F, -0.00106F, 0.00151F), new Vector3(355.1072F, 2.28489F, 180.1681F), new Vector3(0.00171F, 0.00171F, 0.00171F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "Pelvis", new Vector3(-0.00165F, -0.00058F, -0.00129F), new Vector3(342.3314F, 182.8015F, 155.542F), new Vector3(0.00042F, 0.00042F, 0.00042F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "Chest", new Vector3(0.00003F, 0.00004F, -0.00055F), new Vector3(0F, 0F, 0F), new Vector3(0.01F, 0.01F, 0.01F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "ThighL", new Vector3(0.00165F, 0.00162F, 0.00004F), new Vector3(348.787F, 58.97269F, 187.1566F), new Vector3(0.00252F, 0.00252F, 0.00252F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "Gun", new Vector3(-0.00087F, 0.00192F, -0.00047F), new Vector3(0F, 335.0365F, 358.1814F), new Vector3(0.00075F, 0.00075F, 0.00075F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoid", "Chest", new Vector3(0F, 0.0025F, 0.0025F), new Vector3(0F, 0F, 0F), new Vector3(0.003F, 0.003F, 0.003F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar,Eye", "Head", new Vector3(0F, 0.00232F, 0.00275F), new Vector3(0F, 0F, 0F), new Vector3(0.00241F, 0.00241F, 0.00241F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "Sword", new Vector3(-0.00047F, 0.0145F, 0.00052F), new Vector3(344.3029F, 169.2036F, 190.3225F), new Vector3(0.01476F, 0.01476F, 0.01476F)));
            #endregion


            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            //itemDisplayRuleSet.GenerateRuntimeValues();

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }


    }
}
