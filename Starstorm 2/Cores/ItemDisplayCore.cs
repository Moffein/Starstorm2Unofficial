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

        public static bool aetheriumInstalled = false;
        public static bool supplyDropInstalled = false;
        public static bool scepterInstalled = false;

        public ItemDisplayCore()
        {
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KomradeSpectre.Aetherium")) aetheriumInstalled = true;
            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.K1454.SupplyDrop")) supplyDropInstalled = true;
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

        // none of this actually worked fuck my entire life
        /*public static void AddItemDisplayRule(string bodyName, string itemName, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            AddItemDisplayRule(bodyName, itemName, LoadDisplay(prefabName), childName, position, rotation, scale);
        }

        public static void AddItemDisplayRule(string bodyName, string itemName, GameObject displayPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body");
            if (!bodyPrefab) { LogCore.logger.LogError("Body name is invalid: " + bodyName); return; }

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            if (!modelLocator) { LogCore.logger.LogError("Failed to add item display: there is no ModelLocator component on target body " + bodyName); return; }

            CharacterModel characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
            if (!characterModel) { LogCore.logger.LogError("Failed to add item display: there is no CharacterModel component on target body " + bodyName); return; }

            ItemDisplayRuleSet itemDisplayRuleSet = characterModel.itemDisplayRuleSet;
            ItemDisplayRuleSet.NamedRuleGroup[] item = itemDisplayRuleSet.namedItemRuleGroups;

            Array.Resize(ref item, item.Length + 1);
            item[item.Length - 1] = CreateGenericDisplayRule(itemName, displayPrefab, childName, position, rotation, scale);
            itemDisplayRuleSet.namedItemRuleGroups = item;
            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
        }

        public static void AddEquipmentDisplayRule(string bodyName, string itemName, string prefabName, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            AddItemDisplayRule(bodyName, itemName, LoadDisplay(prefabName), childName, position, rotation, scale);
        }

        public static void AddEquipmentDisplayRule(string bodyName, string itemName, GameObject displayPrefab, string childName, Vector3 position, Vector3 rotation, Vector3 scale)
        {
            GameObject bodyPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body");
            if (!bodyPrefab) { LogCore.logger.LogError("Body name is invalid: " + bodyName); return; }

            ModelLocator modelLocator = bodyPrefab.GetComponent<ModelLocator>();
            if (!modelLocator) { LogCore.logger.LogError("Failed to add item display: there is no ModelLocator component on target body " + bodyName); return; }

            CharacterModel characterModel = modelLocator.modelTransform.GetComponent<CharacterModel>();
            if (!characterModel) { LogCore.logger.LogError("Failed to add item display: there is no CharacterModel component on target body " + bodyName); return; }

            ItemDisplayRuleSet itemDisplayRuleSet = characterModel.itemDisplayRuleSet;
            ItemDisplayRuleSet.NamedRuleGroup[] equip = itemDisplayRuleSet.namedEquipmentRuleGroups;

            Array.Resize(ref equip, equip.Length + 1);
            equip[equip.Length - 1] = CreateGenericDisplayRule(itemName, displayPrefab, childName, position, rotation, scale);
            itemDisplayRuleSet.namedEquipmentRuleGroups = equip;
        }*/

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
            ItemDisplayRuleSet itemDisplayRuleSet = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;
            ItemDisplayRuleSet.KeyAssetRuleGroup[] equip = itemDisplayRuleSet.keyAssetRuleGroups;

            //TODO: find new way to fix capacitor display if it's broken again
            //capacitorPrefab = PrefabAPI.InstantiateClone(itemDisplayRuleSet.FindEquipmentDisplayRuleGroup("Lightning").rules[0].followerPrefab, "DisplayCustomLightning", true);
            //capacitorPrefab.AddComponent<UnityEngine.Networking.NetworkIdentity>();
            //var limbMatcher = capacitorPrefab.GetComponent<LimbMatcher>();
            //limbMatcher.limbPairs[0].targetChildLimb = "ShoulderL";
            //limbMatcher.limbPairs[1].targetChildLimb = "ElbowL";
            //limbMatcher.limbPairs[2].targetChildLimb = "HandL";

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }

            for (int i = 0; i < equip.Length; i++)
            {
                ItemDisplayRule[] rules = equip[i].displayRuleGroup.rules;
                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;

                    if (followerPrefab)
                    {
                        string name2 = followerPrefab.name;
                        string key2 = (name2 != null) ? name2.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key2))
                        {
                            itemDisplayPrefabs[key2] = followerPrefab;
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