using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.NemesisInvasion.Components.Body
{
    public class NemesisDropItemOnDeath : MonoBehaviour
    {
        private CharacterBody characterBody;
        private HealthComponent healthComponent;
        public ItemIndex itemToDrop = ItemIndex.None;

        public delegate void NemesisKilledDelegate(CharacterBody body);
        public static NemesisKilledDelegate onNemesisKilledServer;

        public void Awake()
        {
            healthComponent = base.GetComponent<HealthComponent>();
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void FixedUpdate()
        {
            if (NetworkServer.active && itemToDrop != ItemIndex.None)
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
            PickupIndex pi = PickupCatalog.FindPickupIndex(itemToDrop);
            if (pi != PickupIndex.none)
            {
                PickupDropletController.CreatePickupDroplet(pi, base.transform.position, Vector3.up * 20f);
            }
            itemToDrop = ItemIndex.None;
            Destroy(this);
        }
    }
}
