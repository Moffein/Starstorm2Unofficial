using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;

//TODO: i don't think this works for non-host in multiplayer

namespace Starstorm2Unofficial.Cores.Items
{
    class BrokenBloodTester : SS2Item<BrokenBloodTester>
    {
        private static float healAccum = 0f;
        public override string NameInternal => "BloodTester";
        public override string Name => "Broken Blood Tester";
        public override string Pickup => "Gain money on healing.";
        public override string Description => $"Gain <style=cIsUtility>{StaticValues.testerGold} gold</style> <style=cStack>(+{StaticValues.testerGold} gold per stack)</style> for every <style=cIsHealing>{StaticValues.testerHealing} hp</style> you heal.";
        public override string Lore => "Return: Broken Blood Tester\nTracking Number: 288*****\nEstimated Delivery: 06/17/2056\nShipping Method: Priority\nShipping Address: Dionysus Pharmaceuticals Headquarters, Pluto\nShipping Details:\n\nWe're going to need this one repaired before it does our company more harm than good. This thing kept giving us the wrong results every time our MED-E used it. Not only does it clog our workflow, we wind up having to pay out of pocket to the patients since we wrongfully diagnose them, and stick them with needles needlessly. And we can't claim it as the patient being wrong since it's all immediately backed up to the servers. We can't keep shelling out this kind of money, or we'll be bankrupt by next quarter.\n";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Utility,
            ItemTag.AIBlacklist
        };
        public override string PickupIconPath => "BloodTester_Icon";
        public override string PickupModelPath => "MDLBloodTester";

        public override void RegisterHooks()
        {
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
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
                    childName = "Pelvis",
localPos = new Vector3(0.1824F, -0.0858F, -0.001F),
localAngles = new Vector3(2.6649F, 75.6855F, 176.4035F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(-0.0062F, -0.0952F, 0.1152F),
localAngles = new Vector3(10.1501F, 0.3952F, 179.1961F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.2131F, -0.0993F, -0.043F),
localAngles = new Vector3(4.834F, 95.4655F, 199.7518F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1444F, 0.0053F, -0.1933F),
localAngles = new Vector3(2.673F, 161.2628F, 179.8488F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.2053F, 0.0275F, -0.0637F),
localAngles = new Vector3(27.8597F, 97.9709F, 176.7718F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.0023F, -0.0208F, -0.353F),
localAngles = new Vector3(352.9314F, 180.0029F, 359.9534F),
localScale = new Vector3(0.188F, 0.188F, 0.188F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.0751F, -0.1042F, -0.2108F),
localAngles = new Vector3(343.0136F, 184.803F, 180.5884F),
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
localPos = new Vector3(1.9841F, 2.9825F, 1.9692F),
localAngles = new Vector3(0F, 85.607F, 0F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "WeaponPlatformEnd",
localPos = new Vector3(0.1825F, -0.1119F, 0.2563F),
localAngles = new Vector3(0.5702F, 90.3798F, 88.3474F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "LowerArmL",
localPos = new Vector3(-1.1987F, 3.4378F, -0.4143F),
localAngles = new Vector3(353.7809F, 247.0507F, 358.5906F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });

            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.0017F, 0F, -0.0021F),
localAngles = new Vector3(2.5642F, 150.8249F, 176.8134F),
localScale = new Vector3(0.0015F, 0.0015F, 0.0015F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.0013F, -0.0006F, -0.0016F),
localAngles = new Vector3(6.0814F, 148.3985F, 175.9165F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            return rules;
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self)
            {
                var body = self.body;
                if (body)
                {
                    var bloodTesters = GetCount(body);
                    var amountToGift = bloodTesters * StaticValues.testerGold;
                    if (bloodTesters > 0 && !(Run.instance.isRunStopwatchPaused))
                    {
                        var tele = TeleporterInteraction.instance;
                        //stop giving money if we're waiting to teleport (prevents a softlock)
                        if (!tele || !tele.isCharged)
                        {
                            if (body.teamComponent && body.isPlayerControlled && self.health < self.fullHealth && nonRegen == true)
                            {
                                healAccum += amount;
                            }
                        }
                        if (healAccum >= StaticValues.testerHealing)
                        {
                            healAccum -= StaticValues.testerHealing;
                            //sound/vfx here
                            body.master.GiveMoney((uint)amountToGift);
                        }
                    }
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            int coffeeCount = GetCount(self);
            self.moveSpeed += self.baseMoveSpeed * 0.1f * coffeeCount;
            self.attackSpeed += self.baseAttackSpeed * 0.07f * coffeeCount;
            //★ ...what?
        }
    }
}