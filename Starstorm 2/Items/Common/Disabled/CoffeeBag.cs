using R2API;
using RoR2;
using System;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Items
{
    class CoffeeBag : SS2Item<CoffeeBag>
    {
        public override string NameInternal => "SS2U_Coffee";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage,
            ItemTag.Utility
        };
        public override string PickupIconPath => "CoffeeBag_Icon";
        public override string PickupModelPath => "MDLCoffeeBag";

        public override void RegisterHooks()
        {
            SharedHooks.GetStatCoefficients.HandleStatsInventoryActions += HandleStats;
        }

        public override ItemDisplayRuleDict CreateDisplayRules()
        {
            displayPrefab = LegacyResourcesAPI.Load<GameObject>(PickupModelPath);
            var disp = displayPrefab.AddComponent<ItemDisplay>();
            disp.rendererInfos = Utils.SetupRendererInfos(displayPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.15f, 0.07f, 0.1f),
                    localAngles = new Vector3(0f, 65f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.1f, 0.16f, 0.15f),
                    localAngles = new Vector3(0f, 50f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.08f, 0.16f, 0.2f),
                    localAngles = new Vector3(0f, 30f, 180f),
                    localScale = new Vector3(0.08f, 0.08f, 0.08f)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0f, 0.16f, 0.2f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.12f, 0.2f, 0.12f),
                    localAngles = new Vector3(0f, 40f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0f, 0.2f, 0.24f),
                    localAngles = new Vector3(0f, 0f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.08f, 0.1f, 0.2f),
                    localAngles = new Vector3(0f, 30f, 180f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0f, 0.29f, 1f),
                    localAngles = new Vector3(0f, 0f, 250f),
                    localScale = new Vector3(1f, 1f, 1f)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighFrontL",
                    localPos = new Vector3(-0.0129F, 0F, -0.05F),
                    localAngles = new Vector3(0F, 220F, 180F),
                    localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(1f, -0.6f, -0.5f),
                    localAngles = new Vector3(0f, 90f, 130f),
                    localScale = new Vector3(1f, 1f, 1f)
                }
            });

            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.0002F, 0.0019F, -0.0016F),
                    localAngles = new Vector3(0F, 180F, 180F),
                    localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.0013F, 0.0018F, 0.0007F),
                    localAngles = new Vector3(0F, 65F, 180F),
                    localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            return rules;
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            int itemCount = inventory.GetItemCount(itemDef);
            args.moveSpeedMultAdd += 0.07f * itemCount;
            args.attackSpeedMultAdd += 0.075f * itemCount;
        }
    }
}