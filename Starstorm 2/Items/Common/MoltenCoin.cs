using RoR2;
using UnityEngine;
using Starstorm2Unofficial.Cores;
using R2API;
using Starstorm2Unofficial.Modules;
using static UnityEngine.Rendering.DebugUI;

namespace Starstorm2Unofficial.Cores.Items
{
    class MoltenCoin : SS2Item<MoltenCoin>
    {
        public static NetworkSoundEventDef procSound;
        public override string NameInternal => "SS2U_MoltenCoin";
        public override string Name => "Molten Coin";
        public override string Pickup => "Chance to ignite and earn $1 on hit.";
        public override string Description => "<style=cIsDamage>6%</style> chance to <style=cIsDamage>ignite</style> on hit for <style=cIsDamage>320%</style> base damage <style=cStack>(+320% per stack)</style> and earn <style=cIsUtility>$1</style>. Scales with time.";
        public override string Lore => "Order: Molten Coin\nTracking Number: 446*****\nEstimated Delivery: 09/22/2056\nShipping Method: Standard\nShipping Address: Venetian Police Department, Brithen, Venus\nShipping Details:\n\nHey, I found another piece of evidence relating to that bombing at Parathan Square a couple days ago.\n\nSo, you know how the investigators had to postpone their analysis because the area was way too hot? Well, the space finally cooled off this morning, and I managed to find this melted coin stuck to the pavement. A quick chemical test says it's probably made mostly of tungsten with a gold coating, and it also hasn't cooled off since I peeled it off the ground.\n\nI'm not really sure what it could mean, though. Maybe a good luck charm, or a calling card? Either way, I'm sending it to you, since, quite frankly, you need all the help you can get. I mean, 7 people dead, 18 injured, nearly half a million in property damages, and all you've got for evidence is a melted coin and a blown-up cafe? Well, you've got my sympathy, man. Good luck on the case.\n";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage,
            ItemTag.Utility,
            ItemTag.AIBlacklist //Base Damage = instakill players
        };
        public override string PickupIconPath => "MoltenCoin_Icon";
        public override string PickupModelPath => "MDLMoltenCoin";

        protected override void RegisterItem()
        {
            base.RegisterItem();

            //object???.GetComponentInChildren<renderer>().material = AssetsCore.CreateMaterial("MoltenCoinDiffuse2");
            procSound = Assets.CreateNetworkSoundEventDef("SS2UMoltenCoin");
        }

        public override void RegisterHooks()
        {
            SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += ProcCoin;
        }

        public override ItemDisplayRuleDict CreateDisplayRules()
        {
            displayPrefab = Assets.mainAssetBundle.LoadAsset<GameObject>(PickupModelPath);
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

        private void ProcCoin(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
        {
            int itemCount = attackerInventory.GetItemCount(itemDef);
            CharacterMaster attackerMaster = attackerBody.master;
            if (itemCount <= 0 || !Util.CheckRoll(6f * damageInfo.procCoefficient, attackerMaster)) return;

            EffectManager.SimpleSoundEffect(procSound.index, damageInfo.position, true);

            //Deal Damage
            InflictDotInfo inflictDotInfo = new InflictDotInfo
            {
                victimObject = victimBody.gameObject,
                attackerObject = damageInfo.attacker,
                totalDamage = new float?(attackerBody.damage * 3.2f * itemCount),
                dotIndex = DotController.DotIndex.Burn,
                damageMultiplier = 0.75f + 0.25f * itemCount    //speed up damage application with stacks, QoL
            };
            StrengthenBurnUtils.CheckDotForUpgrade(attackerInventory, ref inflictDotInfo);
            DotController.InflictDot(ref inflictDotInfo);

            //Give Money
            if (attackerMaster && !BazaarChecker.InBazaar())
            {
                int money = 1;
                if (Run.instance && Stage.instance) money = Run.instance.GetDifficultyScaledCost(1, Stage.instance.entryDifficultyCoefficient);
                attackerMaster.GiveMoney((uint)money);
            }
        }
    }
}
