using EntityStates;
using RoR2;
using Starstorm2.Components;
using Starstorm2.Survivors.Nemmando.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Starstorm2States.Nemmando
{
	public class DodgeState : BaseSkillState
	{
		public float duration = 0.6f;
		public float initialSpeedCoefficient = 7.5f;
		public float finalSpeedCoefficient = 1f;
		public static float dodgeFOV = -1f;

		private float rollSpeed;
		private Vector3 forwardDirection;
		private Vector3 previousPosition;
		private Animator animator;
        private NemmandoController nemmandoController;

		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
            this.nemmandoController = base.GetComponent<NemmandoController>();
			Util.PlaySound(EntityStates.Commando.DodgeState.dodgeSoundString, base.gameObject);

			base.PlayCrossfade("FullBody, Override", "Utility", "Utility.rate", this.duration * 1.25f, 0.05f);
			base.PlayCrossfade("UpperBody, Override", "BufferEmpty", 0.05f);

            //don't add buffs unless you're the server ty
            if (NetworkServer.active) base.characterBody.AddTimedBuff(RoR2Content.Buffs.SmallArmorBoost, 1.5f * this.duration);

			if (base.isAuthority && base.inputBank && base.characterDirection)
			{
				this.forwardDirection = ((base.inputBank.moveVector == Vector3.zero) ? base.characterDirection.forward : base.inputBank.moveVector).normalized;
			}

			RecalculateRollSpeed();
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity.y = 0f;
				base.characterMotor.velocity = this.forwardDirection * this.rollSpeed;
			}

            // dunno why this didn't work. FUCK HOPOO!
            //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.DodgeState.jetEffect, base.gameObject, "JetMuzzleL", true);
            //EffectManager.SimpleMuzzleFlash(EntityStates.Commando.DodgeState.jetEffect, base.gameObject, "JetMuzzleR", true);

            Vector3 velocity = base.characterMotor ? base.characterMotor.velocity : Vector3.zero;
			this.previousPosition = base.transform.position - velocity;

            this.nemmandoController.rolling = true;
            //this.nemmandoController.ActivateThrusters();
        }

		private void RecalculateRollSpeed()
		{
			this.rollSpeed = this.moveSpeedStat * Mathf.Lerp(this.initialSpeedCoefficient, this.finalSpeedCoefficient, base.fixedAge / this.duration);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			RecalculateRollSpeed();

			if(base.isAuthority)
			{
				Vector3 normalized = (base.transform.position - this.previousPosition).normalized;
				if (base.characterMotor && base.characterDirection && normalized != Vector3.zero)
				{
					Vector3 vector = normalized * this.rollSpeed;
					float y = vector.y;
					vector.y = 0f;
					float d = Mathf.Max(Vector3.Dot(vector, this.forwardDirection), 0f);
					vector = this.forwardDirection * d;
					vector.y += Mathf.Max(y, 0f);
					base.characterMotor.velocity = vector;

					Vector3 rhs = base.inputBank ? base.characterDirection.forward : this.forwardDirection;
					Vector3 rhs2 = Vector3.Cross(Vector3.up, rhs);
					float num = Vector3.Dot(this.forwardDirection, rhs);
					float num2 = Vector3.Dot(this.forwardDirection, rhs2);
					this.animator.SetFloat("forwardSpeed", num);
					this.animator.SetFloat("rightSpeed", num2);
				}

				this.previousPosition = base.transform.position;
				if (base.fixedAge >= this.duration && base.isAuthority)
				{
					this.outer.SetNextStateToMain();
					return;
				}
			}
		}

		public override void OnExit()
		{
			if (base.cameraTargetParams)
			{
				base.cameraTargetParams.fovOverride = -1f;
			}

            this.nemmandoController.rolling = false;
            //this.nemmandoController.DeactivateThrusters();

            base.OnExit();
		}

		public override void OnSerialize(NetworkWriter writer)
		{
			base.OnSerialize(writer);
			writer.Write(this.forwardDirection);
		}

		public override void OnDeserialize(NetworkReader reader)
		{
			base.OnDeserialize(reader);
			this.forwardDirection = reader.ReadVector3();
		}
	}
}
