using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Components.Projectiles
{
	public class ShootableProjectileComponent : MonoBehaviour
	{
		public DamageAPI.ModdedDamageType targetDamageType; //DamageType that detonates the projectile
        private bool hasFired = false;

        private void OnShootActionsInternal(DamageInfo damageInfo)
        {
            if (hasFired) return;
            hasFired = true;
            OnShootActions(damageInfo);
        }

        public virtual void OnShootActions(DamageInfo damageInfo) { }

		private void Start()
		{
			if (NetworkServer.active)
			{
				TeamComponent teamComponent = base.GetComponent<TeamComponent>();
				if (teamComponent) teamComponent.teamIndex = TeamIndex.None;
			}
		}

		internal static void AddHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += ShootableProjectileHook;
        }

        private static void ShootableProjectileHook(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active)
            {
                ShootableProjectileComponent sp = self.GetComponent<ShootableProjectileComponent>();
                if (sp)
                {
                    damageInfo.procCoefficient = 0f;
                    if (damageInfo.HasModdedDamageType(sp.targetDamageType))
                    {
                        sp.OnShootActionsInternal(damageInfo);
                    }
                    else
                    {
                        damageInfo.rejected = true;
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}
