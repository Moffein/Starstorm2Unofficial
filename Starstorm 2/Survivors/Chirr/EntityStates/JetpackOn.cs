using UnityEngine;

namespace EntityStates.SS2UStates.Chirr
{
	public class JetpackOn : BaseState
	{
        public override void OnEnter()
        {
            base.OnEnter();
			base.PlayCrossfade("Jetpack, Override", "Sprint", 0.5f);
        }

        public override void FixedUpdate()
		{
			base.FixedUpdate();
			if (base.isAuthority)
			{
				float num = base.characterMotor.velocity.y;
				num = Mathf.MoveTowards(num, JetpackOn.hoverVelocity, JetpackOn.hoverAcceleration * Time.fixedDeltaTime);
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
			}
		}

        public override void OnExit()
		{
			base.PlayCrossfade("Jetpack, Override", "BufferEmpty", 0.5f);
			base.OnExit();
        }

        public static float hoverVelocity = -1f;
		public static float hoverAcceleration = 60f;
	}
}