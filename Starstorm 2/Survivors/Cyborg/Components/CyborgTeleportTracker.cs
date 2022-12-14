using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg.Components
{
    public class CyborgTeleportTracker : NetworkBehaviour
    {
        [SyncVar]
        private Vector3 _teleportPosition = Vector3.zero;

        [SyncVar]
        private bool _canTeleport = false;

        private GameObject teleportObject;

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (_canTeleport && !teleportObject)
                {
                    DestroyTeleporter();
                }
            }
        }

        public Vector3? GetTeleportCoordinates()
        {
            if (!_canTeleport) return null;
            return _teleportPosition;
        }

        public void SetTeleporter(GameObject teleportProjectile)
        {
            if (!NetworkServer.active || !teleportProjectile) return;
            if (teleportObject) DestroyTeleporter();
            teleportObject = teleportProjectile;
            _teleportPosition = teleportObject.transform.position;
            _canTeleport = true;
        }

        public void DestroyTeleporter()
        {
            if (!NetworkServer.active) return;
            if (teleportObject) UnityEngine.Object.Destroy(teleportObject);
            teleportObject = null;
            _canTeleport = false;
        }
    }
}
