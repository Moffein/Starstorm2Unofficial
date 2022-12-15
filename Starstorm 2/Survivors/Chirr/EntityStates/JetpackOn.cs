using UnityEngine;

namespace EntityStates.SS2UStates.Chirr
{
	public class JetpackOn : BaseState
	{
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
		public static float hoverVelocity = -1f;
		public static float hoverAcceleration = 60f;
	}
}
