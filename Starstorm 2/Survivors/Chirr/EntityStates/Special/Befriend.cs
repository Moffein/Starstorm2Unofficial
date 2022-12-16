using RoR2;
using RoR2.Skills;
using Starstorm2.Survivors.Chirr;
using Starstorm2.Survivors.Chirr.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Chirr.Special
{
    public class Befriend : BaseState
    {
        public static SkillDef leashOverrideSkillDef;
        public static float timeoutDuration = 10f;  //Cancels skill if it can't find friend within 10s.

        private ChirrFriendController friendController;
        private bool foundFriend = false;
        private bool appliedSkillOverride = false;
        private int origSpecialStock;
        private float origSpecialRechargeStopwatch;

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
                if (!foundFriend)
                {
                    foundFriend = friendController && friendController.HasFriend();
                    if (!appliedSkillOverride)
                    {
                        if (base.skillLocator)
                        {
                            GenericSkill specialSlot = base.skillLocator.special;
                            if (specialSlot)
                            {
                                origSpecialStock = specialSlot.stock;
                                origSpecialRechargeStopwatch = specialSlot.rechargeStopwatch;

                                appliedSkillOverride = true;
                                specialSlot.SetSkillOverride(this, leashOverrideSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                                specialSlot.stock = 1;
                            }
                        }
                    }
                }

                bool timeout = !foundFriend && base.fixedAge >= timeoutDuration;
                bool noLongerValid = foundFriend && !friendController.HasFriend();
                if (timeout || noLongerValid)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.isAuthority)
            {
                if (appliedSkillOverride)
                {
                    if (base.skillLocator)
                    {
                        GenericSkill specialSlot = base.skillLocator.special;
                        if (specialSlot)
                        {
                            specialSlot.UnsetSkillOverride(this, leashOverrideSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                            specialSlot.rechargeStopwatch = origSpecialRechargeStopwatch;
                            specialSlot.stock = origSpecialStock;
                        }
                    }
                }
            }
            base.OnExit();
        }
    }
}
