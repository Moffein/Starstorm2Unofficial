using EntityStates;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando.ChargeBarrage
{
    public class ChargeBarrageFire : BaseState
    {
        public float charge;

        public static float damageCoefficient = 0.6f;
        public static float procCoefficient = 0.5f;
        public static uint bulletCountPerShot = 8;
        public static float range = 128f;
        public static float maxSpread = 30f;
        public static int minBulletCount = 2;
        public static int maxBulletCount = 6;

        public static float baseDuration = 0.8f;
        public static float minTimeBetweenShots = 0.2f;
        public static float maxTimeBetweenShots = 0.075f;
        public static float recoil = 3f;

        private int totalBulletsFired;
        private int bulletCount;
        public float stopwatchBetweenShots;
        private Animator modelAnimator;
        private Transform modelTransform;
        private float duration;
        private float durationBetweenShots;
        private GameObject muzzleFlashEffect = Resources.Load<GameObject>("Prefabs/Effects/ImpactEffects/FusionCellExplosion");

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetSpreadBloom(0.2f, false);
            base.characterBody.isSprinting = false;
            this.duration = ChargeBarrageFire.baseDuration;
            this.durationBetweenShots = (Util.Remap(this.charge, 0f, 1f, ChargeBarrageFire.minTimeBetweenShots, ChargeBarrageFire.maxTimeBetweenShots)) / this.attackSpeedStat;
            this.bulletCount = (int)(Mathf.RoundToInt(Util.Remap(this.charge, 0f, 1f, ChargeBarrageFire.minBulletCount, ChargeBarrageFire.maxBulletCount)) * this.attackSpeedStat);
            this.modelAnimator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            base.characterBody.SetAimTimer(2f);
            base.characterBody.outOfCombatStopwatch = 0f;

            this.FireBullet();
        }

        private void FireBullet()
        {
            Ray aimRay = base.GetAimRay();
            string muzzleName = "Muzzle";

            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
            base.PlayCrossfade("UpperBody, Override", "Special", "Special.rate", this.durationBetweenShots, 0.05f);
            Util.PlaySound("NemmandoSubmissionFire", base.gameObject);

            base.AddRecoil(-0.8f * ChargeBarrageFire.recoil, -1f * ChargeBarrageFire.recoil, -0.1f * ChargeBarrageFire.recoil, 0.15f * ChargeBarrageFire.recoil);

            if (base.isAuthority)
            {
                // this effect was too noisy, will have to trim it
                /*EffectManager.SpawnEffect(this.muzzleFlashEffect, new EffectData
                {
                    origin = base.FindModelChild(muzzleName).position,
                    scale = 1f
                }, true);*/

                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0,
                    maxSpread = ChargeBarrageFire.maxSpread,
                    bulletCount = ChargeBarrageFire.bulletCountPerShot,
                    damage = ChargeBarrageFire.damageCoefficient * this.damageStat,
                    force = 0.5f * EntityStates.Commando.CommandoWeapon.FireBarrage.force,
                    tracerEffectPrefab = Modules.Projectiles.laserTracer,
                    muzzleName = muzzleName,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    radius = EntityStates.Commando.CommandoWeapon.FireBarrage.bulletRadius,
                    smartCollision = true,
                    damageType = DamageType.Generic,
                    spreadPitchScale = 0.5f,
                    spreadYawScale = 0.5f,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = ChargeBarrageFire.procCoefficient,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = ChargeBarrageFire.range
                }.Fire();
            }

            base.characterBody.AddSpreadBloom(2f * EntityStates.Commando.CommandoWeapon.FireBarrage.spreadBloomValue);
            this.totalBulletsFired++;
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