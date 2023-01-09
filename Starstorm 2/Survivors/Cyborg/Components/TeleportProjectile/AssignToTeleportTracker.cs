using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components.TeleportProjectile
{
    public class AssignToTeleportTracker : MonoBehaviour
    {
        private Rigidbody rigidBody;
        private CyborgTeleportTracker ctt;

        public void Start()
        {
            if (NetworkServer.active)
            {
                rigidBody = base.GetComponent<Rigidbody>();
                ProjectileController pc = base.GetComponent<ProjectileController>();
                if (pc && pc.owner)
                {
                    ctt = pc.owner.GetComponent<CyborgTeleportTracker>();
                    ctt.SetTeleporterServer(base.gameObject);
                }
            }
        }
    }
}
