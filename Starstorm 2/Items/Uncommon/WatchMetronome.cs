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
    class WatchMetronome : SS2Item<WatchMetronome>
    {
        public override string NameInternal => "SS2U_Metronome";
        public override string Name => "Watch Metronome";
        public override string Pickup => "Walking on the gruond charges up the speed of your next sprint.";
        public override string Description => "<style=cIsUtility>Walking on the ground</style> charges the <style=cIsUtility>speed of your next sprint</style> by up to <style=cIsUtility>200%</style>. Decays over <style=cIsUtility>4s</style> <style=cStack>(+2s per stack)</style>.";
        public override string Lore => "Order: Broken Watch Metronome\nTracking Number: 88******\nEstimated Delivery: 04/14/2056:\nShipping Method: Priority\nShipping Address: 33 Skyview Drive, Albumen, Venus\nShipping Details:\n\nHey, this is the watch metronome I told you needed fixing. Apparently, the things been falling outta whack for a couple weeks now, and me and the boys can't get any recording done with it speeding up all the time. I'd like it back fairly quickly, since usin' the in-ear metronome hurts my ears if it's in for too long. Oh, and be careful with it, too. The watch is a gift from my pops, and it really means a lot to me. Thanks a bunch in advance!\n";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Utility,
            ItemTag.SprintRelated
        };
        public override string PickupIconPath => "WatchMetronome_Icon";
        public override string PickupModelPath => "MDLWatchMetronome";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
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


        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
                self.AddItemBehavior<MetronomeBehavior>(GetCount(self));
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!sender.isSprinting) return;
            int buffCount = sender.GetBuffCount(BuffCore.watchMetronomeBuff);
            if (buffCount > 0)
            {
                args.moveSpeedMultAdd += (float)Math.Sqrt(0.1f * buffCount) * 2f;
            }
        }
    }

    public class MetronomeBehavior : CharacterBody.ItemBehavior
    {
        public static float chargeDuration = 8f;
        public Starstorm2ItemManager manager;

        private float notSprintingStopwatch;
        private float clientCharge;


        public void Awake()
        {
            notSprintingStopwatch = 0f;
            body = gameObject.GetComponent<CharacterBody>();
            manager = body.gameObject.AddOrGetComponent<Starstorm2ItemManager>();
            clientCharge = 0f;
        }

        public void FixedUpdate()
        {
            //Calculate buffs clientside
            if (!body || !body.hasEffectiveAuthority) return;

            bool isGrounded = body.characterMotor && body.characterMotor.isGrounded;
            if (!body.isSprinting)
            {
                if (isGrounded) notSprintingStopwatch += Time.fixedDeltaTime;
            }
            else
            {
                notSprintingStopwatch = 0f;
            }

            if (notSprintingStopwatch > 0f)
            {
                if (isGrounded && clientCharge < 1f)
                {
                    float newcharge = clientCharge + (Time.fixedDeltaTime / MetronomeBehavior.chargeDuration);
                    clientCharge = Math.Min(newcharge, 1f);
                }
            }
            else if (clientCharge > 0f)
            {
                float newcharge = clientCharge - (Time.fixedDeltaTime / (2.0f + 2.0f * stack));
                clientCharge = Math.Max(newcharge, 0f);
            }

            //Only notify server if buffcount needs to change
            //This method has an authority check built-in.
            int buffCount = body.GetBuffCount(BuffCore.watchMetronomeBuff.buffIndex);
            int desiredBuffCount = Mathf.FloorToInt(clientCharge * 10);
            if (buffCount != desiredBuffCount) manager.SetMetronomeBuffsAuthority(desiredBuffCount);
        }

        private void OnDestroy()
        {
            if (body && body.hasEffectiveAuthority) manager.ClearMetronomeBuffsAuthority();
        }
    }
}