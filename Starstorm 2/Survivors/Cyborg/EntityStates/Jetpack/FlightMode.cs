using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Cyborg.Jetpack
{
    public class FlightMode : BaseState
    {
        public static float baseDuration = 1f/0.6f;
        public static float speedMultCoefficient = 3f;
        public static float damageCoefficient = 4f;
        public static float force = 2400f;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Toolbot/ImpactToolbotDashLarge.prefab").WaitForCompletion();

        public static float hitStopDuration = 0.1f;
        private bool inHitPause;
        private float hitPauseTimer;
        private BaseState.HitStopCachedState hitStopCachedState;
        private Vector3 storedVelocity;
        private Animator animator;
        private float speedDamageFactor;

        private float stopwatch;
        private float baseSpeed;
        private float desiredSpeed;
        private OverlapAttack attack;
        private CyborgEnergyComponent energyComponent;

        private float lastUpdateTime;

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            inHitPause = false;
            hitPauseTimer = 0f;
            stopwatch = 0f;

            animator = base.GetModelAnimator();

            base.StartAimMode(FlightMode.baseDuration + 1f);
            baseSpeed = this.moveSpeedStat;
            desiredSpeed = this.moveSpeedStat * FlightMode.speedMultCoefficient;
            Util.PlaySound("Play_MULT_shift_start", base.gameObject);

            speedDamageFactor = Mathf.Max(desiredSpeed / (7f * 1.45f * FlightMode.speedMultCoefficient), 1f);

            if (base.isAuthority)
            {
                InitOverlapAttack();
            }

            if (Starstorm2Unofficial.Survivors.Cyborg.CyborgCore.useEnergyRework.Value) energyComponent = base.GetComponent<CyborgEnergyComponent>();
            if (energyComponent)
            {
                energyComponent.energySkillsActive++;
            }
        }

        private void InitOverlapAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "RamHitbox");
            }

            if (hitBoxGroup)
            {
                this.attack = new OverlapAttack
                {
                    attacker = base.gameObject,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    pushAwayForce = 0f,
                    damage = this.damageStat * FlightMode.damageCoefficient * speedDamageFactor,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Stun1s,
                    forceVector = Vector3.zero,
                    hitBoxGroup = hitBoxGroup,
                    hitEffectPrefab = FlightMode.hitEffectPrefab,
                    inflictor = base.gameObject,
                    isCrit = base.RollCrit(),
                    teamIndex = base.GetTeam(),
                    procChainMask = default,
                    procCoefficient = 1f
                };
                this.attack.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ScaleForceToMass);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            if (energyComponent)
            {
                energyComponent.ConsumeEnergy(deltaTime / FlightMode.baseDuration);
            }

            this.hitPauseTimer -= deltaTime;
            if (!this.inHitPause)
            {
                this.stopwatch += deltaTime;
            }

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                if (this.hitPauseTimer <= 0f && this.inHitPause)
                {
                    base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
                    this.inHitPause = false;
                    base.characterMotor.velocity = this.storedVelocity;
                }

                if (this.inHitPause)
                {
                    if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
                    //if (this.animator) this.animator.SetFloat(animatorParam, 0f);
                }

                if (base.characterMotor)
                {
                    if (base.characterMotor.isGrounded && base.characterMotor.Motor) base.characterMotor.Motor.ForceUnground();
                    base.characterMotor.velocity = Vector3.zero;
                    base.characterMotor.rootMotion += deltaTime * this.desiredSpeed * aimRay.direction;
                }
                if (this.attack != null)
                {
                    this.attack.forceVector = aimRay.direction * FlightMode.force;
                    if (this.attack.Fire())
                    {
                        OnHitEnemyAuthority();
                    }
                }

                bool keyPressed = base.inputBank && base.inputBank.skill3.down;
                bool hasEnergy = energyComponent && !energyComponent.energyDepleted;

                if (!Starstorm2Unofficial.Survivors.Cyborg.CyborgCore.useEnergyRework.Value)
                {
                    keyPressed = stopwatch <= FlightMode.baseDuration;
                    hasEnergy = true;
                }

                if (!keyPressed || !hasEnergy)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        private void OnHitEnemyAuthority()
        {
            Util.PlaySound("Play_MULT_shift_hit", base.gameObject);
            if (!this.inHitPause && hitStopDuration > 0f)
            {
                this.storedVelocity = base.characterMotor.velocity;
                this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
                this.hitPauseTimer = hitStopDuration / this.attackSpeedStat;
                this.inHitPause = true;
            }
        }

        public override void OnExit()
        {
            Util.PlaySound("Play_MULT_shift_end", base.gameObject);
            if (base.isAuthority && base.characterMotor && !base.characterMotor.isGrounded)
            {
                base.characterMotor.velocity = base.GetAimRay().direction * this.desiredSpeed;
            }
            if (energyComponent)
            {
                energyComponent.energySkillsActive--;
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
