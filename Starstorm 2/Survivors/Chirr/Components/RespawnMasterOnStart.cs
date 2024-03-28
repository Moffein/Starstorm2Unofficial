using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Chirr.Components
{
    //Jank to get around nullrefs
    public class RespawnMasterOnStart : MonoBehaviour
    {
        private void Start()
        {
            CharacterBody body = base.GetComponent<CharacterBody>();
            if (body.master)
            {
                body.master.Respawn(body.footPosition, base.transform.rotation);
                Destroy(this);
            }
        }
    }
}
