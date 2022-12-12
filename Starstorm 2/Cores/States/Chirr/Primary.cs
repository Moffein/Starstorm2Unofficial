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

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Chirr
{
    public class ChirrFireDarts : BaseSkillState
    {
        public static float damageCoefficient = 0.9f;
        public float baseDuration = 0.6f;
        public float recoil = 1f;
        public static GameObject tracerEffectPrefab = Resources.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");
        public GameObject effectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public GameObject critEffectPrefab = Resources.Load<GameObject>("prefabs/effects/impacteffects/critspark");

        private float duration;
        private float fireDuration;
        private bool firstShot = false;
        private bool secondShot = false;
        private bool thirdShot = false;
        private Animator animator;
        private string muzzleString;
        private BullseyeSearch search = new BullseyeSearch();


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Lowerarm.L_end";


            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Primary", "Primary.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireTrackshot()
        {
            base.characterBody.AddSpreadBloom(0.1f);
            Ray aimRay = base.GetAimRay();
            //EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol.effectPrefab, base.gameObject, this.muzzleString, false);
            TeamComponent team = base.characterBody.GetComponent<TeamComponent>();

            //Util.PlaySound(Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);
            //if (NetworkServer.active)
            //    Chat.AddMessage("m1servertest");
            if (base.isAuthority)
            {
                ProjectileManager.instance.FireProjectile(
                    ChirrCore.chirrDart, 
                    aimRay.origin, 
                    Util.QuaternionSafeLookRotation(aimRay.direction), 
                    base.gameObject, 
                    damageCoefficient * this.damageStat, 
                    0f, 
                    Util.CheckRoll(this.critStat, base.characterBody.master), 
                    DamageColorIndex.Default, 
                    null, 
                    -1f);
                //ProjectileManager.instance.FireProjectile(ExampleSurvivor.ExampleSurvivor.bfgProjectile, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.fireDuration) && !firstShot)
            {
                FireTrackshot();
                firstShot = true;
            }
            if ((base.fixedAge >= this.fireDuration * 2) && !secondShot)
            {
                FireTrackshot();
                secondShot = true;
            }
            if ((base.fixedAge >= this.fireDuration * 3) && !thirdShot)
            {
                FireTrackshot();
                thirdShot = true;
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