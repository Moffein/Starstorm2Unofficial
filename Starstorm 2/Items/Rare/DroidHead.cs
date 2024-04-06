using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using Starstorm2Unofficial.Modules;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Items
{
    class DroidHead : SS2Item<DroidHead>
    {
        public static NetworkSoundEventDef procSound;
        public override string NameInternal => "SS2U_DroidHead";
        //public override string Lore => "Order: Security Robot\nTracking Number: 1138***\nEstimated Delivery: 5/14/2056\nShipping Method: Priority / Fragile\nShipping Address: RaCom Robotics, Asimov, Mars\nShipping Details:\n\nThis was one of the few complete bits we could salvage from those stolen security robots... That militant group on Pluto had stolen a bunch of those ER-14s, and had been using 'em at their bases for a while. This one specifically, ER-14XPC5VVUFF, lines up with the ones you'd tried to recall a while back. I don't know this history, I just know there was a big bounty for whoever could find the remains for this batch. There's other remnants, too, but they're much less complete... Shouldn't be any danger anymore, at least. Gimme a heads up if you want those shipped your way, too.\n";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.Damage,
            ItemTag.OnKillEffect
        };
        public override string PickupIconPath => "DroidHead_Icon";
        public override string PickupModelPath => "MDLDroidHead";

        public override void RegisterHooks()
        {
            procSound = Assets.CreateNetworkSoundEventDef("SS2UDroidHead");
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            SharedHooks.OnCharacterDeathGlobal.OnCharacterDeathInventoryActions += ProcDroidHead;
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
                self.AddItemBehavior<DroidHeadBehavior>(GetCount(self));
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
localPos = new Vector3(0.154F, 0F, -0.1602F),
localAngles = new Vector3(15.2417F, 141.4141F, 180F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1927F, -0.078F, 0.0495F),
localAngles = new Vector3(21.4305F, 58.3257F, 183.0518F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.0888F, -0.0221F, -0.2011F),
localAngles = new Vector3(6.7902F, 170.4913F, 182.1258F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.2395F, 0.0231F, -0.1579F),
localAngles = new Vector3(11.0259F, 149.285F, 189.7534F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1758F, 0.0498F, -0.1555F),
localAngles = new Vector3(28.0175F, 139.2946F, 175.1604F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.1795F, 0.0094F, 0.0094F),
localAngles = new Vector3(9.2046F, 95.0137F, 189.3625F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(0.0516F, -0.0898F, -0.2502F),
localAngles = new Vector3(357.1606F, 183.0843F, 181.5753F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                   childName = "Head",
localPos = new Vector3(-0.000F, 1.6F, 1.3F),
localAngles = new Vector3(310F, 180F, 0F),
localScale = new Vector3(3F, 2.5F, 3.5F)
                }
            });

            rules.Add("mdlTreebot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "PlatformBase",
localPos = new Vector3(-0.5005F, 0.6628F, 0.6538F),
localAngles = new Vector3(0F, 341.659F, 0F),
localScale = new Vector3(0.2F, 0.2F, 0.2F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Hip",
localPos = new Vector3(-2.4027F, 2.3874F, 0.2516F),
localAngles = new Vector3(339.9485F, 299.0386F, 213.9489F),
localScale = new Vector3(1F, 1F, 1F)
                }
            });
            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Pelvis",
localPos = new Vector3(-0.0027F, -0.0005F, -0.0016F),
localAngles = new Vector3(13.8719F, 230.8945F, 167.7166F),
localScale = new Vector3(0.0013F, 0.0013F, 0.0013F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Sword",
localPos = new Vector3(-0.0003F, 0.0109F, -0.0001F),
localAngles = new Vector3(353.8759F, 221.1168F, 2.4036F),
localScale = new Vector3(0.0013F, 0.0013F, 0.0013F)
                }
            });


            return rules;
        }

        private void ProcDroidHead(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody)
        {
            if (!victimBody.isElite) return;
            int itemCount = attackerInventory.GetItemCount(itemDef);
            if (itemCount <= 0) return;

            DroidHeadBehavior dhb = attackerBody.GetComponent<DroidHeadBehavior>();
            if (!dhb || !dhb.CanSpawnDroid()) return;

            MasterSummon droneSummon = new MasterSummon();
            droneSummon.position = victimBody.corePosition;
            droneSummon.masterPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterMasters/DroneBackupMaster");
            droneSummon.summonerBodyObject = attackerBody.gameObject;
            droneSummon.ignoreTeamMemberLimit = true;
            droneSummon.useAmbientLevel = true;

            CharacterMaster droneMaster = droneSummon.Perform();
            if (droneMaster)
            {
                dhb.AddDroid(droneMaster);
                if (droneMaster.inventory)
                {
                    float damageBoost = 0.5f + itemCount * 0.5f;
                    int boostDamageCount = Mathf.RoundToInt(damageBoost / 0.1f);
                    if (boostDamageCount > 0) droneMaster.inventory.GiveItem(RoR2Content.Items.BoostDamage, boostDamageCount);

                    if (ModCompat.RiskyMod.AllyMarkerItem != ItemIndex.None)
                        droneMaster.inventory.GiveItem(ModCompat.RiskyMod.AllyMarkerItem, 1);
                    if (ModCompat.RiskyMod.AllyAllowVoidDeathItem != ItemIndex.None)
                        droneMaster.inventory.GiveItem(ModCompat.RiskyMod.AllyAllowVoidDeathItem, 1);
                    if (ModCompat.RiskyMod.AllyAllowOverheatDeathItem != ItemIndex.None)
                        droneMaster.inventory.GiveItem(ModCompat.RiskyMod.AllyAllowOverheatDeathItem, 1);
                    if (ModCompat.RiskyMod.AllyScalingItem != ItemIndex.None)
                        droneMaster.inventory.GiveItem(ModCompat.RiskyMod.AllyScalingItem, 1);

                    if (victimBody.inventory)
                    {
                        EquipmentDef ed = EquipmentCatalog.GetEquipmentDef(victimBody.inventory.currentEquipmentIndex);
                        if (ed && ed.passiveBuffDef && ed.passiveBuffDef.isElite)
                        {
                            droneMaster.inventory.SetEquipmentIndex(victimBody.inventory.currentEquipmentIndex);
                        }
                    }
                }

                droneMaster.gameObject.AddComponent<MasterSuicideOnTimer>().lifeTimer = 30f;

                EffectManager.SimpleSoundEffect(procSound.index, victimBody.corePosition, true);
            }
        }

        public class DroidHeadBehavior : CharacterBody.ItemBehavior
        {
            public static int maxDroids = 4;
            private List<CharacterMaster> activeDroids = new List<CharacterMaster>();

            public bool CanSpawnDroid()
            {
                ClearDeadDroids();
                return activeDroids.Count < DroidHeadBehavior.maxDroids;
            }

            public void AddDroid(CharacterMaster master)
            {
                activeDroids.Add(master);
            }

            private void ClearDeadDroids()
            {
                activeDroids = activeDroids.Where(master => (master != null && !master.IsDeadAndOutOfLivesServer())).ToList();
            }
        }
    }
}