using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine.Networking;

namespace EntityStates.Starstorm2States.Cyborg.Special
{
    public class UseTeleporter : BaseState
    {
        public static GameObject explosionEffectPrefab;
        public static float damageCoefficient = 12f;
        public static float radius = 14f;
        public static float baseDuration = 0.5f;
        private CyborgTeleportTracker teleTracker;
        private bool teleported;

        public override void OnEnter()
        {
            base.OnEnter();

            teleported = false;
            base.PlayAnimation("Gesture, Override", "UseTP", "FireM1.playbackRate", UseTeleporter.baseDuration);
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
            if (base.isAuthority && base.characterMotor && teleTracker)
            {
                Vector3? teleportLocation = teleTracker.GetTeleportCoordinates();
                if (teleportLocation != null)
                {
                    Util.PlaySound("Play_UI_charTeleport", base.gameObject);
                    base.characterMotor.velocity.y = 0f;
                    base.characterMotor.disableAirControlUntilCollision = false;

                    base.characterMotor.Motor.SetPosition((Vector3)teleportLocation, true);
                    TelefragExplosionAuthority((Vector3)teleportLocation);
                    teleported = true;
                    teleTracker.CmdDestroyTeleporter();
                }
            }
        }

        private void TelefragExplosionAuthority(Vector3 position)
        {
            BlastAttack ba = new BlastAttack
            {
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.Shock5s,
                baseDamage = this.damageStat * UseTeleporter.damageCoefficient,
                radius = UseTeleporter.radius,
                baseForce = 4000f,
                canRejectForce = false,
                bonusForce = Vector3.zero,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                crit = base.RollCrit(),
                falloffModel = BlastAttack.FalloffModel.None,
                position = position,
                procChainMask = default,
                procCoefficient = 1f,
                teamIndex = base.GetTeam()
            };
            ba.Fire();
            EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData
            {
                origin = position,
                scale = UseTeleporter.radius
            }, true);
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

        public override void OnExit()
        {
            if (!teleported && base.isAuthority) TelefragExplosionAuthority(base.transform.position);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
