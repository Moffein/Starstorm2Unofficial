using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using R2API;
using Starstorm2Unofficial.Components;
using EntityStates.SS2UStates.Common;
using Starstorm2Unofficial.Survivors.Nemmando.Components;

namespace EntityStates.SS2UStates.Nemmando
{
    public class FireSwordBeam : BaseCustomSkillState
    {
        public float charge;

        public static float maxEmission = 75f;
        public static float minEmission = 25f;
        public static float maxDamageCoefficient = 6f;
        public static float minDamageCoeffficient = 2f;
        public static float procCoefficient = 1f;
        public static float maxRecoil = 2f;
        public static float minRecoil = 0.25f;
        public static float maxProjectileSpeed = 120f;
        public static float minProjectileSpeed = 50f;
        public float baseDuration = 0.4f;
        public static GameObject projectilePrefab;

        private float emission;
        private Material swordMat;
        private float damageCoefficient;
        private float recoil;
        private float projectileSpeed;
        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;
        private NemmandoController nemmandoController;
        private float minimumEmission;
        private float maximumEmission;

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";
            this.hasFired = false;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, FireSwordBeam.minDamageCoeffficient, FireSwordBeam.maxDamageCoefficient);
            this.recoil = Util.Remap(this.charge, 0f, 1f, FireSwordBeam.minRecoil, FireSwordBeam.maxRecoil);
            this.projectileSpeed = Util.Remap(this.charge, 0f, 1f, FireSwordBeam.minProjectileSpeed, FireSwordBeam.maxProjectileSpeed);
            this.emission = Util.Remap(this.charge, 0f, 1f, FireSwordBeam.minEmission, FireSwordBeam.maxEmission);
            this.fireDuration = 0.1f * this.duration;
            this.nemmandoController = base.GetComponent<NemmandoController>();

            this.minimumEmission = this.effectComponent.defaultSwordEmission;
            this.maximumEmission = this.minimumEmission + 150f;

            string fireAnim = charge > 0.6f ? "Secondary3(Strong)" : "Secondary3(Weak)";

            bool moving = this.animator.GetBool("isMoving");
            bool grounded = this.animator.GetBool("isGrounded");

            if (!moving && grounded && !this.nemmandoController.chargingDecisiveStrike && !this.nemmandoController.rolling)
            {
                base.PlayCrossfade("FullBody, Override", fireAnim, "Secondary.rate", this.duration, 0.05f);
            }

            base.PlayCrossfade("UpperBody, Override", fireAnim, "Secondary.rate", this.duration, 0.05f);

            this.swordMat = base.GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

            Util.PlaySound("SS2UNemmandoFireBeam2", base.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();

            this.swordMat.SetFloat("_EmPower", this.minimumEmission);
        }

        public virtual void FireBeam()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                bool isCrit = base.RollCrit();

                //Util.PlayAttackSpeedSound("NemmandoSwing1", base.gameObject, this.attackSpeedStat);

                base.AddRecoil(-2f * this.recoil, -3f * this.recoil, -1f * this.recoil, 1f * this.recoil);
                base.characterBody.AddSpreadBloom(0.33f * this.recoil);
                EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, base.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    float damage = this.damageCoefficient * this.damageStat;

                    Ray aimRay = base.GetAimRay();

                    ProjectileManager.instance.FireProjectile(FireSwordBeam.projectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, damage, 0f, base.RollCrit(), DamageColorIndex.Default, null, this.projectileSpeed);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.swordMat)
            {
                this.swordMat.SetFloat("_EmPower", Util.Remap(base.fixedAge, 0, this.duration, this.emission, this.minimumEmission));
            }

            if (base.fixedAge >= this.fireDuration)
            {
                this.FireBeam();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}