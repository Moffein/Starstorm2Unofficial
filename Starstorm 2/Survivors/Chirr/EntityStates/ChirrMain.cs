using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.CharacterAI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using Starstorm2Unofficial.Survivors.Chirr;
using Starstorm2Unofficial.Survivors.Chirr.Components;


//TODO: should check that secondary is ion gun before attempting to store/load charges

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrMain : GenericCharacterMain
    {
        public static string wingSoundStart = "SS2UChirrSprintStart";
        public static string wingSoundLoop = "SS2UChirrSprintLoop";
        public static string wingSoundStop  = "SS2UChirrSprintStop";

        private ChirrFriendController friendController;
        private uint wingSoundID;
        private bool playingWingSound = false;
        private bool inJetpackState = false;
        private EntityStateMachine jetpackStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");

            friendController = base.GetComponent<ChirrFriendController>();
            if (NetworkServer.active && friendController)
            {
                friendController.TryGetSavedMaster();
            }

            //Set ending text.
            //Very bad way to do this, this is a mess.
            if (base.characterBody && base.isAuthority)
            {
                CharacterMaster ownerMaster = base.characterBody.master;
                if (ownerMaster && ownerMaster.loadout != null)
                {
                    int skinIndex = (int)ownerMaster.loadout.bodyLoadoutManager.GetSkinIndex(ChirrCore.bodyIndex);
                    SkinDef equippedSkin = HG.ArrayUtils.GetSafe<SkinDef>(BodyCatalog.GetBodySkins(ChirrCore.bodyIndex), skinIndex);
                    bool isMaid = equippedSkin == ChirrSkins.maidSkin;

                    if (ChirrCore.survivorDef && ChirrCore.survivorDef.outroFlavorToken != "SS2UCHIRR_OUTRO_BROTHER_EASTEREGG")
                    {
                        ChirrCore.survivorDef.outroFlavorToken = "SS2UCHIRR_OUTRO_FLAVOR";
                        if (base.isAuthority && isMaid) ChirrCore.survivorDef.outroFlavorToken = "SS2UCHIRR_OUTRO_MAID_FLAVOR";
                    }
                }
            }
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

            inJetpackState = this.jetpackStateMachine && this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
            bool shouldPlayWingSound = inJetpackState;//base.characterBody.isSprinting || 
            if (shouldPlayWingSound != playingWingSound)
            {
                if (!playingWingSound)
                {
                    playingWingSound = true;
                    //Util.PlaySound(wingSoundStart, base.gameObject);
                    wingSoundID = Util.PlaySound(wingSoundLoop, base.gameObject);
                }
                else
                {
                    playingWingSound = false;
                    AkSoundEngine.StopPlayingID(this.wingSoundID);
                    Util.PlaySound(wingSoundStop, base.gameObject);
                }
            }

            //Technically don't need a network check.
            if (NetworkServer.active)
            {
                if (friendController && base.skillLocator && base.skillLocator.special && base.skillLocator.special.skillDef == ChirrCore.specialScepterDef)
                {
                    friendController.canBefriendChampion = true;
                }

                //Dont set it to false, in case Mithrix steals scepter (I think it's already blacklisted from stealing)
                /*else
                {
                    friendController.canBefriendChampion = false;
                }*/
            }
        }

        public override void Update()
        {
            base.Update();

            if (base.isAuthority && base.characterMotor.isGrounded)
            {
                if (Input.GetKeyDown(Starstorm2Unofficial.Modules.Config.restKeybind))
                {
                    this.outer.SetInterruptState(new EntityStates.SS2UStates.Common.Emotes.RestEmote() { animDuration = 8f }, InterruptPriority.Any);
                    return;
                }
                else if (Input.GetKeyDown(Starstorm2Unofficial.Modules.Config.tauntKeybind))
                {
                    this.outer.SetInterruptState(new EntityStates.SS2UStates.Common.Emotes.TauntEmote() { duration = 0f }, InterruptPriority.Any);
                    return;
                }
            }
        }

        public override void OnExit()
        {
            Util.PlaySound(wingSoundStop, base.gameObject);
            base.OnExit();
        }
    }
}