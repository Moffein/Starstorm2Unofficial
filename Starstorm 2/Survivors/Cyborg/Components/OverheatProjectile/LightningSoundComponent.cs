using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace Starstorm2.Survivors.Cyborg.Components.OverheatProjectile
{
    [RequireComponent(typeof(ProjectileProximityBeamController))]
    public class LightningSoundComponent : MonoBehaviour
    {
        public static NetworkSoundEventDef lightningSound;
        private ProjectileProximityBeamController pbc;

        private void Start()
        {
            pbc = base.GetComponent<ProjectileProximityBeamController>();
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && pbc)
            {
                if (pbc.listClearTimer - Time.fixedDeltaTime <= 0f)
                {
                    EffectManager.SimpleSoundEffect(lightningSound.index, base.transform.position, true);
                }
            }
        }
    }
}
