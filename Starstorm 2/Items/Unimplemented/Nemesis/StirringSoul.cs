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
    class StirringSoul : SS2Item<StirringSoul>
    {
        public static GameObject MonsterSoulPickup;

        public override string NameInternal => "SoulOnKill";
        public override string Name => "<color=#FFCCED>Stirring Soul</color>";
        public override string Pickup => "<color=#FFCCED>What am I fighting for?</color>";
        public override string Description => $"Slain monsters leave a <style=cIsUtility>soul</style> which has a <style=cIsUtility>{StaticValues.soulChance}%</style> chance to <style=cIsUtility>spawn an item</style> on contact.";
        public override string Lore => "A spirit of determination..\n<color=#FFCCED>...A spirit to be stopped</color>\nAn unwavering will to survive...\n<color=#FFCCED>...A flickering will to survive</color>\nWilling to desecrate their identity...\n<color=#FFCCED>...Willing to desecrate their memory</color>\nWilling to sacrifice their civilization...\n<color=#FFCCED>...Willing to sacrifice their humanity</color>\n\nTo what lengths will they go to live?\n<color=#FFCCED>To what lengths will they go to bring themselves ruin?</color>\nTo what lengths will they go to escape?\n<color=#FFCCED>To what lengths will they go to destroy?</color>\nAll in the name of self preservation...\n<color=#FFCCED>...All in the name of narcissism</color>\nAll for the sake of an uncertain hope...\n<color=#FFCCED>...All for the sake of a null chance</color>\n\nWhat would happen if given an opportunity?\n<color=#FFCCED>What would happen if given an ally?</color>\nWhat would happen if their fire was stoked?\n<color=#FFCCED>What would happen if they were given false hope?</color>\nMaybe they should be given a chance.\n<color=#FFCCED>Maybe they should be given strength.</color>\nMaybe they should be given an offer.\n<color=#FFCCED>Maybe they should be taken first.</color>";
        public override bool CanRemove => false;
        public override ItemTier Tier => ItemTier.Boss;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.WorldUnique,
        };
        public override string PickupIconPath => "StirringSoul_Icon";
        public override string PickupModelPath => "StirlingSoul.prefab";
        public override bool DropInMultiBlacklist => true;

        public override void Init()
        {
            base.Init();
            SetUpSoulPickup();
        }

        public override void RegisterHooks()
        {
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
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
                    childName = "Head",
localPos = new Vector3(-0.0009F, 0.2272F, -0.0417F),
localAngles = new Vector3(90F, 30F, 0F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlHuntress", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.0003F, 0.0733F, -0.0143F),
localAngles = new Vector3(342.0413F, 76.995F, 347.0571F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlMage", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.0164F, 0.043F, -0.1033F),
localAngles = new Vector3(75.4036F, 348.4647F, 340.9313F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlEngi", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "HeadCenter",
localPos = new Vector3(0.0002F, 0.0038F, -0.0283F),
localAngles = new Vector3(22.8253F, 179.6242F, 149.6239F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlMerc", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(0.007F, 0.194F, -0.0042F),
localAngles = new Vector3(14.4202F, 178.2166F, 202.7005F),
localScale = new Vector3(0.08F, 0.08F, 0.08F)
                }
            });

            rules.Add("mdlLoader", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(0.0479F, 0.102F, -0.0097F),
localAngles = new Vector3(13.7816F, 195.8768F, 199.8034F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlCaptain", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.0104F, 0.1035F, 0.0001F),
localAngles = new Vector3(51.3166F, 176.8947F, 167.6958F),
localScale = new Vector3(0.1F, 0.1F, 0.1F)
                }
            });

            rules.Add("mdlToolbot", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "HeadCenter",
localPos = new Vector3(0.4261F, 0.9458F, -1.1529F),
localAngles = new Vector3(349.9306F, 290.3599F, 306.7653F),
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
localPos = new Vector3(-0.1356F, 0.2677F, -0.181F),
localAngles = new Vector3(357.5828F, 180.0001F, 123.9136F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });

            rules.Add("mdlCroco", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.0546F, 1.116F, 0.562F),
localAngles = new Vector3(90F, 30F, 0F),
localScale = new Vector3(0.5F, 0.5F, 0.5F)
                }
            });

            rules.Add("mdlExecutioner", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(-0.0001F, 0.0021F, -0.0004F),
localAngles = new Vector3(53.1893F, 234.3853F, 213.5773F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            rules.Add("mdlNemmando", new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = displayPrefab,
                    childName = "Head",
localPos = new Vector3(0F, 0.0019F, -0.0008F),
localAngles = new Vector3(90F, 30F, 0F),
localScale = new Vector3(0.001F, 0.001F, 0.001F)
                }
            });

            return rules;
        }


        protected override void SetupMaterials(GameObject modelPrefab) {
            Renderer rend = modelPrefab.GetComponentInChildren<MeshRenderer>();
            rend.materials[0].shader = Modules.Assets.hotpoo;
            rend.materials[1] = Modules.Assets.CreateMaterial("matTempWhite", 1, new Color(0.37f, 0.28f, 0.4f), 0);
        }

        private void SetUpSoulPickup()
        {
            MonsterSoulPickup = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), "MonsterSoul", true);
            MonsterSoulPickup.transform.localScale = Vector3.one * 2.5f;
            MonsterSoulPickup.GetComponent<Rigidbody>().drag = 15f; //default is 2
            MonsterSoulPickup.GetComponent<DestroyOnTimer>().duration = 20;
            MonsterSoulPickup.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 19;

            //attach pickup to game object
            var pickupObj = MonsterSoulPickup.transform.Find("PickupTrigger").gameObject;
            UnityEngine.Object.Destroy(pickupObj.GetComponent<HealthPickup>());
            var pickupComp = pickupObj.AddComponent<SoulPickup>();
            pickupComp.baseObject = MonsterSoulPickup;

            //change visuals
            var soulFx = MonsterSoulPickup.transform.Find("HealthOrbEffect").gameObject;
            var soulCore = soulFx.transform.Find("VFX").Find("Core").gameObject;
            var coreMat = soulCore.GetComponent<ParticleSystem>().main;
            Color soulColor = new Color(0.85f, 0.07f, 1);
            coreMat.startColor = soulColor;
            var soulPulseGlow = soulFx.transform.Find("VFX").Find("PulseGlow").gameObject;
            var pulseMat = soulPulseGlow.GetComponent<ParticleSystem>().main;
            pulseMat.startColor = soulColor;
            UnityEngine.Object.Destroy(soulFx.transform.Find("TrailParent").gameObject);
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport rpt)
        {
            if (NetworkServer.active && !Run.instance.isRunStopwatchPaused)
            {
                CharacterBody victimBody = rpt.victimBody;
                CharacterBody attackerBody = rpt.attackerBody;
                if (victimBody && attackerBody && attackerBody.inventory)
                {
                    if (GetCount(attackerBody) > 0 && victimBody.master)
                    {
                        GameObject soul = UnityEngine.Object.Instantiate(MonsterSoulPickup, victimBody.corePosition, UnityEngine.Random.rotation);
                        soul.GetComponent<TeamFilter>().teamIndex = rpt.attackerTeamIndex;
                        soul.GetComponentInChildren<SoulPickup>().team = soul.GetComponent<TeamFilter>();
                        NetworkServer.Spawn(soul);
                    }
                }
            }
        }
    }

    class SoulPickup : MonoBehaviour
    {
        /* per shooty's suggestion:
         * - store drop chance on ss2item object
         * - start drop chance at 1%, increase by 1 on every kill up to 20
         * - if pickup drops item, reset drop chance to 1
         * there are flaws to this implementation that would make it unworkable. if we nerf the item it'd be better to just reduce the drop chance for now
         */
        public GameObject baseObject;
        public TeamFilter team;
        private bool alive = true;

        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && this.alive && TeamComponent.GetObjectTeam(other.gameObject) == this.team.teamIndex)
            {
                CharacterBody body = other.GetComponent<CharacterBody>();
                if (body)
                {
                    alive = false;

                    Util.PlaySound("SS2UStirringSoul", this.gameObject);

                    // this should not be any higher than 3, in its current state
                    if (Util.CheckRoll(StaticValues.soulChance, body.master))
                    {
                        //~1% red, ~16% green
                        ItemCore.DropShipCall(this.transform, 1, 5);
                    }
                    UnityEngine.Object.Destroy(this.baseObject);
                }
            }
        }
    }
}

