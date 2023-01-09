using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace Starstorm2.Survivors.Pyro.Components.Projectile
{
    public class FlareProjectileController : MonoBehaviour, IProjectileImpactBehavior
    {
        public GameObject explosionEffectPrefab;
        public float initialDamageCoefficient = 1f;
        public float initialRadius = 2.4f;
        public NetworkSoundEventDef initialImpactSound;

        public float explosionDamageCoefficient = 0.25f;
        public float explosionRadius = 8f;

        public float delayBetweenExplosions = 0.3f;

        private int timesExploded;
        private int totalExplosionCount;
        private float explosionStopwatch;
        private bool hasStarted;

        private ProjectileDamage projectileDamage;
        private ProjectileController projectileController;
        private TeamFilter teamFilter;

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (hasStarted || !NetworkServer.active) return;
            timesExploded = 0;
            hasStarted = true;
            explosionStopwatch = 0f;

            if (projectileController && teamFilter && projectileDamage)
            {
                ProjectileSimple ps = base.GetComponent<ProjectileSimple>();
                if (ps) ps.lifetime = 1000000f;

                BlastAttack ba = new BlastAttack
                {
                    attacker = projectileController.owner,
                    inflictor = base.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = projectileDamage.damage * initialDamageCoefficient,
                    baseForce = 0f,
                    bonusForce = Vector3.zero,
                    canRejectForce = true,
                    crit = projectileDamage.crit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = projectileDamage.damageType,
                    falloffModel = BlastAttack.FalloffModel.None,
                    position = base.transform.position,
                    procChainMask = default,
                    procCoefficient = 1f,
                    radius = initialRadius,
                    teamIndex = teamFilter.teamIndex
                };
                if (explosionEffectPrefab) EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData { origin = base.transform.position, scale = ba.radius }, true);
                if (initialImpactSound != null)
                {
                    EffectManager.SimpleSoundEffect(initialImpactSound.index, base.transform.position, true);
                }
                ba.Fire();
            }
        }

        private void Awake()
        {
            totalExplosionCount = 1;
            explosionStopwatch = 0f;
            timesExploded = 0;
            hasStarted = false;
            teamFilter = base.GetComponent<TeamFilter>();
            projectileController = base.GetComponent<ProjectileController>();
            projectileDamage = base.GetComponent<ProjectileDamage>();
        }

        private void Start()
        {
            if (projectileDamage)
            {
                totalExplosionCount = Mathf.Max(1, Mathf.RoundToInt(projectileDamage.force));
                projectileDamage.force = 0f;
            }
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active && hasStarted)
            {
                explosionStopwatch += Time.fixedDeltaTime;
                if (explosionStopwatch >= delayBetweenExplosions)
                {
                    ExplodeServer();
                    explosionStopwatch -= delayBetweenExplosions;
                }
            }
        }

        public void ExplodeServer()
        {
            if (projectileController && teamFilter && projectileDamage)
            {
                BlastAttack ba = new BlastAttack
                {
                    attacker = projectileController.owner,
                    inflictor = base.gameObject,
                    attackerFiltering = AttackerFiltering.Default,
                    baseDamage = projectileDamage.damage * explosionDamageCoefficient,
                    baseForce = 0f,
                    bonusForce = Vector3.zero,
                    canRejectForce = true,
                    crit = projectileDamage.crit,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = projectileDamage.damageType,
                    falloffModel = BlastAttack.FalloffModel.None,
                    position = base.transform.position,
                    procChainMask = default,
                    procCoefficient = 1f,
                    radius = explosionRadius,
                    teamIndex = teamFilter.teamIndex
                };
                if (explosionEffectPrefab) EffectManager.SpawnEffect(explosionEffectPrefab, new EffectData { origin = base.transform.position, scale = ba.radius }, true);
                ba.Fire();
            }

            timesExploded++;
            if (timesExploded >= totalExplosionCount)
            {
                Destroy(base.gameObject);
            }
        }
    }
}
