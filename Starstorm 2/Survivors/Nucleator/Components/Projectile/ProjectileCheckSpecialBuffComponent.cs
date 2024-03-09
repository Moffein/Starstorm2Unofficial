using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Projectile
{
    //This is used to prevent scenarios where projectiles dont apply poison if special ends before they hit.
    [RequireComponent(typeof(ProjectileController), typeof(ProjectileDamage))]
    public class ProjectileCheckSpecialBuffComponent : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkServer.active)
            {
                ProjectileController pc = base.GetComponent<ProjectileController>();
                if (pc.owner)
                {
                    CharacterBody ownerBody = pc.owner.GetComponent<CharacterBody>();
                    if (ownerBody && ownerBody.HasBuff(BuffCore.nucleatorSpecialBuff))
                    {
                        ProjectileDamage pd = base.GetComponent<ProjectileDamage>();
                        if (pd) pd.damageType |= DamageType.PoisonOnHit;
                    }
                }
            }
            Destroy(this);
        }
    }
}
