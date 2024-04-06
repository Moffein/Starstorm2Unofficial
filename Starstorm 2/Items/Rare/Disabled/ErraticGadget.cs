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
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/tracers/TracerCaptainDefenseMatrix");
        public override string NameInternal => "SS2U_ErraticGadget";
        //public override string Lore => "<style=cMono>==========================\n========  CallCutter ========\n===== ALPHA 0.1354.1.12 =====\n==========================\n\nCutting into call TdiM3c4fcQQA.34074... done\nAcquiring audio input from Caller A... done\nCaller B........ done\nEncrypting line... done\nEnable live TTS output?\n>Y\n\n==========================\nTEXT-TO-SPEECH OUTPUT BELOW\n==========================\n</style>\nCaller A: Hey, do you still have the gadget?\nCaller B: Yeah, right here, are we still good to go?\nCaller A: Yes, the deal's still on...\nCaller A: You haven't told anyone about this, right?\nCaller B: What, you think I'm stupid? Of course not!\nCaller A: Good. Keep it that way.\nCaller B: I will, I will... Say, are you sure this thing's safe to just, like, send through the system?\nCaller B: This gadget's some sort of high-end weapon modification, isn't it?\nCaller A: Don't sweat it, the gadget's perfectly safe. UES doesn't check their packages. I've used their mailing system loads before, and I'm still doing just fine.\nCaller B: Eh, if you say so. I'll send it through, and I'm expecting fifteen-thousand credits within the next twenty-four hours, as planned.\nCaller A: As planned. Good doing business with you.\n\n<style=cMono>==========================\n=====CALL DISCONNECTED=====\n==========================\n\nSevering line..... done\nBacking up recording........ done\nSave text output?\n>Y\n";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "ErraticGadget_Icon";
        public override string PickupModelPath => "MDLErraticGadget";

        public override void RegisterHooks()
        {
            SharedHooks.OnHitEnemy.OnHitAttackerInventoryActions += ProcGadget;
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


        private void ProcGadget(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory)
        {
            if (!damageInfo.crit || damageInfo.procCoefficient <= 0f || damageInfo.HasModdedDamageType(DamageTypeCore.ModdedDamageTypes.ErraticGadget) || !victimBody.healthComponent) return;

            int gadgetCount = attackerInventory.GetItemCount(itemDef);
            if (gadgetCount <= 0) return;

            if (ErraticGadget.tracerEffectPrefab)
            {
                EffectData effectData = new EffectData
                {
                    origin = victimBody.corePosition,
                    start = attackerBody.corePosition
                };
                EffectManager.SpawnEffect(ErraticGadget.tracerEffectPrefab, effectData, true);
            }

            DamageInfo newDamageInfo = new DamageInfo
            {
                attacker = damageInfo.attacker,
                inflictor = damageInfo.inflictor,
                crit = damageInfo.crit,
                canRejectForce = damageInfo.canRejectForce,
                damageType = DamageType.Generic,
                damageColorIndex = DamageColorIndex.Item,
                dotIndex = damageInfo.dotIndex,
                force = damageInfo.force * 0.5f,
                position = damageInfo.position,
                procChainMask = damageInfo.procChainMask,
                rejected = damageInfo.rejected,
                procCoefficient = damageInfo.procCoefficient * 0.5f
            };
            newDamageInfo.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ErraticGadget);
            newDamageInfo.damage = damageInfo.damage * gadgetCount * 0.5f;

            victimBody.healthComponent.TakeDamage(newDamageInfo);
            GlobalEventManager.instance.OnHitEnemy(newDamageInfo, victimBody.gameObject);
        }

        private void HandleStats(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory)
        {
            if (inventory.GetItemCount(itemDef) > 0) args.critAdd += 10f;
        }
    }
}

