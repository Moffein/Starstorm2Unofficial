using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.Items
{
    class Diary : SS2Item<Diary>
    {
        public override string NameInternal => "ExpOverTime";
        public override string Name => "Diary";
        public override string Pickup => "Earn experience over time.";
        public override string Description => $"Gain <style=cIsUtility>3 experience</style> <style=cStack>(+3 per stack, scaling with level)</style> every <style=cIsUtility>{StaticValues.diaryTime}</style> seconds.";
        public override string Lore => "<style=cMono>Security footage transmission complete of Hallway Section 14-B.\n\nPrinting...\n\n</style>\"So this book's the only thing in the storage container?\"\n\"Pretty much. Other than some old furniture, this is it.\"\n\"So what's this book's deal? If this is all we're getting, it better be good.\"\n\"It's a diary, I think. First written in by one Alan Howizer, about 26 years ago. Seemed like a pretty smart guy, said he worked at a chemical testing plant back on Earth.\"\n\"That's it? This is just some old guy's diary?\"\n\"Hold on, there's more to it. While I was flipping through, about a hundred pages in, the handwriting changed, and sure enough, it was found by a different person on a park bench, apparently. Same thing again, about thirty pages later. And again, after about another seventy. And it keeps on going. There's a couple dozen different people who wrote in this diary, writing down their fond memories, odd mysteries, riddles, their answers to those riddles, all sorts of things.\"\n\"Huh. This thing must be a real treasure trove of knowledge then, yeah? Any clue as to who last owned it?\"\n\"Uh, yeah, actually. Someone named Chel was the last one to write in the book, and it looks like they left an address inside the back cover. Why, what are you thinking?\"\n\n<style=cMono>End of notable section.</style>";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Utility,
            ItemTag.AIBlacklist
        };
        public override string PickupIconPath => "Diary_Icon";
        public override string PickupModelPath => "MDLDiary";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.CharacterBody.OnTeamLevelChanged += CharacterBody_OnTeamLevelChanged;
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
            self.AddItemBehavior<DairyBehavior>(GetCount(self));
            orig(self);
        }

        private void CharacterBody_OnTeamLevelChanged(On.RoR2.CharacterBody.orig_OnTeamLevelChanged orig, CharacterBody self)
        {
            orig(self);

            if (self && self.master && self.master.inventory)
            {
                if (GetCount(self) > 0)
                {
                    Util.PlaySound("DiaryLevelUp", self.gameObject);
                }
            }
        }
    }

    public class DairyBehavior : CharacterBody.ItemBehavior
    {
        //VICE VS YOU
        //YOUUUUU
        private float timer;
        private bool fatalLogged;

        public Starstorm2ItemManager manager;
        public void Awake()
        {
            body = gameObject.GetComponent<CharacterBody>();
            manager = body.gameObject.AddOrGetComponent<Starstorm2ItemManager>();
        }

        public void FixedUpdate()
        {
            if (Run.instance && !Run.instance.isRunStopwatchPaused)
            {
                CharacterMaster master = body.master;
                if (master && master.playerCharacterMasterController)
                {
                    timer += Time.deltaTime;
                    if (timer >= StaticValues.diaryTime)
                    {
                        if (body.teamComponent)
                        {
                            //TODO: add sound
                            /*
                            if (body.hasAuthority && Util.CheckRoll(20))
                                Util.PlaySound("DiaryWritingSound", body.gameObject);
                            */
                            uint exp = (uint)(stack * Math.Pow(2, 1 + (TeamManager.instance.GetTeamLevel(body.teamComponent.teamIndex) / 3.75d)));
                            manager.AddExperienceAuthority(exp);
                        }
                        else
                        {
                            if (!fatalLogged)
                            {
                                fatalLogged = true;
                                LogCore.LogFatal(Language.GetString(body.baseNameToken) + " doesn't have a team component!");
                            }
                        }
                        timer = 0;
                    }
                }
            }
        }
    }
}
