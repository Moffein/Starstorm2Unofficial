using UnityEngine;
using R2API;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Starstorm2.Cores
{
    public class ItemDisplayCore
    {
        public static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        public static GameObject capacitorPrefab;

        public static bool scepterInstalled = false;

        public ItemDisplayCore()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter")) scepterInstalled = true;

            PopulateDisplays();
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGenericDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return CreateGenericDisplayRule(keyAsset, LoadDisplay(prefabName), childName, position, rotation, scale);
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateGenericDisplayRule(Object keyAsset, GameObject itemPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = itemPrefab,
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateMirroredDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = new Vector3(-1f * position.x, position.y, position.z),
                            localAngles = rotation,
                            localScale = new Vector3(scale.x, scale.y, -1f * scale.z)
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateZMirroredDisplayRule(Object keyAsset, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        },
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = childName,
                            followerPrefab = LoadDisplay(prefabName),
                            limbMask = LimbFlags.None,
                            localPos = new Vector3(-1f * position.x, position.y, position.z),
                            localAngles = rotation,
                            localScale = new Vector3(-1f * scale.x, scale.y, scale.z)
                            //look i'm sure there was a better solution than creating another display rule but i'm very high and can't figure out how to do this otherwise
                            // nigga this is literally making shit so easy a monkey could do it, cry about it
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateFollowerDisplayRule(Object keyAsset, GameObject displayPrefab, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup displayRule = new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = keyAsset,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            childName = "Base",
                            followerPrefab = displayPrefab,
                            limbMask = LimbFlags.None,
                            localPos = position,
                            localAngles = rotation,
                            localScale = scale
                        }
                    }
                }
            };

            return displayRule;
        }

        public static ItemDisplayRuleSet.KeyAssetRuleGroup CreateFollowerDisplayRule(Object keyAsset, string prefabName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return CreateFollowerDisplayRule(keyAsset, LoadDisplay(prefabName), position, rotation, scale);
        }

        public static ItemDisplayRule[] CreateDisplayRule(string childName, GameObject displayPrefab, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            return new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    childName = childName,
                    followerPrefab = displayPrefab,
                    limbMask = LimbFlags.None,
                    localPos = position,
                    localAngles = rotation,
                    localScale = scale,
                    ruleType = ItemDisplayRuleType.ParentedPrefab
                }
            };
        }


        public static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }
            Debug.LogWarning("could not find item display prefab for " + name);
            return null;
        }

        private static void PopulateDisplays()
        {
            PopulateFromBody("Commando");
            PopulateFromBody("Croco");
            PopulateFromBody("Mage");
        }
        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = name?.ToLower();
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static ItemDef LoadScepterObject()
        {
            return AncientScepter.AncientScepterItem.instance.ItemDef;
        }
    }
}