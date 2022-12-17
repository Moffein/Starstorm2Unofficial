using RoR2;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.Networking;

//Fixes the case where you have a minion without being in the Befriend state
namespace Starstorm2.Survivors.Chirr.Components
{
    public class ChirrLeashSkillOverrideController : MonoBehaviour
    {
        private ChirrFriendController friendController;
        private GenericSkill specialSlot;
        private EntityStateMachine befriendMachine;

        private bool appliedSkillOverride = false;

        public void Awake()
        {
            friendController = base.GetComponent<ChirrFriendController>();
            SkillLocator skillLocator = base.GetComponent<SkillLocator>();
            if (skillLocator && skillLocator.special) specialSlot = skillLocator.special;
            befriendMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Befriend");
            if (!befriendMachine || !friendController || !specialSlot)
            {
                Destroy(this);
                return;
            }
        }

        public void FixedUpdate()
        {
            if (!appliedSkillOverride)
            {
                if (friendController.HasFriend() && befriendMachine.state.GetType() == typeof(EntityStates.Idle))
                {
                    appliedSkillOverride = true;
                    specialSlot.SetSkillOverride(this, EntityStates.SS2UStates.Chirr.Special.Befriend.leashOverrideSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                    specialSlot.stock = 1;
                }
            }
            else
            {
                if (!friendController.HasFriend() && befriendMachine.state.GetType() == typeof(EntityStates.Idle))
                {
                    appliedSkillOverride = false;
                    specialSlot.UnsetSkillOverride(this, EntityStates.SS2UStates.Chirr.Special.Befriend.leashOverrideSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
        }
    }
}
