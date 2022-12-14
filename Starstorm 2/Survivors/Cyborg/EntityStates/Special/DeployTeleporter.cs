using RoR2;
using UnityEngine;
using RoR2.Projectile;
using RoR2.Skills;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine.Networking;

namespace EntityStates.Starstorm2States.Cyborg.Special
{
    public class DeployTeleporter : BaseState
    {
        public static GameObject projectilePrefab;
        public static float damageCoefficient = 12f;
        public static float baseDuration = 0.5f;
        public static SkillDef teleportSkillDef;
        private CyborgTeleportTracker teleTracker;

        public override void OnEnter()
        {
            base.OnEnter();
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
            base.PlayAnimation("Gesture, Override", "CreateTP", "FireM1.playbackRate", DeployTeleporter.baseDuration);

            FireTeleportProjectile();
        }

        private void FireTeleportProjectile()
        {
            Util.PlaySound("CyborgSpecialPlace", base.gameObject);
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    crit = base.RollCrit(),
                    damage = this.damageStat * DeployTeleporter.damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 0f,
                    owner = base.gameObject,
                    position = aimRay.origin,
                    procChainMask = default(ProcChainMask),
                    projectilePrefab = DeployTeleporter.projectilePrefab,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    target = null
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= DeployTeleporter.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            /*if (NetworkServer.active)
            {
                teleTracker.DestroyTeleporter();
            }*/
            base.OnExit();
        }
    }
}
