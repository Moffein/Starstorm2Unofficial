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
                pi = Tier3DropTable.GenerateDrop(Run.instance.treasureRng);
            }

            if (pi != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pi, base.transform.position, Vector3.up * 20f);
            }

            Destroy(this);
        }
    }
}
