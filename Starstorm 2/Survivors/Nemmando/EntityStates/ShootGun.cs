using EntityStates;
using EntityStates.SS2UStates.Common;
using RoR2;
using UnityEngine;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ShootGun : BaseCustomSkillState
    {
        public static float damageCoefficient = 2.1f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.3f;
        public static float force = 400f;
        public static float recoil = 1.5f;
        public static float range = 200f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");

        private float duration;
        private float fireTime;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = ShootGun.baseDuration / this.attackSpeedStat;
            this.fireTime = 0.1f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";

            base.FindModelChildGameObject("GunSpinEffect").SetActive(false);

            //base.PlayCrossfade("RightArm, Override", "ShootGun", "ShootGun.playbackRate", 3f * this.duration, 0.05f);
            base.PlayCrossfade("RightArm, Override", "ShootGunShort", "ShootGun.playbackRate", this.duration, 0.05f);
        }

        public override void OnExit()
        {
            base.OnExit();

            float rechargeTime = Mathf.Clamp(this.skillLocator.secondary.finalRechargeInterval, 0.25f, Mathf.Infinity);
            base.PlayAnimation("Gesture, Override", "ReloadGun", "Reload.playbackRate", 0.5f * (rechargeTime - this.duration - 0.3f));
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                bool isCrit = base.RollCrit();
                base.characterBody.AddSpreadBloom(1.5f);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

                string soundString = base.effectComponent.shootSound;
                //if (isCrit) soundString += "Crit";
                Util.PlaySound(soundString, base.gameObject);

                if (base.isAuthority)
                {
                    Ray aimRay = base.GetAimRay();
                    base.AddRecoil(-1f * ShootGun.recoil, -2f * ShootGun.recoil, -0.5f * ShootGun.recoil, 0.5f * ShootGun.recoil);

                    new BulletAttack
                    {
                        bulletCount = 1,
                        aimVector = aimRay.direction,
                        origin = aimRay.origin,
                        damage = ShootGun.damageCoefficient * this.damageStat,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Generic,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        maxDistance = ShootGun.range,
                        force = ShootGun.force,
                        hitMask = LayerIndex.CommonMasks.bullet,
                        minSpread = 0f,
                        maxSpread = 0f,
                        isCrit = isCrit,
                        owner = base.gameObject,
                        muzzleName = muzzleString,
                        smartCollision = false,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 0.75f,
                        sniper = false,
                        stopperMask = LayerIndex.CommonMasks.bullet,
                        weapon = null,
                        tracerEffectPrefab = ShootGun.tracerEffectPrefab,
                        spreadPitchScale = 0f,
                        spreadYawScale = 0f,
                        queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                        hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab
                    }.Fire();
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime)
            {
                this.Fire();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
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