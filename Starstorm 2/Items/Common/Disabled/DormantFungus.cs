using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Items
{
    class DormantFungus : SS2Item<DormantFungus>
    {
        public override string NameInternal => "SS2U_Dungus";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Healing,
            ItemTag.SprintRelated,
            ItemTag.AIBlacklist,
            ItemTag.FoodRelated,
            ItemTag.CanBeTemporary
        };
        public override string PickupIconPath => "DormantFungus_Icon";
        public override string PickupModelPath => "MDLDormantFungus";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
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
                    childName = "UpperArmL",
localPos = new Vector3(0.0534F, 0.0483F, 0.1874F),
localAngles = new Vector3(303.7209F, 98.2102F, 101.9909F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.2236F, -0.0119F, -0.0677F),
localAngles = new Vector3(328.9899F, 191.0635F, 127.5044F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.1692F, 0.0751F, -0.0135F),
localAngles = new Vector3(335.7093F, 201.6389F, 86.6875F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.2328F, 0.0785F, 0.0293F),
localAngles = new Vector3(76.4825F, 42.1719F, 337.1561F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.2783F, 0.066F, 0.1656F),
localAngles = new Vector3(25.532F, 151.5236F, 98.2747F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.0931F, 0.2321F, -0.1714F),
localAngles = new Vector3(333.906F, 248.7815F, 91.473F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(0.2454F, 0.051F, -0.1518F),
localAngles = new Vector3(351.9315F, 209.8458F, 97.4965F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(3.0099F, 2.7818F, 3.2627F),
localAngles = new Vector3(336.3833F, 303.6277F, 312.8879F),
localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "PlatformBase",
localPos = new Vector3(0.3147F, -0.167F, 0.7414F),
localAngles = new Vector3(303.7524F, 279.8964F, 292.4984F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "UpperArmL",
localPos = new Vector3(-1.9929F, -2.4683F, 1.0449F),
localAngles = new Vector3(47.8033F, 24.9564F, 112.5967F),
localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
            });
            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ShoulderL",
localPos = new Vector3(0.0018F, -0.001F, 0.0006F),
localAngles = new Vector3(313.9065F, 139.7861F, 135.2533F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ShoulderL",
localPos = new Vector3(0.0026F, -0.0002F, 0.0005F),
localAngles = new Vector3(2.1902F, 170.2704F, 117.2842F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            return rules;
        }


        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                self.AddItemBehavior<DormantFungusBehavior>(GetCount(self));
            }
            orig(self);
        }
    }

    public class DormantFungusBehavior : CharacterBody.ItemBehavior
    {
        private HealthComponent healthComponent;
        private float timer;

        public void Awake()
        {
            body = gameObject.GetComponent<CharacterBody>();
            healthComponent = gameObject.GetComponent<HealthComponent>();
        }

        public void FixedUpdate()
        {
            if (!NetworkServer.active || !healthComponent) return;
            if (body.isSprinting)
            {
                timer += Time.deltaTime;
                if (timer >= 1f)
                {
                    healthComponent.HealFraction(0.006f + 0.006f * stack, default);
                    timer = 0;
                }
            }
            else
            {
                timer = 0;
            }
        }
    }
}
