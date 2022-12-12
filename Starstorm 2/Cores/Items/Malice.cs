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
    class Malice : SS2Item<Malice>
    {
        public static GameObject maliceEffect = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("MaliceEffect");

        public override string NameInternal => "AoeOnHit";
        public override string Name => "Malice";
        public override string Pickup => "Damage dealt spreads to nearby enemies.";
        public override string Description => $"On damaging an enemy, up to <style=cIsDamage>1</style> other enemy <style=cStack>(+1 per stack)</style> within {StaticValues.maliceRangeValue}m of that enemy <style=cStack>(+{StaticValues.maliceRangeStackValue}m per stack)</style> receives <style=cIsDamage>{StaticValues.maliceDmgReductionValue * 100}%</style> TOTAL damage. Each additional hit is scaled a further <style=cIsDamage>{StaticValues.maliceDmgReductionValue * 100}%</style>.";
        public override string Lore => "You are weak.\n\nPhysically? Perhaps not. You fight to survive, and you fight well enough. But in the mind? My friend, you are frail. You fight and kill, but feel remorse for that which you kill. Why? The animals, they do not feel remorse towards you. To them, you are but prey to stalk, or predator to flee from. Those hulking titans? They bear no emotion, only a silent directive to destroy. To destroy you.\n\nAnd I know you. My friend, you do not wish to be weak. You wish to be powerful, in mind as well as in body. Do not feel pity for these mindless beasts, for they are just that. Fight to kill, not just to survive.\n\nBut man's malice can only take you so far. Take this. Be pulled further. Fight to kill, and kill to be feared.\n";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "Malice_Icon";
        public override string PickupModelPath => "MDLMalice";

        public override void RegisterHooks()
        {
            var maliceComp = maliceEffect.AddComponent<EffectComponent>();
            Modules.Assets.effectDefs.Add(new EffectDef(maliceEffect));
            maliceEffect.AddComponent<DestroyOnTimer>().duration = 5f;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        /*public override ItemDisplayRuleDict CreateDisplayRules()
         * 
         * Not sure what to do with Malice for now. Will wait on remodel.
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    childName = "Chest",
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
                    localPos = new Vector3(0.11f, 0.29f, 0.24f),
                    localAngles = new Vector3(90f, 30f, 0f),
                    localScale = new Vector3(0.1f, 0.1f, 0.1f)
                }
            });

            return rules;
        }
        */

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {
            var attacker = damageInfo.attacker;
            if (attacker)
            {
                var attackerBody = attacker.GetComponent<CharacterBody>();
                int maliceCount = GetCount(attackerBody);
                if (maliceCount > 0)
                {
                    int hits = 0;
                    float maliceDmg = StaticValues.maliceDmgReductionValue;
                    float maliceRadius = StaticValues.maliceRangeValue + (maliceCount * StaticValues.maliceRangeStackValue);
                    EffectData maliceData = new EffectData();
                    if (victim) maliceData.origin = victim.transform.position;
                    else maliceData.origin = attackerBody.corePosition;
                    maliceData.scale = maliceRadius;
                    EffectManager.SpawnEffect(maliceEffect, maliceData, true);
                    DamageInfo di = new DamageInfo();
                    di.attacker = attacker;
                    di.crit = damageInfo.crit;
                    di.damage = damageInfo.damage;
                    di.damageColorIndex = DamageColorIndex.Default;
                    di.damageType = DamageType.AOE;
                    di.dotIndex = DotController.DotIndex.None;
                    di.inflictor = attacker;
                    di.procChainMask = default(ProcChainMask);
                    di.procCoefficient = StaticValues.maliceProcCoefValue;
                    Collider[] aoe = Physics.OverlapSphere(damageInfo.position, maliceRadius);
                    for (int i = 0; hits < maliceCount && i < aoe.Length; i++)
                    {
                        HealthComponent hp = aoe[i].GetComponent<HealthComponent>();
                        if (hp && hp.GetComponent<TeamComponent>().teamIndex != TeamComponent.GetObjectTeam(attacker) && hp.body != victim.GetComponent<CharacterBody>())
                        {
                            di.position = aoe[i].transform.position;
                            //diminishing scaling per stack (remove multiplier from di.damage)
                            di.damage *= maliceDmg;
                            hp.TakeDamage(di);
                            hits++;
                        }
                    }
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}
