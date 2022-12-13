using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using R2API;
using R2API.Utils;
using EntityStates;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Starstorm2.Cores;
using UnityEngine.Networking;
using KinematicCharacterController;

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Cyborg
{
    public class CyborgFireBaseShot : BaseSkillState
    {
        public static float damageCoefficient = 2.5f;
        public float baseDuration = 0.5f;
        public float recoil = 1f;
        public static GameObject tracerEffectPrefab;//Prefabs/Effects/Tracers/TracerHuntressSnipe
        public static GameObject hitEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        public static bool switchHand;

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;



        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            if (switchHand)
            {
                this.muzzleString = "Lowerarm.R_end";
                base.PlayCrossfade("Gesture, Override", "FireM1", "FireM1.playbackRate", this.duration, 0.1f);
            }
            else
            {
                this.muzzleString = "Lowerarm.L_end";
                base.PlayCrossfade("Gesture, Override", "FireM1Alt", "FireM1.playbackRate", this.duration, 0.1f);
            }
            switchHand = !switchHand;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireShot()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(0.35f);
                Ray aimRay = base.GetAimRay();
                EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

                /*if (!switchHand) // use a variable for that once sounds are no longer placeholders.
                    Util.PlaySound(Commando.CommandoWeapon.FirePistol.firePistolSoundString, base.gameObject);
                else
                    Util.PlaySound(Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);*/
                string soundString = "CyborgPrimary";//base.effectComponent.shootSound;
                //if (isCrit) soundString += "Crit";
                Util.PlaySound(soundString, base.gameObject);

                if (base.isAuthority)
                {
                    new BulletAttack
                    {
                        owner = base.gameObject,
                        weapon = base.gameObject,
                        origin = aimRay.origin,
                        aimVector = aimRay.direction,
                        minSpread = 0,
                        maxSpread = 0,
                        damage = damageCoefficient * this.damageStat,
                        force = 1000f,
                        radius = 1f,
                        smartCollision = true,
                        tracerEffectPrefab = tracerEffectPrefab,
                        muzzleName = muzzleString,
                        hitEffectPrefab = hitEffectPrefab,
                        isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                        falloffModel = BulletAttack.FalloffModel.None,
                        damageType = DamageType.SlowOnHit,
                        maxDistance = 1000f
                    }.Fire();
                    //ProjectileManager.instance.FireProjectile(ExampleSurvivor.ExampleSurvivor.bfgProjectile, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
                }
                EffectManager.SimpleMuzzleFlash(CyborgFireBaseShot.muzzleflashEffectPrefab, base.gameObject, muzzleString, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireShot();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}