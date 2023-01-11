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
using Starstorm2Unofficial.Cores;
using UnityEngine.Networking;
using KinematicCharacterController;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class CyborgFireTrackshot : BaseSkillState
    {
        public static float damageCoefficient = 3.0f;
        public float baseDuration = 0.4f;
        public float recoil = 1f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");
        public GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public GameObject critEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/critspark");

        private float duration;
        private float fireDuration;
        private bool firstShot = false;
        private bool secondShot = false;
        private bool thirdShot = false;
        private bool isCrit = false;
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

            isCrit = base.RollCrit();

            base.PlayAnimation("Gesture, Override", "FireM2", "FireArrow.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireTrackshot()
        {
            base.characterBody.AddSpreadBloom(0.75f);
            Ray aimRay = base.GetAimRay();
            Vector3 targetV;
            BullseyeSearch bullseyeSearch = new BullseyeSearch();
            EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
            TeamComponent team = base.characterBody.GetComponent<TeamComponent>();
            bullseyeSearch.teamMaskFilter = TeamMask.all;
            bullseyeSearch.teamMaskFilter.RemoveTeam(team.teamIndex);
            bullseyeSearch.filterByLoS = true;
            //bullseyeSearch.filterByDistinctEntity = true;
            bullseyeSearch.searchOrigin = aimRay.origin;
            bullseyeSearch.searchDirection = aimRay.direction;
            bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
            bullseyeSearch.maxDistanceFilter = 10000;
            bullseyeSearch.maxAngleFilter = 30f;
            bullseyeSearch.RefreshCandidates();
            targetV = aimRay.direction;
            HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
            if ((bool)target)
            {
                targetV = target.transform.position - aimRay.origin;

            }

            string soundString = "SS2UCyborgSecondary";//base.effectComponent.shootSound;
            //if (isCrit) soundString += "Crit";
            Util.PlaySound(soundString, base.gameObject);
            if (base.isAuthority)
            {
                BulletAttack bullet = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = targetV.normalized,
                    minSpread = 0,
                    maxSpread = 0,
                    damage = damageCoefficient * this.damageStat,
                    force = 100,
                    tracerEffectPrefab = CyborgFireTrackshot.tracerEffectPrefab,
                    muzzleName = muzzleString,
                    hitEffectPrefab = isCrit ? critEffectPrefab : effectPrefab,
                    isCrit = isCrit,
                    damageType = DamageType.Stun1s
                };
                bullet.Fire();
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
            return InterruptPriority.Pain;
        }
    }
}