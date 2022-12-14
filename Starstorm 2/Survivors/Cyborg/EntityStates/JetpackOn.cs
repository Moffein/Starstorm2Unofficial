using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.AddressableAssets;

namespace EntityStates.Starstorm2States.Cyborg
{
	public class JetpackOn : BaseState
	{
        public override void OnEnter()
        {
            base.OnEnter();
			Util.PlaySound("Play_mage_m1_impact", base.gameObject);

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
							scale = 0.25f,
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
							scale = 0.25f,
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
		public static float hoverVelocity = -1f;
		public static float hoverAcceleration = 60f;
		public static GameObject activationEffectPrefab;// Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Engi/MuzzleflashEngiTurret.prefab").WaitForCompletion();	//Too big, need to find suitable VFX.
	}
}
