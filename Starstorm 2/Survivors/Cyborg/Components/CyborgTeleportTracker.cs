using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg.Components
{
    public class CyborgTeleportTracker : NetworkBehaviour
    {
        [SyncVar]
        private bool _canTeleport = false;

        public bool CanTeleport()
        {
            return _canTeleport;
        }

        public Vector3? GetTeleportCoordinates()
        {
            return null;
        }

        [Server]
        public void SetTeleporter(GameObject teleporterObject)
        {
            if (!NetworkServer.active) return;
            DestroyTeleporter();
        }

        [Server]
        public void DestroyTeleporter()
        {
            if (!NetworkServer.active) return;
        }
    }
}
