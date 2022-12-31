using RoR2;
using UnityEngine;
using RoR2.Projectile;
using RoR2.Skills;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine.Networking;
using Starstorm2.Survivors.Cyborg;

namespace EntityStates.SS2UStates.Cyborg.Special
{
    public class DeployTeleporter : BaseState
    {
        public static float timeoutDuration = 10f;  //Cancels skill if it can't find teleporter gameobject within 10s.
        public static GameObject projectilePrefab;
        public static CyborgTeleSkillDef teleportSkillDef;
        private CyborgTeleportTracker teleTracker;

        private bool foundTeleporter;
        private bool appliedSkillOverride;

        private int origSpecialStock;
        private float origSpecialRechargeStopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            teleTracker = base.GetComponent<CyborgTeleportTracker>();
            base.PlayAnimation("Gesture, Override", "CreateTP", "FireM1.playbackRate", 1f);

            appliedSkillOverride = false;
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
                    if (foundTeleporter)
                    {
                        if (!appliedSkillOverride)
                        {
                            if (base.skillLocator)
                            {
                                GenericSkill skillSlot = base.skillLocator.utility;
                                if (skillSlot)
                                {
                                    origSpecialStock = skillSlot.stock;
                                    origSpecialRechargeStopwatch = skillSlot.rechargeStopwatch;

                                    appliedSkillOverride = true;
                                    skillSlot.SetSkillOverride(this, teleportSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                                    skillSlot.stock = 1;
                                }
                            }
                        }
                    }
                }

                bool timeout = !foundTeleporter && base.fixedAge >= timeoutDuration;
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
            if (base.isAuthority)
            {
                if (appliedSkillOverride)
                {
                    if (base.skillLocator)
                    {
                        GenericSkill skillSlot = base.skillLocator.utility;
                        if (skillSlot)
                        {
                            skillSlot.UnsetSkillOverride(this, teleportSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                            skillSlot.rechargeStopwatch = origSpecialRechargeStopwatch;
                            skillSlot.stock = origSpecialStock;
                        }
                    }
                }
            }

            //This runs before teleport: causes teleporter be destroyed before it gets used.
            /*if (NetworkServer.active && teleTracker)
            {
                teleTracker.DestroyTeleporter();
            }*/
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
