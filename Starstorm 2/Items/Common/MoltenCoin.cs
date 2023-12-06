using RoR2;
using UnityEngine;
using Starstorm2Unofficial.Cores;
using R2API;

namespace Starstorm2Unofficial.Cores.Items
{
    class MoltenCoin : SS2Item<MoltenCoin>
    {
        public override string NameInternal => "Coin";
        public override string Name => "Molten Coin";
        public override string Pickup => "Chance to incinerate for gold on hit.";
        public override string Description => $"<style=cIsDamage>{StaticValues.coinChance}%</style> chance to <style=cIsDamage>ignite</style> enemies on hit for <style=cIsDamage>200%</style> base damage <style=cStack>(+50% per stack)</style> and earn <style=cIsUtility>${StaticValues.coinMoneyGained}</style> <style=cStack>(+${StaticValues.coinMoneyGained} per stack, scales with stages cleared).";
        public override string Lore => "Order: Molten Coin\nTracking Number: 446*****\nEstimated Delivery: 09/22/2056\nShipping Method: Standard\nShipping Address: Venetian Police Department, Brithen, Venus\nShipping Details:\n\nHey, I found another piece of evidence relating to that bombing at Parathan Square a couple days ago.\n\nSo, you know how the investigators had to postpone their analysis because the area was way too hot? Well, the space finally cooled off this morning, and I managed to find this melted coin stuck to the pavement. A quick chemical test says it's probably made mostly of tungsten with a gold coating, and it also hasn't cooled off since I peeled it off the ground.\n\nI'm not really sure what it could mean, though. Maybe a good luck charm, or a calling card? Either way, I'm sending it to you, since, quite frankly, you need all the help you can get. I mean, 7 people dead, 18 injured, nearly half a million in property damages, and all you've got for evidence is a melted coin and a blown-up cafe? Well, you've got my sympathy, man. Good luck on the case.\n";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage,
            ItemTag.Utility
        };
        public override string PickupIconPath => "MoltenCoin_Icon";
        public override string PickupModelPath => "MDLMoltenCoin";

        protected override void RegisterItem()
        {
            base.RegisterItem();

            //object???.GetComponentInChildren<renderer>().material = AssetsCore.CreateMaterial("MoltenCoinDiffuse2");
        }

        public override void RegisterHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
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
                    localPos = new Vector3(0.12f, 0.16f, 0.2f),
                    localAngles = new Vector3(90f, 30f, 0f),
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
                    localPos = new Vector3(0.13f, 0.16f, 0.26f),
                    localAngles = new Vector3(90f, 30f, 0f),
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
                    localPos = new Vector3(0.15f, 0.16f, 0.24f),
                    localAngles = new Vector3(90f, 40f, 0f),
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
                    localPos = new Vector3(0.12f, 0.16f, 0.23f),
                    localAngles = new Vector3(90f, 30f, 0f),
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
                    childName = "ThighL",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
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
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
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
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ThighL",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            return rules;
        }


        protected override void SetupMaterials(GameObject modelPrefab) {

            modelPrefab.GetComponentInChildren<Renderer>().material = Modules.Assets.CreateMaterial("matMoltenCoin", 1, Color.yellow, 0);
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            var attacker = damageInfo.attacker;
            if (attacker && victim)
            {
                var attackerBody = attacker.GetComponent<CharacterBody>();
                int coinCount = GetCount(attackerBody);
                if (coinCount > 0 && Util.CheckRoll(StaticValues.coinChance, attackerBody.master))
                {
                    if (damageInfo.procCoefficient > 0)
                    {
                        var dotInfo = new InflictDotInfo()
                        {
                            attackerObject = attacker,
                            victimObject = victim,
                            dotIndex = DotController.DotIndex.Burn,
                            duration = damageInfo.procCoefficient * StaticValues.coinDuration,
                            damageMultiplier = coinCount * StaticValues.coinDamage
                            //If you're trying to configure this and are so desperate you've come here, I don't have a damn clue.
                        };
                        DotController.InflictDot(ref dotInfo);
                        attackerBody.master.GiveMoney((uint)(coinCount * (Run.instance.stageClearCount + (1 * StaticValues.coinMoneyGained))));
                        Util.PlaySound("SS2UMoltenCoin", victim.gameObject);
                    }
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}
