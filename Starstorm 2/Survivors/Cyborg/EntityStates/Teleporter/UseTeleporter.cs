using RoR2;
using UnityEngine;
using RoR2.Projectile;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Cyborg.Special
{
    public class UseTeleporter : BaseState
    {
        public static GameObject explosionEffectPrefab;
        public static float damageCoefficient = 12f;
        public static float radius = 14f;
        public static float baseDuration = 1f;
        private CyborgTeleportTracker teleTracker;
        private bool teleported;

        public override void OnEnter()
        {
            base.OnEnter();
            teleported = false;
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
        }

        private void Teleport()
        {
            if (base.isAuthority && base.characterMotor && teleTracker)
            {
                Vector3? teleportLocation = teleTracker.GetTeleportCoordinates();
                if (teleportLocation != null)
                {
                    Util.PlaySound("CyborgSpecialTeleport", base.gameObject);
                    base.characterMotor.velocity.y = 0f;
                    base.characterMotor.disableAirControlUntilCollision = false;

                    base.characterMotor.Motor.SetPosition((Vector3)teleportLocation, true);
                    TelefragExplosionAuthority((Vector3)teleportLocation);
                    teleported = true;
                    teleTracker.CmdDestroyTeleporter();
                    base.PlayAnimation("Gesture, Override", "UseTP", "FireM1.playbackRate", UseTeleporter.baseDuration);
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

            GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(base.gameObject);
            if (teleportEffectPrefab)
            {
                EffectManager.SimpleEffect(teleportEffectPrefab, position, Quaternion.identity, true);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (!base.inputBank.skill3.down)
                {
                    if (!teleported && base.fixedAge <= UseTeleporter.baseDuration)
                    {
                        Teleport();
                    }
                }
                
                if (base.fixedAge >= UseTeleporter.baseDuration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                if (teleTracker && !teleported)
                {
                    teleTracker.CmdDestroyTeleporter();
                    Util.PlaySound("Play_railgunner_m2_reload_fail", base.gameObject);
                    if (base.skillLocator && base.skillLocator.utility)
                    {
                        base.skillLocator.utility.AddOneStock();
                    }
                }
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
