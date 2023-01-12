using RoR2;
using Starstorm2Unofficial.Survivors.Pyro.Components;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using R2API;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Pyro
{
    public class Airblast : BaseState  //based on Enforcer's shield deflect code https://github.com/GnomeModder/EnforcerMod/blob/master/EnforcerMod_VS/Secondary.cs
    {
        public override void OnEnter()
        {
            base.OnEnter();
            heatController = base.GetComponent<HeatController>();
            if (heatController)
            {
                int stocks = 1;
                if (base.skillLocator && base.skillLocator.secondary) stocks = base.skillLocator.secondary.maxStock;
                heatController.ConsumeHeat(Airblast.heatCost, stocks);
            }
            reflected = false;

            EffectManager.SimpleMuzzleFlash(Airblast.effectPrefab, base.gameObject, "Muzzle", false);

            Util.PlaySound(Airblast.attackSoundString, base.gameObject);

            this.aimRay = base.GetAimRay();
            this.childLocator = base.GetModelTransform().GetComponent<ChildLocator>();

            base.StartAimMode(aimRay, 2f, false);

            if (base.isAuthority && base.characterBody && base.characterMotor && !base.characterMotor.isGrounded)
            {
                if (base.characterMotor.velocity.y < 0f)
                {
                    base.characterMotor.velocity.y = 0f;
                }
                base.characterMotor.ApplyForce(-aimRay.direction * Airblast.selfForce, true, false);
            }

            if (NetworkServer.active)
            {
                PushEnemiesServer();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (NetworkServer.active && base.fixedAge <= Airblast.reflectWindowDuration)
            {
                DeflectServer();
            }

            if (base.isAuthority && base.fixedAge >= Airblast.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        private void DeflectServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            Ray aimRay = base.GetAimRay();

            int projectilesReflected = 0;
            Collider[] array = Physics.OverlapBox(base.transform.position + aimRay.direction * Airblast.hitboxOffset, Airblast.hitboxDimensions, Quaternion.LookRotation(aimRay.direction, Vector3.up), LayerIndex.projectile.mask);
            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc && !pc.cannotBeDeleted)
                {
                    if (pc.owner != base.gameObject)
                    {
                        Vector3 aimSpot = (aimRay.origin + 90f * aimRay.direction) - pc.gameObject.transform.position;

                        pc.owner = base.gameObject;
                        projectilesReflected++;

                        FireProjectileInfo info = new FireProjectileInfo()
                        {
                            projectilePrefab = pc.gameObject,
                            position = pc.gameObject.transform.position,
                            rotation = base.transform.rotation * Quaternion.FromToRotation(new Vector3(0, 0, 1), aimSpot),
                            owner = base.gameObject,
                            damage = this.damageStat * Airblast.reflectDamageCoefficient,
                            force = 2000f,
                            crit = base.RollCrit(),
                            damageColorIndex = DamageColorIndex.Default,
                            target = null,
                            speedOverride = 120f,
                            useSpeedOverride = true,
                            fuseOverride = -1f,
                            useFuseOverride = false
                        };
                        ProjectileManager.instance.FireProjectile(info);

                        Destroy(pc.gameObject);

                        if (!reflected)
                        {
                            reflected = true;
                            if (reflectSound)
                            {
                                EffectManager.SimpleSoundEffect(reflectSound.index, base.transform.position, true);
                            }
                        }
                    }
                }
            }
        }

        private void PushEnemiesServer()
        {
            if (!NetworkServer.active)
            {
                return;
            }
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapBox(base.transform.position + aimRay.direction * Airblast.hitboxOffset, Airblast.hitboxDimensions, Quaternion.LookRotation(aimRay.direction, Vector3.up), LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                    if (healthComponent && !pc && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        TeamComponent component2 = healthComponent.GetComponent<TeamComponent>();
                        if (component2.teamIndex != base.teamComponent.teamIndex)
                        {
                            CharacterBody cb = healthComponent.body;
                            if (cb)
                            {
                                Vector3 forceVector = Airblast.force * aimRay.direction;
                                if (forceVector.y < 1200f && !cb.isFlying) forceVector.y = 1200f;

                                DamageInfo damageInfo = new DamageInfo
                                {
                                    attacker = base.gameObject,
                                    inflictor = base.gameObject,
                                    damage = 0f,
                                    damageColorIndex = DamageColorIndex.Default,
                                    damageType = DamageType.NonLethal | DamageType.Silent | DamageType.BypassArmor | DamageType.BypassBlock,
                                    crit = false,
                                    dotIndex = DotController.DotIndex.None,
                                    force = forceVector,
                                    position = base.transform.position,
                                    procChainMask = default(ProcChainMask),
                                    procCoefficient = 0f
                                };
                                damageInfo.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ScaleForceToMass);
                                damageInfo.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ResetVictimForce);
                                healthComponent.TakeDamage(damageInfo);
                            }
                        }
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }

        public static string attackSoundString = "Play_treeBot_shift_shoot";
        public static NetworkSoundEventDef reflectSound;
        public static float baseDuration = 0.75f;
        public static float reflectWindowDuration = 0.2f;
        public static Vector3 hitboxDimensions = new Vector3(8f, 8f, 16f);
        public static float force = 3000f;
        public static float selfForce = 3000f;
        public static float heatCost = 0.25f;
        public static GameObject effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotShockwaveEffect.prefab").WaitForCompletion();

        public static float reflectDamageCoefficient = 10f;

        private bool reflected;
        private ChildLocator childLocator;
        private Ray aimRay;
        private HeatController heatController;

        private static float hitboxOffset = (16f / 2f - 0.5f);
    }
}
