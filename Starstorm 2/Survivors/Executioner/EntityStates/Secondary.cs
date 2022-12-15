using EntityStates.SS2UStates.Common;
using R2API;
using RoR2;
using Starstorm2.Cores;
using Starstorm2.Cores.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Executioner
{
    public class ExecutionerIonGun : BaseCustomSkillState
    {
        public static float damageCoefficient = 3.2f;
        public static float procCoefficient = 1.0f;
        public static float baseDuration = 0.12f;
        public static float recoil = 1f;
        public static float range = 1000f;
        public static GameObject muzzlePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/MuzzleflashHuntressFlurry.prefab").WaitForCompletion();
        public GameObject tracerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject hitPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion();

        private float duration;
        private BulletAttack bullet;
        private string muzzleString;
        private float shotTimer;
        private int shotsToFire;
        private GenericSkill skill;
        private Animator animator;
        private EffectData ionEffectData;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            //tracerPrefab = Starstorm2.Modules.Assets.exeIonBurstTracer;   //doesn't sync up with the impact effect well enough
            this.animator = base.GetModelAnimator();

            //how do we get this skill's slot without hardcoding like this
            // doesn't really matter
            skill = base.characterBody.skillLocator.GetSkill(SkillSlot.Secondary);
            this.shotsToFire = skill.stock;
            //if (shotsToFire > 10) shotsToFire = 10;
            this.duration = baseDuration;// / this.attackSpeedStat;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";
            
            isCrit = base.RollCrit();

            Shoot();
            shotsToFire--;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.shotTimer += Time.fixedDeltaTime;
            if (this.shotTimer >= this.duration)
            {
                //fire successive shots if more than one stored, deduct charges manually
                this.shotTimer = 0;
                if (this.shotsToFire > 0)
                {
                    this.Shoot();
                    this.shotsToFire--;
                }
                else
                {
                    if (base.isAuthority) this.outer.SetNextStateToMain();
                }
            }
        }

        private void Shoot()
        {
            //Util.PlayAttackSpeedSound(base.effectComponent.ionShootSound, base.gameObject, this.attackSpeedStat);
            Util.PlaySound("ExecutionerSecondaryClassic", base.gameObject);
            base.AddRecoil(-2f * recoil, -3f * recoil, -1f * recoil, 1f * recoil);
            //base.characterBody.AddSpreadBloom(Commando.CommandoWeapon.FirePistol2.spreadBloomValue * 1.0f);
            EffectManager.SimpleMuzzleFlash(ExecutionerIonGun.muzzlePrefab, base.gameObject, this.muzzleString, false);
            ionEffectData = new EffectData()
            {
                origin = base.GetModelChildLocator().FindChild(this.muzzleString).position,
                rotation = Quaternion.LookRotation(base.GetAimRay().direction)
            };

            EffectManager.SpawnEffect(Starstorm2.Modules.Assets.exeIonEffect, ionEffectData, true);

            float animSpeed = 4f;
            if (this.shotsToFire == 1) animSpeed = 8f;

            base.PlayAnimation("Gesture, Override", "Secondary", "Secondary.playbackRate", this.duration * animSpeed);

            if (base.isAuthority)
            {
                float dmg = damageCoefficient * this.damageStat;
                Ray r = base.GetAimRay();
                Vector3 vec = r.direction;
                bullet = new BulletAttack
                {
                    aimVector = vec,
                    origin = r.origin,
                    damage = damageCoefficient * damageStat,
                    damageType = DamageType.Shock5s,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = range,
                    force = Commando.CommandoWeapon.FireBarrage.force,
                    isCrit = isCrit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procCoefficient = procCoefficient,
                    radius = 1f,
                    weapon = base.gameObject,
                    tracerEffectPrefab = tracerPrefab,
                    hitEffectPrefab = hitPrefab,
                    stopperMask = LayerIndex.world.mask
                    //HitEffectNormal = ClayBruiser.Weapon.MinigunFire.bulletHitEffectNormal
                };
                bullet.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ExtendFear);
                bullet.Fire();

                if (!base.characterBody.HasBuff(Starstorm2.Cores.BuffCore.exeSuperchargedBuff)) skill.DeductStock(1);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}