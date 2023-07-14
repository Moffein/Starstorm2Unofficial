using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using Starstorm2Unofficial.Cores.NemesisInvasion.Components.Body;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace Starstorm2Unofficial.Cores.NemesisInvasion.Components
{
    public class NemesisInvasionManager : MonoBehaviour
    {
        public static PostProcessProfile voidPostProcess = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/DLC1/Common/Void/ppSceneVoidStage.asset").WaitForCompletion();
        public static List<NemesisCard> nemesisCards = new List<NemesisCard>();
        public static NemesisInvasionManager instance;
        public static HashSet<ItemIndex> itemBlacklist = new HashSet<ItemIndex>();
        public static HashSet<ItemIndex> itemWhitelist = new HashSet<ItemIndex>();

        private bool playedEventStartChatMessage = false;
        private bool playedEventEndChatMessage = false;

        private List<NemesisCard> remainingCards;
        public bool voidClearedSuccessfully;
        public static NemesisCard currentCard;
        private int cardSpawnCount;

        public static bool useVoidTeam = false;
        public static bool useAIBlacklist = true;
        public static bool useMithrixBlacklist = true;
        public static bool useEngiTurretBlacklist = true;
        public static bool useHealingBlacklist = true;

        public static bool forceRemoveBlacklistedItems = true;

        public static string nemesisItemBlacklistString;

        public static void Initialize()
        {
            RoR2.Run.onRunStartGlobal += VoidInvasionManagerSetup;
            On.RoR2.ArenaMissionController.MissionCompleted.OnEnter += MissionCompleted_OnEnter;
            RoR2.Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            RoR2.RoR2Application.onLoad += InitNemesisBlacklist;
        }

        private static void VoidInvasionManagerSetup(Run run)
        {
            if (NemesisInvasionManager.instance != null) Destroy(NemesisInvasionManager.instance);
            NemesisInvasionManager.instance = run.AddComponent<NemesisInvasionManager>();
        }

        private static void InitNemesisBlacklist()
        {
            //Build vengeanceBlacklist
            nemesisItemBlacklistString = new string(nemesisItemBlacklistString.ToCharArray().Where(c => !System.Char.IsWhiteSpace(c)).ToArray());
            string[] vsplitBlacklist = nemesisItemBlacklistString.Split(',');
            foreach (string str in vsplitBlacklist)
            {
                NemesisInvasionManager.BlacklistItem(str);
            }

            itemWhitelist.Add(RoR2Content.Items.UseAmbientLevel.itemIndex);
            itemWhitelist.Add(RoR2Content.Items.TeleportWhenOob.itemIndex);
            itemWhitelist.Add(RoR2Content.Items.AdaptiveArmor.itemIndex);
            itemWhitelist.Add(NemesisInvasionCore.NemesisMarkerItem.itemIndex);
        }

        private static void Stage_onStageStartGlobal(Stage obj)
        {
            if (NemesisInvasionManager.instance)
            {
                SceneDef sd = SceneCatalog.GetSceneDefForCurrentScene();
                if (sd && !sd.isFinalStage)
                {
                    if (NemesisInvasionManager.instance.voidClearedSuccessfully)
                    {
                        NemesisInvasionManager.instance.SpawnNemesis();
                    }
                }
            }
        }

        public static void BlacklistItem(string itemName)
        {
            ItemIndex id = ItemCatalog.FindItemIndex(itemName);
            if (id != ItemIndex.None) NemesisInvasionManager.itemBlacklist.Add(id);
        }

        private static void MissionCompleted_OnEnter(On.RoR2.ArenaMissionController.MissionCompleted.orig_OnEnter orig, ArenaMissionController.MissionCompleted self)
        {
            orig(self);
            if (NemesisInvasionManager.instance) NemesisInvasionManager.instance.voidClearedSuccessfully = true;
        }

        private void Awake()
        {
            voidClearedSuccessfully = false;
            remainingCards = new List<NemesisCard>(nemesisCards);
            cardSpawnCount = 0;
        }

        public void SpawnNemesis()
        {
            if (Run.instance && cardSpawnCount < nemesisCards.Count)
            {
                cardSpawnCount++;

                //Todo: give items and spawn
                if (NetworkServer.active)
                {
                    if (remainingCards.Count > 0)
                    {
                        int spawnIndex = new Xoroshiro128Plus(Run.instance.spawnRng).RangeInt(0, remainingCards.Count);
                        NemesisCard card = remainingCards[spawnIndex];
                        remainingCards.Remove(card);

                        StartCoroutine(SpawnNemesisServer(card, new Xoroshiro128Plus(Run.instance.seed + (ulong)Run.instance.stageClearCount)));
                    }
                }

                UnityEngine.SceneManagement.Scene currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                if (currentScene != null)
                {
                    ApplyVoidFieldsVisuals(currentScene.name);
                }
            }
            else
            {
                if (NetworkServer.active)
                {
                    if (!playedEventEndChatMessage)
                    {
                        StartCoroutine(VoidEventEndChat());
                    }
                }
            }
        }

        IEnumerator VoidEventEndChat()
        {
            yield return new WaitForSeconds(3f);
            if (!NetworkServer.active) yield return null;
            playedEventEndChatMessage = true;
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = "NEMESIS_MODE_DEACTIVATED"
            });
            yield return null;
        }

        IEnumerator SpawnNemesisServer(NemesisCard card, Xoroshiro128Plus rng)
        {
            if (!NetworkServer.active) yield return null;

            yield return new WaitForSeconds(3f);
            if (!playedEventStartChatMessage)
            {
                playedEventStartChatMessage = true;
                Chat.SendBroadcastChat(new Chat.SimpleChatMessage
                {
                    baseToken = "NEMESIS_MODE_ACTIVE_WARNING"
                });
            }
            yield return new WaitForSeconds(12f);

            Transform spawnOnTarget = null;
            DirectorCore.MonsterSpawnDistance input = DirectorCore.MonsterSpawnDistance.Far;
            if (TeleporterInteraction.instance)
            {
                spawnOnTarget = TeleporterInteraction.instance.transform;
                input = DirectorCore.MonsterSpawnDistance.Close;
            }
            else
            {
                List<CharacterBody> playerList = CharacterBody.readOnlyInstancesList.Where(cb => (cb.isPlayerControlled && cb.teamComponent && cb.teamComponent.teamIndex == TeamIndex.Player)).ToList();
                if (playerList.Count > 0)
                {
                    CharacterBody firstBody = playerList.FirstOrDefault();
                    if (firstBody)
                    {
                        spawnOnTarget = firstBody.transform;
                    }
                }
            }

            if (spawnOnTarget && Run.instance)
            {
                DirectorPlacementRule directorPlacementRule = new DirectorPlacementRule
                {
                    spawnOnTarget = spawnOnTarget,
                    placementMode = DirectorPlacementRule.PlacementMode.NearestNode
                };

                DirectorSpawnRequest directorSpawnRequest = new DirectorSpawnRequest(card.spawnCard, directorPlacementRule, rng);
                directorSpawnRequest.teamIndexOverride = useVoidTeam ? TeamIndex.Void : TeamIndex.Monster;
                directorSpawnRequest.ignoreTeamMemberLimit = true;

                CombatSquad combatSquad = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/Encounters/ShadowCloneEncounter")).GetComponent<CombatSquad>();
                directorSpawnRequest.onSpawnedServer = (Action<SpawnCard.SpawnResult>)Delegate.Combine(directorSpawnRequest.onSpawnedServer, new Action<SpawnCard.SpawnResult>(delegate (SpawnCard.SpawnResult result)
                {
                    CharacterMaster resultMaster = result.spawnedInstance.GetComponent<CharacterMaster>();
                    if (resultMaster)
                    {
                        combatSquad.AddMember(resultMaster);

                        if (resultMaster.inventory)
                        {
                            resultMaster.inventory.GiveItem(RoR2Content.Items.TeleportWhenOob);
                            resultMaster.inventory.GiveItem(RoR2Content.Items.AdaptiveArmor);
                            resultMaster.inventory.GiveItem(RoR2Content.Items.UseAmbientLevel);
                            resultMaster.inventory.GiveItem(NemesisInvasionCore.NemesisMarkerItem);

                            if (RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.MonsterTeamGainsItems) && directorSpawnRequest.teamIndexOverride != TeamIndex.Monster)
                            {
                                resultMaster.inventory.AddItemsFrom(RoR2.Artifacts.MonsterTeamGainsItemsArtifactManager.monsterTeamInventory);
                            }

                            if (card.itemStacks != null)
                            {
                                for (int i = 0; i < card.itemStacks.Length; i++)
                                {
                                    if (card.itemStacks[i] > 0)
                                    {
                                        resultMaster.inventory.GiveItem((ItemIndex)i, card.itemStacks[i]);
                                    }
                                }
                            }

                            if (card.shouldGrantRandomItems)
                            {
                                //Copied from CHEF
                                List<PickupIndex> list = Run.instance.availableTier1DropList.Where(PickupIsNonBlacklistedItem).ToList();
                                List<PickupIndex> list2 = Run.instance.availableTier2DropList.Where(PickupIsNonBlacklistedItem).ToList();
                                List<PickupIndex> list3 = Run.instance.availableTier3DropList.Where(PickupIsNonBlacklistedItem).ToList();
                                List<PickupIndex> availableEquipmentDropList = Run.instance.availableEquipmentDropList;
                                GrantItems(resultMaster.inventory, list, 3, 9);
                                GrantItems(resultMaster.inventory, list2, 2, 4);
                                GrantItems(resultMaster.inventory, list3, 1, 1);
                            }
                        }

                        CharacterBody cb = resultMaster.GetBody();
                        if (cb)
                        {
                            NemesisDropItemOnDeath ndi = cb.gameObject.AddComponent<NemesisDropItemOnDeath>();

                            if (card.itemDropName != string.Empty)
                            {
                                ItemIndex toDrop = ItemCatalog.FindItemIndex(card.itemDropName);
                                ndi.itemToDrop = toDrop;
                            }
                            else
                            {
                                ndi.itemToDrop = ItemIndex.None;
                            }
                        }
                    }
                }));
                GameObject spawnedObject = DirectorCore.instance.TrySpawnObject(directorSpawnRequest);
                NetworkServer.Spawn(combatSquad.gameObject);
            }
            yield return null;
        }

        private static void GrantItems(Inventory inventory, List<PickupIndex> list, int types, int stackSize)
        {
            for (int i = 0; i < types; i++)
            {
                PickupIndex pickupIndex = list[UnityEngine.Random.Range(0, list.Count)];
                PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
                inventory.GiveItem((pickupDef != null) ? pickupDef.itemIndex : ItemIndex.None, stackSize);
            }
        }

        private static bool PickupIsNonBlacklistedItem(PickupIndex pickupIndex)
        {
            PickupDef pickupDef = PickupCatalog.GetPickupDef(pickupIndex);
            if (pickupDef == null || NemesisInvasionManager.itemBlacklist.Contains(pickupDef.itemIndex))
            {
                return false;
            }
            ItemDef itemDef = ItemCatalog.GetItemDef(pickupDef.itemIndex);
            return !(itemDef == null)
                 && (!useAIBlacklist || itemDef.DoesNotContainTag(ItemTag.AIBlacklist))
                && (!useMithrixBlacklist || itemDef.DoesNotContainTag(ItemTag.BrotherBlacklist))
                && (!useEngiTurretBlacklist || itemDef.DoesNotContainTag(ItemTag.CannotCopy))
                && (!useHealingBlacklist || itemDef.DoesNotContainTag(ItemTag.Healing))
                && itemDef.DoesNotContainTag(ItemTag.EquipmentRelated)
                && itemDef.DoesNotContainTag(ItemTag.HoldoutZoneRelated)
                && itemDef.DoesNotContainTag(ItemTag.ObliterationRelated)
                && itemDef.DoesNotContainTag(ItemTag.Scrap)
                && itemDef.DoesNotContainTag(ItemTag.PriorityScrap)
                && itemDef.DoesNotContainTag(ItemTag.OnStageBeginEffect)
                && itemDef.DoesNotContainTag(ItemTag.Healing)
                && itemDef.DoesNotContainTag(ItemTag.CannotCopy);
        }

        private void ApplyVoidFieldsVisuals(string sceneName)
        {
            //PostProcessProfile[] profiles = Resources.FindObjectsOfTypeAll<PostProcessProfile>();
            //PostProcessProfile destProfile = profiles.Where(p => p.name == "ppSceneArenaSick").FirstOrDefault();
            PostProcessProfile destProfile = NemesisInvasionManager.voidPostProcess;

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
    }
}
