using RoR2;
using RoR2.Orbs;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using Starstorm2.Cores.States;
using UnityEngine.Networking;
using EntityStates.SS2UStates.Common;

namespace EntityStates.SS2UStates.Executioner
{
    public class ExecutionerPistol : BaseCustomSkillState
    {
        public static float damageCoefficient = 3f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.65f;
        public static float recoil = 5f;
        public static float spreadBloom = Commando.CommandoWeapon.FirePistol2.spreadBloomValue * 2.5f;
        public static GameObject tracerPrefab;
        public static GameObject hitPrefab = Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;
        public static float force = 100f;

        private float duration;
        private float fireDuration;
        private string muzzleString;
        private bool hasFired;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.duration = baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.1f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";
            this.hasFired = false;

            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                Shoot();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void Shoot()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                bool isCrit = base.RollCrit();

                string soundString = base.effectComponent.shootSound;
                if (isCrit) soundString += "Crit";
                Util.PlaySound(soundString, base.gameObject);
                base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);

                EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
                tracerPrefab = Starstorm2.Modules.Assets.exeGunTracer;

                if (base.isAuthority)
                {
                    Ray r = base.GetAimRay();
                    BulletAttack bullet = new BulletAttack {
                        aimVector = r.direction,
                        origin = r.origin,
                        damage = damageCoefficient * damageStat,
                        damageType = DamageType.Generic,
                        damageColorIndex = DamageColorIndex.Default,
                        minSpread = 0f,
                        maxSpread = base.characterBody.spreadBloomAngle * 0.5f,
                        falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                        force = ExecutionerPistol.force,
                        isCrit = isCrit,
                        owner = base.gameObject,
                        muzzleName = muzzleString,
                        smartCollision = true,
                        procChainMask = default(ProcChainMask),
                        procCoefficient = procCoefficient,
                        radius = 0.35f,
                        weapon = base.gameObject,
                        tracerEffectPrefab = tracerPrefab,
                        hitEffectPrefab = hitPrefab
                    };
                    bullet.Fire();
                }
                base.characterBody.AddSpreadBloom(spreadBloom);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }

    public class ExecutionerTaser : BaseSkillState
    {
        public static float damageCoefficient = 0.8f;
        public static float procCoefficient = 0.75f;
        public static float baseDuration = 0.4f;
        public static float recoil = 0f;
        public static float spreadBloom = Commando.CommandoWeapon.FirePistol2.spreadBloomValue * 2.5f;
        public static GameObject tracerPrefab;
        public static GameObject hitPrefab = Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;

        private float duration;
        private float fireDuration;
        private string muzzleString;
        private bool hasFired;
        private Animator animator;
        private BullseyeSearch search;
        private float minAngleFilter = 0;
        private float maxAngleFilter = 45;
        private float attackRange = 28;
        private List<HealthComponent> previousTargets;
        private int attackFireCount = 1;


        public override void OnEnter()
        {
            base.OnEnter();
            this.animator = base.GetModelAnimator();
            this.duration = baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.1f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.muzzleString = "Muzzle";
            this.hasFired = false;
            base.characterBody.outOfCombatStopwatch = 0f;

            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                Shoot();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void Shoot()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;
                bool isCrit = base.RollCrit();

                //Util.PlaySound(Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);
                string soundString = "ExecutionerPrimary";
                if (isCrit) soundString += "Crit";
                //Util.PlaySound(soundString, base.gameObject);
                base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);

                EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, this.muzzleString, false);
                tracerPrefab = Starstorm2.Modules.Assets.exeGunTracer;

                if (NetworkServer.active)
                {
                    float dmg = damageCoefficient * this.damageStat;
                    Ray r = base.GetAimRay();
                    this.search = new BullseyeSearch();
                    this.search.searchOrigin = base.transform.position;
                    this.search.searchDirection = r.direction;
                    this.search.sortMode = BullseyeSearch.SortMode.Distance;
                    this.search.teamMaskFilter = TeamMask.allButNeutral;
                    this.search.teamMaskFilter.RemoveTeam(base.GetTeam());
                    this.search.filterByLoS = false;
                    this.search.minAngleFilter = this.minAngleFilter;
                    this.search.maxAngleFilter = this.maxAngleFilter;
                    this.search.maxDistanceFilter = this.attackRange;
                    this.search.RefreshCandidates();
                    HurtBox hurtBox = search.GetResults().FirstOrDefault<HurtBox>();
                    if (hurtBox)
                    {
                        Util.PlaySound(soundString, base.gameObject);
                        //this.previousTargets.Add(hurtBox.healthComponent);
                        LightningOrb lightningOrb = new LightningOrb();
                        lightningOrb.bouncedObjects = new List<HealthComponent>();
                        lightningOrb.attacker = base.gameObject;
                        lightningOrb.inflictor = base.gameObject;
                        lightningOrb.teamIndex = base.GetTeam();
                        lightningOrb.damageValue = dmg;
                        lightningOrb.isCrit = isCrit;
                        lightningOrb.origin = base.transform.position;
                        lightningOrb.bouncesRemaining = 4;
                        lightningOrb.lightningType = LightningOrb.LightningType.Ukulele;
                        lightningOrb.procCoefficient = procCoefficient;
                        lightningOrb.target = hurtBox;
                        lightningOrb.damageColorIndex = DamageColorIndex.Default;
                        lightningOrb.damageType = DamageType.Generic;
                        OrbManager.instance.AddOrb(lightningOrb);
                    }
                }
                base.characterBody.AddSpreadBloom(spreadBloom);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}