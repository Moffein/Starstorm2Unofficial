using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.AddressableAssets;
using Starstorm2Unofficial.Survivors.Cyborg.Components;

namespace EntityStates.SS2UStates.Cyborg.Jetpack
{
	public class JetpackOn : BaseState
	{
		public static float hoverDuration = 30f;

		private CyborgEnergyComponent energyComponent;
        public override void OnEnter()
        {
            base.OnEnter();
			Util.PlaySound("Play_mage_m1_impact", base.gameObject);
			Util.PlaySound("Play_engi_sprint_start", base.gameObject);

			energyComponent = base.GetComponent<CyborgEnergyComponent>();
			if (energyComponent)
			{
				energyComponent.energySkillsActive++;
			}

			if (JetpackOn.activationEffectPrefab)
			{
				ChildLocator cl = base.GetModelChildLocator();
				if (cl)
				{
					Transform thrusterEffectL = cl.FindChild("ThrusterEffectL");
					if (thrusterEffectL)
					{
						EffectManager.SpawnEffect(activationEffectPrefab, new EffectData
						{
							scale = 0.2f,
							origin = thrusterEffectL.transform.position,
							rotation = thrusterEffectL.transform.rotation,
							rootObject = thrusterEffectL.gameObject
						}, false);
					}

					Transform thrusterEffectR = cl.FindChild("ThrusterEffectR");
					if (thrusterEffectR)
					{
						EffectManager.SpawnEffect(activationEffectPrefab, new EffectData
						{
							scale = 0.2f,
							origin = thrusterEffectR.transform.position,
							rotation = thrusterEffectR.transform.rotation,
							rootObject = thrusterEffectR.gameObject
						}, false);
					}
				}
			}
		}

        public override void OnExit()
		{
			Util.PlaySound("Play_engi_sprint_end", base.gameObject);
            if (energyComponent)
            {
                energyComponent.energySkillsActive--;
            }
            base.OnExit();
        }

        public override void FixedUpdate()
		{
			base.FixedUpdate();

			if (energyComponent)
			{
				energyComponent.ConsumeEnergy(Time.fixedDeltaTime / hoverDuration);
			}

			if (base.isAuthority)
			{
				float num = base.characterMotor.velocity.y;
				num = Mathf.MoveTowards(num, JetpackOn.hoverVelocity, JetpackOn.hoverAcceleration * Time.fixedDeltaTime);
				base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
			}
		}
		public static float hoverVelocity = -1f;
		public static float hoverAcceleration = 60f;
		public static GameObject activationEffectPrefab;
	}
}
