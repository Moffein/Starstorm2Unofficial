using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

//FIXME: appears to break when stacked via shrine of order

namespace Starstorm2.Cores.Items
{
    class HuntersSigil : SS2Item<HuntersSigil>
    {
        public override string NameInternal => "Sigil";
        public override string Name => "Hunter's Sigil";
        public override string Pickup => "Standing still grants bonus armor and critical strike chance.";
        public override string Description => "While <style=cIsUtility>standing still</style> for at least 3/4ths of a second, gain <style=cIsUtility>15</style> armor <style=cStack>(+10 armor per stack)</style> and <style=cIsDamage>25%</style> critical chance <style=cStack>(+20% per stack)</style>.";
        public override string Lore => "<style=cMono>Audio transcription complete from portable recorder.\n\nPrinting...\n\n</style>\"You think they'll take the bait?\"\n\"They're gonna have to. Not like we have any other options.\"\n\"I still think using our food like this is too risky.\"\n\"If we don't try, then we'll be dead in a week.\"\n\"Just... make sure we don't lose too much.\"\n\"Nothing more than our lives, at least.\"\n\"...\"\n\"They're here.\"\n\"Oh, good. Take one out and let's get out of here.\"\n\"Not yet...\"\n\"What? It's a waste of our food the longer you wait!\"\n\"Hold on...\"\n\"Take the shot, man, we only have so much!\"\n\"Almost...\"\n\"TAKE THE SHOT.\"\n\"Just about...\"\n\"SHOOT OR GIVE ME THE RIFLE.\"\n\"...\"\n\"GIVE IT TO ME.\"\n\"Hey, be CARE-\"\n\n<style=cMono>End of recording.</style>\n";
        public override ItemTier Tier => ItemTier.Tier2;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Damage,
            ItemTag.Utility
        };
        public override string PickupIconPath => "HuntersSigil_Icon";
        public override string PickupModelPath => "MDLSigil";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
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
                    childName = "PlatformBase",
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


        protected override void SetupMaterials(GameObject modelPrefab) {
            modelPrefab.GetComponentInChildren<Renderer>().material = Modules.Assets.CreateMaterial("matHuntersSigil", 1, Color.white, 1);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            self.AddItemBehavior<SigilBehavior>(GetCount(self));
            orig(self);
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            int sigilCount = GetCount(self);
            if (sigilCount > 0 && self.HasBuff(BuffCore.sigilBuff))
            {
                self.armor += StaticValues.sekiroArmor + StaticValues.sekiroArmorStack * (sigilCount - 1);
                self.crit += StaticValues.sekiroCrit + StaticValues.sekiroCritStack * (sigilCount - 1);
            }
        }
    }

    public class SigilBehavior : CharacterBody.ItemBehavior
    {
        private bool sigilActive = false;
        private bool hasBuff = false;
        private float activeStopwatch;
        private GameObject sigilEffectInstance;

        public void Awake()
        {
            body = gameObject.GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            activeStopwatch -= Time.fixedDeltaTime;

            if (sigilActive) activeStopwatch = 0.75f;

            if (activeStopwatch <= 0 && hasBuff)
            {
                hasBuff = false;
                body.RemoveBuff(BuffCore.sigilBuff);
                body.statsDirty = true;
            }

            if (body.notMovingStopwatch > 1f && !sigilActive)
            {
                body.AddBuff(BuffCore.sigilBuff);
                hasBuff = true;
                sigilActive = true;
                body.statsDirty = true;
                activeStopwatch = 0.75f;

                SpawnEffect();
            }
            else if (body.notMovingStopwatch == 0f && sigilActive)
            {
                sigilActive = false;

                DestroyEffect();
            }
        }

        private void DestroyEffect()
        {
            if (sigilEffectInstance)
            {
                sigilEffectInstance.transform.GetChild(0).GetComponent<ParticleSystem>().Stop();
                CancelInvoke();
                Invoke("DestroyEffectInstance", 1f);
            }
        }

        private void DestroyEffectInstance()
        {
            if (sigilEffectInstance)
            {
                Destroy(sigilEffectInstance);
                NetworkServer.UnSpawn(sigilEffectInstance);
            }
        }

        private void SpawnEffect()
        {
            if (sigilEffectInstance) return;

            sigilEffectInstance = GameObject.Instantiate(Modules.Assets.sigilFX);
            sigilEffectInstance.transform.parent = body.coreTransform;
            sigilEffectInstance.transform.localPosition = new Vector3(0, -0.3f, 0);
            sigilEffectInstance.transform.parent = null;

            NetworkServer.Spawn(sigilEffectInstance);
        }

        public void OnDisable()
        {
            if (hasBuff) body.RemoveBuff(BuffCore.sigilBuff);
            DestroyEffect();
        }

        private void OnDestroy()
        {
            if (hasBuff) body.RemoveBuff(BuffCore.sigilBuff);
            DestroyEffect();
        }
    }

    public class SigilSoundComponent : MonoBehaviour
    {
        private void Awake()
        {
            // only way i know how to network this kek
            Util.PlaySound("SigilActivation", this.gameObject);
        }
    }
}