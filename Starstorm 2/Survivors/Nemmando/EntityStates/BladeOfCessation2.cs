using EntityStates.SS2UStates.Common;
using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Nemmando.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nemmando
{
	public class BladeOfCessation2 : BaseCustomSkillState
    {
		public static string hitboxString = "SwordHitbox";

		public static float damageCoefficient = 1.6f;
		public static float procCoefficient = 1f;

		public static float baseDuration = 1.1f;
		public static float baseEarlyExitTime = 0.65f;

		public static float hitboxStartTime = 0.07f;
		public static float hitboxEndTime = 0.469f;

		public static float baseEffectTime = 0.08f;

		public static float attackRecoil = 0.5f;
		public static float hitHopVelocity = 5f;
		public static float hitPauseDuration = 0.11f;
		public static float hitBloom = 0.5f;

		public int currentSwing;

		private OverlapAttack overlapAttack;
		private Animator animator;
		//private ChildLocator childLocator;
		//private Transform modelBaseTransform;

		private float duration;
		private float earlyExitDuration;

		private bool hasFired;
		private bool hasHopped;
		private bool playedEffect;

		private float stopwatch;
		private float hitPauseTimer;
		private BaseState.HitStopCachedState hitStopCachedState;
		private bool inHitPause;
		private float effectTime;
        private NemmandoController nemmandoController;
        private Vector3 storedVelocity;

		private float lastUpdateTime;

		public override void OnEnter()
        {
			base.OnEnter();
			lastUpdateTime = Time.time;
			this.duration = baseDuration / this.attackSpeedStat;
			this.earlyExitDuration = this.duration * baseEarlyExitTime;
			this.effectTime = duration * baseEffectTime;
			this.hasFired = false;
            this.nemmandoController = base.GetComponent<NemmandoController>();
            base.characterBody.outOfCombatStopwatch = 0f;

			this.animator = base.GetModelAnimator();
			//this.childLocator = base.GetModelChildLocator();
			//this.modelBaseTransform = base.GetModelBaseTransform();

			HitBoxGroup hitBoxGroup = base.FindHitBoxGroup(hitboxString);

            bool isCrit = base.RollCrit();

			this.overlapAttack = new OverlapAttack();
			this.overlapAttack.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.GougeOnHit);
			this.overlapAttack.attacker = base.gameObject;
			this.overlapAttack.inflictor = base.gameObject;
			this.overlapAttack.teamIndex = base.GetTeam();
			this.overlapAttack.damage = BladeOfCessation2.damageCoefficient * base.damageStat;
			this.overlapAttack.procCoefficient = BladeOfCessation2.procCoefficient;
			this.overlapAttack.hitEffectPrefab = base.effectComponent.impactEffect;
			this.overlapAttack.forceVector = Vector3.zero;
			this.overlapAttack.pushAwayForce = 169f;
			this.overlapAttack.hitBoxGroup = hitBoxGroup;
			this.overlapAttack.isCrit = isCrit;
            this.overlapAttack.impactSound = base.effectComponent.impactSoundDef.index;

            string soundString = base.effectComponent.swingSound;
            //if (isCrit) soundString += "Crit";
            Util.PlaySound(soundString, base.gameObject);
            this.PlayAnimation();
		}

		public override void FixedUpdate()
        {
			base.FixedUpdate();
			float deltaTime = Time.time - lastUpdateTime;
			lastUpdateTime = Time.time;
			base.StartAimMode();

			this.hitPauseTimer -= deltaTime;

			if (this.hitPauseTimer <= 0f && this.inHitPause)
            {
				base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
				this.inHitPause = false;
                if (this.storedVelocity != Vector3.zero) base.characterMotor.velocity = this.storedVelocity;
			}

			if (!this.inHitPause)
            {
				this.stopwatch += deltaTime;
			}
            else
            {
				if (base.characterMotor) base.characterMotor.velocity = Vector3.zero;
				if (this.animator) this.animator.SetFloat("Primary.rate", 0f);
			}

			if (base.isAuthority)
            {
				if (this.stopwatch >= this.duration * BladeOfCessation2.hitboxStartTime && this.stopwatch <= this.duration * BladeOfCessation2.hitboxEndTime)
                {
					this.FireAttack();
				}

				if (this.fixedAge > this.effectTime && !this.playedEffect)
                {
					this.playedEffect = true;
					string swingMuzzle = currentSwing % 2 == 0 ? "SwingLeft" : "SwingRight";
                    base.PlaySwingEffect(swingMuzzle);
				}

				if (base.fixedAge >= this.earlyExitDuration && base.inputBank.skill1.down)
                {
					var nextSwing = new BladeOfCessation2();
					nextSwing.currentSwing = this.currentSwing + 1;
					this.outer.SetNextState(nextSwing);
					return;
				}

				if (base.fixedAge >= this.duration)
                {
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		public override void OnExit()
        {
            if (!this.hasFired) this.FireAttack();

			base.OnExit();
		}

		private void PlayAnimation()
        {
			string swingAnimState = currentSwing % 2 == 0 ? "Primary1" : "Primary2";

			bool moving = this.animator.GetBool("isMoving");
			bool grounded = this.animator.GetBool("isGrounded");

            if (!this.nemmandoController.chargingDecisiveStrike)
            {
                // play normal animations when not using decisive strike
                if (!moving && grounded && !this.nemmandoController.rolling)
                {
                    // don't play this one when rolling as it overrides the roll anim and looks stupid
                    base.PlayCrossfade("FullBody, Override", swingAnimState, "Primary.rate", this.duration, 0.05f);
                }
                base.PlayCrossfade("UpperBody, Override", swingAnimState, "Primary.rate", this.duration, 0.05f);
            }
            else
            {
                // put custom animation for this eventually
                //base.PlayCrossfade("UpperBody, Override", swingAnimState, "Primary.rate", this.duration, 0.05f);
            }
		}

		public void FireAttack()
        {
			if (!this.hasFired)
            {
				this.hasFired = true;

				base.AddRecoil(-1f * BladeOfCessation2.attackRecoil, 
							   -2f * BladeOfCessation2.attackRecoil, 
							   -0.5f * BladeOfCessation2.attackRecoil,
							   0.5f * BladeOfCessation2.attackRecoil);
			}

			if (base.isAuthority)
            {
				if (this.overlapAttack.Fire())
                {
					base.characterBody.AddSpreadBloom(BladeOfCessation2.hitBloom);

					base.AddRecoil(-1f * BladeOfCessation2.attackRecoil,
								   -2f * BladeOfCessation2.attackRecoil,
								   -0.5f * BladeOfCessation2.attackRecoil,
								   0.5f * BladeOfCessation2.attackRecoil);

					if (!this.hasHopped)
                    {
						if (base.characterMotor && !base.characterMotor.isGrounded)
                        {
							base.SmallHop(base.characterMotor, BladeOfCessation2.hitHopVelocity / Mathf.Sqrt(this.attackSpeedStat));
						}

						this.hasHopped = true;
					}

					if (!this.inHitPause)
                    {
						this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Primary.rate");
						this.hitPauseTimer = (BladeOfCessation2.hitPauseDuration) / this.attackSpeedStat;
						this.inHitPause = true;
                        if (base.characterMotor.velocity != Vector3.zero) this.storedVelocity = base.characterMotor.velocity;
					}
				}
			}
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.currentSwing);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.currentSwing = (int)reader.ReadByte();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			//if (base.fixedAge >= this.durationBeforeInterruptable)
			//{
			//	return InterruptPriority.Any;
			//}
			return InterruptPriority.Skill;
		}
	}
}