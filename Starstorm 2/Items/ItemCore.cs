using BepInEx;
using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.ContentManagement;
using Starstorm2Unofficial.Cores.Items;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores
{
    class ItemCore
    {
        public static ItemCore instance;

        /* To add an item:
         * 1. create a class for it in Cores/Items, following the format for other items
         * 2. add it to the items list below
         * 3. add its language tokens to starstorm2.language
         */

        public List<SS2Item> items = new List<SS2Item>();
        public List<ItemDef> itemDefs = new List<ItemDef>();

        //No api to create new item tiers afaik so we'll piggyback on NoTier
        //Using anything higher than NoTier works until e.g. another mod tries to sort your inventory display
        //Leaving unused for now
        public static ItemTier sibTier = ItemTier.NoTier;

        public static bool dropInMultiplayerInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.niwith.DropInMultiplayer");

        public ItemCore()
        {
            //LogCore.LogInfo("Initializing Core: " + base.ToString());
            instance = this;
        }

        public void InitItems()
        {
            //FIXME: sibylline items need to be rewritten to work this way
            List<ItemIndex> dropInInvalidItems = new List<ItemIndex>();
            foreach (var item in items)
            {
                item.Init();
                if (item.DropInMultiBlacklist)
                    dropInInvalidItems.Add(item.itemDef.itemIndex);
                itemDefs.Add(item.itemDef);
            }
            /*if (dropInMultiplayerInstalled)
                AddDropInBlacklist(dropInInvalidItems);*/
        }

        //Shared item functions

        //broken
        /*public void AddDropInBlacklist(List<ItemIndex> items)
        {
            DropInMultiplayer.DropInMultiplayer.AddInvalidItems(items);
        }*/

        public static void DropShipCall(Transform transform, int itemCount, uint teamLevel = 1)
        {
            List<PickupIndex> dropList;
            float rarityscale = itemCount * (float)(Math.Sqrt(teamLevel * 13) - 4);
            if (Util.CheckRoll(0.5f * rarityscale - 1))
                dropList = Run.instance.availableTier3DropList;
            else if (Util.CheckRoll(4 * rarityscale))
                dropList = Run.instance.availableTier2DropList;
            else
                dropList = Run.instance.availableTier1DropList;
            int item = Run.instance.treasureRng.RangeInt(0, dropList.Count);

            PickupDropletController.CreatePickupDroplet(dropList[item], transform.position, new Vector3(0, 0, 0));
        }

        public static void RollNkota(Transform transform, int itemCount)
        {
            int rerollCount = itemCount - 1;

            float t1Chance = 79.2f;
            float t2Chance = 19.8f;
            float t3Chance = 1f;

            int tier = RollTierSmallChest();
            for (int i = 0; i < rerollCount && tier < 3; i++)
            {
                int newTier = RollTierSmallChest();
                if (newTier > tier) tier = newTier;
            }

            List<PickupIndex> dropList;
            switch (tier)
            {
                case 3:
                    dropList = Run.instance.availableTier3DropList;
                    break;
                case 2:
                    dropList = Run.instance.availableTier2DropList;
                    break;
                default:
                    dropList = Run.instance.availableTier1DropList;
                    break;
            }

            int item = Run.instance.treasureRng.RangeInt(0, dropList.Count);
            PickupDropletController.CreatePickupDroplet(dropList[item], transform.position, new Vector3(0, 0, 0));
        }

        private static int RollTierSmallChest()
        {
            int tier = 1;

            if (Util.CheckRoll(1))
            {
                tier = 3;
            }
            else if (Util.CheckRoll(100f * (19.8f/99f)))
            {
                tier = 2;
            }

            return tier;
        }
    }
}
