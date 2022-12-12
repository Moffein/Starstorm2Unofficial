using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

//Future plans:
// - guaranteed nemesis boss spawn on 9th void cell, like nemesis enforcer
// - larger 9th void cell radius?
// - void mode active: nemesis bosses and void elites added to director
// - void mode deactivated after all nemeses are defeated (each removed from director after spawning)
//TODO: commencement outside is more green than purple, experiment with it more
//FIXME: visuals don't appear under unknown circumstances

namespace Starstorm2.Cores
{
    public class VoidCore
    {
        public static VoidCore instance { get; private set; }
        internal Xoroshiro128Plus rng = new Xoroshiro128Plus(0ul);

        public struct NemesisSpawnData
        {
            /// <summary>
            /// The monster prefab to be spawned.
            /// </summary>
            public GameObject masterPrefab;
            /// <summary>
            /// The item to be dropped by the monster when it is defeated.
            /// </summary>
            public ItemDef itemDrop;
            /// <summary>
            /// The music that plays during the fight. Not currently implemented
            /// </summary>
            public string musicString;
        }

        public static ItemDef nemesisBossItem;

        /// <summary>
        /// List of enemy prefabs to be added to the game director when void mode is active. Add to this directly using nemesisSpawns.add(new NemesisSpawnData {...})
        /// </summary>
        public static List<NemesisSpawnData> nemesisSpawns = new List<NemesisSpawnData>()
        {
            new NemesisSpawnData
            {
                masterPrefab = Modules.Survivors.Nemmando.bossMasterPrefab,
                itemDrop = Items.StirringSoul.instance.itemDef,
                musicString = ""
            }
        };
        private Transform hudRoot;

        //whether void fields has been cleared in the current run
        public bool voidCleared { get; private set; }
        //nemesis spawn cards to be added to director
        private List<DirectorAPI.DirectorCardHolder> nemCards;

        public VoidCore()
        {
            instance = this;

            nemesisBossItem = ScriptableObject.CreateInstance<ItemDef>();
            nemesisBossItem.tier = ItemTier.NoTier;
            nemesisBossItem.canRemove = false;
            nemesisBossItem.hidden = true;
            nemesisBossItem.name = "NemesisBoss";
            nemesisBossItem.nameToken = "";

            Modules.Items.itemDefs.Add(nemesisBossItem);

            LanguageAPI.Add("VOID_MODE_ACTIVE_WARNING", "<style=cIsHealth>An unnatural force emanates from the void...</style>");
            LanguageAPI.Add("VOID_MODE_DEACTIVATED", "<style=cWorldEvent>The void's influence fades...</style>");

            On.RoR2.Run.Start += (orig, run) =>
            {
                orig(run);

                //change this to true to test void mode without having to clear void fields
                voidCleared = false;

                if (NetworkServer.active)
                    rng.ResetSeed(run.seed);

                PrepareNemesisSpawnCards();
                AdjustNemesisItemBlacklists();

                foreach (DirectorAPI.DirectorCardHolder c in nemCards)
                {
                    NemesisSpawnCard nem = (NemesisSpawnCard)c.Card.spawnCard;
                    nem.hasBeenSpawned = false;
                }
            };

            //prepare nemesis spawn cards
            nemCards = new List<DirectorAPI.DirectorCardHolder>();
            //to investigate: if other mods are adding nemesis fights to us, this may need to be called later (Starstorm.awake?)
            foreach (var spawn in nemesisSpawns)
            {
                NemesisSpawnCard nemCard = NemesisSpawnCard.CreateCard(spawn.masterPrefab);
                if (nemCard)
                {
                    DirectorCard dirCard = new DirectorCard();
                    dirCard.spawnCard = nemCard;
                    //FIXME: nemeses should have a combined selection weight; otherwise each new nemesis messes with existing spawn weights
                    //but selection weights have to be integers. hmm
                    dirCard.selectionWeight = 10000;
                    nemCards.Add(new DirectorAPI.DirectorCardHolder()
                    {
                        Card = dirCard,
                        MonsterCategory = DirectorAPI.MonsterCategory.Champions
                    });
                }
            }

            //On.RoR2.ArenaMissionController.OnStartServer += ArenaMissionController_OnStartServer;
            //On.RoR2.UI.HUD.Awake += HUD_Awake;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.Run.AdvanceStage += Run_AdvanceStage;
            On.RoR2.SceneCatalog.OnActiveSceneChanged += SceneCatalog_OnActiveSceneChanged;
            On.RoR2.SceneDirector.Start += SceneDirector_Start;
            //On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.TeleporterInteraction.ChargingState.OnEnter += ChargingState_OnEnter;

            //add cards to director under void mode
            DirectorAPI.MonsterActions += delegate (List<DirectorAPI.DirectorCardHolder> list, DirectorAPI.StageInfo stage)
            {
                if (voidCleared)
                {
                    foreach (DirectorAPI.DirectorCardHolder holder in nemCards)
                    {
                        NemesisSpawnCard nemCard = (NemesisSpawnCard)holder.Card.spawnCard;
                        if (!nemCard.hasBeenSpawned && !list.Contains(holder))
                        {
                            //scale credit cost to match what the boss director for this stage will get - prevents multiple nemesis spawns unless shrine of the mountain is used
                            nemCard.directorCreditCost = ((int)(600f * Mathf.Pow(Run.instance.compensatedDifficultyCoefficient, 0.5f)));
                            /*
                            int weight = 0;
                            foreach (DirectorAPI.DirectorCardHolder c in list)
                            {
                                if (c.MonsterCategory == DirectorAPI.MonsterCategory.Champions)
                                    weight += c.Card.selectionWeight;
                            }
                            holder.Card.selectionWeight = weight * 5;
                            */
                            list.Add(holder);
                        }
                    }
                }
            };
        }

