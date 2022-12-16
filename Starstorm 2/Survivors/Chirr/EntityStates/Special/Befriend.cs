using RoR2;
using Starstorm2.Survivors.Chirr;
using Starstorm2.Survivors.Chirr.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Chirr.Special
{
    public class Befriend : BaseState
    {
        public static LeashSkillDef leashOverrideSkillDef;
        public static float timeoutDuration = 10f;  //Cancels skill if it can't find friend within 10s.

        private ChirrFriendController friendController;
        private bool foundFriend = false;
        private bool appliedSkillOverride = false;

        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("ChirrSpecial", base.gameObject);
            friendController = base.GetComponent<ChirrFriendController>();
            if (NetworkServer.active && friendController)
            {
                friendController.BefriendServer(base.GetTeam());
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if(base.isAuthority)
            {
                bool timeout = !foundFriend && base.fixedAge > timeoutDuration;
                bool noLongerValid = foundFriend && !friendController.HasFriend();
                if (timeout || noLongerValid)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }
    }
}
