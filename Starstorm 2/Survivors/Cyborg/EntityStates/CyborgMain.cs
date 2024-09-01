using EntityStates.SS2UStates.Cyborg.Jetpack;
using RoR2;
using RoR2.UI;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
using UnityEngine;

namespace EntityStates.SS2UStates.Cyborg
{
    public class CyborgMain : GenericCharacterMain
    {
        public static GameObject chargeRifleCrosshair;

        private float hoverVelocity = -1f;    //was -1.1
        private float hoverAcceleration = 60f;  //was 25f

        private Transform thrusterEffectL;
        private Transform thrusterEffectR;
        private bool inJetpackState = false;

        private EntityStateMachine jetpackStateMachine;
        private CyborgEnergyComponent energyComponent;

        //Copied fron new artificer code
        public bool jumpButtonState;
        private bool heldPress;
        private bool jumpToggledState;
        private float oldJumpHeldTime;
        private float jumpButtonHeldTime;

        public override void OnEnter()
        {
            base.OnEnter();
            energyComponent = base.GetComponent<CyborgEnergyComponent>();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");

            ChildLocator cl = base.GetModelChildLocator();
            if (cl)
            {
                thrusterEffectL = cl.FindChild("ThrusterEffectL");
                if (thrusterEffectL)
                {
                    thrusterEffectL.gameObject.SetActive(false);
                }

                thrusterEffectR = cl.FindChild("ThrusterEffectR");
                if (thrusterEffectR)
                {
                    thrusterEffectR.gameObject.SetActive(false);
                }
            }
        }

        public override void ProcessJump()
        {
            inJetpackState = this.jetpackStateMachine.state.GetType() != typeof(Idle);

            if (this.jetpackStateMachine.state.GetType() == typeof(FlightMode)) return;

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

            inJetpackState = this.jetpackStateMachine.state.GetType() != typeof(Idle);

            //Need separate effect intensity for Jetpack/Sprint, or else it doesn't look good.
            bool shouldShowThruster = inJetpackState;//(inJetpackState || (base.characterBody && base.characterBody.isSprinting));
            if (shouldShowThruster)
            {
                if (thrusterEffectL)
                {
                    thrusterEffectL.gameObject.SetActive(true);
                }

                if (thrusterEffectR)
                {
                    thrusterEffectR.gameObject.SetActive(true);
                }
            }
            else
            {
                if (thrusterEffectL)
                {
                    thrusterEffectL.gameObject.SetActive(false);
                }

                if (thrusterEffectR)
                {
                    thrusterEffectR.gameObject.SetActive(false);
                }
            }

            // rest idle!!
            //if (this.animator) this.animator.SetBool("inCombat", (!base.characterBody.outOfCombat || !base.characterBody.outOfDanger));
        }
    }
}