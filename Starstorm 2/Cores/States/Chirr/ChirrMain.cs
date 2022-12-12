using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.CharacterAI;
using RoR2.Skills;
using EntityStates.Chirr;
using Starstorm2.Cores;
using System;
using System.Linq;
using System.Collections.Generic;
using JetBrains.Annotations;
using RoR2.Navigation;
using UnityEngine.AI;
using UnityEngine.Networking;


//TODO: should check that secondary is ion gun before attempting to store/load charges

namespace Starstorm2.Cores.States.Chirr
{
    public class ChirrMain : GenericCharacterMain
    {
        private float hoverVelocity = -1.1f;
        private float hoverAcceleration = 25f;
        private Indicator targetIndicator;
        private Indicator befriendIndicator;

        public void OnInventoryChanged()
        {
            //Chat.AddMessage("inventory changed test");
            if (characterBody.GetComponent<ChirrInfoComponent>().friend && NetworkServer.active)
            {
                CharacterBody newFriend = characterBody.GetComponent<ChirrInfoComponent>().friend;
                newFriend.master.inventory.CopyItemsFrom(characterBody.GetComponent<ChirrInfoComponent>().baseInventory);
                newFriend.master.inventory.AddItemsFrom(base.characterBody.inventory);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.WardOnLevel.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.BeetleGland.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.CrippleWardOnLevel.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.TPHealingNova.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.FocusConvergence.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.TitanGoldDuringTP.itemIndex);
                newFriend.master.inventory.ResetItem(RoR2Content.Items.ExtraLife.itemIndex);
            }
        }

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
            On.RoR2.Stage.Start += (orig, self) =>
            {
                orig(self);
                characterBody.GetComponent<ChirrInfoComponent>().friend = null;
                characterBody.GetComponent<ChirrInfoComponent>().friendState = false;
            };
            On.RoR2.PingerController.SetCurrentPing += PingerController_SetCurrentPing;
            this.characterBody.master.inventory.onInventoryChanged += OnInventoryChanged;

            this.targetIndicator = new Indicator(base.gameObject, null);
            this.targetIndicator.visualizerPrefab = ChirrCore.chirrTargetIndicator;
            this.befriendIndicator = new Indicator(base.gameObject, null);
            this.befriendIndicator.visualizerPrefab = ChirrCore.chirrBefriendIndicator;
            /*base.characterBody.master.OnInventoryChanged += (orig, self) =>
            {
                Chat.AddMessage("inventory changed test");
                if (characterBody.GetComponent<ChirrInfoComponent>().friend)
                {
                    CharacterBody newFriend = characterBody.GetComponent<ChirrInfoComponent>().friend;
                    newFriend.master.inventory.CopyItemsFrom(base.characterBody.inventory);
                    newFriend.master.inventory.ResetItem(ItemIndex.WardOnLevel);
                    newFriend.master.inventory.ResetItem(ItemIndex.BeetleGland);
                    newFriend.master.inventory.ResetItem(ItemIndex.CrippleWardOnLevel);
                    newFriend.master.inventory.ResetItem(ItemIndex.TPHealingNova);
                    newFriend.master.inventory.ResetItem(ItemIndex.FocusConvergence);
                    newFriend.master.inventory.ResetItem(ItemIndex.TitanGoldDuringTP);
                }
            };*/
        }

        public override void OnExit()
        {
            On.RoR2.PingerController.SetCurrentPing -= PingerController_SetCurrentPing;
            this.characterBody.master.inventory.onInventoryChanged -= OnInventoryChanged;

            this.targetIndicator.active = false;

            base.OnExit();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                bool hoverInput = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;

                if (base.isAuthority && hoverInput)
                {
                    float num = base.characterMotor.velocity.y;
                    num = Mathf.MoveTowards(num, hoverVelocity, hoverAcceleration * Time.fixedDeltaTime);
                    base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
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

            // rest idle!!
            //if (this.animator) this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));
        }

        public override void Update()
        {
            base.Update();

            /*if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Starstorm.restKeybind))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Emotes.RestEmote))), InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Starstorm.tauntKeybind))
                {
                    this.outer.SetInterruptState(EntityState.Instantiate(new SerializableEntityStateType(typeof(Emotes.TauntEmote))), InterruptPriority.Any);
                    return;
                }
            }*/
            // I ll need those eventually
        }
    }
}