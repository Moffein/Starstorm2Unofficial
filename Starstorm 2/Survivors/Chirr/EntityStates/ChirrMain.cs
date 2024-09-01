using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.CharacterAI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Networking;
using Starstorm2Unofficial.Survivors.Chirr;
using Starstorm2Unofficial.Survivors.Chirr.Components;
using EntityStates.SS2UStates.Chirr.Taunt;


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

        //Copied fron new artificer code
        public bool jumpButtonState;
        private bool heldPress;
        private bool jumpToggledState;
        private float oldJumpHeldTime;
        private float jumpButtonHeldTime;

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");

            friendController = base.GetComponent<ChirrFriendController>();
            /*if (NetworkServer.active && friendController)
            {
                friendController.TrySpawnSavedMaster();
            }*/

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
            inJetpackState = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                NetworkUser networkUser = NetworkUser.readOnlyLocalPlayersList[0];
                bool? flag;
                if (networkUser == null)
                {
                    flag = null;
                }
                else
                {
                    LocalUser localUser = networkUser.localUser;
                    flag = ((localUser != null) ? new bool?(localUser.userProfile.toggleArtificerHover) : null);
                }
                if (flag ?? true)
                {
                    if (base.inputBank.jump.down)
                    {
                        this.oldJumpHeldTime = this.jumpButtonHeldTime;
                        this.jumpButtonHeldTime += Time.deltaTime;
                        this.heldPress = (this.oldJumpHeldTime < 0.5f && this.jumpButtonHeldTime >= 0.5f);
                    }
                    else
                    {
                        this.oldJumpHeldTime = 0f;
                        this.jumpButtonHeldTime = 0f;
                        this.heldPress = false;
                    }
                    if (!base.characterMotor.isGrounded)
                    {
                        if (base.characterMotor.jumpCount == base.characterBody.maxJumpCount)
                        {
                            if (base.inputBank.jump.justPressed)
                            {
                                this.jumpButtonState = !this.jumpButtonState;
                            }
                        }
                        else if (this.heldPress)
                        {
                            this.jumpButtonState = !this.jumpButtonState;
                        }
                    }
                    else
                    {
                        this.jumpButtonState = false;
                    }
                }
                else
                {
                    this.jumpButtonState = base.inputBank.jump.down;
                }
                bool shouldJetpack = this.jumpButtonState && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                if (shouldJetpack && !inJetpackState)
                {
                    this.jetpackStateMachine.SetNextState(new JetpackOn());
                }
                if (!shouldJetpack && inJetpackState)
                {
                    this.jetpackStateMachine.SetNextState(new Idle());
                }
            }
            base.ProcessJump();
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
                if (friendController && base.skillLocator && base.skillLocator.special && (base.skillLocator.special.skillDef == ChirrCore.specialScepterDef || friendController.HasScepter()))
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
                if (Starstorm2Unofficial.Modules.Config.GetKeyPressed(Starstorm2Unofficial.Modules.Config.RestKeybind))
                {
                    this.outer.SetInterruptState(new ChirrRestEmote(), InterruptPriority.Any);
                    return;
                }
                else if (Starstorm2Unofficial.Modules.Config.GetKeyPressed(Starstorm2Unofficial.Modules.Config.TauntKeybind))
                {
                    this.outer.SetInterruptState(new ChirrTauntLoopEmote(), InterruptPriority.Any);
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