﻿using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components
{
    public class CyborgTeleportTracker : NetworkBehaviour
    {
        [SyncVar]
        private Vector3 _teleportPosition = Vector3.zero;

        [SyncVar]
        private bool _canTeleport = false;

        private GameObject teleportObject;
        public static GameObject teleportDestroyEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/OmniExplosionVFXEngiTurretDeath.prefab").WaitForCompletion();

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (_canTeleport)
                {
                    if (!teleportObject)
                    {
                        DestroyTeleporterServer();
                    }
                    else
                    {
                        Vector3 newTelePos = teleportObject.transform.position;
                        if (newTelePos != _teleportPosition)
                        {
                            _teleportPosition = newTelePos;
                        }
                    }
                }
            }
        }

        private void OnDestroy()
        {
            if (NetworkServer.active) DestroyTeleporterServer();
        }

        public Vector3? GetTeleportCoordinates()
        {
            if (!_canTeleport) return null;
            return _teleportPosition;
        }

        [Server]
        public void SetTeleporterServer(GameObject teleportProjectile)
        {
            if (!NetworkServer.active) return;
            if (!teleportProjectile) return;

            if (teleportObject) DestroyTeleporterServer();
            teleportObject = teleportProjectile;
            _teleportPosition = teleportObject.transform.position;
            _canTeleport = true;
        }

        [Server]
        public void DestroyTeleporterServer()
        {
            if (!NetworkServer.active) return;

            if (teleportObject)
            {
                EffectManager.SimpleEffect(CyborgTeleportTracker.teleportDestroyEffect, teleportObject.transform.position, default, true);
                UnityEngine.Object.Destroy(teleportObject);
            }
            teleportObject = null;
            _canTeleport = false;
        }

        [Command]
        public void CmdDestroyTeleporter()
        {
            if (NetworkServer.active) DestroyTeleporterServer();
        }


    }
}
