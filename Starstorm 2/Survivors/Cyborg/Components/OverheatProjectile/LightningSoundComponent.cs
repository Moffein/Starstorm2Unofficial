using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;
using System.Linq;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components.OverheatProjectile
{
    [RequireComponent(typeof(ProjectileProximityBeamController))]
    public class LightningSoundComponent : MonoBehaviour
    {
        public static NetworkSoundEventDef lightningSound;
        private ProjectileProximityBeamController pbc;

        private void Start()
        {
            pbc = base.GetComponent<ProjectileProximityBeamController>();
            if (!pbc) Destroy(this);
        }

        private void FixedUpdate()
        {
            if (NetworkServer.active)
            {
                if (pbc.listClearTimer - Time.fixedDeltaTime <= 0f && HasTarget(pbc))
                {
                    EffectManager.SimpleSoundEffect(lightningSound.index, base.transform.position, true);
                }
            }
        }

        //PBC FindNextTarget filters out already-hit hurtboxes
        private bool HasTarget(ProjectileProximityBeamController pbc)
        {
            BullseyeSearch search = pbc.search;
            Vector3 position = base.transform.position;
            Vector3 forward = base.transform.forward;
            search.searchOrigin = position;
            search.searchDirection = forward;
            search.sortMode = BullseyeSearch.SortMode.Distance;
            search.teamMaskFilter = TeamMask.allButNeutral;
            search.teamMaskFilter.RemoveTeam(pbc.myTeamIndex);
            search.filterByLoS = false;
            search.minAngleFilter = pbc.minAngleFilter;
            search.maxAngleFilter = pbc.maxAngleFilter;
            search.maxDistanceFilter = pbc.attackRange;
            search.RefreshCandidates();
            return search.GetResults().FirstOrDefault() != default;
        }
    }
}
