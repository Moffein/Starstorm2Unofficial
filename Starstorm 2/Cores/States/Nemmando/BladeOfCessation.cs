using System;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.States.Nemmando
{
    class BladeOfCessation : BasicMeleeAttack, SteppedSkillDef.IStepSetter
	{

		public int step;
		public static float baseDamageCoefficient = 1.6f;

		public static float baseDurationNormal = 1.1f;
		public static float baseDurationBeforeInterruptable = 0.65f;
		public static float baseHitPauseDuration = 0.04f;
		public static float baseEffectTime = 0.1f;

		#region comboFinisher(deprecated)
		public static float comboFinisherBaseDuration = 0.7f;
		public static float comboFinisherBaseDurationBeforeInterruptable = 0.65f;
		public static float comboFinisherhitPauseDuration = 0.04f;
		public static float comboFinisherBloom = 0.5f;
        #endregion
        public static float recoilAmplitude = 0.5f;
		public float bloom = 0.5f;
		private float effectTime;
		private bool playedEffect;
		private string animationStateName;
		private float durationBeforeInterruptable;

		private bool isComboFinisher
		{
			get
			{
				return this.step == 2;
			}
		}

		public override bool allowExitFire
		{
			get
			{
				return base.characterBody && !base.characterBody.isSprinting;
			}
		}

		void SteppedSkillDef.IStepSetter.SetStep(int i)
		{
			this.step = i;
		}

		public override void OnEnter()
		{
			this.hitBoxGroupName = "SwordHitbox";
			base.baseDuration = BladeOfCessation.baseDurationNormal;
			base.duration = base.baseDuration / base.attackSpeedStat;
			base.hitPauseDuration = BladeOfCessation.baseHitPauseDuration;
			base.damageCoefficient = BladeOfCessation.baseDamageCoefficient;
            base.procCoefficient = 1.0f;

			base.mecanimHitboxActiveParameter = "Primary.Hitbox";
			this.effectTime = duration * baseEffectTime;

			if (this.isComboFinisher)
			{
				base.hitPauseDuration = BladeOfCessation.comboFinisherhitPauseDuration;
				this.bloom = BladeOfCessation.comboFinisherBloom;
				base.hitBoxGroupName = "SwordHitboxLarge";
				base.baseDuration = BladeOfCessation.comboFinisherBaseDuration;
                Util.PlaySound(EntityStates.Merc.Weapon.GroundLight2.slash3Sound, base.gameObject);
			}
            else
                Util.PlaySound(EntityStates.Merc.Weapon.GroundLight2.slash1Sound, base.gameObject);

			base.OnEnter();

            base.overlapAttack.damageType = DamageType.BlightOnHit;
            base.overlapAttack.hitEffectPrefab = Modules.Assets.nemImpactFX;
			base.characterDirection.forward = base.GetAimRay().direction;
			this.durationBeforeInterruptable = (this.isComboFinisher ? (BladeOfCessation.comboFinisherBaseDurationBeforeInterruptable / base.attackSpeedStat) : (BladeOfCessation.baseDurationBeforeInterruptable / base.attackSpeedStat));
		}

		public override void FixedUpdate() {
			base.FixedUpdate();
			base.StartAimMode();

			if (!this.playedEffect && this.fixedAge > this.effectTime) 
			{
				this.playedEffect = true;
                string swingMuzzle = "SwingLeft";
                if (this.step > 0) swingMuzzle = "SwingRight";
				EffectManager.SimpleMuzzleFlash(Modules.Assets.nemSwingFX, base.gameObject, swingMuzzle, true);
			}
        }

        public override void OnExit()
		{
			base.OnExit();
		}

		public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
            /*if(this.isComboFinisher)
            {
				overlapAttack.damageType = DamageTypeCore.gougeOnHit | DamageType.Generic;
            }
            else
            {
				overlapAttack.damageType = DamageType.Generic;
            }*/
			base.AuthorityModifyOverlapAttack(overlapAttack);
        }

		public override void PlayAnimation()
		{
			this.animationStateName = "";
			switch (this.step)
			{
				case 0:
					this.animationStateName = "Primary1";
					break;
				case 1:
					this.animationStateName = "Primary2";
					break;
				case 2:
					this.animationStateName = "Primary3";
					break;
			}
            bool moving = this.animator.GetBool("isMoving");
            bool grounded = this.animator.GetBool("isGrounded");

            if (!moving && grounded) {
                base.PlayCrossfade("FullBody, Override", this.animationStateName, "Primary.rate", this.duration, 0.05f);
            } else {
                base.PlayCrossfade("UpperBody, Override", this.animationStateName, "Primary.rate", this.duration, 0.05f);
            }
        }

        public override void OnMeleeHitAuthority()
		{
			base.OnMeleeHitAuthority();
			base.characterBody.AddSpreadBloom(this.bloom);
            //just criminal not having hit sounds OR effects.... cmon
            Util.PlaySound(EntityStates.Merc.GroundLight.hitSoundString, base.gameObject);
		}

		public override void BeginMeleeAttackEffect()
		{
			this.swingEffectMuzzleString = this.animationStateName;
			base.AddRecoil(-0.1f * BladeOfCessation.recoilAmplitude, 0.1f * BladeOfCessation.recoilAmplitude, -1f * BladeOfCessation.recoilAmplitude, 1f * BladeOfCessation.recoilAmplitude);
            base.BeginMeleeAttackEffect();
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write((byte)this.step);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.step = (int)reader.ReadByte();
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge >= this.durationBeforeInterruptable)
			{
				return InterruptPriority.Any;
			}
			return InterruptPriority.Skill;
		}

	}
}

