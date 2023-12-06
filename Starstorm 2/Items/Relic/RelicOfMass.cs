using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using UnityEngine;
using Starstorm2Unofficial.Cores;
using R2API;

//FIXME: adds health after applying transcendence shields

namespace Starstorm2Unofficial.Cores.Items
{
    class RelicOfMass : SS2Item<RelicOfMass>
    {
        public override string NameInternal => "RelicMass";
        public override string Name => "Relic of Mass";
        public override string Pickup => "Double your max health. <style=cDeath>Your movement has momentum.</style>";
        public override string Description => $"<style=cIsHealing>Increase maximum health</style> by <style=cIsHealing>{StaticValues.massHealthGain * 100}%</style> <style=cStack>(+100% per stack)</style>. <style=cDeath>Reduce acceleration and deceleration</style> by a factor of <style=cDeath>8</style> <style=cStack>(+8 per stack)</style>.";
        public override string Lore => "Ah, welcome back! Youngling, what have you found?\n\nHmmm. More scavengings from the blasted heath? Very well. You may keep those things, if you wish. Have you found anything else?\n\nAh, you have? Let me see it then, youngling.\n\nOh! This shape that you've found! It is stalwart, but not loyal. Powerful, but yet also frail at the same time. It is a peculiar shape, to say the least of it. Where did you find this, youngling?\n\n...You found it on the planet? Hmm. Worrisome. This was not created by those sandfolk. Too complex, too powerful. Nor by those Lemurians. Tell me, youngling, you did not steal this, did you?\n\n...You did not. I see. Very well. I would like to keep this shape. This shape is dangerous if you do not understand it.\n\nOh, my youngling, do not look so saddened. This shape is remarkable, and I laud your work regardless of this. I simply want to keep it safe. Here, perhaps a reward is in order. Come with me.";
        public override ItemTier Tier => ItemTier.Lunar;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Utility
        };
        public override string PickupIconPath => "RelicOfMassLunar";
        public override string PickupModelPath => "MDLRelicOfMass";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        /*public override ItemDisplayRuleDict CreateDisplayRules()
        {
            var trackerObject = LegacyResourcesAPI.Load<GameObject>("RelicOfMassFollowerObject");
            displayPrefab = LegacyResourcesAPI.Load<GameObject>(PickupModelPath);
            var itemFollower = trackerObject.AddComponent<ItemFollower>();
            itemFollower.itemDisplay = trackerObject.AddComponent<ItemDisplay>();
            itemFollower.itemDisplay.rendererInfos = Utils.SetupRendererInfos(trackerObject);
            itemFollower.followerPrefab = displayPrefab;
            itemFollower.distanceDampTime = 0.25f;
            itemFollower.distanceMaxSpeed = 100f;
            itemFollower.targetObject = trackerObject;
            var disp = displayPrefab.AddComponent<ItemDisplay>();
            disp.rendererInfos = Utils.SetupRendererInfos(displayPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = trackerObject,
                    childName = "Base",
                    //set ^ to "tracketObject" once this shit works LOL
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
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
                    followerPrefab = trackerObject,
                    childName = "Base",
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            return rules;
        }*/


        protected override void SetupMaterials(GameObject modelPrefab) {
            modelPrefab.GetComponentInChildren<Renderer>().material = Modules.Assets.CreateMaterial("matRelicOfMass");
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            //base function does its own max health calculations disregarding what we do here; any hp gained that surpasses this max is lost on recalculate
            //(e.g. sprinting sets you back to 50% hp)
            //this is a really really bad bandage workaround, like 100% guaranteed to backfire somehow somewhere, and it should probably be rewritten
            // wait you're doing a stat hook like this with an on hook.............. kek
            // really needs to be an IL hook at some point but i cba doing that
            //★ oh hey guys fancy seeing you here

            float prevHealth = (float)self.healthComponent?.health;
            float prevShield = (float)self.healthComponent.shield;

            orig(self);

            int rmassCount = GetCount(self);
            if (rmassCount > 0)
            {
                if (self.inventory?.GetItemCount(RoR2Content.Items.ShieldOnly.itemIndex) > 0)
                {
                    float shieldBoost = (self.maxShield * rmassCount);
                    self.maxShield += shieldBoost;
                    self.healthComponent.shield = prevShield;
                }
                else
                {
                    float healthBoost = (self.baseMaxHealth + self.levelMaxHealth * (self.level - 1)) * (rmassCount * StaticValues.massHealthGain);
                    int glassCount = (int)self.inventory?.GetItemCount(RoR2Content.Items.LunarDagger.itemIndex);
                    if (glassCount > 0)
                        healthBoost /= glassCount;
                    self.maxHealth += healthBoost;
                    self.healthComponent.health = prevHealth;
                }
                self.acceleration = self.baseAcceleration / (rmassCount * StaticValues.massFactor);
            }
        }
    }
}

