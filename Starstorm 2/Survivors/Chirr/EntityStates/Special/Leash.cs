using RoR2;
using Starstorm2.Survivors.Chirr.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Chirr.Special
{
    public class Leash : BaseState
    {
        public static float baseDuration = 0.5f;
        public override void OnEnter()
        {
            base.OnEnter();
            if (NetworkServer.active)
            {
                ChirrFriendController friendController = base.GetComponent<ChirrFriendController>();
                if (friendController)// && friendController.CanLeash()
                {
                    friendController.LeashFriend(base.transform.position);
                }
            }
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}