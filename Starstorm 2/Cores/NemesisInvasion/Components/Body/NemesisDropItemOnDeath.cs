using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.NemesisInvasion.Components.Body
{
    public class NemesisDropItemOnDeath : MonoBehaviour
    {
        private CharacterBody characterBody;
        private HealthComponent healthComponent;
        public ItemIndex itemToDrop = ItemIndex.None;
        private bool droppedItem = false;

        public delegate void NemesisKilledDelegate(CharacterBody body);
        public static NemesisKilledDelegate onNemesisKilledServer;
        private static PickupDropTable Tier3DropTable = Addressables.LoadAssetAsync<PickupDropTable>("RoR2/Base/GoldChest/dtGoldChest.asset").WaitForCompletion();

        public void Awake()
        {
            healthComponent = base.GetComponent<HealthComponent>();
            characterBody = base.GetComponent<CharacterBody>();
        }

        //Remove Blacklisted Items added via EWI
        /*public void Start()
        {
            if (NetworkServer.active && NemesisInvasionManager.forceRemoveBlacklistedItems && characterBody.inventory)
            {
                for (int i = 0; i< characterBody.inventory.itemStacks.Length; i++)
                {
                    if (characterBody.inventory.itemStacks[i] > 0)
                    {
                        ItemDef id = ItemCatalog.GetItemDef((ItemIndex)i);
                        if (id)
                        {
                            if (((id.ContainsTag(ItemTag.AIBlacklist) && NemesisInvasionManager.useAIBlacklist)
                                || (id.ContainsTag(ItemTag.BrotherBlacklist) && NemesisInvasionManager.useMithrixBlacklist)
                                || (id.ContainsTag(ItemTag.CannotCopy) && NemesisInvasionManager.useEngiTurretBlacklist)
                                || (id.ContainsTag(ItemTag.Healing) && NemesisInvasionManager.useHealingBlacklist)
                                || NemesisInvasionManager.itemBlacklist.Contains(id.itemIndex)) && !NemesisInvasionManager.itemWhitelist.Contains(id.itemIndex))
                            {
                                int itemcount = characterBody.inventory.GetItemCount(id.itemIndex);
                                if (itemcount > 0) characterBody.inventory.RemoveItem(id, itemcount);
                            }
                        }
                    }
                }
            }
        }*/

        public void FixedUpdate()
        {
            if (NetworkServer.active && !droppedItem)
            {
                if (healthComponent && !healthComponent.alive && healthComponent.body && healthComponent.body.master && healthComponent.body.master.IsDeadAndOutOfLivesServer())
                {
                    onNemesisKilledServer?.Invoke(characterBody);
                    DropItem();
                }
            }
        }

        private void DropItem()
        {
            if (droppedItem) return;
            droppedItem = true;

            PickupIndex pi = PickupIndex.none;
            if (itemToDrop != ItemIndex.None)
            {
                pi = PickupCatalog.FindPickupIndex(itemToDrop);
            }
            else
            {
                var pickup = Tier3DropTable.GeneratePickup(Run.instance.treasureRng);
                if (pickup != null)
                {
                    PickupDropletController.CreatePickupDroplet(pickup, base.transform.position, Vector3.up * 20f);
                    Destroy(this);
                    return;
                }
            }

            if (pi != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pi, base.transform.position, Vector3.up * 20f);
            }

            Destroy(this);
        }
    }
}
