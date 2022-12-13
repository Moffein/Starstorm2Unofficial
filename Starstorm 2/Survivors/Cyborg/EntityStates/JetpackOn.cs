using RoR2;
using UnityEngine;
using EntityStates;

namespace EntityStates.Starstorm2States.Cyborg
{
	public class JetpackOn : BaseState
	{
        public override void OnEnter()
        {
            base.OnEnter();
			Util.PlaySound("Play_mage_m1_impact", base.gameObject);

			childLocator = base.GetModelChildLocator();
			if (childLocator)
			{
				Transform thrusterEffectL = childLocator.FindChild("ThrusterEffectL");
				if (thrusterEffectL) thrusterEffectL.gameObject.SetActive(true);

				Transform thrusterEffectR = childLocator.FindChild("ThrusterEffectR");
				if (thrusterEffectR) thrusterEffectR.gameObject.SetActive(true);
			}
		}

        public override void OnExit()
		{
			if (childLocator)
			{
				Transform thrusterEffectL = childLocator.FindChild("ThrusterEffectL");
				if (thrusterEffectL) thrusterEffectL.gameObject.SetActive(false);

				Transform thrusterEffectR = childLocator.FindChild("ThrusterEffectR");
				if (thrusterEffectR) thrusterEffectR.gameObject.SetActive(false);
			}
			base.OnExit();
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
		private ChildLocator childLocator;
		public static float hoverVelocity = -1f;
		public static float hoverAcceleration = 60f;
	}
}
