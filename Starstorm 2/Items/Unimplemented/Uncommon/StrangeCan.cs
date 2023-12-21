using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Items
{
    class StrangeCan : SS2Item<StrangeCan>
    {
        public override string NameInternal => "PoisonOnHit";
        public override string Name => "Strange Can";
        public override string Pickup => "Chance to intoxicate enemies, causing damage over time.";
        public override string Description => $"<style=cIsDamage>{StaticValues.canBaseChance}%</style> <style=cStack>(+{StaticValues.canStackChance}% per stack)</style> chance to <style=cIsDamage>intoxicate</style> enemies dealing <style=cIsDamage>{StaticValues.canDuration * StaticValues.canDamage * 100}% damage over time</style>.";
        public override string Lore => "Two figures sit, one shivering among a sea of rubble in a frozen burrow. The only lights given are the small remains of a fire, and old sparking wires. Barely any protection against the biting cold.\n\n\"Hey...\" He looks up, distantly hopeful. \"Do you think we'll ever make it off this rock?\"\n\n\"Hard to say.\" She takes a bite of a ration she found.\n\n\"Well there has to be something we've overlooked, right? Some method we just haven't thought of yet?\" He stands up, and begins pacing around anxiously.\n\n\"Could be.\" Another bite.\n\n\"Maybe there's some wreckage with working parts we just haven't found yet, or a problem in the current ones we could fix?\" A shine returning to his eyes, getting himself more hopeful and excited.\n\n\"Definitely possible.\" Another bite.\n\nHe stops as a disgusted look crosses his face. \"Ugh, could you please help me think of something instead of just eating?\"\n\n\"I wouldn't worry about it.\" Another bite.\n\nHis disgust quickly gives way to blatant frustration and anger. \"Why not?! Don't you want to get out of here?!\"\n\nShe sighs, and runs a hand through her hair. \"Well sure. But that doesn't mean we've gotta stress over every little thing. We're alive, ain't we? We're lucky we have that much.\"\n\n\"Well... I suppose you're right... It'd be bad to get worked up right now...\" His expression relaxes as he looks back to the ground.\n\n\"See? Just sit down, relax, and let my old stash warm you up.\" Another bite, as she passes the can.\n";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "StrangeCan_Icon";
        public override string PickupModelPath => "MDLStrangeCan";

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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
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
                    childName = "Pelvis",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            return rules;
        }


        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            GameObject attacker = damageInfo.attacker;
            if (self && attacker)
            {
                var attackerBody = attacker.GetComponent<CharacterBody>();
                var victimBody = victim.GetComponent<CharacterBody>();
                int canCount = GetCount(attackerBody);
                if (canCount > 0)
                {
                    bool flag = (damageInfo.damageType & DamageType.PoisonOnHit) > DamageType.Generic;
                    if ((canCount > 0 || flag) && (flag || Util.CheckRoll((StaticValues.canBaseChance + (StaticValues.canDamage * (float)canCount)) * damageInfo.procCoefficient, attackerBody.master)))
                    {
                        ProcChainMask procChainMask = damageInfo.procChainMask;
                        procChainMask.AddProc(ProcType.BleedOnHit);
                        var dotInfo = new InflictDotInfo()
                        {
                            attackerObject = attacker,
                            victimObject = victim,
                            dotIndex = DoTCore.StrangeCanPoison,
                            duration = StaticValues.canDuration,
                            damageMultiplier = StaticValues.canDamage
                        };
                        DotController.InflictDot(ref dotInfo);
                    }
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}
