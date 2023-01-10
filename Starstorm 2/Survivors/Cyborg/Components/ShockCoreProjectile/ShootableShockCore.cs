using R2API;
using RoR2;
using RoR2.Projectile;
using Starstorm2Unofficial.Components.Projectiles;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components.ShockCoreProjectile
{
    public class ShootableShockCore : ShootableProjectileComponent
    {
        public GameObject implosionStartEffectPrefab;
        public GameObject explosionEffectPrefab;
        public float radius = 20f;
        public float delayBeforeExplosion = 0.5f;
        public float implosionDamageCoefficient = 2f;

        private float explosionTimer = 0f;
        private bool beginTimer = false;

        public void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (beginTimer)
                {
                    explosionTimer -= Time.fixedDeltaTime;
                    if (explosionTimer <= 0f)
                    {
                        beginTimer = false;

                        ProjectileImpactExplosion pie = base.gameObject.GetComponent<ProjectileImpactExplosion>();
                        if (pie)
                        {
                            pie.blastRadius = radius;
                            pie.falloffModel = BlastAttack.FalloffModel.None;
                            pie.explosionEffect = explosionEffectPrefab;

                            pie.Detonate();
                            Destroy(base.gameObject);
                        }
                    }
                }
            }
        }

        public override void OnShootActions(DamageInfo damageInfo)
        {
            base.OnShootActions(damageInfo);

            explosionTimer = delayBeforeExplosion;
            beginTimer = true;

            //damageInfo.crit = true;
            damageInfo.force = Vector3.zero;
            damageInfo.rejected = true;
            damageInfo.damage = 0f;

            ProjectileDamage pd = base.gameObject.GetComponent<ProjectileDamage>();
            if (pd)
            {
                pd.damage *= implosionDamageCoefficient;
            }

            DamageAPI.ModdedDamageTypeHolderComponent mdc = base.gameObject.GetComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            if (!mdc) mdc = base.gameObject.AddComponent<DamageAPI.ModdedDamageTypeHolderComponent>();
            mdc.Add(this.targetDamageType);

            TeamIndex teamIndex = TeamIndex.None;
            TeamFilter tf = base.GetComponent<TeamFilter>();
            if (tf) teamIndex = tf.teamIndex;

            GameObject owner = null;
            ProjectileController pc = base.GetComponent<ProjectileController>();
            if (pc) owner = pc.owner;

            if (implosionStartEffectPrefab) EffectManager.SpawnEffect(implosionStartEffectPrefab, new EffectData { origin = base.transform.position, scale = 4f }, true);
            RootPulse(base.transform.position, radius, teamIndex, owner);

            ProjectileSimple ps = base.GetComponent<ProjectileSimple>();
            ps.desiredForwardSpeed = 0f;
        }

        //Based on EntityStates.TreeBot.TreebotFlower.TreebotFlower2Projectile.RootPulse
        private void RootPulse(Vector3 position, float radius, TeamIndex teamIndex, GameObject attacker)
        {
            List<CharacterBody> rootedBodies = new List<CharacterBody>();
            foreach (HurtBox hurtBox in new SphereSearch
            {
                origin = position,
                radius = radius,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(teamIndex)).OrderCandidatesByDistance().FilterCandidatesByDistinctHurtBoxEntities().GetHurtBoxes())
            {
                CharacterBody body = hurtBox.healthComponent.body;
                if (body && !rootedBodies.Contains(body) && !HGMath.IsVectorNaN(hurtBox.transform.position))//Why is this NaN check need here but not in the original code?
                {
                    rootedBodies.Add(body);
                    Vector3 a = hurtBox.transform.position - position;
                    float magnitude = a.magnitude;
                    Vector3 a2 = a / magnitude;
                    Rigidbody component = hurtBox.healthComponent.GetComponent<Rigidbody>();
                    float num = component ? component.mass : 1f;
                    float num2 = magnitude;// - 6f;    //REX yankIdealDistance = 6f
                    float num3 = EntityStates.Treebot.TreebotFlower.TreebotFlower2Projectile.yankSuitabilityCurve.Evaluate(num);
                    Vector3 vector = component ? component.velocity : Vector3.zero;

                    if (HGMath.IsVectorNaN(vector))
                    {
                        vector = Vector3.zero;
                    }

                    Vector3 a3 = -vector;
                    if (num2 > 0f)
                    {
                        a3 = a2 * -Trajectory.CalculateInitialYSpeedForHeight(num2, -body.acceleration);
                    }
                    Vector3 force = a3 * (num * num3);

                    //Why is this NaN check needed here but not in the original code?
                    if (!HGMath.IsVectorNaN(force))
                    {
                        DamageInfo damageInfo = new DamageInfo
                        {
                            attacker = attacker,
                            inflictor = base.gameObject,
                            crit = false,
                            damage = 0f,
                            damageColorIndex = DamageColorIndex.Default,
                            damageType = DamageType.NonLethal | DamageType.Silent,
                            force = force,
                            position = hurtBox.transform.position,
                            procChainMask = default,
                            procCoefficient = 0f
                        };
                        hurtBox.healthComponent.TakeDamageForce(damageInfo, true, false);
                    }
                }
            }
        }
    }
}
