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
        public static float timeoutDuration = 10f;  //Cancels skill if it can't find teleporter gameobject within 10s.
        public static GameObject projectilePrefab;
        public static SkillDef teleportSkillDef;
        private CyborgTeleportTracker teleTracker;

        private bool foundTeleporter;

        public override void OnEnter()
        {
            base.OnEnter();
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
            base.PlayAnimation("Gesture, Override", "CreateTP", "FireM1.playbackRate", 1f);

            foundTeleporter = false;

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
                    crit = false,
                    damage = 0f,
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

            if (base.isAuthority)
            {
                if (!foundTeleporter)
                {
                    foundTeleporter = teleTracker && teleTracker.GetTeleportCoordinates() != null;
                }

                bool timeout = base.fixedAge >= timeoutDuration && !foundTeleporter;
                bool teleNoLongerValid = foundTeleporter && teleTracker && teleTracker.GetTeleportCoordinates() == null;
                if (timeout || teleNoLongerValid)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (NetworkServer.active && teleTracker)
            {
                teleTracker.DestroyTeleporter();
            }
            base.OnExit();
        }
    }
}
