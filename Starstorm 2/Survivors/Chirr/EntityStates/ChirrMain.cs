using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.CharacterAI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using Starstorm2.Survivors.Chirr.Components;
using Starstorm2.Survivors.Chirr;


//TODO: should check that secondary is ion gun before attempting to store/load charges

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrMain : GenericCharacterMain
    {
        private float hoverVelocity = -1f;
        private float hoverAcceleration = 60f;
        private Indicator targetIndicator;
        private Indicator befriendIndicator;

        private bool inJetpackState = false;
        private EntityStateMachine jetpackStateMachine;

        public void PingerController_SetCurrentPing(On.RoR2.PingerController.orig_SetCurrentPing orig, PingerController self, PingerController.PingInfo ping)
        {
            if (ping.targetGameObject)
            {
                if (ping.targetGameObject.GetComponent<CharacterBody>() && characterBody.GetComponent<ChirrInfoComponent>().friend)
                {
                    BaseAI friendAI = characterBody.GetComponent<ChirrInfoComponent>().friend.master.GetComponent<BaseAI>();
                    friendAI.currentEnemy.gameObject = ping.targetGameObject.GetComponent<CharacterBody>().gameObject;
                    characterBody.GetComponent<ChirrInfoComponent>().pingTarget = ping.targetGameObject.GetComponent<CharacterBody>();
                    //Chat.AddMessage("Now targeting :" + ping.targetGameObject.GetComponent<CharacterBody>().name);
                    friendAI.UpdateTargets();
                }
                //Chat.AddMessage(ping.targetGameObject.ToString());
                //Chat.AddMessage(ping.targetGameObject.GetComponent<CharacterBody>().name);
            }
            orig(self, ping);
        }

        public HurtBox GetBefriendTarget()
        {
            HurtBox futureFriend = null;
            if (!characterBody.GetComponent<ChirrInfoComponent>().friend)
            {
                //base.characterBody.AddSpreadBloom(0.75f);
                Ray aimRay = base.GetAimRay();
                Vector3 targetV;
                BullseyeSearch bullseyeSearch = new BullseyeSearch();
                TeamComponent team = base.characterBody.GetComponent<TeamComponent>();
                bullseyeSearch.teamMaskFilter = TeamMask.all;
                bullseyeSearch.teamMaskFilter.RemoveTeam(team.teamIndex);
                bullseyeSearch.filterByLoS = true;
                //bullseyeSearch.filterByDistinctEntity = true;
                bullseyeSearch.searchOrigin = aimRay.origin;
                bullseyeSearch.searchDirection = aimRay.direction;
                bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                bullseyeSearch.maxDistanceFilter = 10000;
                bullseyeSearch.maxAngleFilter = 30f;
                bullseyeSearch.RefreshCandidates();
                targetV = aimRay.direction;
                List<HurtBox> targets = bullseyeSearch.GetResults().ToList();
                foreach (HurtBox target in targets)
                {
                    if (!target.healthComponent.body.master.isBoss // boss check
                       && (target.healthComponent.combinedHealthFraction < .5f + (this.characterBody.executeEliteHealthFraction / 2f)) // hp check
                       && !(target.healthComponent.body.master.GetComponent<PlayerCharacterMasterController>())) // player check
                    {
                        futureFriend = target;
                        break;
                    }
                }
                if (futureFriend)
                {
                    this.befriendIndicator.targetTransform = futureFriend.transform;
                    this.befriendIndicator.active = true;
                }
                else
                {
                    this.befriendIndicator.targetTransform = (null);
                    this.befriendIndicator.active = false;
                }
            }
            else
            {
                this.befriendIndicator.targetTransform = (null);
                this.befriendIndicator.active = false;
            }
            return futureFriend;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");
            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                characterBody.GetComponent<ChirrInfoComponent>().friend = null;
                characterBody.GetComponent<ChirrInfoComponent>().friendState = false;
            };
            On.RoR2.PingerController.SetCurrentPing += PingerController_SetCurrentPing;

            this.targetIndicator = new Indicator(base.gameObject, null);
            this.targetIndicator.visualizerPrefab = ChirrCore.chirrTargetIndicator;
            this.befriendIndicator = new Indicator(base.gameObject, null);
            this.befriendIndicator.visualizerPrefab = ChirrCore.chirrBefriendIndicator;
        }

        public override void OnExit()
        {
            On.RoR2.PingerController.SetCurrentPing -= PingerController_SetCurrentPing;

            this.targetIndicator.active = false;

            base.OnExit();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            inJetpackState = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                bool inputPressed = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                if (inputPressed && !inJetpackState)
                {
                    this.jetpackStateMachine.SetNextState(new JetpackOn());
                }
                if (inJetpackState && !inputPressed)
                {
                    this.jetpackStateMachine.SetNextState(new Idle());
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            //if (characterBody.GetComponent<ChirrInfoComponent>().friend)
            //    Chat.AddMessage(characterBody.GetComponent<ChirrInfoComponent>().friend.ToString());
            if (!characterBody.GetComponent<ChirrInfoComponent>().friendState)
            {
                if (characterBody.GetComponent<ChirrInfoComponent>().friend)
                {
                    //outer.SetNextStateToMain();
                    //base.skillLocator.special.SetSkillOverride(base.skillLocator.special, ChirrCore.specialDef2, GenericSkill.SkillOverridePriority.Replacement);
                    //Chat.AddMessage("Tried changing skill");
                    base.skillLocator.special.SetBaseSkill(ChirrCore.specialDef2);
                    characterBody.GetComponent<ChirrInfoComponent>().friendState = true;
                }
            }
            else
            {
                if (!characterBody.GetComponent<ChirrInfoComponent>().friend)
                {
                    base.skillLocator.special.SetBaseSkill(ChirrCore.specialDef1);
                    characterBody.GetComponent<ChirrInfoComponent>().friendState = false;
                    characterBody.GetComponent<ChirrInfoComponent>().pingTarget = null;
                    characterBody.GetComponent<ChirrInfoComponent>().mouseTarget = null;
                }
            }
            if (characterBody.GetComponent<ChirrInfoComponent>().friend) // NetworkServer.active
            {
                if (characterBody.GetComponent<ChirrInfoComponent>().futureFriend)
                {
                    characterBody.GetComponent<ChirrInfoComponent>().futureFriend = null;
                    this.befriendIndicator.targetTransform = (null);
                    this.befriendIndicator.active = false;
                }
                BaseAI friendAI = characterBody.GetComponent<ChirrInfoComponent>().friend.master.GetComponent<BaseAI>();
                if (characterBody.GetComponent<ChirrInfoComponent>().pingTarget)
                {
                    /*if (NetworkServer.active)
                    {
                        friendAI.currentEnemy.gameObject = characterBody.GetComponent<ChirrInfoComponent>().pingTarget.gameObject;
                        friendAI.UpdateTargets();
                    }*/
                    //characterBody.GetComponent<ChirrInfoComponent>().CmdForceTarget(characterBody.GetComponent<ChirrInfoComponent>().pingTarget);
                    //Chat.AddMessage("targetp: " + characterBody.GetComponent<ChirrInfoComponent>().pingTarget.gameObject.ToString());
                    this.targetIndicator.active = true;
                    this.targetIndicator.targetTransform = (friendAI.currentEnemy.characterBody.mainHurtBox.transform);
                }
                else if (characterBody.GetComponent<ChirrInfoComponent>().mouseTarget)
                {
                    if (true) //NetworkServer.active
                    {
                        friendAI.currentEnemy.gameObject = characterBody.GetComponent<ChirrInfoComponent>().mouseTarget.gameObject;
                        friendAI.UpdateTargets();
                        //Chat.AddMessage("targetserver: " + characterBody.GetComponent<ChirrInfoComponent>().mouseTarget.gameObject.ToString());
                    }
                    //Chat.AddMessage("target: " + characterBody.GetComponent<ChirrInfoComponent>().mouseTarget.gameObject.ToString());
                    this.targetIndicator.active = true;
                    this.targetIndicator.targetTransform = (friendAI.currentEnemy.characterBody.mainHurtBox.transform);
                }
                else
                    this.targetIndicator.targetTransform = (null);
                if (!inputBank.skill4.down)
                    characterBody.GetComponent<ChirrInfoComponent>().sharing = false;
                else
                    characterBody.GetComponent<ChirrInfoComponent>().sharing = true;
                if (inputBank.skill1.justPressed)
                {
                    Ray aimRay = base.GetAimRay();
                    BullseyeSearch bullseyeSearch = new BullseyeSearch();
                    bullseyeSearch.teamMaskFilter = TeamMask.all;
                    bullseyeSearch.teamMaskFilter.RemoveTeam(base.characterBody.teamComponent.teamIndex);
                    bullseyeSearch.filterByLoS = true;
                    //bullseyeSearch.filterByDistinctEntity = true;
                    bullseyeSearch.searchOrigin = aimRay.origin;
                    bullseyeSearch.searchDirection = aimRay.direction;
                    bullseyeSearch.sortMode = BullseyeSearch.SortMode.Angle;
                    bullseyeSearch.maxDistanceFilter = 10000;
                    bullseyeSearch.maxAngleFilter = 10f;
                    bullseyeSearch.RefreshCandidates();
                    HurtBox target = bullseyeSearch.GetResults().FirstOrDefault<HurtBox>();
                    if (target)
                        characterBody.GetComponent<ChirrInfoComponent>().mouseTarget = target.healthComponent.body;
                }
            }
            else
            {
                this.targetIndicator.targetTransform = (null);
                this.targetIndicator.active = false;
                characterBody.GetComponent<ChirrInfoComponent>().futureFriend = GetBefriendTarget();
            }
        }
    }
}