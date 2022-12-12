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
    class PrototypeJetBoots : SS2Item<PrototypeJetBoots>
    {
        private GameObject bootsEffect = Modules.Assets.jetBootsFX;
        private GameObject bootsEffectLight = Modules.Assets.lightJetBootsFX;

        public override string NameInternal => "ExplodeOnJump";
        public override string Name => "Prototype Jet Boots";
        public override string Pickup => "Detonate on jump!";
        public override string Description => $"Create an <style=cIsDamage>explosion</style> in a small area around you whenever you jump for <style=cIsDamage>{StaticValues.bootsBase * 100}%</style> <style=cStack>(+{StaticValues.bootsStack * 100}% per stack)</style> damage.";
        public override string Lore => "Record for the postmortem of Hephaestus Metal Works\n\nLocation: Hephaestus Metal Works, Mars\nLocation Status: Interior destroyed via fire\nCasualties: The owner, via fire\nOn Site Report:\n\nI'd assumed the cause of the fire was the work of a forge, furnace, or even a cigarette break. There wasn't much different from any other workshop of the same niche. A bunch of old kinds of swords, armor, farming equipment, et cetera. Though it would seem the forget was a hobbyist himself, as there were what looked to be some projects you normally wouldn't find there in the back.\n\nA fair amount of them were nothing more than motors and metal. He seemed to practice making clockwork. But he eventually moved on to fuel and engine based items. Old lawn mowers, motor cycles, and generators. He eventually moved on to propulsion systems. Small remote planes, helicopters, an old rusty jetpack. It seemed he was working up to something, and we found what it was in the back.\n\nThere was clearly some genius behind him. He was building a pair of jet boots from scratch. They haven't entered production before, since jetpacks do the job fine, and boots can't be as sturdy and heavy while getting the same amount of thrust. He seemed to have the same thought, as he made them more sturdy for his protection, but tried to compensate by turning up the output on the thrust. The result ended in the heat building up and erupting from below him. Unfortunately, his inventive nature cost him his business and his life.\n\nThe surrounding area didn't take any damage so the shop may be renovated soon. The boots have been confiscated and are en route to be studied and decommissioned, and the remaining items from inside the shop have been sent to their next of kin.\n\nThis concludes my report.\n";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "JetBoots_Icon";
        public override string PickupModelPath => "MDLJetBoots";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterMotor.OnLeaveStableGround += CharacterMotor_OnLeaveStableGround;
        }

        /*public override ItemDisplayRuleDict CreateDisplayRules()
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
        */

        private void CharacterMotor_OnLeaveStableGround(On.RoR2.CharacterMotor.orig_OnLeaveStableGround orig, CharacterMotor self)
        {
            bool hasJumped = self.GetComponent<InputBankTest>().jump.justPressed;

            if (self && hasJumped)
            {
                var body = self.GetComponent<CharacterBody>();
                var boots = GetCount(body);

                if (boots > 0)
                {
                    var attacker = body.gameObject;
                    var damage = body.damage * (StaticValues.bootsBase + (StaticValues.bootsStack * boots));

                    EffectData bootsEffectData = new EffectData()
                    {
                        color = new Color32(0, 255, 0, 255),
                        scale = 6f,
                        origin = body.footPosition
                    };

                    new BlastAttack
                    {
                        attacker = attacker,
                        baseDamage = damage,
                        radius = StaticValues.bootsRadius,
                        crit = body.RollCrit(),
                        falloffModel = BlastAttack.FalloffModel.None,
                        procCoefficient = StaticValues.bootsProc,
                        teamIndex = body.teamComponent.teamIndex,
                        position = attacker.transform.position,
                    }.Fire();

                    switch (StaticValues.timbsQuality)
                    {
                        case StaticValues.JetBootsEffectQuality.Default:
                            EffectManager.SpawnEffect(bootsEffect, bootsEffectData, false);
                            break;
                        case StaticValues.JetBootsEffectQuality.Light:
                            EffectManager.SpawnEffect(bootsEffectLight, bootsEffectData, false);
                            break;
                    }
                    
                }
            }

            orig(self);
        }
    }
}