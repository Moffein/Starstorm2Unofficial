using EntityStates;
using EntityStates.Engi.EngiWeapon;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.States.Nucleator
{
    class FireIrradiate : BaseSkillState
    {
        public static float minChargeDamageCoef = 1F;
        public static float maxChargeDamageCoef = 5F;
        public static float maxOverchargeDamageCoef = 9F;
        
        public float charge;

        private float damage;
        private float duration = 0.3f;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();

            CalculateDamage();
            Shoot();

            if (this.charge > NucleatorSkillStateBase.overchargeThreshold)
            {
                base.PlayAnimation("Gesture, Override", "PrimaryBig", "Primary.playbackRate", this.duration);
            }
            else
            {
                base.PlayAnimation("Gesture, Override", "PrimaryLight", "Primary.playbackRate", this.duration);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        private void CalculateDamage()
        {
            float chargeCoef;
            float damageCoef;
            float overchargeThreshold = NucleatorSkillStateBase.overchargeThreshold;

            if (this.charge < overchargeThreshold)
            {
                chargeCoef = this.charge / overchargeThreshold;
                damageCoef = chargeCoef * (maxChargeDamageCoef - minChargeDamageCoef) + minChargeDamageCoef;
                this.damage = this.damageStat * damageCoef;
            }
            else
            {
                chargeCoef = (this.charge - overchargeThreshold) / (1 - overchargeThreshold);
                damageCoef = chargeCoef * (maxOverchargeDamageCoef - maxChargeDamageCoef) + maxChargeDamageCoef;
                this.damage = this.damageStat * damageCoef;
            }
        }

        private void Shoot()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                bool isCrit = base.RollCrit();

                Util.PlayAttackSpeedSound(EntityStates.Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject, this.attackSpeedStat);
                base.characterBody.AddSpreadBloom(EntityStates.Commando.CommandoWeapon.FirePistol2.spreadBloomValue * 1.5f);
                Ray aimRay = base.GetAimRay();
                base.StartAimMode(aimRay, 1f, false);
                if (base.isAuthority)
                {
                    var baseRadius = this.charge * 10;
                    GameObject nucleatorProjectile = Survivors.Nucleator.NucleatorCore.primaryProjectile;
                    nucleatorProjectile.GetComponent<NucleatorProjectile>().baseRadius = baseRadius;
                    nucleatorProjectile.GetComponent<NucleatorProjectile>().charge = this.charge;

                    nucleatorProjectile.GetComponent<ProjectileDamage>().damage = this.damage;

                    if (this.charge > NucleatorSkillStateBase.overchargeThreshold)
                    {
                        nucleatorProjectile.GetComponent<ProjectileDirectionalTargetFinder>().enabled = true;
                        nucleatorProjectile.GetComponent<ProjectileSteerTowardTarget>().enabled = true;
                    }
                    else
                    {
                        nucleatorProjectile.GetComponent<ProjectileDirectionalTargetFinder>().enabled = false;
                        nucleatorProjectile.GetComponent<ProjectileSteerTowardTarget>().enabled = false;
                    }

                    FireProjectileInfo info = new FireProjectileInfo()
                    {
                        crit = base.RollCrit(),
                        damage = 0f,
                        damageColorIndex = RoR2.DamageColorIndex.Default,
                        damageTypeOverride = DamageType.Generic,
                        force = 0f,
                        owner = gameObject,
                        position = aimRay.origin + aimRay.direction.normalized,
                        procChainMask = default,
                        projectilePrefab = nucleatorProjectile,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        target = null,
                        useFuseOverride = false,
                        useSpeedOverride = false                        
                    };
                    ProjectileManager.instance.FireProjectile(info);
                }
            }
        }
    }
}