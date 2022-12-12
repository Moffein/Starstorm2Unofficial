using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.Items
{
    class HottestSauce : SS2Item<HottestSauce>
    {
        public override string NameInternal => "FireOnEquipmentUse";
        public override string Name => "Hottest Sauce";
        public override string Pickup => "Activating your equipment ignites nearby enemies.";
        public override string Description => $"Activating your equipment <style=cIsDamage>ignites</style> enemies within <style=cIsDamage>{StaticValues.hottestSusRadius}</style> for <style=cIsDamage>{StaticValues.hottestSusDuration}</style> seconds, dealing <style=cIsDamage>{StaticValues.hottestSusHit * 100}% base damage</style> plus <style=cIsDamage>{StaticValues.hottestSusDamage * 100}% damage per second</style> <style=cStack>(+50% per stack)</style>.";
        public override string Lore => "<style=cMono>//--Auto-Transcription from room K3 (Kitchen, 3rd Floor) of Kaymar Hotel--//</style>\n\nA tall, thin man walks into the room, holding a large cardboard box. \"Hey, CHEF. How's it going?\" He sets the box on the counter. \"Alright, we've got a bunch of new sauces you'll have to get accustomed with. I'll walk you through them now, if you don't mind.\" The CHEF bot gives him its attention, but continues to work.\n\nThe man proceeds down the rows of various colored bottles, explaining to the CHEF their names, flavors, and uses in cooking. Eventually, he stops at a red-colored bottle, and hesitates briefly.\n\n\"...Okay, now. This bottle? It's unlabelled for a reason. If you get a meal order stating that you use \"<style=cDeath>Hottest Sauce</style>\" on it, you go ahead and use this bottle. But do NOT use it under ANY OTHER CIRCUMSTANCES. I know sometimes you need to improvise to finish a dish, but you should NEVER use this bottle if you're doing that. Do you understand?\"\n\nThe CHEF bot, still working, nods affirmatively.\n\n\"Good. I expect great things from you, CHEF.\"\n\nThe CHEF bot nods once again, and briefly pauses its work to put the bottles in the appropriate locations.\n\n\"...Oh, and also?\"\n\nThe CHEF bot pauses to give its attention to the man once more.\n\n\"Please, don't break that bottle. I know you're a robot and all, but I also know that accidents happen, and that particular accident might leave a hole in the floor.\"\n\nThe CHEF bot gazes at the bottle briefly, before carefully setting it in the back of the cupboard with the rest of the sauces.";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage,
            ItemTag.EquipmentRelated,
            ItemTag.AIBlacklist
        };
        public override string PickupIconPath => "HottestSauce_Icon";
        public override string PickupModelPath => "MDLHottestSauce";

        private float sauceDamageCoef = StaticValues.hottestSusHit;

        public override void RegisterHooks()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        public override ItemDisplayRuleDict CreateDisplayRules()
        {
            displayPrefab = Resources.Load<GameObject>(PickupModelPath);
            var disp = displayPrefab.AddComponent<ItemDisplay>();
            disp.rendererInfos = Utils.SetupRendererInfos(displayPrefab);

            ItemDisplayRuleDict rules = new ItemDisplayRuleDict(new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1783F, -0.0743F, 0.0996F),
localAngles = new Vector3(323.2641F, 153.6151F, 161.313F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1816F, -0.0975F, 0.0582F),
localAngles = new Vector3(23.1899F, 144.6882F, 177.3828F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-0.1189F, 0.3318F, -0.1902F),
localAngles = new Vector3(344.0087F, 24.1534F, 181.6373F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "CannonHeadL",
localPos = new Vector3(0.178F, 0.2201F, 0.1678F),
localAngles = new Vector3(314.4302F, 355.6576F, 59.4658F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(-0.2315F, 0.053F, 0.0078F),
localAngles = new Vector3(309.3626F, 168.683F, 178.801F),
localScale = new Vector3(0.05F, 0.05F, 0.05F)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-0.1028F, 0.2313F, -0.3288F),
localAngles = new Vector3(29.3866F, 6.1308F, 15.1956F),
localScale = new Vector3(0.04F, 0.04F, 0.04F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.185F, 0.1144F, -0.1246F),
localAngles = new Vector3(33.1811F, 278.4078F, 207.8812F),
localScale = new Vector3(0.02F, 0.02F, 0.02F)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(-2.0163F, 3.1193F, -1.3766F),
localAngles = new Vector3(25.5887F, 358.7055F, 359.4573F),
localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "FlowerBase",
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
                    childName = "Chest",
localPos = new Vector3(1.2091F, -2.7822F, 7.4245F),
localAngles = new Vector3(12.33F, 34.4234F, 358.249F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
            });

            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Chest",
localPos = new Vector3(0.0019F, 0.0025F, -0.0026F),
localAngles = new Vector3(25.5056F, 13.5026F, 357.6461F),
localScale = new Vector3(0.0004F, 0.0004F, 0.0004F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "ArmL",
localPos = new Vector3(0.0005F, -0.0017F, 0.0008F),
localAngles = new Vector3(3.1321F, 209.3164F, 158.2628F),
localScale = new Vector3(0.0004F, 0.0004F, 0.0004F)
                }
            });

            return rules;
        }


        protected override void SetupMaterials(GameObject modelPrefab) {

            modelPrefab.transform.GetChild(0).GetComponent<Renderer>().material = Modules.Assets.CreateMaterial("matHottestSauce", 1, Color.red);

            //leave glass material as is
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            //sauce should only proc if the equipment was successfully used - prevents procing on elite affixes, fuel array, unsuccessful recycler
            bool equipUsed = orig(self, equipmentDef);
            if (equipUsed && self)
            {
                var body = self.characterBody;
                if (body)
                {
                    int sauceCount = GetCount(body);
                    if (sauceCount > 0)
                    {
                        Util.PlaySound("HottestSus", self.gameObject);

                        var hits = new List<HurtBox>();
                        SphereSearch sauceSearch = new SphereSearch();
                        sauceSearch.ClearCandidates();
                        sauceSearch.origin = body.corePosition;
                        sauceSearch.mask = LayerIndex.entityPrecise.mask;
                        sauceSearch.radius = StaticValues.hottestSusRadius;
                        sauceSearch.RefreshCandidates();
                        sauceSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(body.teamComponent.teamIndex));
                        sauceSearch.FilterCandidatesByDistinctHurtBoxEntities();
                        sauceSearch.GetHurtBoxes(hits);

                        foreach (HurtBox hit in hits)
                        {
                            if (hit.healthComponent && hit.healthComponent != self.healthComponent)
                            {
                                DamageInfo di = new DamageInfo()
                                {
                                    attacker = self.gameObject,
                                    position = hit.healthComponent.transform.position,
                                    crit = body.RollCrit(),
                                    damage = body.damage * sauceDamageCoef,
                                    procChainMask = default(ProcChainMask)
                                };
                                hit.healthComponent.TakeDamage(di);
                                var dotInfo = new InflictDotInfo()
                                {
                                    attackerObject = self.gameObject,
                                    victimObject = hit.healthComponent.gameObject,
                                    dotIndex = DotController.DotIndex.Burn,
                                    duration = StaticValues.hottestSusDuration,
                                    damageMultiplier = StaticValues.hottestSusDamage + sauceCount
                                };
                                DotController.InflictDot(ref dotInfo);
                            }
                        }
                    }
                }
            }
            return equipUsed;
        }
    }
}
