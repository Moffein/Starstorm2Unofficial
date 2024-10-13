﻿using R2API;
using RoR2;
using Starstorm2Unofficial.Modules;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Items
{
    class Fork : SS2Item<Fork>
    {
        public override string NameInternal => "SS2U_Fork";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "Fork_Icon";
        public override string PickupModelPath => "MDLFork";

        public override void RegisterHooks()
        {
            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += HandleStats;
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int itemCount = inventory.GetItemCount(itemDef);
            args.damageMultAdd += 0.05f * itemCount;
        }

        public override ItemDisplayRuleDict CreateDisplayRules()
        {
            displayPrefab = Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(PickupModelPath);
            var disp = displayPrefab.AddComponent<ItemDisplay>();
            disp.rendererInfos = Utils.SetupRendererInfos(displayPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-0.1586F, 0.3278F, 0.3603F),
localAngles = new Vector3(47.3573F, 332.2906F, 198.0251F),
localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-0.1043F, 0.1095F, 0.3005F),
localAngles = new Vector3(42.8143F, 58.2528F, 291.6888F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.1392F, 0.1651F, 0.1949F),
localAngles = new Vector3(45.0796F, 21.4808F, 194.4862F),
localScale = new Vector3(0.03F, 0.03F, 0.03F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.1973F, 0.1404F, 0.3352F),
localAngles = new Vector3(83.942F, 51.1569F, 227.1741F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.1097F, 0.1232F, 0.2816F),
localAngles = new Vector3(78.4158F, 160.9205F, 348.3299F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.1371F, 0.3891F, 0.1955F),
localAngles = new Vector3(18.5425F, 33.4606F, 213.1825F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-1.2061F, 2.1674F, 2.7863F),
localAngles = new Vector3(7.2876F, 340.4511F, 220.5762F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "FlowerBase",
localPos = new Vector3(-0.7234F, 1.4397F, 0F),
localAngles = new Vector3(0F, 320.333F, 215.4378F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-1.3477F, 0.7851F, -3.2079F),
localAngles = new Vector3(39.0607F, 243.0906F, 267.9274F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });

            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "GunModel",
localPos = new Vector3(-0.0086F, -0.0012F, 0.0417F),
localAngles = new Vector3(344.166F, 88.7058F, 110.4904F),
localScale = new Vector3(0.004F, 0.004F, 0.004F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Sword",
localPos = new Vector3(-0.001F, 0.0146F, 0F),
localAngles = new Vector3(23.7289F, 33.6548F, 27.8921F),
localScale = new Vector3(0.0006F, 0.0006F, 0.0006F)
                }
            });

            return rules;
        }
    }
}
