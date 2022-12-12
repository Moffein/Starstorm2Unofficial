using System;
using System.Security.Policy;
using EntityStates;
using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.States.Nemmando
{
    class FireDecisiveStrike : BasicMeleeAttack
	{
		public float charge = 0f;
		public static float recoilAmplitude = 0.5f;
		public static float baseDurationBeforeInterruptable = 0.5f;
		public float bloom = 1f;
		
		private string animationStateName;

		private float durationBeforeInterruptable;
		private float minDamageCoef = 2f;
		private float maxDamageCoef = 4f;

		public override bool allowExitFire
		{
			get
			{
				return base.characterBody && !base.characterBody.isSprinting;
			}
		}

		public override void OnEnter()
		{
			this.hitBoxGroupName = "SwordHitboxWide";
			this.baseDuration = 0.5f;
			this.hitPauseDuration = 0.01f;
			this.damageCoefficient = Mathf.Lerp(this.minDamageCoef, this.maxDamageCoef, this.charge);
			base.OnEnter();
			base.characterDirection.forward = base.GetAimRay().direction;
			this.durationBeforeInterruptable = BladeOfCessation.baseDurationBeforeInterruptable / this.attackSpeedStat;
            this.procCoefficient = 1.0f;


			Vector3 quat = Quaternion.LookRotation(characterDirection.forward).eulerAngles;

			GameObject slashEffect = Modules.Assets.nemSwingFX;
			
			slashEffect.transform.parent = base.characterMotor.transform;
			EffectData slashEffectData = new EffectData()
			{
				scale = 1,
				origin = base.characterBody.transform.position,
				rotation = Quaternion.Euler(quat.x + 90, quat.y, quat.z)				
			};
            //use EffectManager.SimpleMuzzleFlash instead, with transform "SwingCenter"
            //lazy to test that rn

			EffectManager.SpawnEffect(slashEffect, slashEffectData, false);
            Util.PlaySound(EntityStates.Merc.Uppercut.hitSoundString, base.gameObject);
		}

		public override void OnExit()
		{
			base.OnExit();
		}

		public override void AuthorityModifyOverlapAttack(OverlapAttack overlapAttack)
		{
			base.AuthorityModifyOverlapAttack(overlapAttack);
			overlapAttack.damageType = DamageType.BlightOnHit | DamageType.Generic;
		}

		public override void PlayAnimation()
		{
			this.animationStateName = "Secondary5";
			base.PlayAnimation("FullBody, Override", this.animationStateName, "GroundLight.playbackRate", this.duration);// ; (, this.animationStateName, , this.duration, 0.05f);
		}

		public override void OnMeleeHitAuthority()
		{
			base.OnMeleeHitAuthority();
			base.characterBody.AddSpreadBloom(this.bloom);
		}

		public override void BeginMeleeAttackEffect()
		{
			this.swingEffectMuzzleString = this.animationStateName;
			base.AddRecoil(-0.1f * FireDecisiveStrike.recoilAmplitude, 0.1f * FireDecisiveStrike.recoilAmplitude, -1f * FireDecisiveStrike.recoilAmplitude, 1f * FireDecisiveStrike.recoilAmplitude);
			base.BeginMeleeAttackEffect();
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			if (base.fixedAge >= this.durationBeforeInterruptable)
			{
				return InterruptPriority.Skill;
			}
			return InterruptPriority.PrioritySkill;
		}

	}
}