        private void ChargingState_OnEnter(On.RoR2.TeleporterInteraction.ChargingState.orig_OnEnter orig, EntityStates.BaseState self)
        {
            int oldStacks = 0;

            if (voidCleared)
            {
                if (TeleporterInteraction.instance)
                {
                    oldStacks = TeleporterInteraction.instance.shrineBonusStacks;
                    TeleporterInteraction.instance.shrineBonusStacks = 0;
                }
            }

            orig(self);

            if (voidCleared)
            {
                if (TeleporterInteraction.instance)
                {
                    TeleporterInteraction.instance.shrineBonusStacks = oldStacks;
                }
            }
        }

        private void PrepareNemesisSpawnCards()
        {
            nemCards = new List<DirectorAPI.DirectorCardHolder>();
            foreach (var spawn in nemesisSpawns)
            {
                NemesisSpawnCard nemCard = NemesisSpawnCard.CreateCard(spawn.masterPrefab);
                DirectorCard dirCard = new DirectorCard();
                dirCard.spawnCard = nemCard;
                //FIXME: nemeses should have a combined selection weight; otherwise each new nemesis messes with existing spawn weights
                //but selection weights have to be integers. hmm
                dirCard.selectionWeight = 10000;
                nemCards.Add(new DirectorAPI.DirectorCardHolder()
                {
                    Card = dirCard,
                    MonsterCategory = DirectorAPI.MonsterCategory.Champions
                });

                string music = "NemesisTheme";
                if (spawn.musicString != "") music = spawn.musicString;

                spawn.masterPrefab.AddComponent<Components.NemesisMusicComponentMaster>().musicString = music;
            }
        }

