using EntityStates;
using RoR2;
using UnityEngine;

namespace EntityStates.SS2UStates.Nemmando
{
    public class Submission : BaseState
    {
        public static float damageCoefficient = 1.6f;

        private int totalBulletsFired;
        private int bulletCount;
        public float stopwatchBetweenShots;
        private Animator modelAnimator;
        private Transform modelTransform;
        private float duration;
        private float durationBetweenShots;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetSpreadBloom(0.2f, false);
            this.duration = EntityStates.Commando.CommandoWeapon.FireBarrage.totalDuration;
            this.durationBetweenShots = EntityStates.Commando.CommandoWeapon.FireBarrage.baseDurationBetweenShots / this.attackSpeedStat;
            this.bulletCount = (int)((float)EntityStates.Commando.CommandoWeapon.FireBarrage.baseBulletCount * this.attackSpeedStat);
            this.modelAnimator = base.GetModelAnimator();
            this.modelTransform = base.GetModelTransform();
            base.characterBody.SetAimTimer(2f);

            this.FireBullet();
        }

        private void FireBullet()
        {
            Ray aimRay = base.GetAimRay();
            string muzzleName = "Muzzle";

            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, base.gameObject, muzzleName, false);
            base.PlayCrossfade("UpperBody, Override", "Special", "Special.rate", this.durationBetweenShots, 0.05f);
            Util.PlaySound(EntityStates.Commando.CommandoWeapon.FireBarrage.fireBarrageSoundString, base.gameObject);

            base.AddRecoil(-0.8f * EntityStates.Commando.CommandoWeapon.FireBarrage.recoilAmplitude, -1f * EntityStates.Commando.CommandoWeapon.FireBarrage.recoilAmplitude, -0.1f * EntityStates.Commando.CommandoWeapon.FireBarrage.recoilAmplitude, 0.15f * EntityStates.Commando.CommandoWeapon.FireBarrage.recoilAmplitude);

            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = EntityStates.Commando.CommandoWeapon.FireBarrage.minSpread,
                    maxSpread = EntityStates.Commando.CommandoWeapon.FireBarrage.maxSpread,
                    bulletCount = 1U,
                    damage = Submission.damageCoefficient * this.damageStat,
                    force = EntityStates.Commando.CommandoWeapon.FireBarrage.force,
                    tracerEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    radius = EntityStates.Commando.CommandoWeapon.FireBarrage.bulletRadius,
                    smartCollision = true,
                    damageType = DamageType.Stun1s
                }.Fire();
            }

            base.characterBody.AddSpreadBloom(EntityStates.Commando.CommandoWeapon.FireBarrage.spreadBloomValue);
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
            return InterruptPriority.PrioritySkill;
        }
    }
}