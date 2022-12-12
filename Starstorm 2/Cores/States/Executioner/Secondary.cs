using RoR2;
using Starstorm2.Cores;
using Starstorm2.Cores.States;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Executioner
{
    public class ExecutionerIonGun : BaseCustomSkillState
    {
        public static float damageCoefficient = 3.5f;
        public static float procCoefficient = 1.0f;
        public static float baseDuration = 0.1f;
        public static float recoil = 2f;
        public static float aimSnapAngle = 7.5f;
        public static float range = 200f; //copied from default range
        public static GameObject muzzlePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashHuntressFlurry");
        public static GameObject tracerPrefab;
        public static GameObject hitPrefab = Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        private float duration;
        private BulletAttack bullet;
        private string muzzleString;
        private BullseyeSearch search;
        private List<HurtBox> targets;
        private float shotTimer;
        private int shotsToFire;
        private GenericSkill skill;
        private Animator animator;
        private EffectData ionEffectData;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            //how do we get this skill's slot without hardcoding like this
            // doesn't really matter
            skill = base.characterBody.skillLocator.GetSkill(SkillSlot.Secondary);
            this.shotsToFire = skill.stock;
            if (shotsToFire > 10) shotsToFire = 10;
            this.duration = baseDuration;// / this.attackSpeedStat;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";

            search = new BullseyeSearch();
            search.teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam());
            search.filterByLoS = true;
            search.maxDistanceFilter = ExecutionerIonGun.range;
            search.minAngleFilter = 0;
            search.maxAngleFilter = aimSnapAngle;
            search.sortMode = BullseyeSearch.SortMode.DistanceAndAngle;
            search.filterByDistinctEntity = true;

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
            bool isCrit = base.RollCrit();

            Util.PlayAttackSpeedSound(base.effectComponent.ionShootSound, base.gameObject, this.attackSpeedStat);
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
            tracerPrefab = Starstorm2.Modules.Assets.exeIonBurstTracer;

            if (base.isAuthority)
            {
                float dmg = damageCoefficient * this.damageStat;
                Ray r = base.GetAimRay();
                Vector3 vec = r.direction;

                search.searchOrigin = r.origin;
                search.searchDirection = r.direction;
                search.RefreshCandidates();
                targets = search.GetResults().Where(new Func<HurtBox, bool>(Util.IsValid)).Distinct(default(HurtBox.EntityEqualityComparer)).ToList();

                if (targets.Count > 0 && targets[0].healthComponent)
                {
                    //idx = (idx + 1) % targets.Count;
                    vec = (targets[0].transform.position - r.origin);
                }
                bullet = new BulletAttack
                {
                    aimVector = vec,
                    origin = r.origin,
                    damage = damageCoefficient * damageStat,
                    damageType = DamageType.Generic,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0.2f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = range,
                    force = Commando.CommandoWeapon.FireBarrage.force,
                    isCrit = isCrit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procCoefficient = procCoefficient,
                    radius = 0.5f,
                    weapon = base.gameObject,
                    tracerEffectPrefab = tracerPrefab,
                    hitEffectPrefab = hitPrefab
                    //HitEffectNormal = ClayBruiser.Weapon.MinigunFire.bulletHitEffectNormal
                };
                bullet.Fire();

                if (!base.characterBody.HasBuff(Starstorm2.Modules.Buffs.exeSuperchargedBuff)) skill.DeductStock(1);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}