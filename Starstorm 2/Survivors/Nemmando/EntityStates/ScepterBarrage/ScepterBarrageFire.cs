using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ScepterBarrageFire : BaseState
    {
        public float charge;

        public static float laserDamageCoefficient = 8.2f;
        public static float laserBlastRadius = 8f;
        public static float laserBlastForce = 2000f;

        public static float damageCoefficient = 1.2f;
        public static float procCoefficient = 0.5f;
        public static uint bulletCountPerShot = 4;
        public static float range = 128f;
        public static float maxSpread = 12f;
        public static int minBulletCount = 2;
        public static int maxBulletCount = 5;

        public static float baseDuration = 0.8f;
        public static float minTimeBetweenShots = 0.2f;
        public static float maxTimeBetweenShots = 0.075f;
        public static float recoil = 5f;

        public static float force = 200f;
        public static float bulletRadius = 2f;

        private bool isCrit;

        private int totalBulletsFired;
        private int bulletCount;
        public float stopwatchBetweenShots;
        private Animator modelAnimator;
        private Transform modelTransform;
        private float duration;
        private float durationBetweenShots;
        public static GameObject tracerEffectPrefab;
        private GameObject muzzleFlashEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/FusionCellExplosion");
        public static GameObject impactEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommando.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetSpreadBloom(0.2f, false);
            base.characterBody.isSprinting = false;
            this.duration = ScepterBarrageFire.baseDuration;
            this.durationBetweenShots = (Util.Remap(this.charge, 0f, 1f, ScepterBarrageFire.minTimeBetweenShots, ScepterBarrageFire.maxTimeBetweenShots)) / this.attackSpeedStat;
            this.bulletCount = (int)(Mathf.RoundToInt(Util.Remap(this.charge, 0f, 1f, ScepterBarrageFire.minBulletCount, ScepterBarrageFire.maxBulletCount)) * this.attackSpeedStat);
            this.modelAnimator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            base.characterBody.SetAimTimer(2f);
            base.characterBody.outOfCombatStopwatch = 0f;

            isCrit = base.RollCrit();

            this.FireBullet();
        }

        private void FireBullet()
        {
            Ray aimRay = base.GetAimRay();
            string muzzleName = "Muzzle";

            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
            base.PlayCrossfade("UpperBody, Override", "Special", "Special.rate", this.durationBetweenShots, 0.05f);
            Util.PlaySound("NemmandoSubmissionFire", base.gameObject);
            Util.PlaySound(EntityStates.GolemMonster.FireLaser.attackSoundString, base.gameObject);

            float recoil = ScepterBarrageFire.recoil / this.attackSpeedStat;
            base.AddRecoil(-0.8f * recoil, -1f * recoil, -0.1f * recoil, 0.15f * recoil);

            EffectManager.SimpleMuzzleFlash(EntityStates.GolemMonster.FireLaser.effectPrefab, base.gameObject, "Muzzle", false);

            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0,
                    maxSpread = ScepterBarrageFire.maxSpread,
                    bulletCount = ScepterBarrageFire.bulletCountPerShot,
                    damage = ScepterBarrageFire.damageCoefficient * this.damageStat,
                    force = force,
                    tracerEffectPrefab = ScepterBarrageFire.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = ScepterBarrageFire.impactEffectPrefab,
                    isCrit = isCrit,
                    radius = bulletRadius,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    spreadPitchScale = 0.5f,
                    spreadYawScale = 0.5f,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = ScepterBarrageFire.procCoefficient,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = ScepterBarrageFire.range
                }.Fire();

                this.FireLaser();
            }

            base.characterBody.AddSpreadBloom(2f * EntityStates.Commando.CommandoWeapon.FireBarrage.spreadBloomValue);
            this.totalBulletsFired++;
        }

        private void FireLaser()
        {
            Ray aimRay = base.GetAimRay();
            Vector3 blastPosition = aimRay.origin + aimRay.direction * 1000f;

            RaycastHit raycastHit;
            if (Physics.Raycast(aimRay, out raycastHit, 1000f, LayerIndex.world.mask | LayerIndex.defaultLayer.mask | LayerIndex.entityPrecise.mask))
            {
                blastPosition = raycastHit.point;
            }

            BlastAttack blast = new BlastAttack
            {
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                baseDamage = this.damageStat * ScepterBarrageFire.laserDamageCoefficient,
                baseForce = ScepterBarrageFire.laserBlastForce * 0.2f,
                position = blastPosition,
                radius = ScepterBarrageFire.laserBlastRadius,
                falloffModel = BlastAttack.FalloffModel.SweetSpot,
                bonusForce = ScepterBarrageFire.laserBlastForce * aimRay.direction
            };

            blast.Fire();

            if (this.modelTransform)
            {
                ChildLocator childLocator = this.modelTransform.GetComponent<ChildLocator>();
                if (childLocator)
                {
                    int childIndex = childLocator.FindChildIndex("Muzzle");
                    if (EntityStates.GolemMonster.FireLaser.tracerEffectPrefab)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = blastPosition,
                            start = aimRay.origin
                        };

                        effectData.SetChildLocatorTransformReference(base.gameObject, childIndex);

                        EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
                        EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatchBetweenShots += Time.fixedDeltaTime;

            if (this.stopwatchBetweenShots >= this.durationBetweenShots && this.totalBulletsFired < this.bulletCount)
            {
                this.stopwatchBetweenShots -= this.durationBetweenShots;
                this.FireBullet();
            }

            if (base.fixedAge >= this.duration && this.totalBulletsFired == this.bulletCount && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}