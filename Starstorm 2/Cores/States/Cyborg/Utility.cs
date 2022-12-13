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
using Starstorm2.Cores;
using UnityEngine.Networking;
using KinematicCharacterController;
using JetBrains.Annotations;
using RoR2.Networking;
using Starstorm2.Components;

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Cyborg
{
    public class CyborgFireBFG : BaseSkillState
    {
        public float damageCoefficient = 1.85f;
        public float baseDuration = 0.5f;
        public float recoil = 1f;

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;

        private CyborgController cyborgController;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Muzzle";
            cyborgController = base.GetComponent<CyborgController>();
            if (cyborgController)
            {
                cyborgController.allowJetpack = false;
            }

            base.PlayAnimation("Gesture, Override", "FireSpecial", "FireArrow.playbackRate", this.duration);

            //old code
            /*if (base.isAuthority && base.characterBody && base.characterBody.characterMotor)
            {
                float height = base.characterBody.characterMotor.isGrounded ? this.groundKnockbackDistance : this.airKnockbackDistance;
                float num3 = base.characterBody.characterMotor ? base.characterBody.characterMotor.mass : 1f;
                float acceleration2 = base.characterBody.acceleration;
                float num4 = Trajectory.CalculateInitialYSpeedForHeight(height, -acceleration2);
                base.characterBody.characterMotor.ApplyForce(-num4 * num3 * base.GetAimRay().direction, false, false);
            }*/

            //copied from sniper
            Vector3 direction = -base.GetAimRay().direction;
            if (base.isAuthority)
            {
                direction.y = Mathf.Max(direction.y, 0.05f);
                Vector3 a = direction.normalized * 4f * 10f;
                Vector3 b = Vector3.up * 7f;
                Vector3 b2 = new Vector3(direction.x, 0f, direction.z).normalized * 3f;


                if (base.characterMotor)
                {
                    base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = a + b + b2;
                    base.characterMotor.velocity.y *= 0.8f;
                    if (base.characterMotor.velocity.y < 0) base.characterMotor.velocity.y *= 0.1f;
                }
                if (base.characterDirection)
                {

                    base.characterDirection.moveVector = direction;
                }
            }
        }

        public override void OnExit()
        {
            if (cyborgController)
            {
                cyborgController.allowJetpack = true;
            }
            base.OnExit();
        }

        private void FireBFG()
        {
            if (!this.hasFired)
            {
                string soundString = "CyborgUtility";//base.effectComponent.shootSound;
                //if (isCrit) soundString += "Crit";
                Util.PlaySound(soundString, base.gameObject);
                this.hasFired = true;

                base.characterBody.AddSpreadBloom(0.75f);
                Ray aimRay = base.GetAimRay();
                EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    ProjectileManager.instance.FireProjectile(CyborgCore.bfgProjectile,
                        aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction),
                        base.gameObject, this.characterBody.damage * 1f,
                        0f,
                        Util.CheckRoll(this.characterBody.crit,
                        this.characterBody.master),
                        DamageColorIndex.Default, null, -1f);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireBFG();
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