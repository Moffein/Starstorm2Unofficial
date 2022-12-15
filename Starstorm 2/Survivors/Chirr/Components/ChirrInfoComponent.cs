using RoR2;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Chirr.Components
{
    public class ChirrInfoComponent : NetworkBehaviour
    {
        public CharacterBody friend = new CharacterBody();
        public bool friendState = false;
        public bool sharing = false;
        public Inventory baseInventory;
        public CharacterBody mouseTarget;
        public CharacterBody pingTarget;
        public HurtBox futureFriend;
    }
}
