using RoR2;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Chirr.Special
{
    public class Leash : BaseState
    {
        public static float baseDuration = 1f;

        private ChirrFriendController friendController;
        private bool leashed = false;
        public override void OnEnter()
        {
            base.OnEnter();
            friendController = base.GetComponent<ChirrFriendController>();
        }


        public override void FixedUpdate()
        {
            base.FixedUpdate();


            if (base.isAuthority)
            {
                if (!base.inputBank.skill4.down)
                {
                    if (!leashed && base.fixedAge <= baseDuration)
                    {
                        leashed = true;
                        if (friendController)// && friendController.CanLeash()
                        {
                            friendController.LeashFriendClient(base.transform.position);
                        }
                    }
                }

                if (base.fixedAge >= Leash.baseDuration)
                {
                    if (!leashed)
                    {
                        friendController.RemoveFriendClient();
                    }

                    this.outer.SetNextStateToMain();
                    return;
                }
            }
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