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
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage,
            ItemTag.Utility,
            ItemTag.AIBlacklist, //Base Damage = instakill players
            ItemTag.CanBeTemporary
        };
        public override string PickupIconPath => "MoltenCoin_Icon";
        public override string PickupModelPath => "MDLMoltenCoin";

        protected override void RegisterItem()
        {
            base.RegisterItem();

            //object???.GetComponentInChildren<renderer>().material = AssetsCore.CreateMaterial("MoltenCoinDiffuse2");
            procSound = Starstorm2Unofficial.Modules.Assets.CreateNetworkSoundEventDef("SS2UMoltenCoin");
        }

        public override void RegisterHooks()
        {
            SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += ProcCoin;
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
