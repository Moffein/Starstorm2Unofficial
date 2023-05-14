using RoR2;
using RoR2.ExpansionManagement;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Chirr;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Starstorm2Unofficial.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            PopulateFromBody("Commando");
            PopulateFromBody("Croco");
            PopulateFromBody("Mage");
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = name?.ToLower();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }

            return null;
        }

        internal static void GatherDisplayNames()
        {
            StringBuilder sb = HG.StringBuilderPool.RentStringBuilder();

            foreach (var bodyPrefab in BodyCatalog.bodyPrefabs)
            {
                ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
                if (!modelLocator) continue;
                Transform modelTransform = modelLocator.modelTransform;
                if (!modelTransform) continue;
                CharacterModel characterModel = modelTransform.GetComponent<CharacterModel>();
                if (!characterModel) continue;
                ItemDisplayRuleSet itemDisplayRuleSet = characterModel.itemDisplayRuleSet;
                if (!itemDisplayRuleSet) continue;
                ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;
                if (item.Length == 0) continue;
                sb.AppendLine("====Body: " + bodyPrefab.name);

                for (int i = 0; i < item.Length; i++)
                {
                    ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;
                    for (int j = 0; j < rules.Length; j++)
                    {
                        ItemDisplayRule itemDisplayRule = rules[j];
                        GameObject followerPrefab = itemDisplayRule.followerPrefab;
                        if (followerPrefab)
                        {
                            string name = followerPrefab.name;
                            string key = name?.ToLower();
                            if (!itemDisplayPrefabs.ContainsKey(key))
                            {
                                itemDisplayPrefabs[key] = followerPrefab;
                                sb.AppendLine(followerPrefab.name);
                            }
                        }
                    }
                }
            }
            Debug.Log(sb.ToString());
            HG.StringBuilderPool.ReturnStringBuilder(sb);
        }




        //This isn't used for anything, unless you want to modify it for that, this is for copypasting
        //For the ones with group displays, that's a pick and choose for some, like Bear vs BearSit, and the lighting and thorns shit
        //special ones have a note above them,
        public static void DefaultItemDisplays()
        {
            /*
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringe", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Syringe, "DisplaySyringeCluster", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [SYRINGE] = Choose one, but you might just only need SyringeCluster
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Syringe,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySyringe"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySyringeCluster"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBear", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bear, "DisplayBearSit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [BEAR] = Choose one
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Bear,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBear"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBearSit"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Behemoth, "DisplayBehemoth", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Missile, "DisplayMissileLauncher", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExplodeOnDeath, "DisplayWilloWisp", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Dagger, "DisplayDagger", "Head", Vector3.zero, Vector3.zero, Vector3.one));

            //IDRS NOTE: [TOOTH] = Decal is the 'string' of the necklace and its a projected decal.
            //IDRS NOTE: [TOOTH] = Order is: Small1, Small1, Large, Small2, Small2 on the character
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
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshLarge"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall1"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayToothMeshSmall2"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });
            //might also be able to just do this but thats not the spirit of it
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Tooth, RoR2/Base/Tooth/mdlToothNecklaceDisplay.fbx
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.CritGlasses, "DisplayGlasses", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Hoof, "DisplayHoof", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Feather, "DisplayFeather", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ChainLightning, "DisplayUkulele", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Seed, "DisplaySeed", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Icicle, "DisplayFrostRelic", Vector3.zero, Vector3.zero, Vector3.one));
            //this seems used for the ice elite? then why that asset path? 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Icicle, "DisplayIcicle", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GhostOnKill, "DisplayMask", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Mushroom, "DisplayMushroom", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Crowbar, "DisplayCrowbar", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AttackSpeedOnCrit, "DisplayWolfPelt", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHit, "DisplayTriTip", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintOutOfCombat, "DisplayWhip", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FallBoots, "DisplayGravBoots", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WardOnLevel, "DisplayWarbanner", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Phasing, "DisplayStealthkit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealOnCrit, "DisplayScythe", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HealWhileSafe, "DisplaySnail", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.PersonalShield, "DisplayShieldGenerator", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EquipmentMagazine, "DisplayBattery", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.NovaOnHeal, "DisplayDevilHorns", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShockNearby, "DisplayTeslaCoil", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Infusion, "DisplayInfusion", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Clover, "DisplayClover", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Medkit, "DisplayMedkit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Bandolier, "DisplayBandolier", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BounceNearby, "DisplayHook", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IgniteOnKill, "DisplayGasoline", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StunChanceOnHit, "DisplayStunGrenade", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Firework, "DisplayFirework", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarDagger, "DisplayLunarDagger", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.GoldOnHit, "DisplayBoneCrown", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.WarCryOnMultiKill, "DisplayPauldron", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Items.ShieldOnly, "DisplayShieldBug", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AlienHead, "DisplayAlienHead", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.Talisman, "DisplayTalisman", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Knurl, "DisplayKnurl", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BeetleGland, "DisplayBeetleGland", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintBonus, "DisplaySoda", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SecondarySkillMagazine, "DisplayDoubleMag", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.StickyBomb, "DisplayStickyBomb", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TreasureCache, "DisplayKey", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BossDamageBonus, "DisplayAPRound", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintArmor, "DisplayBuckler", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.IceRing, "DisplayIceRing", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireRing, "DisplayFireRing", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SlowOnHit, "DisplayBauble", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExtraLife, "DisplayHippo", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburner", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.UtilitySkillMagazine, "DisplayAfterburnerShoulderRing", "Head", Vector3.zero, Vector3.zero, Vector3.one));
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
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayAfterburner"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayAfterburnerShoulderRing"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.HeadHunter, "DisplaySkullcrown", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.KillEliteFrenzy, "DisplayBrainstalk", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RepeatHeal, "DisplayCorpseflower", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.AutoCastEquipment, "DisplayFossil", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateMirroredDisplayRule(RoR2Content.Items.IncreaseHealing, "DisplayAntler", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.JumpBoost, "DisplayWaxBird", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ExecuteLowHealthElite, "DisplayGuillotine", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.EnergizedOnEquipmentUse, "DisplayWarHorn", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnOverHeal, "DisplayAegis", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TitanGoldDuringTP, "DisplayGoldHeart", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SprintWisp, "DisplayBrokenMask", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BarrierOnKill, "DisplayBrooch", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorReductionOnHit, "DisplayWarhammer", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.TPHealingNova, "DisplayGlowFlower", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NearbyDamageBonus, "DisplayDiamond", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarUtilityReplacement, "DisplayBirdFoot", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonsoonPlayerHelper, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireCoiled", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeft", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireLeftVoidSurvivor", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Thorns, "DisplayRazorwireRight", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [THORNS] = Choose one, Left and Right have a LimbMatcher
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.Thorns,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayRazorwireCoiled"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayRazorwireLeft"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayRazorwireLeftVoidSurvivor"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayRazorwireRight"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                    }
                }
            });
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakCurved", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FlatHealth, "DisplaySteakFlat", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [FLATHEALTH] = Choose one
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = RoR2Content.Items.FlatHealth,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySteakCurved"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySteakFlat"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Pearl, "DisplayPearl", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ShinyPearl, "DisplayShinyPearl", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BonusGoldPackOnKill, "DisplayTome", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LaserTurbine, "DisplayLaserTurbine", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarPrimaryReplacement, "DisplayBirdEye", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.NovaOnLowHealth, "DisplayJellyGuts", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarTrinket, "DisplayBeads", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.InvadingDoppelganger, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ArmorPlate, "DisplayRepulsionArmorPlate", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Squid, "DisplaySquidTurret", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.DeathMark, "DisplayDeathMark", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.Plant, "DisplayInterstellarDeskPlant", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Items.FocusConvergence, "DisplayFocusedConvergence", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.FireballsOnHit, "DisplayFireballsOnHit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LightningStrikeOnHit, "DisplayChargedPerforator", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.BleedOnHitAndExplode, "DisplayBleedOnHitAndExplode", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SiphonOnLowHealth, "DisplaySiphonOnLowHealth", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.MonstersOnShrineUse, "DisplayMonstersOnShrineUse", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RandomDamageZone, "DisplayRandomDamageZone", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSecondaryReplacement, "DisplayBirdClaw", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.LunarSpecialReplacement, "DisplayBirdHeart", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.RoboBallBuddy, "DisplayEmpathyChip", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.ParentEgg, "DisplayParentEgg", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Items.SummonedEcho, 
            
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CommandMissile, "DisplayMissileRack", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Fruit, "DisplayFruit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Meteor, "DisplayMeteor", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateZMirroredDisplayRule(RoR2Content.Equipment.AffixRed, "DisplayEliteHorn", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixBlue, "DisplayEliteRhinoHorn", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixWhite, "DisplayEliteIceCrown", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixPoison, "DisplayEliteUrchinCrown", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Blackhole, "DisplayGravCube", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CritOnUse, "DisplayNeuralImplant", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DroneBackup, "DisplayRadio", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BFG, "DisplayBFG", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Jetpack, "DisplayBugWings", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            
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
            });

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GoldGat, "DisplayGoldGat", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LunarPotion, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.BurnNearby, "DisplayPotion", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Scanner, "DisplayScanner", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.CrippleWard, "DisplayEffigy", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Gateway, "DisplayVase", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Tonic, "DisplayTonic", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.QuestVolatileBattery, "DisplayBatteryArray", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Cleanse, "DisplayWaterPack", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.FireBallDash, "DisplayEgg", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixHaunted, "DisplayEliteStealthCrown", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.GainArmor, "DisplayElephantFigure", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(RoR2Content.Equipment.Saw, "DisplaySawmerang", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.Recycle, "DisplayRecycler", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.LifestealOnHit, "DisplayLifestealOnHit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.TeamWarCry, "DisplayTeamWarCry", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.DeathProjectile, "DisplayDeathProjectile", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixEcho, 
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar, Fire", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(RoR2Content.Equipment.AffixLunar, "DisplayEliteLunar,Eye", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [AFFIXLUNAR] = I don't think EliteLunar, Fire is used?

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoveSpeedOnKill, "DisplayGrappleHook", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotion, "DisplayHealingPotion", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HealingPotionConsumed, 
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PermanentDebuffOnHit, "DisplayScorpion", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.AttackSpeedAndMoveSpeed, "DisplayCoffee", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritDamage, "DisplayLaserSight", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BearVoid, "DisplayBearVoidSit", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //IDRS NOTE: [BEARVOID] = Choose one.
            itemRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = DLC1Content.Items.BearVoid,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBearVoid"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayBearVoidSit"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MushroomVoid, "DisplayMushroomVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CloverVoid, "DisplayCloverVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.StrengthenBurn, "DisplayGasTank", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RegeneratingScrap, "DisplayRegeneratingScrap", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.BleedOnHitVoid, "DisplayTriTipVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.CritGlassesVoid, "DisplayGlassesVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.TreasureCacheVoid, "DisplayKeyVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.SlowOnHitVoid, "DisplayBaubleVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MissileVoid, "DisplayMissileLauncherVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ChainLightningVoid, "DisplayUkuleleVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExtraLifeVoid, "DisplayHippoVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.EquipmentMagazineVoid, "DisplayFuelCellVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ExplodeOnDeathVoid, "DisplayWillowWispVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FragileDamageBonus, "DisplayDelicateWatch", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.OutOfCombatArmor, "DisplayOddlyShapedOpal", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.MoreMissile, "DisplayICBM", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ImmuneToDebuff, "DisplayRainCoatBelt", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.RandomEquipmentTrigger, "DisplayBottledChaos", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.PrimarySkillShuriken, "DisplayShuriken", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.RandomlyLunar, "DisplayDomino", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.GoldOnHurt, "DisplayRollOfPennies", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfAttackSpeedHalfCooldowns, "DisplayLunarShoulderNature", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.HalfSpeedDoubleHealth, "DisplayLunarShoulderStone", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.FreeChest, "DisplayShippingRequestForm", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.ElementalRingVoid, "DisplayVoidRing", "Head", Vector3.zero, Vector3.zero, Vector3.one));
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
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplaySunHeadNeck"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
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
            itemRules.Add(ItemDisplayCore.CreateFollowerDisplayRule(DLC1Content.Items.MinorConstructOnKill, "DisplayDefenseNucleus", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Items.VoidMegaCrabItem, "DisplayMegaCrabItem", "Head", Vector3.zero, Vector3.zero, Vector3.one));

            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.Molotov, "DisplayMolotov", "Head", Vector3.zero, Vector3.zero, Vector3.one));
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
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayVendingMachine"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayVendingMachine2"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
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
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplayCore.LoadDisplay("DisplayTricornGhost"),
                            childName = "Head",
							localPos = Vector3.zero,
							localAngles = Vector3.zero,
							localScale = Vector3.one,
                            limbMask = LimbFlags.None
                        }
                    }
                }
            });
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.BossHunterConsumed, "DisplayTricornUsed", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.GummyClone, "DisplayGummyClone", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.MultiShopCard, "DisplayExecutiveCard", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.LunarPortalOnUse, "DisplayLunarPortalOnUse", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Equipment.EliteVoidEquipment, "DisplayAffixVoid", "Head", Vector3.zero, Vector3.zero, Vector3.one));
            //from claymen git
            itemRules.Add(ItemDisplayCore.CreateGenericDisplayRule(DLC1Content.Elites.Earth.eliteEquipmentDef, "DisplayEliteMendingAntlers", "Head", Vector3.zero, Vector3.zero, Vector3.one));
             */
        }
    }
}