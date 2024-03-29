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
            base.ProcessJump();
            inJetpackState = this.jetpackStateMachine.state.GetType() != typeof(Idle);

            if (this.jetpackStateMachine.state.GetType() == typeof(Idle) || this.jetpackStateMachine.state.GetType() == typeof(JetpackOn))
            {
                if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
                {
                    bool inputPressed = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded && !(energyComponent && energyComponent.energyDepleted);

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