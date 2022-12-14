using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine.Networking;

namespace EntityStates.Starstorm2States.Cyborg.Special
{
    public class UseTeleporter : BaseState
    {
        public static float damageCoefficient = 12f;
        public static float radius = 14f;
        public static float baseDuration = 0.5f;
        private CyborgTeleportTracker teleTracker;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Gesture, Override", "UseTP", "FireM1.playbackRate", UseTeleporter.baseDuration);
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
            if (base.isAuthority && base.characterMotor && teleTracker)
            {
                Vector3? teleportLocation = teleTracker.GetTeleportCoordinates();
                if (teleportLocation != null)
                {
                    base.characterMotor.Motor.SetPosition((Vector3)teleportLocation, true);
                    TelefragExplosion((Vector3)teleportLocation);
                }
            }
        }

        private void TelefragExplosion(Vector3 position)
        {
            BlastAttack ba = new BlastAttack
            {
                
            };
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= UseTeleporter.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }
    }
}
