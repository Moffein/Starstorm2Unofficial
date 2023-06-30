using UnityEngine;
using RoR2;
using System.Collections.Generic;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Chirr
{
    public static class ChirrItemDisplays
    {
        public static ItemDisplayRuleSet itemDisplayRuleSet;

        public static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemRules;

        public static void RegisterDisplays()
        {
            GameObject bodyPrefab = ChirrCore.chirrPrefab;
            GameObject model = bodyPrefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemRules = new List<ItemDisplayRuleSet.KeyAssetRuleGroup>();

            /* Model
			MainHurtbox
			HeadbuttHitbox
			MuzzleHeadbutt
			Head
			HeadCenter
			Neck
			Base
			WingL1
			WingL2
			WingLEnd
			WingR1 */

            #region DisplayRules
            #region RoR2Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "Chest", new Vector3(-0.15023F, 0.12526F, 0.45821F), new Vector3(6.25538F, 345.4568F, 42.6496F), new Vector3(0.30366F, 0.30366F, 0.30366F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBearSit", "Chest", new Vector3(0.00567F, 0.22306F, 0.5638F), new Vector3(336.991F, 0.49044F, 1.00001F), new Vector3(0.32535F, 0.32535F, 0.32535F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Chest", new Vector3(-0.02081F, 0.71763F, 0.28906F), new Vector3(343.7892F, 235.066F, 45.66403F), new Vector3(0.09F, 0.09F, 0.09F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "ElbowR", new Vector3(-0.04616F, -0.60304F, -0.08146F), new Vector3(356.7162F, 168.417F, 187.8333F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Pelvis", new Vector3(-0.46048F, -0.48838F, -0.23426F), new Vector3(62.58829F, 179.054F, 177.8278F), new Vector3(0.13254F, 0.13254F, 0.13254F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Chest", new Vector3(-0.18854F, 0.55266F, 0.09293F), new Vector3(300.2466F, 292.0094F, 240.7119F), new Vector3(1F, 1F, 1F)));
            //Tooth
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Tooth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                                {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothNecklaceDecal"),
                            childName = "Chest",
                            localPos = new Vector3(-0.02534F, 0.65133F, -0.0682F),
                            localAngles = new Vector3(294.631F, 357.532F, 1.85058F),
                            localScale = new Vector3(0.96688F, 0.52618F, 0.96688F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshLarge"),
                            childName = "Chest",
                            localPos = new Vector3(-0.01865F, 0.37971F, -0.36382F),
                            localAngles = new Vector3(53.04276F, 3.02555F, 1.58643F),
                            localScale = new Vector3(2.39344F, 3.73495F, 2.39344F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "Chest",
localPos = new Vector3(-0.07398F, 0.38544F, -0.34308F),
localAngles = new Vector3(40.64451F, 350.1534F, 323.6163F),
localScale = new Vector3(1.43896F, 1.43896F, 1.43896F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "Chest",
localPos = new Vector3(-0.12198F, 0.40744F, -0.30539F),
localAngles = new Vector3(18.632F, 13.77735F, 333.1451F),
localScale = new Vector3(1.31001F, 1.31001F, 1.31001F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "Chest",
localPos = new Vector3(0.03461F, 0.38508F, -0.33694F),
localAngles = new Vector3(56.85872F, 12.13364F, 57.47216F),
localScale = new Vector3(1.47862F, 1.47862F, 1.47862F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "Chest",
localPos = new Vector3(0.07587F, 0.40531F, -0.32007F),
localAngles = new Vector3(53.03017F, 339.519F, 19.58785F),
localScale = new Vector3(1.47632F, 1.47632F, 1.47632F),
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, RoR2/Base/Tooth/mdlToothNecklaceDisplay.fbx
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", new Vector3(0.01546F, 0.31216F, 0.54586F), new Vector3(317.8046F, 0.07596F, 358.8435F), new Vector3(1.50537F, 1F, 1.17131F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "HandL", new Vector3(0.00455F, -0.09714F, -0.05172F), new Vector3(79.37135F, 1.58121F, 5.49504F), new Vector3(0.09361F, 0.09361F, 0.06845F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "ShoulderL", new Vector3(-0.13628F, 0.15534F, 0.26413F), new Vector3(332.8565F, 77.47125F, 54.80316F), new Vector3(0.04375F, 0.04375F, 0.04375F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Chest", new Vector3(-0.02517F, 0.2486F, 0.63842F), new Vector3(317.349F, 359.2465F, 0.83228F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "Pelvis", new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F), new Vector3(0.01F, 0.01F, 0.01F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", new Vector3(1F, -1.44374F, 1.00245F), new Vector3(79.1022F, 5.29585F, 6.18342F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", new Vector3(0.00746F, 0.14667F, 0.40238F), new Vector3(314.4365F, 359.6817F, 0.32543F), new Vector3(3.7724F, 1F, 2.1725F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "Chest", new Vector3(-0.2302F, 0.24834F, 0.36928F), new Vector3(55.48694F, 5.29165F, 41.68101F), new Vector3(0.13386F, 0.13386F, 0.13386F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Chest", new Vector3(0.12011F, 0.30736F, 0.73098F), new Vector3(7.60706F, 81.33895F, 301.6433F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", new Vector3(-0.00572F, 0.58101F, -0.37234F), new Vector3(354.9276F, 0.89379F, 1.00378F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Chest", new Vector3(-0.279F, 0.49017F, -0.14875F), new Vector3(34.94321F, 57.60104F, 311.9139F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "WingR1", new Vector3(0.01065F, 0.60301F, -0.44011F), new Vector3(86.01606F, 15.49513F, 14.54624F), new Vector3(1F, 1F, 1F)));
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
                            childName = "HandR",
                            localPos = new Vector3(-0.00984F, -0.11173F, 0.01F),
                            localAngles = new Vector3(2.91213F, 180.9317F, 178.9989F),
                            localScale = new Vector3(0.16715F, 0.16715F, 0.16715F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayGravBoots"),
                            childName = "HandL",
                            localPos = new Vector3(0.0081F, -0.11142F, 0.00969F),
                            localAngles = new Vector3(2.91213F, 180.9317F, 178.9989F),
                            localScale = new Vector3(0.16715F, 0.16715F, 0.16715F),
                            limbMask = LimbFlags.None
                        }
						//cant add it to the backleg since there's no nametransformpair in the childlocator
						//sad!
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Pelvis", new Vector3(-0.37928F, 0.30007F, 0.01175F), new Vector3(2.82264F, 0.3134F, 341.7837F), new Vector3(0.54208F, 0.54208F, 0.54208F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "ShoulderL", new Vector3(-0.17874F, 0.52203F, -0.07117F), new Vector3(78.43723F, 248.4556F, 182.0916F), new Vector3(0.42424F, 0.68323F, 0.68323F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "Pelvis", new Vector3(0.10931F, 0.2641F, 0.28469F), new Vector3(38.92262F, 280.1779F, 284.7581F), new Vector3(0.21985F, 0.21985F, 0.21985F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "ShoulderR", new Vector3(0.12764F, 0.02011F, -0.14796F), new Vector3(302.4363F, 237.1508F, 42.04613F), new Vector3(0.18444F, 0.18444F, 0.18444F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "Chest", new Vector3(-0.01319F, 0.60932F, 0.34391F), new Vector3(300.3394F, 191.9624F, 168.454F), new Vector3(0.28206F, 0.28206F, 0.28206F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "Pelvis", new Vector3(0.23711F, -0.16623F, 0.28675F), new Vector3(38.85259F, 278.8196F, 358.9197F), new Vector3(0.30529F, 0.30529F, 0.30529F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", new Vector3(-0.76854F, 0.19512F, -0.13695F), new Vector3(316.2369F, 352.1038F, 187.1849F), new Vector3(2.10714F, 2.10714F, 2.10714F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Head", new Vector3(0.022F, -0.85371F, -0.44901F), new Vector3(306.9829F, 2.90805F, 358.7383F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Chest", new Vector3(-0.43019F, -0.23488F, 0.18538F), new Vector3(12.11927F, 290.5139F, 351.905F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Head", new Vector3(0.65994F, 0.23503F, -0.5935F), new Vector3(296.1083F, 174.9901F, 186.3231F), new Vector3(0.73227F, 0.73227F, 0.73227F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "Chest", new Vector3(0.294F, -0.14077F, 0.51293F), new Vector3(278.9928F, 327.992F, 106.0074F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Chest", new Vector3(0.03952F, 0.1524F, -0.09973F), new Vector3(323.4799F, 52.82478F, 125.8604F), new Vector3(1.1F, 1.1F, 1.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Chest", new Vector3(0.17172F, 0.51184F, 0.41762F), new Vector3(320.4774F, 175.7188F, 27.72161F), new Vector3(0.62847F, 0.62847F, 0.62847F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "WingL1", new Vector3(0.01351F, 0.85619F, -0.42017F), new Vector3(48.05252F, 354.9252F, 5.29162F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "WingLEnd", new Vector3(-0.02729F, 0.02033F, -0.05153F), new Vector3(349.159F, 0.79105F, 308.9109F), new Vector3(1.16712F, 1.16712F, 1.16712F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Chest", new Vector3(0.06502F, 0.93282F, 0.33266F), new Vector3(276.1395F, 190.3199F, 170.6098F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "WingR2", new Vector3(-0.0065F, 0.1342F, -0.12539F), new Vector3(42.94081F, 188.9775F, 94.10269F), new Vector3(0.438F, 0.438F, 0.438F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", new Vector3(-0.01938F, 0.40242F, -0.24337F), new Vector3(0.70255F, 1.01255F, 0.98901F), new Vector3(2.35414F, 2.35414F, 2.33825F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "ShoulderL", new Vector3(-0.16028F, 0.1485F, -0.10518F), new Vector3(59.79329F, 276.0453F, 47.3881F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", new Vector3(0.28248F, 0.59512F, -0.28753F), new Vector3(6.03945F, 257.9412F, 33.06877F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Pelvis", new Vector3(-0.41048F, -0.41826F, -0.26212F), new Vector3(20.62992F, 26.01957F, 304.6966F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", new Vector3(1.45758F, 0.00135F, -0.74336F), new Vector3(8.23762F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Chest", new Vector3(-0.32155F, 0.19367F, -0.37164F), new Vector3(278.9392F, 187.3527F, 198.2888F), new Vector3(0.13478F, 0.13478F, 0.13478F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Chest", new Vector3(-0.41854F, -0.30217F, 0.35018F), new Vector3(5.05936F, 276.7172F, 1.00413F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Chest", new Vector3(0.38529F, 0.04209F, -0.37359F), new Vector3(301.9651F, 359.3799F, 1.88888F), new Vector3(0.35162F, 0.35162F, 0.35162F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "WingR1", new Vector3(-0.05069F, 0.45175F, -0.29928F), new Vector3(1F, 1F, 1F), new Vector3(0.15137F, 0.15137F, 0.15137F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "WingR1", new Vector3(-0.06306F, 1.03674F, -0.44728F), new Vector3(1F, 1F, 1F), new Vector3(0.39297F, 0.39465F, 0.39465F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "HandR", new Vector3(-0.0343F, -0.38311F, 0.08292F), new Vector3(350.2878F, 251.5455F, 272.1064F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "LowerArmR", new Vector3(0.06666F, 0.0625F, 0.16629F), new Vector3(78.19929F, 287.5869F, 291.5888F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "UpperArmR", new Vector3(0.16306F, 0.65225F, -0.04339F), new Vector3(288.6337F, 82.54973F, 270.6353F), new Vector3(0.23865F, 0.23865F, 0.23865F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "ShoulderL", new Vector3(0.0064F, 0.90037F, 0.02357F), new Vector3(67.77676F, 299.2639F, 351.0946F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "ShoulderR", new Vector3(0.00964F, 0.68832F, 0.01936F), new Vector3(280.3314F, 314.3229F, 47.53419F), new Vector3(1.27506F, 1.27847F, 1.27847F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "WingR2", new Vector3(-0.00319F, -1.12287F, -0.80266F), new Vector3(350.9676F, 58.72039F, 335.148F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Chest", new Vector3(-0.01106F, 0.24168F, -0.4664F), new Vector3(358.996F, 180.7725F, 359.004F), new Vector3(0.44216F, 0.44216F, 0.44216F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Pelvis", new Vector3(0.00267F, -0.13704F, 0.01621F), new Vector3(285.7728F, 184.5249F, 176.3194F), new Vector3(1.129F, 1.129F, 1.129F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullcrown", "Pelvis", new Vector3(0.00696F, 0.05595F, -0.04846F), new Vector3(1F, 1F, 173.9113F), new Vector3(1.7206F, 0.28452F, 0.3967F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", new Vector3(0.00397F, 0.26022F, -0.0849F), new Vector3(317.6588F, 2.18407F, 359.5154F), new Vector3(0.85891F, 0.85891F, 0.44609F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseflower", "Head", new Vector3(-0.6032F, 0.0986F, -0.55638F), new Vector3(304.9672F, 177.3004F, 184.5284F), new Vector3(0.37671F, 0.37671F, 0.37671F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Ghost, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "LowerArmL", new Vector3(-0.1052F, 0.50997F, 0.03391F), new Vector3(80.2589F, 332.1017F, 331.619F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", new Vector3(0.4725F, 0.30171F, -0.24015F), new Vector3(297.734F, 0.01084F, 358.6537F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", new Vector3(-0.40925F, -0.11607F, -0.19022F), new Vector3(2.44045F, 0.99267F, 1.01493F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "Chest", new Vector3(0.22974F, 0.24791F, 0.4794F), new Vector3(295.7652F, 238.2417F, 148.8397F), new Vector3(0.25665F, 0.25665F, 0.25665F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Pelvis", new Vector3(0.48157F, -0.0613F, 0.02283F), new Vector3(1.07836F, 275.6779F, 298.4128F), new Vector3(0.51274F, 0.51274F, 0.51274F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "LowerArmL", new Vector3(-0.09182F, 0.06795F, -0.11802F), new Vector3(85.12505F, 202.0774F, 168.7453F), new Vector3(0.21873F, 0.21873F, 0.21873F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TonicAffliction, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldHeart", "Chest", new Vector3(0.30478F, -0.32459F, -0.11352F), new Vector3(327.4489F, 359.7195F, 0.73533F), new Vector3(0.33018F, 0.33018F, 0.33018F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "Head", new Vector3(0.51911F, 0.54707F, -0.09855F), new Vector3(300.7584F, 11.76619F, 313.548F), new Vector3(0.25801F, 0.25801F, 0.25801F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Chest", new Vector3(0.23214F, 0.36201F, -0.22746F), new Vector3(51.086F, 151.7892F, 21.96281F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Chest", new Vector3(-0.04282F, 0.60151F, 0.35958F), new Vector3(320.0241F, 180.968F, 179.1135F), new Vector3(0.34534F, 0.34534F, 0.34534F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Neck", new Vector3(0.26799F, 0.03852F, -0.07754F), new Vector3(358.7633F, 85.44554F, 333.6164F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Pelvis", new Vector3(0.45102F, 0F, -0.00001F), new Vector3(1F, 1F, 1F), new Vector3(0.1298F, 0.1298F, 0.1298F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "Pelvis", new Vector3(0F, 0.33696F, 0.31244F), new Vector3(358.8429F, 257.0013F, 190.6211F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeftVoidSurvivor", "ElbowL", new Vector3(-0.09733F, -0.23549F, -0.12083F), new Vector3(278.8405F, 297.3141F, 63.81865F), new Vector3(1.2048F, 1.2048F, 1.2048F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Head", new Vector3(0F, 0.1679F, 0.72457F), new Vector3(281.0661F, 358.9223F, 356.9044F), new Vector3(0.1327F, 0.1327F, 0.1327F)));
            //todo
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "LowerArmL", new Vector3(0F, 0.172F, 0F), new Vector3(270F, 0.00001F, 0F), new Vector3(0.07F, 0.07F, 0.07F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "LowerArmR", new Vector3(0F, 0F, 0F), new Vector3(270F, 0F, 0F), new Vector3(0.07F, 0.07F, 0.07F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "Chest", new Vector3(-0.10376F, 0.20155F, 0.56549F), new Vector3(315.3498F, 351.2206F, 10.31494F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "UpperArmR", new Vector3(-0.12434F, 0.16757F, 0.07909F), new Vector3(6.08667F, 290.4821F, 252.8224F), new Vector3(0.5F, 0.5F, 0.5F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "Head", new Vector3(0.00484F, 0.44638F, 0.31547F), new Vector3(332.8555F, 178.8018F, 181.9016F), new Vector3(0.34473F, 0.34473F, 0.34473F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Head", new Vector3(0.83507F, 0.37624F, -0.78891F), new Vector3(290.2606F, 357.3579F, 0.14143F), new Vector3(0.13063F, 0.13063F, 0.13063F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Head", new Vector3(0.93804F, 0.45057F, -0.76999F), new Vector3(10.8654F, 186.5376F, 283.7511F), new Vector3(1.51536F, 1.51536F, 1.51536F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.InvadingDoppelganger, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "LowerArmR", new Vector3(-0.05483F, 0.42953F, 0.01824F), new Vector3(81.83345F, 192.4065F, 207.7989F), new Vector3(0.78911F, 0.78911F, 0.78911F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "Head", new Vector3(-0.58446F, 0.47933F, -0.33597F), new Vector3(0F, 0F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "WingR1", new Vector3(0.01357F, 0.82464F, -0.25284F), new Vector3(0.38246F, 48.96363F, 82.46727F), new Vector3(0.0564F, 0.0564F, 0.0564F))); //not perfect but
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Chest", new Vector3(-0.27641F, 0.14312F, 0.39589F), new Vector3(329.3064F, 11.17662F, 344.1488F), new Vector3(0.0755F, 0.0755F, 0.0755F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", new Vector3(0.443F, -1.29099F, -1.20587F), new Vector3(270F, 0.32262F, 0F), new Vector3(0.075F, 0.075F, 0.075F))); //todo as follower
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "UpperArmR", new Vector3(-0.21633F, 0.65065F, -0.03582F), new Vector3(344.0815F, 147.0711F, 299.8145F), new Vector3(0.08234F, 0.08234F, 0.08234F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "UpperArmL", new Vector3(0.20725F, 0.65464F, 0.07406F), new Vector3(6.85846F, 87.05473F, 53.06927F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "LowerArmR", new Vector3(-0.15407F, -0.63656F, 0.49147F), new Vector3(10.53178F, 168.8489F, 209.1654F), new Vector3(0.05013F, 0.05013F, 0.05013F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "Chest", new Vector3(-0.29371F, -0.15212F, 0.52658F), new Vector3(2.31172F, 89.91504F, 15.27005F), new Vector3(0.0573F, 0.0573F, 0.0573F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "WingL1", new Vector3(0.00563F, 0.39232F, -0.1553F), new Vector3(40.47644F, 350.5995F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "Pelvis", new Vector3(0F, 0.25973F, 0.30578F), new Vector3(35.26917F, 180F, 180F), new Vector3(0.10719F, 0.10719F, 0.10719F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "UpperArmR", new Vector3(-0.05166F, 0.35473F, 0.01664F), new Vector3(333.0368F, 182.5115F, 67.91328F), new Vector3(1F, 1F, 1F)));

            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", new Vector3(-1.4677F, -0.3564F, -0.7053F), new Vector3(7.40916F, 270.837F, 94.74821F), new Vector3(0.34151F, 0.34151F, 0.34151F)));
            //This is a gameobject spawned
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RoboBallBuddy, "DisplayEmpathyChip", ));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "Pelvis", new Vector3(0F, 0F, 0F), new Vector3(0F, 0F, 0F), new Vector3(0.12177F, 0.12177F, 0.12177F)));
           
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Chest", new Vector3(-0.0204F, 0.49999F, 0.39458F), new Vector3(13.87488F, 0.58435F, 0.0971F), new Vector3(0.47632F, 0.47632F, 0.47632F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "ShoulderR", new Vector3(0F, 0F, 0F), new Vector3(343.7116F, 325.1237F, 320.823F), new Vector3(0.24461F, 0.24461F, 0.24461F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", new Vector3(-0.55437F, -0.0261F, -1.01856F), new Vector3(270F, 0.32262F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", new Vector3(0.32533F, 0.46202F, -0.18691F), new Vector3(0F, 0F, 0F), new Vector3(0.20446F, 0.20446F, 0.20446F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", new Vector3(0.00856F, 0.57427F, -0.02452F), new Vector3(282.8624F, 174.4761F, 186.0091F), new Vector3(0.74495F, 0.74495F, 0.74495F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", new Vector3(0.01051F, 0.52361F, -0.51862F), new Vector3(306.2482F, 177.9596F, 182.6374F), new Vector3(0.05021F, 0.05021F, 0.05021F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", new Vector3(0.00958F, 0.53205F, -0.37046F), new Vector3(297.0258F, 177.2962F, 183.258F), new Vector3(0.15152F, 0.15152F, 0.15152F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", new Vector3(-1.45538F, -0.02608F, -1.01349F), new Vector3(270F, 0.32262F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Head", new Vector3(0.00224F, 0.53675F, 0.89609F), new Vector3(328.138F, 0.53373F, 359.6873F), new Vector3(1.25901F, 0.74514F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Head", new Vector3(0.81913F, 0.2276F, 0.30262F), new Vector3(4.72695F, 231.9432F, 85.54887F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "ShoulderL", new Vector3(-0.1831F, 0.18367F, 0.04956F), new Vector3(312.1308F, 179.8003F, 289.8329F), new Vector3(0.55565F, 0.55565F, 0.55565F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Chest", new Vector3(-0.00556F, 0.11936F, 0.40453F), new Vector3(0F, 180F, 0F), new Vector3(0.37679F, 0.37679F, 0.37679F)));
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
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayLightningArmRight"),
                            childName = "Head",
                            localPos = Vector3.one,
                            localAngles = Vector3.one,
                            localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Chest", new Vector3(-0.49818F, 0.5014F, 0.19441F), new Vector3(43.20132F, 258.0447F, 310.3215F), new Vector3(0.1587F, 0.1587F, 0.1587F)));
            ////itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.PassiveHealing, //handled by attachment
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LunarPotion, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Pelvis", new Vector3(0.47532F, 0F, -0.00003F), new Vector3(24.76932F, 0F, 0F), new Vector3(0.04116F, 0.04116F, 0.04116F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "ShoulderL", new Vector3(0.00441F, 0.3243F, 0.08073F), new Vector3(351.4334F, 284.5128F, 224.2097F), new Vector3(0.5201F, 0.5201F, 0.5201F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Chest", new Vector3(-0.5024F, -0.32432F, -0.04251F), new Vector3(356.998F, 106.08F, 65.02573F), new Vector3(0.54903F, 0.54903F, 0.54903F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "ShoulderR", new Vector3(0.30289F, 0.32553F, 0.12158F), new Vector3(296.1528F, 87.98994F, 179.9712F), new Vector3(0.69059F, 0.69059F, 0.69059F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Chest", new Vector3(-0.38861F, -0.19473F, -0.19039F), new Vector3(346.2986F, 58.63267F, 19.53799F), new Vector3(0.26855F, 0.26855F, 0.26855F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Chest", new Vector3(-0.0355F, 0.42978F, 0.5152F), new Vector3(325.1208F, 358.4429F, 0.43592F), new Vector3(0.56483F, 0.56483F, 0.56483F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Chest", new Vector3(-0.02764F, 0.68675F, 0.16716F), new Vector3(351.3396F, 359.6454F, 0.00642F), new Vector3(0.18198F, 0.18198F, 0.18198F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Pelvis", new Vector3(0.10218F, 0.02832F, -0.00117F), new Vector3(39.52095F, 0F, 0F), new Vector3(0.30072F, 0.30072F, 0.30072F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", new Vector3(0.01056F, 0.57135F, -0.41752F), new Vector3(62.99248F, 0.13626F, 186.629F), new Vector3(0.11666F, 0.11666F, 0.11666F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Chest", new Vector3(-0.36021F, -0.38208F, 0.22049F), new Vector3(357.008F, 4.32376F, 22.47773F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", "Chest", new Vector3(-1.87522F, 1.37483F, -0.61257F), new Vector3(84.9408F, 179.6061F, 179.9815F), new Vector3(0.15F, 0.15F, 0.15F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Chest", new Vector3(0.35555F, -0.29918F, 0.47806F), new Vector3(12.1111F, 345.1084F, 356.8066F), new Vector3(0.063F, 0.063F, 0.063F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", new Vector3(0.01072F, 0.48826F, -0.78804F), new Vector3(350.5901F, 345.2021F, 351.3283F), new Vector3(0.32204F, 0.32204F, 0.32204F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Chest", new Vector3(0.42201F, -0.29445F, -0.12135F), new Vector3(27.01526F, 122.2887F, 299.4163F), new Vector3(0.08969F, 0.08969F, 0.06393F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Chest", new Vector3(-0.49119F, -0.25172F, -0.10299F), new Vector3(344.4935F, 268.0937F, 340.5231F), new Vector3(0.11269F, 0.11269F, 0.11269F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixEcho,
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Equipment.AffixLunar,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayEliteLunar,Eye"),
                            childName = "Head",
                            localPos = new Vector3(0.00238F, 0.46029F, 0.82285F),
                            localAngles = new Vector3(326.6886F, 0.60057F, 359.6052F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            #endregion
            #region DLC1Content
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "WingR2", new Vector3(-0.0319F, 0.14791F, -0.3891F), new Vector3(0F, 0F, 0F), new Vector3(0.2F, 0.2F, 0.2F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "WingL1", new Vector3(0.03087F, 2.07135F, -0.58706F), new Vector3(87.58573F, 253.4877F, 247.0912F), new Vector3(0.13252F, 0.13252F, 0.13252F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "Pelvis", new Vector3(0.03711F, 0.00819F, -0.0694F), new Vector3(32.44383F, 180F, 180F), new Vector3(3F, 3F, 3F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "Pelvis", new Vector3(0.35016F, -0.62932F, -0.36503F), new Vector3(7.93441F, 144.723F, 40.78794F), new Vector3(0.25F, 0.25F, 0.25F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "Head", new Vector3(-0.01149F, 0.49715F, 0.08563F), new Vector3(0F, 90F, 6.71617F), new Vector3(0.18555F, 0.18555F, 0.18555F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoidSit", "Chest", new Vector3(0.00567F, 0.22306F, 0.5638F), new Vector3(336.991F, 0.49044F, 1.00001F), new Vector3(0.32535F, 0.32535F, 0.32535F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "Chest", new Vector3(-0.2302F, 0.24834F, 0.36928F), new Vector3(55.48694F, 5.29165F, 41.68101F), new Vector3(0.13386F, 0.13386F, 0.13386F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "Head", new Vector3(0.6262F, 0.26936F, -0.5714F), new Vector3(293.738F, 174.5184F, 186.8068F), new Vector3(0.73227F, 0.73227F, 0.73227F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "WingR2", new Vector3(-0.06044F, 0.0511F, -0.35608F), new Vector3(0F, 0F, 0F), new Vector3(0.13066F, 0.13066F, 0.13066F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GummyCloneIdentifier, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "Chest", new Vector3(-0.40356F, -0.06715F, 0.44204F), new Vector3(348.6971F, 293.5712F, 338.4493F), new Vector3(0.12007F, 0.12007F, 0.12007F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrapConsumed, //no display
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "ShoulderR", new Vector3(0.18518F, -0.77175F, -0.63465F), new Vector3(43.56147F, 63.76232F, 138.5817F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "Head", new Vector3(-0.00848F, 0.27443F, 0.50568F), new Vector3(319.4023F, 0.12552F, 1.31686F), new Vector3(1.49488F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "HandR", new Vector3(-0.03378F, -0.41593F, 0.09592F), new Vector3(352.4407F, 307.0146F, 263.4155F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "WingR2", new Vector3(-0.00319F, -1.12287F, -0.80266F), new Vector3(350.9676F, 58.72039F, 335.148F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "ElbowR", new Vector3(-0.07161F, -0.58049F, -0.14376F), new Vector3(348.906F, 172.3322F, 190.4321F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "Chest", new Vector3(-0.03399F, 0.27351F, 0.6403F), new Vector3(318.713F, 359.3023F, 0.80868F), new Vector3(0.7F, 0.7F, 0.7F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "Chest", new Vector3(-0.01106F, 0.24168F, -0.4664F), new Vector3(358.996F, 180.7725F, 359.004F), new Vector3(0.44216F, 0.44216F, 0.44216F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoidConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "Pelvis", new Vector3(0.21988F, -0.3463F, 0.3285F), new Vector3(340.5811F, 0.63003F, 327.4721F), new Vector3(0.35529F, 0.35529F, 0.35529F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "Pelvis", new Vector3(-0.46048F, -0.48838F, -0.23426F), new Vector3(62.58829F, 179.054F, 177.8278F), new Vector3(0.13254F, 0.13254F, 0.13254F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "HandL", new Vector3(0.02404F, -0.17044F, -0.02602F), new Vector3(79.17068F, 235.4588F, 74.08698F), new Vector3(0.64414F, 0.64414F, 0.64414F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonusConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "Pelvis", new Vector3(0.36519F, 0.08453F, 0.1513F), new Vector3(3.24596F, 256.8709F, 2.57618F), new Vector3(0.37935F, 0.37935F, 0.37935F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ScrapWhiteSuppressed, "DisplayScrapVoidWhite", ));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ScrapGreenSuppressed, "DisplayScrapVoidGreen", ));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ScrapRedSuppressed, "DisplayScrapVoidRed", ));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "WingR2", new Vector3(0.01164F, 0.00399F, -0.2423F), new Vector3(0F, 334.91F, 0F), new Vector3(0.1F, 0.1F, 0.1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "Chest", new Vector3(0.00715F, -0.18159F, -0.05281F), new Vector3(290.9177F, 354.1962F, 3.8727F), new Vector3(2.95532F, 1.27291F, 1.27291F)));
            //RoR2/DLC1/ImmuneToDebuff/mdlRaincoatDisplayBelt.fbx
            //RoR2/DLC1/ImmuneToDebuff/mdlRaincoatDisplayFolded.fbx
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "Pelvis", new Vector3(0.38179F, 0.33733F, -0.01783F), new Vector3(334.7498F, 66.24985F, 83.83425F), new Vector3(0.14641F, 0.14641F, 0.14641F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "HandR", new Vector3(-0.01229F, -0.1835F, 0.07199F), new Vector3(354.3894F, 353.3548F, 5.67876F), new Vector3(0.34119F, 0.34119F, 0.34119F)));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", new Vector3(0.687F, -0.30488F, -0.69297F), new Vector3(277.8341F, 180F, 180F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "UpperArmL", new Vector3(0.13413F, 0.50973F, -0.16932F), new Vector3(58.5436F, 307.2332F, 262.348F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "ShoulderR", new Vector3(0.11455F, -0.09253F, 0.084F), new Vector3(3.70443F, 315.4499F, 213.5794F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "ShoulderL", new Vector3(-0.15238F, 0.206F, 0.01224F), new Vector3(8.46096F, 173.2564F, 212.9194F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "Pelvis", new Vector3(-0.32823F, 0.45358F, -0.04389F), new Vector3(315.7032F, 145.4739F, 301.5168F), new Vector3(1F, 1F, 1F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ConvertCritChanceToCritDamage, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "ShoulderR", new Vector3(0.00463F, 0.76168F, 0.0238F), new Vector3(273.7516F, 207.0382F, 133.05F), new Vector3(1.18541F, 1.18541F, 1.18541F)));

            if (Modules.Config.ChirrEgoFullHeadReplacement.Value)
            {
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
                            localPos = new Vector3(0.00281F, 0.20885F, 0.00621F),
                            localAngles = new Vector3(4.00844F, 359.9461F, 0.01786F),
                            localScale = new Vector3(5.38898F, 1.93572F, 4.37334F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "Neck",
                            localPos = new Vector3(0.02074F, 0.07013F, 0.05536F),
                            localAngles = new Vector3(4.97696F, 5.94505F, 8.78821F),
                            localScale = new Vector3(3.20744F, 2.59127F, 3.20744F),
                            limbMask = LimbFlags.None
                        }
                        }
                    }
                });
            }
            else
            {
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
                            localPos = new Vector3(0.00675F, 0.50966F, 0.05856F),
                            localAngles = new Vector3(75.00004F, 0F, 0F),
                            localScale = new Vector3(0.75F, 0.75F, 0.75F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "Neck",
                            localPos = new Vector3(0.02074F, 0.07013F, 0.05536F),
                            localAngles = new Vector3(4.97696F, 5.94505F, 8.78821F),
                            localScale = new Vector3(3.20744F, 2.59127F, 3.20744F),
                            limbMask = LimbFlags.None
                        }
                        }
                    }
                });
            }

            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", new Vector3(0.55F, -1.83803F, -1.30853F), new Vector3(326.9531F, 0F, 0.32262F), new Vector3(0.4F, 0.4F, 0.4F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "Head", new Vector3(-0.01446F, -0.79292F, 0.51989F), new Vector3(335.283F, 0.40798F, 359.7498F), new Vector3(0.24436F, 0.24436F, 0.24436F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "ShoulderR", new Vector3(-0.19002F, 0.14201F, 0.10526F), new Vector3(18.24455F, 171.2352F, 190.5506F), new Vector3(0.55564F, 0.55564F, 0.55564F)));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.VendingMachine, "DisplayVendingMachine2", "Head", new Vector3(-0.83022F, 0.18775F, 0.46236F), new Vector3(358.3845F, 133.6339F, 277.618F), new Vector3(0.26726F, 0.26726F, 0.26726F)));
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
                            localPos = new Vector3(1.26644F, 0.90127F, 0.873F),
                            localAngles = new Vector3(55.9059F, 0.90615F, 2.3868F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "Head",
                            localPos = new Vector3(0.01058F, 0.58483F, -0.4099F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(1F, 1F, 1F),
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "Head", new Vector3(0.01058F, 0.58483F, -0.4099F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "Chest", new Vector3(-0.29304F, 0.35178F, -0.30096F), new Vector3(21.27639F, 356.3309F, 357.941F), new Vector3(0.33509F, 0.33509F, 0.33509F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "Chest", new Vector3(-0.35586F, -0.4621F, 0.3318F), new Vector3(58.99131F, 179.4167F, 250.5275F), new Vector3(1F, 1F, 1F)));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", ));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUseFollower", ));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "Head", new Vector3(0.02166F, 0.21442F, 0.09454F), new Vector3(7.22854F, 1.40685F, 0.30024F), new Vector3(0.97717F, 0.97717F, 0.97717F)));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "Head", new Vector3(0.0083F, 0.49201F, -0.29676F), new Vector3(0F, 0F, 0F), new Vector3(1F, 1F, 1F)));
            #endregion
            #endregion

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemRules.ToArray();
            itemDisplayRuleSet.keyAssetRuleGroups = item;
            //itemDisplayRuleSet.GenerateRuntimeValues();

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }


    }
}
