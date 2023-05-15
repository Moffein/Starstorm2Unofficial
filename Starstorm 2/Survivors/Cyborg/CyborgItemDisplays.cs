using UnityEngine;
using RoR2;
using System.Collections.Generic;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Cyborg
{
    public static class CyborgItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = CyborgCore.cybPrefab;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            // Leaving those for swuff when he does the item displays properly.
            //
            // Head : head_end
            // HeadCenter : head
            // Pelvis : Waist
            // Chest : Chest
            // ShoulderL : Upperarm.L
            // ShoulderR : Upperarm.R
            // ElbowL : Lowerarm.L
            // ElbowR : Lowerarm.R
            // HandL : Lowerarm.L_end
            // HandR : Lowerarm.R_end
            // ThighL : Upperleg.L
            // ThighR : Upperleg.R
            // CalfL : lowerleg.L
            // CalfR : lowerleg.R
            // FootL : lowerleg.L_end
            // FootR : lowerleg.R_end
            #region Display Rules
            #region RoR2Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "head_end", new Vector3(0F, 0.27F, 0.12F), new Vector3(312.4455F, 358.7655F, 1.55823F), new Vector3(0.36F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "Upperarm.L", new Vector3(0.01744F, 0.16661F, 0.01049F), new Vector3(354.9183F, 305.1207F, 230.4652F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Lowerarm.R_end", new Vector3(-0.0213F, 0.06416F, -0.01324F), new Vector3(0F, 0F, 0F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Lowerarm.R_end", new Vector3(-0.02634F, 0.07575F, -0.01386F), new Vector3(2.18114F, 0.41645F, 0.02294F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Chest", new Vector3(-0.30034F, 0.02553F, -0.0019F), new Vector3(2.13443F, 9.99999F, 20F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.001f, 0.0035f, -0f), new Vector3(0f, 45f, 45f), new Vector3(0.01f, 0.01f, 0.01f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "lowerleg.R", new Vector3(0.00986F, 0.38462F, 0.10298F), new Vector3(75.70033F, 184.4452F, 359.1921F), new Vector3(0.1F, 0.12F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.07F, 0.35F, -0.27F), new Vector3(346.9894F, 188.9663F, 54.97551F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "head_end", new Vector3(0.00414F, 0.26565F, 0.15424F), new Vector3(330F, 0F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "Upperarm.R", new Vector3(-0.14767F, -0.11289F, -0.00628F), new Vector3(0F, 340F, 90F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "head_end", new Vector3(0F, 0.35F, 0.05F), new Vector3(310F, 0F, 0F), new Vector3(0.6F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Lowerarm.L_end", new Vector3(0F, -0.05F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Chest", new Vector3(-0.00118F, 0.20073F, -0.30663F), new Vector3(270F, 90F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "Lowerarm.R", new Vector3(0F, 0.07F, 0.03F), new Vector3(90F, 358F, 0F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Lowerarm.R_end", new Vector3(-0.01413F, 0.05694F, 0.003F), new Vector3(348.7779F, 1.89384F, 1.55342F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "Lowerarm.L", new Vector3(0.0227F, 0.2407F, -0.06751F), new Vector3(90F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "head_end", new Vector3(0.02103F, 0.03829F, -0.0656F), new Vector3(325.3311F, 359.1125F, 1.43278F), new Vector3(1.82653F, 1.82653F, 1.82653F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "Upperarm.L", new Vector3(0.10319F, 0.04916F, -0.00681F), new Vector3(64.6027F, 96.66527F, 9.01371F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "Lowerarm.R", new Vector3(-0.10469F, 0.34879F, 0.04276F), new Vector3(6.40999F, 330F, 0F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Lowerarm.L_end", new Vector3(0.015F, -0.00026F, -0.00709F), new Vector3(89.29308F, 180F, 180F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Lowerarm.R_end", new Vector3(0.015F, -0.00026F, -0.00709F), new Vector3(89.29308F, 180F, 180F), new Vector3(0.3F, 0.3F, 0.3F)));
            //itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerUpperarm.Ring", "Chest", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "head_end", new Vector3(0F, -0.21348F, -0.1F), new Vector3(0F, 0F, 0F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Lowerarm.L_end", new Vector3(0.01818F, 0.6F, 0F), new Vector3(270F, 0F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "Upperleg.R", new Vector3(-0.00551F, 0.29786F, -0.11933F), new Vector3(90F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "Upperarm.R", new Vector3(0F, -0.15612F, 0F), new Vector3(0F, 180F, 180F), new Vector3(0.03F, 0.03F, 0.03F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Waist", new Vector3(-0.25759F, 0.00044F, 0.10041F), new Vector3(310F, 30F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("FallBoots", "DisplayGrabBoots", "lowerleg.R_end", new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, 0f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "head_end", new Vector3(0F, 0.15972F, 0.19263F), new Vector3(270.934F, 0F, 0F), new Vector3(0.14829F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "Upperleg.L", new Vector3(0.00143F, 0.22947F, -0.16788F), new Vector3(65.82771F, 173.486F, 173.7576F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "head_end", new Vector3(0.118F, 0.17421F, -0.00001F), new Vector3(0F, 90F, 0F), new Vector3(0.8F, 0.8F, 0.8F)));
            //https://discord.com/channels/753709254598328400/755273415719387146/793188879557328916
            //https://discord.com/channels/753709254598328400/757459787117101096/785685039177793547
            //https://discord.com/channels/753709254598328400/757459787117101096/785641674977706034
            //this could've been done once but now must be done twice
            //:damnation:
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Chest", new Vector3(0F, -0.17726F, 0.23F), new Vector3(0F, 20F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "Waist", new Vector3(0.24943F, -0.0122F, -0.00248F), new Vector3(77.91518F, 125.5138F, 239.753F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(0.0013F, 0.2276F, -0.02399F), new Vector3(45.00002F, 80F, 0F), new Vector3(0.67429F, 0.8F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(-0.13F, 0.35F, -0.3F), new Vector3(0F, 0F, 4F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "Waist", new Vector3(-0.22156F, 0.0879F, 0.0992F), new Vector3(71.76019F, 322.3472F, 303.6967F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "Upperleg.R", new Vector3(-0.04749F, 0.27383F, -0.12237F), new Vector3(81.58548F, 157.0071F, 324.6693F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Chest", new Vector3(-0.20218F, 0.06408F, -0.22863F), new Vector3(270.0839F, 18.04952F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Waist", new Vector3(0.25946F, -0.16925F, -0.10237F), new Vector3(0F, 0F, 260F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(0F, 0.00311F, 0.18F), new Vector3(0F, 0F, 0F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Waist", new Vector3(-0.29934F, 0.06663F, 0.09816F), new Vector3(16.68907F, 13.7035F, 307.2074F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Waist", new Vector3(0.28F, -0.04112F, -0.11235F), new Vector3(85.00005F, 180F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Lowerarm.R_end", new Vector3(-0.02259F, 0.11612F, -0.00162F), new Vector3(15F, 270F, 0F), new Vector3(0.06F, 0.06F, 0.06F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Waist", new Vector3(-0.26061F, 0.12153F, -0.14269F), new Vector3(343.4653F, 295.4415F, 9.41367F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Chest", new Vector3(0.14725F, 0.01318F, 0.19875F), new Vector3(13.457F, 124.6577F, 98.06718F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "Upperarm.R", new Vector3(0F, 0.15F, -0.1F), new Vector3(270F, 42.70859F, 0F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Waist", new Vector3(0.15904F, 0.67785F, 0.59582F), new Vector3(0.0273F, 89.20187F, 179.9936F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(0F, 0.3F, -0.3F), new Vector3(0F, 180F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "head_end", new Vector3(0F, 0.09876F, 0.02713F), new Vector3(345F, 0F, 0F), new Vector3(0.3F, 0.4F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseFlower", "head_end", new Vector3(0.19538F, 0.20106F, -0.10442F), new Vector3(90F, 100F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Chest", new Vector3(0F, -0.08421F, 0.2014F), new Vector3(0F, 90F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "head_end", new Vector3(0.001F, 0.002F, -0.0005F), new Vector3(0F, 90F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            //at around this point i scrolled down to check how many more items i had to do then lay my head on my desk at the realization
            //remember when i mentioned multiple times to wait for classic model
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldheart", "Chest", new Vector3(-0.06358F, -0.10085F, 0.17992F), new Vector3(359.7715F, 347.8719F, 19.2114F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "Upperarm.L", new Vector3(0.04F, 0.15F, 0F), new Vector3(0F, 90F, 180F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.10383F, 0.22287F, 0.27624F), new Vector3(75.47183F, 2.16902F, 2.03127F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Chest", new Vector3(0.2264F, -0.06811F, 0.05447F), new Vector3(0F, 70F, 90F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "lowerleg.R", new Vector3(0F, 0.2F, 0.21F), new Vector3(0F, 270F, 280F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "Upperarm.L", new Vector3(-0.05F, 0F, 0F), new Vector3(270F, 0F, 0F), new Vector3(0.8F, 0.6F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "head_end", new Vector3(0.00822F, -0.10221F, 0.147F), new Vector3(90F, 180F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "head_end", new Vector3(-0.0005f, 0.0004f, -0.00f), new Vector3(310, 0, 0), new Vector3(0.2f, 0.2f, 0.2f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Waist", new Vector3(0.3F, 0.05F, 0.1F), new Vector3(0F, 0F, 270F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(0.17996F, 0.42699F, -0.2002F), new Vector3(290F, 180F, 180F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Chest", new Vector3(-0.00016F, 0.06758F, 0.22978F), new Vector3(0F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "Lowerarm.R_end", new Vector3(-0.1F, 0.05F, 0F), new Vector3(300F, 0F, 270F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Waist", new Vector3(-0.25F, 0.05F, 0.2F), new Vector3(346.3581F, 223.5623F, 7.72966F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "lowerleg.L", new Vector3(0F, 0.2F, -0.1F), new Vector3(326.8031F, 137.6122F, 66.41169F), new Vector3(0.06F, 0.06F, 0.06F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Waist", new Vector3(0.2328F, -0.0191F, -0.11869F), new Vector3(7.11854F, 20.06703F, 179.4315F), new Vector3(0.6F, 0.6F, 0.6F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("CooldownOnCrit", "DisplaySkull", "Lowerarm.R_end", new Vector3(-0.001f, 0.008f, 0), new Vector3(90, 180, 0), new Vector3(0.005f, 0.005f, 0.005f)));
            //commenting out wicked ring until someone decides to actually make a full mod for it (i don't count pikmin88's since he refuses to add an icon for it)
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "Lowerarm.R", new Vector3(-0.00789F, 0.1415F, 0.09656F), new Vector3(90F, 180F, 0F), new Vector3(0.3F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "Upperarm.R", new Vector3(0.03626F, 0.47549F, -0.06284F), new Vector3(280F, 180F, 0F), new Vector3(0.2F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Chest", new Vector3(-0.0022F, 0.01466F, -0.21238F), new Vector3(270F, 0F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "head_end", new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0.5f, 0.5f, 0.5f)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Waist", new Vector3(-0.26F, 0F, 0F), new Vector3(90F, 90F, 0F), new Vector3(1.2F, 1.2F, 1.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullCrown", "head_end", new Vector3(0.00386F, 0.05454F, -0.0082F), new Vector3(320F, 0F, 0F), new Vector3(0.71195F, 0.28478F, 0.28478F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Waist", new Vector3(0F, 0F, 0.23F), new Vector3(0F, 0F, 180F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Chest", new Vector3(-0.00031F, 0.22606F, 0.2602F), new Vector3(330F, 0F, 0F), new Vector3(0.16F, 0.16F, 0.16F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, "DisplayToothMeshLarge", "Chest", new Vector3(-0.32488F, 0.50417F, -0.09858F), new Vector3(348.6591F, 343.4745F, 46.56552F), new Vector3(7F, 7F, 7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Lowerarm.L_end", new Vector3(0F, 0F, 0F), new Vector3(90F, 0F, 0F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Lowerarm.L_end", new Vector3(0F, -0.1233998F, 0F), new Vector3(90F, 0F, 0F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "Waist", new Vector3(0F, 0F, 0.2F), new Vector3(0F, 0F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "Upperarm.L", new Vector3(0.10489F, 0.07537F, -0.11045F), new Vector3(0F, 35F, 250F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Chest", new Vector3(0F, 0.26169F, -0.33093F), new Vector3(15F, 0F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Lowerarm.R_end", new Vector3(-0.03296F, 0.15147F, -0.0153F), new Vector3(273.0413F, 245.4623F, 20.12859F), new Vector3(0.05165F, 0.05165F, 0.05165F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "Upperleg.L", new Vector3(0.06034F, 0.30139F, -0.10304F), new Vector3(0.61974F, 278.9955F, 183.0156F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Chest", new Vector3(-0.05F, 0.24F, 0.26963F), new Vector3(0F, 0F, 45F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "Upperleg.R", new Vector3(-0.11112F, 0.33026F, 0.00168F), new Vector3(30F, 180F, 180F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "Lowerarm.R_end", new Vector3(-0.16208F, -0.11779F, 0.02191F), new Vector3(352.4889F, 78.21933F, 91.00771F), new Vector3(0.04F, 0.05F, 0.05F)));
            //itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule("UtilitySkillMagazine", "DisplayAfterburnerUpperarm.Ring", "Chest", new Vector3(0.002f, 0.003f, 0f), new Vector3(70f, 180f, 190f), new Vector3(0.01f, 0.01f, 0.01f)));
            //set up thing to make this work (needs mirror that mirrors z - rotation)
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "Upperleg.R", new Vector3(0.00064F, 0.2691F, -0.02684F), new Vector3(0F, 0F, 180F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "head_end", new Vector3(-0.1F, 0.2F, 0F), new Vector3(0F, 0F, 0F), new Vector3(-0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "head_end", new Vector3(-0.00062F, 0.20347F, 0.1749F), new Vector3(333.4247F, 0F, 0F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "head_end", new Vector3(0F, 0.4F, 0F), new Vector3(283.8257F, 180F, 180F), new Vector3(0.03F, 0.03F, 0.03F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "head_end", new Vector3(0F, 0.3F, 0F), new Vector3(270F, 0F, 0F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "head", new Vector3(0.01268F, 0.40631F, 0.31288F), new Vector3(0F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "head_end", new Vector3(0F, 0.38338F, -0.00001F), new Vector3(90F, 180F, 0F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Waist", new Vector3(-0.13F, 0F, 0.2F), new Vector3(5.86697F, 349.9574F, 6.95035F), new Vector3(0.6F, 0.6F, 0.6F)));

            //TODO
            //RoR2Content.Equipment.Lightning doesn't match any of the childlocators so its crumpled and weird
            //no clue how to do this rob........ :(meru
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Waist", new Vector3(0.14999F, -0.01687F, 0.17782F), new Vector3(12.72614F, 330.7571F, 332.6467F), new Vector3(0.05F, 0.05F, 0.05F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Waist", new Vector3(-0.16196F, -0.0382F, -0.20261F), new Vector3(3.21876F, 28.06466F, 358.7917F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "head_end", new Vector3(0.00341F, 0.03218F, 0.01353F), new Vector3(0F, 0F, 0F), new Vector3(1.9F, 1.8F, 1.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0F, 0.34F, -0.3F), new Vector3(0F, 90F, 15F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Waist", new Vector3(0.1989F, -0.06035F, -0.14481F), new Vector3(282.1245F, 265.0938F, 95.15285F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(-0.00094F, -0.08337F, -0.21559F), new Vector3(0F, 180F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Waist", new Vector3(0.15035F, -0.10178F, 0.19175F), new Vector3(0.85334F, 23.0247F, 354.3975F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Waist", new Vector3(-0.2756F, 0.05027F, 0.10025F), new Vector3(0.52403F, 286.3992F, 6.34143F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Chest", new Vector3(0.23845F, 0.37449F, -0.00154F), new Vector3(297.6407F, 70.9798F, 303.4817F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Waist", new Vector3(-0.00054F, 0.00275F, -0.28021F), new Vector3(0.06797F, 175.9081F, 355.1628F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "head_end", new Vector3(0.0004F, 0.3781F, -0.27842F), new Vector3(30F, 0F, 0F), new Vector3(0.15F, 0.15F, 0.15F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(-0.0025F, -0.14539F, 0.21546F), new Vector3(15F, 0F, 0F), new Vector3(0.06F, 0.06F, 0.06F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(-0.003F, 0.40561F, -0.28331F), new Vector3(70.00002F, 180F, 0F), new Vector3(0.7F, 0.7F, 0.7F)));
            //equipmentRules.Add(ItemDisplayCore.CreateGenericDisplayRule("Lightning", "???", "Chest", new Vector3(0f, 0.004f, -0.002f), new Vector3(70f, 180f, 0f), new Vector3(0.007f, 0.007f, 0.007f)));
            //I have no clue what the model name is for the Capacitator, and both the Miner / Enforcer gits do some weird fucky shit.

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", "head", new Vector3(-0.49036F, 0.80385F, -0.10177F), new Vector3(63.39054F, 2.82026F, 1.80769F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", "head", new Vector3(0.92099F, 0.19093F, -0.24115F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", "head", new Vector3(0.41061F, 0.81408F, -0.09813F), new Vector3(0F, 0F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", "head", new Vector3(-0.05565F, 0.58127F, -0.8698F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", "head", new Vector3(-0.05565F, 0.58127F, -0.8698F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(-0.00124F, 0.09531F, -0.20699F), new Vector3(0F, 0F, 0F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(0.34165F, 0.55398F, -0.07725F), new Vector3(15.88125F, 79.78722F, 326.7364F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Chest", new Vector3(0.24595F, 0.29929F, -0.00278F), new Vector3(359.8722F, 0.2585F, 326.2169F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(0F, 0.2F, -0.33F), new Vector3(0F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", "head", new Vector3(-0.72531F, 0.06584F, -0.46914F), new Vector3(275.8587F, 318.1681F, 44.29575F), new Vector3(0.2F, 0.2F, 0.2F)));
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
                            childName = "lowerleg.L",
localPos = new Vector3(0.00039F, 0.49533F, 0.00001F),
localAngles = new Vector3(356.7153F, 311.6767F, 169.8087F),
localScale = new Vector3(0.37162F, 0.37162F, 0.52027F),
                            limbMask = LimbFlags.None
                        },
                        //For some reason, only appears on one leg?
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "lowerleg.R",
localPos = new Vector3(0.00039F, 0.49533F, 0.00001F),
localAngles = new Vector3(0F, 0F, 179.4167F),
localScale = new Vector3(0.37162F, 0.37162F, 0.52027F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion
            #region DLC1Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "Upperarm.R", new Vector3(-0.0407F, 0.05762F, 0.04535F), new Vector3(5.22303F, 328.7101F, 178.185F), new Vector3(0.2271F, 0.2271F, 0.2271F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "Chest", new Vector3(0.19434F, -0.0007F, 0.18559F), new Vector3(0F, 0F, 0F), new Vector3(0.03333F, 0.03333F, 0.03333F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "Chest", new Vector3(0F, -0.13022F, -0.21275F), new Vector3(6.98145F, 0F, 0F), new Vector3(1.16932F, 1.16932F, 1.16932F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "Waist", new Vector3(0.31036F, -0.00235F, -0.00218F), new Vector3(0F, 0F, 0F), new Vector3(0.26748F, 0.26748F, 0.26748F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "head", new Vector3(0F, 0.31132F, 0F), new Vector3(315F, 90F, 0F), new Vector3(0.20656F, 0.20656F, 0.20656F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoid", "Chest", new Vector3(-0.00016F, 0.06758F, 0.22978F), new Vector3(0F, 0F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "Upperarm.R", new Vector3(-0.14767F, -0.11289F, -0.00628F), new Vector3(0F, 340F, 90F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "Lowerarm.R_end", new Vector3(-0.01413F, 0.05694F, 0.003F), new Vector3(348.7779F, 1.89384F, 1.55342F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "Upperarm.L", new Vector3(0.0422F, 0.2134F, 0.10468F), new Vector3(0F, 0F, 0F), new Vector3(0.21706F, 0.21706F, 0.21706F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "Chest", new Vector3(0.10365F, -0.03266F, -0.25778F), new Vector3(0.60785F, 186.5007F, 1.09132F), new Vector3(0.07187F, 0.07187F, 0.07187F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "Lowerarm.L_end", new Vector3(0F, -0.05F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "head_end", new Vector3(-0.00075F, 0.2974F, 0.11365F), new Vector3(307.0807F, 358.1367F, 2.27793F), new Vector3(0.36F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "Chest", new Vector3(0.14725F, 0.01318F, 0.19875F), new Vector3(13.457F, 124.6577F, 98.06718F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "Waist", new Vector3(0.15904F, 0.67785F, 0.59582F), new Vector3(0.0273F, 89.20187F, 179.9936F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "Chest", new Vector3(-0.30034F, 0.02553F, -0.0019F), new Vector3(2.13443F, 9.99999F, 20F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "Chest", new Vector3(-0.07F, 0.35F, -0.27F), new Vector3(346.9894F, 188.9663F, 54.97551F), new Vector3(0.8F, 0.8F, 0.8F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "Chest", new Vector3(0F, 0.3F, -0.3F), new Vector3(0F, 180F, 0F), new Vector3(0.3F, 0.3F, 0.3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "Upperleg.L", new Vector3(0.00143F, 0.22947F, -0.16788F), new Vector3(335.7576F, 174.7429F, 175.7614F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "Waist", new Vector3(-0.25F, 0.05F, 0.2F), new Vector3(344.337F, 224.041F, 5.75463F), new Vector3(0.08F, 0.08F, 0.08F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "head", new Vector3(0.01072F, 0.02234F, 0.18806F), new Vector3(70.58395F, 353.9931F, 352.8643F), new Vector3(1F, 0.82754F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "Chest", new Vector3(0.00494F, 0.02039F, 0.22476F), new Vector3(14.7022F, 0F, 0F), new Vector3(0.1544F, 0.1544F, 0.1544F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "head", new Vector3(-0.01168F, 0.56092F, -0.06593F), new Vector3(359.9035F, 309.5071F, 2.20686F), new Vector3(0.7009F, 0.04675F, 0.77977F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "Waist", new Vector3(-0.03839F, 0.00031F, 0.02357F), new Vector3(0F, 0F, 0F), new Vector3(1.19943F, 1.01134F, 1.01134F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "Waist", new Vector3(0.23713F, 0.02732F, 0.10536F), new Vector3(347.3455F, 65.43384F, 10.69646F), new Vector3(0.08125F, 0.08125F, 0.08125F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "Lowerarm.R", new Vector3(0F, 0.34173F, 0.11263F), new Vector3(0F, 0F, 0F), new Vector3(0.50177F, 0.50177F, 0.50177F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", "head", new Vector3(-0.40181F, 0.25421F, -0.30328F), new Vector3(82.966F, 341.9433F, 338.0252F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "Waist", new Vector3(0.25013F, 0.05032F, 0.08599F), new Vector3(0.13516F, 0.04265F, 20.06562F), new Vector3(0.62148F, 0.62148F, 0.62148F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "Chest", new Vector3(-0.40721F, 0.35652F, 0.01299F), new Vector3(359.3603F, 0.00133F, 342.7037F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "Chest", new Vector3(0.39885F, 0.39849F, -0.01684F), new Vector3(0F, 193.1714F, 332.5494F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "Waist", new Vector3(0.16612F, -0.00107F, 0.19053F), new Vector3(83.67088F, 15.04621F, 352.947F), new Vector3(0.30334F, 0.30334F, 0.30334F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "Chest", new Vector3(0F, -0.07625F, -0.0001F), new Vector3(89.92339F, 0F, 0F), new Vector3(0.6617F, 0.6617F, 0.6617F)));
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
                            childName = "head",
localPos = new Vector3(0.005F, 0.34227F, 0.16657F),
localAngles = new Vector3(2.62072F, 0.033F, 359.847F),
localScale = new Vector3(0.97255F, 1.64988F, 0.24631F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "head",
localPos = new Vector3(-0.01634F, 0.00596F, 0.05355F),
localAngles = new Vector3(0.56289F, 1.62524F, 13.09743F),
localScale = new Vector3(2.86155F, 2.86155F, 2.93507F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", "head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "Chest", new Vector3(0.06565F, -0.08502F, -0.20579F), new Vector3(354.1336F, 180F, 48.85915F), new Vector3(0.12792F, 0.12792F, 0.12792F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "head", new Vector3(-0.25431F, 0.02618F, 0.23001F), new Vector3(341.9067F, 353.7088F, 342.7318F), new Vector3(0.26446F, 0.26446F, 0.26446F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.VendingMachine, "DisplayVendingMachine2", "head", new Vector3(-0.24985F, 0.20061F, 0.26953F), new Vector3(334.4594F, 87.06461F, 339.7497F), new Vector3(0.19051F, 0.19051F, 0.19051F)));
            
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
                            childName = "head",
localPos = new Vector3(0.499F, 0.32531F, 0.09139F),
localAngles = new Vector3(81.01185F, 134.6402F, 136.0429F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "head",
localPos = new Vector3(0F, 0.64766F, 0F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "head", new Vector3(0F, 0.64766F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "head", new Vector3(0.2004F, 0.28753F, 0.09826F), new Vector3(9.04862F, 260.3409F, 5.25775F), new Vector3(0.14829F, 0.14829F, 0.14829F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "Chest", new Vector3(0F, 0.20042F, 0.30629F), new Vector3(76.42161F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", "head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "head", new Vector3(0.01136F, 0.29265F, 0.12021F), new Vector3(73.63046F, 0F, 0F), new Vector3(0.40867F, 0.40867F, 0.40867F)));
            //from claymen git
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "head_end", new Vector3(0F, 0.19335F, 0F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "Lowerarm.R", new Vector3(0.13884F, 0.3245F, -0.02297F), new Vector3(283.2397F, 107.9838F, 58.95483F), new Vector3(0.14314F, 0.14314F, 0.14314F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "Upperarm.R", new Vector3(-0.073F, 0.01192F, 0.03027F), new Vector3(3.87674F, 29.46614F, 355.7458F), new Vector3(0.44277F, 0.44277F, 0.44277F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", "head", new Vector3(0.73439F, 0.9596F, -0.15579F), new Vector3(0F, 0F, 0F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "Chest", new Vector3(0.0069F, -0.07487F, 0.23316F), new Vector3(0F, 0F, 0F), new Vector3(0.0331F, 0.0331F, 0.0331F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "Lowerarm.L", new Vector3(0.03645F, 0.35521F, 0.09256F), new Vector3(12.94629F, 357.8604F, 188.2494F), new Vector3(1F, 1F, 1F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburner", "head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [UTILITYSKILLMAGAZINE] = Choose either one, and if its Ring, then put on both shoulders
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.UtilitySkillMagazine,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "Chest",
localPos = new Vector3(0.34809F, 0.24314F, -0.01422F),
localAngles = new Vector3(359.8502F, 180F, 194.1998F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "Chest",
localPos = new Vector3(-0.34809F, 0.24314F, -0.01422F),
localAngles = new Vector3(359.8502F, 180F, 169.2147F),
localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                    }
                }
            }); 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar,Eye", "head", new Vector3(0.02047F, 0.40061F, 0.44407F), new Vector3(0F, 0F, 0F), new Vector3(0.30852F, 0.30852F, 0.30852F)));
            #endregion
            #endregion
            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            //itemDisplayRuleSet.GenerateRuntimeValues();

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        public static void RegisterModdedDisplays()
        {
            /*
            #region Aetherium
            if (ItemDisplayCore.aetheriumInstalled)
            {
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_ACCURSED_POTION", ItemDisplayCore.LoadAetheriumDisplay("AccursedPotion"), "Upperleg.L", new Vector3(-0.002f, 0, 0), new Vector3(0, 0, 180), new Vector3(0.01f, 0.01f, 0.01f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_VOID_HEART", ItemDisplayCore.LoadAetheriumDisplay("VoidHeart"), "Chest", new Vector3(0, 0.002f, 0), new Vector3(0, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_SHARK_TEETH", ItemDisplayCore.LoadAetheriumDisplay("SharkTeeth"), "lowerleg.L", new Vector3(-0.001f, 0.003f, 0), new Vector3(0, 0, 335), new Vector3(0.005f, 0.005f, 0.005f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_BLOOD_SOAKED_SHIELD", ItemDisplayCore.LoadAetheriumDisplay("BloodSoakedShield"), "Lowerarm.L", new Vector3(0.0012f, 0.002f, 0), new Vector3(0, 90, 0), new Vector3(0.003f, 0.003f, 0.003f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_FEATHERED_PLUME", ItemDisplayCore.LoadAetheriumDisplay("FeatheredPlume"), "head_end", new Vector3(0, 0.002f, 0), new Vector3(335, 0, 0), new Vector3(0.005f, 0.005f, 0.005f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_SHIELDING_CORE", ItemDisplayCore.LoadAetheriumDisplay("ShieldingCore"), "Chest", new Vector3(0, 0.002f, -0.003f), new Vector3(0, 180, 0), new Vector3(0.002f, 0.002f, 0.002f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_UNSTABLE_DESIGN", ItemDisplayCore.LoadAetheriumDisplay("UnstableDesign"), "Chest", new Vector3(0, 0, -0.0025f), new Vector3(0, 45, 0), new Vector3(0.01f, 0.01f, 0.01f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_WEIGHTED_ANKLET", ItemDisplayCore.LoadAetheriumDisplay("WeightedAnklet"), "lowerleg.R", new Vector3(0, 0.001f, 0), new Vector3(0, 0, 0), new Vector3(0.003f, 0.003f, 0.003f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_BLASTER_SWORD", ItemDisplayCore.LoadAetheriumDisplay("BlasterSword"), "Lowerarm.R_end", new Vector3(-0.005f, 0.004f), new Vector3(0, 0, 240), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_WITCHES_RING", ItemDisplayCore.LoadAetheriumDisplay("WitchesRing"), "Lowerarm.L_end", new Vector3(0, 0, 0), new Vector3(0, 180, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));

                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_ALIEN_MAGNET", ItemDisplayCore.LoadAetheriumDisplay("AlienMagnet"), new Vector3(0.015f, 0.016f, -0.014f), new Vector3(0, 0, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("ITEM_INSPIRING_DRONE", ItemDisplayCore.LoadAetheriumDisplay("InspiringDrone"), new Vector3(-0.014f, 0.014f, 0), new Vector3(0, 0, 0), new Vector3(0.0015f, 0.0015f, 0.0015f)));

                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("EQUIPMENT_JAR_OF_RESHAPING", ItemDisplayCore.LoadAetheriumDisplay("JarOfReshaping"), new Vector3(0.01f, 0.02f, 0), new Vector3(270, 0, 0), new Vector3(0.001f, 0.001f, 0.001f)));
            }
            #endregion
            #region SupplyDrop
            if (ItemDisplayCore.supplyDropInstalled)
            {
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPElectroPlankton", ItemDisplayCore.LoadSupplyDropDisplay("ElectroPlankton"), "Chest", new Vector3(0, 0, -0.003f), new Vector3(0, 0, 90), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPHardenedBoneFragments", ItemDisplayCore.LoadSupplyDropDisplay("HardenedBoneFragments"), "Chest", new Vector3(-0.002f, 0.0035f, 0), new Vector3(0, 270, 0), new Vector3(0.015f, 0.015f, 0.015f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPQSGen", ItemDisplayCore.LoadSupplyDropDisplay("QSGen"), "Lowerarm.L", new Vector3(0, 0.002f, 0), new Vector3(0, 0, 270), new Vector3(0.001f, 0.001f, 0.001f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPSalvagedWires", ItemDisplayCore.LoadSupplyDropDisplay("SalvagedWires"), "Upperleg.R", new Vector3(-0.0025f, 0.002f, 0.0015f), new Vector3(90, 90, 0), new Vector3(0.006f, 0.006f, 0.006f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPShellPlating", ItemDisplayCore.LoadSupplyDropDisplay("ShellPlating"), "Waist", new Vector3(0, 0, -0.0032f), new Vector3(0, 180, 180), new Vector3(0.0025f, 0.0025f, 0.0025f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPPlagueHat", ItemDisplayCore.LoadSupplyDropDisplay("PlagueHat"), "head_end", new Vector3(0, 0.004f, 0f), new Vector3(0, 180, 0), new Vector3(0.0025f, 0.0025f, 0.0025f)));
                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPPlagueMask", ItemDisplayCore.LoadSupplyDropDisplay("PlagueMask"), "head_end", new Vector3(0, 0.0025f, 0.003f), new Vector3(0, 180, 0), new Vector3(0.0025f, 0.0025f, 0.0025f)));

                itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule("SUPPDRPBloodBook", ItemDisplayCore.LoadSupplyDropDisplay("BloodBook"), new Vector3(-0.015f, 0.02f, 0.008f), new Vector3(0, 0, 0), new Vector3(0.08f, 0.08f, 0.08f)));
            }
            #endregion

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            */
        }
    }
}