        private void AdjustNemesisItemBlacklists()
        {
            var t1 = Run.instance.availableTier1DropList.Where(new Func<PickupIndex, bool>(PickupIsNonBlacklistedItem)).ToList();
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Bear.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Medkit.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.HealWhileSafe.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Crowbar.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Tooth.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.ArmorPlate.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.StickyBomb.itemIndex));
            t1.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.BleedOnHit.itemIndex));

            var t2 = Run.instance.availableTier2DropList.Where(new Func<PickupIndex, bool>(PickupIsNonBlacklistedItem)).ToList();
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Thorns.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Missile.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.SlowOnHit.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.HealOnCrit.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.CritHeal.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.FireRing.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.IceRing.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Seed.itemIndex));
            t2.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.ChainLightning.itemIndex));

            var t3 = Run.instance.availableTier3DropList.Where(new Func<PickupIndex, bool>(PickupIsNonBlacklistedItem)).ToList();
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.ShockNearby.itemIndex)); //jesus tapdancing christ why isn't this blacklisted in vanilla yet
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.NovaOnHeal.itemIndex));
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.ExtraLife.itemIndex));
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Plant.itemIndex)); //this one is an on-kill effect and hopoo forgot to tag it as such; should already be blacklisted
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.Behemoth.itemIndex));
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.IncreaseHealing.itemIndex));
            t3.Remove(PickupCatalog.FindPickupIndex(RoR2Content.Items.BounceNearby.itemIndex));

            NemesisSpawnCard.availableTier1Items = t1;
            NemesisSpawnCard.availableTier2Items = t2;
            NemesisSpawnCard.availableTier3Items = t3;
        }

        private bool PickupIsNonBlacklistedItem(PickupIndex idx)
        {
            ItemTag[] forbiddenTags = new ItemTag[]
            {
                ItemTag.AIBlacklist,
                ItemTag.BrotherBlacklist,
                ItemTag.EquipmentRelated,
                ItemTag.SprintRelated,
                ItemTag.OnKillEffect
            };
            var def = PickupCatalog.GetPickupDef(idx);
            if (def == null)
                return false;
            var itemdef = ItemCatalog.GetItemDef(def.itemIndex);
            if (itemdef == null)
                return false;
            foreach(ItemTag tag in forbiddenTags)
            {
                if (itemdef.ContainsTag(tag))
                    return false;
            }
            return true;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            float prevHealth = (float)self.healthComponent?.health;
            orig(self);
            Inventory inv = self.inventory;
            if (inv)
            {
                if (inv.GetItemCount(nemesisBossItem) > 0)
                {
                    /*
                    float lvl = self.level - 1;
                    self.maxHealth = (self.baseMaxHealth + self.levelMaxHealth * lvl) * 12;
                    self.healthComponent.health = prevHealth;
                    self.damage = (self.baseDamage + self.levelDamage * lvl) * 0.75f;
                    */
                }
            }
        }

        //hud code for displaying nemesis's inventory under the objective panel
        //this is unfinished and doesn't work, but it would be nice for gameplay and polish
        private void HUD_Awake(On.RoR2.UI.HUD.orig_Awake orig, RoR2.UI.HUD self)
        {
            orig(self);

            hudRoot = self.transform.root;
            var invObj = new GameObject("NemesisInventoryDisplay");
            invObj.layer = 5;
            invObj.transform.SetParent(hudRoot.Find("MainContainer").Find("MainUIArea").Find("UpperRightCluster").Find("TimerRoot").Find("RightInfoBar"));
            var canvas = invObj.AddComponent<CanvasRenderer>();
            var img = invObj.AddComponent<Image>();
            img.sprite = Resources.Load<Sprite>("textures/itemicons/texBearIcon");
            /*
            var tr = invObj.AddComponent<RectTransform>();
            tr.anchorMin = Vector2.zero;
            tr.anchorMax = Vector2.one;
            tr.sizeDelta = Vector2.zero;
            tr.anchoredPosition = Vector2.zero;
            */
            var invDisp = invObj.AddComponent<RoR2.UI.ExpBar>();
            invDisp.enabled = true;
            //Inventory testinv = new Inventory();
            //testinv.GiveItem(ItemIndex.Hoof, 3);
            //testinv.GiveItem(ItemIndex.Crowbar, 3);
            //invDisp.SetItems(testinv.itemAcquisitionOrder, testinv.itemStacks);
            //invDisp.SetSubscribedInventory(testinv);
            //invDisp.enabled = true;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport rpt)
        {
            //make nemesis drop its unique item
            var victim = rpt.victimMaster;
            if (victim)
            {
                foreach (var spawn in nemesisSpawns)
                {
                    if (MasterCatalog.GetMasterPrefab(victim.masterIndex) == spawn.masterPrefab)
                    {
                        PickupDropletController.CreatePickupDroplet(PickupCatalog.FindPickupIndex(spawn.itemDrop.itemIndex), rpt.victimBody.corePosition, Vector3.up * 20f);
                    }
                }
            }
        }

        //changes to void fields behavior go here
        //FIXME: this doesn't fire, or fires when we aren't looking for it to fire
        // just call ArenaMissionController_BeginRound and check for round 0, lol
        private void ArenaMissionController_OnStartServer(On.RoR2.ArenaMissionController.orig_OnStartServer orig, ArenaMissionController self)
        {
            orig(self);
        }

        //might be running on server only
        private void Run_AdvanceStage(On.RoR2.Run.orig_AdvanceStage orig, Run self, SceneDef nextScene)
        {
            orig(self, nextScene);

            SceneDef scene = Stage.instance.sceneDef;
            if (scene.baseSceneName == "arena")
            {
                voidCleared = true;

                //FIXME: doesn't pop up chat when it broadcasts
                self.StartCoroutine(Utils.BroadcastChat("VOID_MODE_ACTIVE_WARNING"));
            }
        }

        private void SceneDirector_Start(On.RoR2.SceneDirector.orig_Start orig, SceneDirector self)
        {
            if (voidCleared)
            {
                if (AllNemesesDead())
                {
                    voidCleared = false;
                    self.StartCoroutine(Utils.BroadcastChat("VOID_MODE_DEACTIVATED"));
                }
                else
                    ApplyVoidFieldsVisuals(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
            }
            orig(self);
        }

        //changes to stages post-void fields go here
        private void SceneCatalog_OnActiveSceneChanged(On.RoR2.SceneCatalog.orig_OnActiveSceneChanged orig, UnityEngine.SceneManagement.Scene oldScene, UnityEngine.SceneManagement.Scene newScene)
        {
            orig(oldScene, newScene);
            if (voidCleared)
            {
                //if (!AllNemesesDead())
            }
        }

        private bool AllNemesesDead()
        {
            foreach (var c in nemCards)
            {
                NemesisSpawnCard nem = (NemesisSpawnCard)c.Card.spawnCard;
                if (!nem.hasBeenSpawned)
                    return false;
            }
            return true;
        }

        private void ApplyVoidFieldsVisuals(string sceneName)
        {
            PostProcessProfile[] profiles = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
            PostProcessProfile destProfile = profiles.Where(p => p.name == "ppSceneArenaSick").FirstOrDefault();

            //there are at least three different places that might be holding the post-process volume we're looking for - thanks hopoo
            //most stages store it in sceneinfo
            PostProcessVolume ppvol = null;

            SceneInfo sceneInfo = SceneInfo.instance;
            if (sceneInfo)
            {
                ppvol = sceneInfo.GetComponent<PostProcessVolume>();
            }
            if (!ppvol)
            {
                //some stages (e.g. titanic plains) put it here
                var ppvolObj = GameObject.Find("PP + Amb");
                //at least one stage (bazaar) puts it here
                if (!ppvolObj)
                    ppvolObj = GameObject.Find("PP, Global");
                //title screen puts it here (this will happen after finishing a run that has been to void fields)
                if (!ppvolObj)
                    ppvolObj = GameObject.Find("GlobalPostProcessVolume, Base");
                if (ppvolObj)
                {
                    ppvol = ppvolObj.GetComponent<PostProcessVolume>();
                }
                else
                {
                    //if this is a modded stage that does things differently or hopoo decides to totally change every stage for some reason, then OH WELL!!
                    ppvol = null;
                }
            }

            if (ppvol)
            {
                //we can't mess with the existing postprocessing on commencement or the game spergs the fuck out
                if (sceneName == "moon" || sceneName == "moon2")
                {
                    ppvol = sceneInfo.AddComponent<PostProcessVolume>();
                    ppvol.enabled = true;
                    ppvol.isGlobal = true;
                    ppvol.priority = 9999;
                }
                ppvol.profile = destProfile;
                ppvol.weight = 0.85f;
            }
            else
            {
                LogCore.LogWarning("Could not find a suitable post-process volume to modify - skipping void visuals");
            }

        }

        private PickupIndex PickNemesisItemDrop()
        {
            return Run.instance.availableTier3DropList[UnityEngine.Random.Range(0, Run.instance.availableTier3DropList.Count)];
        }
    }

    class NemesisSpawnCard : CharacterSpawnCard
    {
        public bool hasBeenSpawned;

        internal static List<PickupIndex> availableTier1Items;
        internal static List<PickupIndex> availableTier2Items;
        internal static List<PickupIndex> availableTier3Items;

        //TODO: dio
        private static readonly ItemTag[] forbiddenTags = new ItemTag[]
        {
            ItemTag.AIBlacklist,
            ItemTag.EquipmentRelated,
            //ItemTag.Cleansable,
            ItemTag.Scrap
        };

        public static NemesisSpawnCard CreateCard(GameObject basePrefab)
        {
            if (!basePrefab)
            {
                LogCore.LogWarning("Skipping creation of nemesis spawn card (no prefab; character disabled in config?)");
                return null;
            }
            CharacterMaster master = basePrefab.GetComponent<CharacterMaster>();
            if (!master)
            {
                LogCore.LogWarning($"Skipping creation of nemesis spawn card for {basePrefab.name} (prefab has no CharacterMaster)");
                return null;
            }
            CharacterBody body = master.bodyPrefab?.GetComponent<CharacterBody>();
            if (!body)
            {
                LogCore.LogWarning($"Skipping creation of nemesis spawn card for {basePrefab.name} (prefab has no CharacterBody)");
                return null;
            }
            NemesisSpawnCard card = ScriptableObject.CreateInstance<NemesisSpawnCard>();
            body.isChampion = true;

            card.directorCreditCost = 600;
            card.forbiddenAsBoss = false;
            //this.requiredFlags = NodeFlags.None;
            //this.forbiddenFlags = NodeFlags.None;
            card.noElites = true;
            card.prefab = basePrefab;
            card.hullSize = body.hullClassification;
            card.nodeGraphType = body.isFlying ? MapNodeGroup.GraphType.Air : MapNodeGroup.GraphType.Ground;
            card.sendOverNetwork = true;
            card.runtimeLoadout = new Loadout();
            card.hasBeenSpawned = false;

            return card;
        }

        //this really activates my almonds
        public override Action<CharacterMaster> GetPreSpawnSetupCallback()
        {
            if (!this.hasBeenSpawned)
                return new Action<CharacterMaster>(PreSpawnSetup);
            return null;
        }

        private void PreSpawnSetup(CharacterMaster spawnedMaster)
        {
            //only one at a time should ever spawn even when the director wants to spawn a group of them
            //really bad idea that probably doesn't work
            //FIXME: this doesn't work
            /*
            if (this.hasBeenSpawned)
            {
                spawnedMaster.DestroyBody();
                Destroy(spawnedMaster);
                return;
            }
            */
            this.hasBeenSpawned = true;

            Inventory inv = spawnedMaster.inventory;
            float hpBoostCoefficient = 1f;
            float damageBoostCoefficient = 1f;
            hpBoostCoefficient += Run.instance.difficultyCoefficient / 2.5f;
            damageBoostCoefficient += Run.instance.difficultyCoefficient / 30f;
            int num3 = Mathf.Max(1, Run.instance.livingPlayerCount);
            hpBoostCoefficient *= Mathf.Pow((float)num3, 0.5f);

            int shrineBonus = 0;
            if (TeleporterInteraction.instance)
            {
                shrineBonus += TeleporterInteraction.instance.shrineBonusStacks;
            }

            inv.GiveItem(RoR2Content.Items.BoostHp.itemIndex, Mathf.RoundToInt((hpBoostCoefficient - 1f) * (10f + (8f * shrineBonus))));
            inv.GiveItem(RoR2Content.Items.BoostDamage.itemIndex, Mathf.RoundToInt((damageBoostCoefficient - 1f) * 2f));
            /*
            Debug.LogFormat("Nemesis Encounter: currentBoostHpCoefficient={0}, currentBoostDamageCoefficient={1}", new object[]
            {
                Mathf.RoundToInt((hpBoostCoefficient-1)*10),
                Mathf.RoundToInt((damageBoostCoefficient-1)*10)
            });
            */
            inv.GiveItem(VoidCore.nemesisBossItem);
            GiveRandomLoadout(inv);

            //RoR2.UI.ItemInventoryDisplay invdisplay = new RoR2.UI.ItemInventoryDisplay();
            //RoR2.UI.EnemyInfoPanel enemypanel = new RoR2.UI.EnemyInfoPanel();
            //enemypanel.inventoryDisplay = invdisplay;
            //enemypanel.inventoryContainer = 
            //if (invdisplay)
            //invdisplay.SetSubscribedInventory(inv);
            //enemypanel.enabled = true;
        }

        private void GiveRandomLoadout(Inventory inv)
        {
            float itemsPerMin = 0.5f;
            float itemsDev = 0.2f;
            int runTimeMins = (int)Math.Round(Run.instance.GetRunStopwatch() / 60);
            int numItems = (int)(runTimeMins * itemsPerMin);
            //add random deviation to item count
            numItems += (int)(numItems * UnityEngine.Random.Range(-itemsDev, itemsDev));

            if (numItems == 0)
                return;

            var selectedTier = new List<PickupIndex>();

            var weightedItems = new WeightedSelection<List<PickupIndex>>(8);
            if (availableTier1Items.Count > 0)
                weightedItems.AddChoice(availableTier1Items, 100);
            if (availableTier2Items.Count > 0)
                weightedItems.AddChoice(availableTier2Items, 60);
            if (availableTier3Items.Count > 0)
                weightedItems.AddChoice(availableTier2Items, 4);
            if (weightedItems.Count == 0)
                return;
            for (int i = 0; i < numItems; i++)
            {
                selectedTier = weightedItems.Evaluate(Run.instance.runRNG.nextNormalizedFloat);
                PickupIndex pickup = selectedTier[Run.instance.runRNG.RangeInt(0, selectedTier.Count)];
                ItemIndex item = pickup.pickupDef.itemIndex;
                inv.GiveItem(item);
                Debug.Log(ItemCatalog.GetItemDef(item).name);
            }

            Debug.Log("Nemesis has spawned. Loadout:");
            foreach(ItemIndex i in inv.itemAcquisitionOrder)
                Debug.Log("  " + inv.GetItemCount(i) + "x " + ItemCatalog.GetItemDef(i).name);
        }
    }
}
