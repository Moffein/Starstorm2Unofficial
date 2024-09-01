﻿using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargeBarrageFire : BaseState
    {
        public float charge;

        public static float damageCoefficient = 1.2f;   //was 0.6
        public static float procCoefficient = 0.5f;
        public static uint bulletCountPerShot = 4;  //was 8
        public static float range = 128f;
        public static float maxSpread = 12f; //was 30
        public static int minBulletCount = 2;
        public static int maxBulletCount = 5;   //was 6

        public static float baseDuration = 0.8f;
        public static float minTimeBetweenShots = 0.2f;
        public static float maxTimeBetweenShots = 0.075f;
        public static float recoil = 3f;

        public static float force = 200f;
        public static float bulletRadius = 2f;

        private int totalBulletsFired;
        private int bulletCount;
        public float stopwatchBetweenShots;
        private Animator modelAnimator;
        private Transform modelTransform;
        private float duration;
        private float durationBetweenShots;

        private bool isCrit;

        public static GameObject tracerEffectPrefab;
        public static GameObject muzzleFlashEffect = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/FusionCellExplosion");
        public static GameObject impactEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommando.prefab").WaitForCompletion();

        private float lastUpdateTime;
        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            base.characterBody.SetSpreadBloom(0.2f, false);
            base.characterBody.isSprinting = false;
            this.duration = ChargeBarrageFire.baseDuration;
            this.durationBetweenShots = (Util.Remap(this.charge, 0f, 1f, ChargeBarrageFire.minTimeBetweenShots, ChargeBarrageFire.maxTimeBetweenShots)) / this.attackSpeedStat;
            this.bulletCount = (int)(Mathf.RoundToInt(Util.Remap(this.charge, 0f, 1f, ChargeBarrageFire.minBulletCount, ChargeBarrageFire.maxBulletCount)) * this.attackSpeedStat);
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
            Util.PlaySound("SS2UNemmandoSubmissionFire", base.gameObject);

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
                    force = ChargeBarrageFire.force,
                    tracerEffectPrefab = ChargeBarrageFire.tracerEffectPrefab,
                    muzzleName = muzzleName,
                    hitEffectPrefab = ChargeBarrageFire.impactEffectPrefab,
                    isCrit = isCrit,
                    radius = ChargeBarrageFire.bulletRadius,
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
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            this.stopwatchBetweenShots += deltaTime;

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