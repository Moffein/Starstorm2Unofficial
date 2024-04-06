using R2API;
using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Items
{
    class DetritiveTrematode : SS2Item<DetritiveTrematode>
    {
        public override string NameInternal => "SS2U_Trematode";
        //public override string Lore => "<style=cMono>$ Transcribing image... done.\n$ Resolving... done.\n$ Outputting text strings... done.\nComplete!\n\n\n</style>This is the logbook of D. Furthen, naturalist aboard the UES [Redacted] in conjunction with the ongoing UES Research and Documentation of Outer Life program. This entry was written at Point Sigma, during an expedition to the dense jungles of Cornea III.\n\n---------------------\n\nI have encountered my first sample of outer life on this planet! What a marvel it is, too. The sample resembles an earthen roundworm, but significantly bigger. I have assigned their common name as 'Detritive Trematode', and will describe their properties below.\n\n• Roughly two inches in diameter, and varying in size from about four inches to a foot and a half in length. White coloration.\n\n• These trematodes were found feasting on a pile of rotting meat in a damp cavern. The meat was of indeterminate origin, but the presence of other, larger animals is exciting nonetheless.\n\n• Upon scooping up a clump of the rotting meat, the trematodes quickly reacted to the movement and began wriggling around, before slowly relaxing. The guard positioned with me, Chevry, I think, was disgusted.\n\n• Upon touching one with an ungloved hand to feel their texture, one of the trematodes latched onto my palm and began chewing. It was extremely painful, both to have the trematode on my hand, and when Chevry cut the thing off. Despite that, other than a light gash on my palm, I believe I am fine.\n";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage, ItemTag.AIBlacklist
        };
        public override string PickupIconPath => "DetritiveTrematode_Icon";
        public override string PickupModelPath => "MDLDetritiveTrematode";

        public override void RegisterHooks()
        {
            SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += ProcItem;
        }

        private void ProcItem(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
        {
            int itemCount = attackerInventory.GetItemCount(itemDef);
            if (itemCount <= 0) return;

            if (!victimBody.healthComponent || victimBody.healthComponent.combinedHealthFraction > 0.25f) return;
            for (int i = 0; i < itemCount; i++)
            {
                var dotInfo = new InflictDotInfo()
                {
                    attackerObject = attackerBody.gameObject,
                    victimObject = victimBody.gameObject,
                    dotIndex = DoTCore.DetritiveTrematodeInfection,
                    duration = 600000f,
                    damageMultiplier = 1f,
                    maxStacksFromAttacker = (uint)itemCount
                };
                DotController.InflictDot(ref dotInfo);
            }
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

            // removed my helpers, king's idrs tool makes it much easier to simply copy paste the values in
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
    }
}