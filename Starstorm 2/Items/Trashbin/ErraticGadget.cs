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
    class ErraticGadget : SS2Item<ErraticGadget>
    {
        public override string NameInternal => "ErraticGadget";
        public override string Name => "Erratic Gadget";
        public override string Pickup => "Critical strikes deal extra damage.";
        public override string Description => $"Gain <style=cIsDamage>{StaticValues.gadgetCrit}% critical chance</style>. Critical strikes deal <style=cIsDamage>{StaticValues.gadgetDamage * 100}%</style> <style=cStack>(+{StaticValues.gadgetDamage * 100}% per stack)</style> extra damage.";
        public override string Lore => "<style=cMono>==========================\n========  CallCutter ========\n===== ALPHA 0.1354.1.12 =====\n==========================\n\nCutting into call TdiM3c4fcQQA.34074... done\nAcquiring audio input from Caller A... done\nCaller B........ done\nEncrypting line... done\nEnable live TTS output?\n>Y\n\n==========================\nTEXT-TO-SPEECH OUTPUT BELOW\n==========================\n</style>\nCaller A: Hey, do you still have the gadget?\nCaller B: Yeah, right here, are we still good to go?\nCaller A: Yes, the deal's still on...\nCaller A: You haven't told anyone about this, right?\nCaller B: What, you think I'm stupid? Of course not!\nCaller A: Good. Keep it that way.\nCaller B: I will, I will... Say, are you sure this thing's safe to just, like, send through the system?\nCaller B: This gadget's some sort of high-end weapon modification, isn't it?\nCaller A: Don't sweat it, the gadget's perfectly safe. UES doesn't check their packages. I've used their mailing system loads before, and I'm still doing just fine.\nCaller B: Eh, if you say so. I'll send it through, and I'm expecting fifteen-thousand credits within the next twenty-four hours, as planned.\nCaller A: As planned. Good doing business with you.\n\n<style=cMono>==========================\n=====CALL DISCONNECTED=====\n==========================\n\nSevering line..... done\nBacking up recording........ done\nSave text output?\n>Y\n";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "ErraticGadget_Icon";
        public override string PickupModelPath => "MDLErraticGadget";

        public override void RegisterHooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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


        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            GameObject attacker = damageInfo.attacker;
            if (self && attacker)
            {
                var attackerBody = attacker.GetComponent<CharacterBody>();
                var victimBody = victim.GetComponent<CharacterBody>();

                int gadgetCount = GetCount(attackerBody);
                if (gadgetCount > 0)
                {
                    if (damageInfo.crit)
                    {
                        GameObject erraticGadgetEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerCaptainDefenseMatrix");
                        if (erraticGadgetEffectPrefab)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = victimBody.corePosition,
                                start = attackerBody.corePosition
                            };
                            EffectManager.SpawnEffect(erraticGadgetEffectPrefab, effectData, true);
                        }
                        DamageInfo newDamageInfo = damageInfo;
                        newDamageInfo.damage = damageInfo.damage * (StaticValues.gadgetDamage * gadgetCount);
                        victim.GetComponent<HealthComponent>().TakeDamage(newDamageInfo);
                        //This needs to be edited so that the effect actually originates from a body attachment
                    }
                }
            }
            orig(self, damageInfo, victim);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (GetCount(self) > 0)
                self.crit += StaticValues.gadgetCrit;
        }
    }
}

