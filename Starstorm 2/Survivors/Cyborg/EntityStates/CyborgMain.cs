using RoR2;
using RoR2.UI;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine;

namespace EntityStates.SS2UStates.Cyborg
{
    public class CyborgMain : GenericCharacterMain
    {
        public static GameObject chargeRifleCrosshair;

        private float hoverVelocity = -1f;    //was -1.1
        private float hoverAcceleration = 60f;  //was 25f
        private CyborgController cyborgController;

        private Transform thrusterEffectL;
        private Transform thrusterEffectR;
        private bool inJetpackState = false;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        private EntityStateMachine jetpackStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");
            cyborgController = base.GetComponent<CyborgController>();

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

            if (chargeRifleCrosshair && base.characterBody && base.skillLocator && base.skillLocator.primary.skillDef == Starstorm2.Survivors.Cyborg.CyborgCore.chargeRifleDef)
            {
                crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, CyborgMain.chargeRifleCrosshair, CrosshairUtils.OverridePriority.Skill);
            }
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            inJetpackState = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                bool inputPressed = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                if (inputPressed && !inJetpackState && cyborgController.allowJetpack)
                {
                    this.jetpackStateMachine.SetNextState(new JetpackOn());
                }
                if (inJetpackState &&(!inputPressed || !cyborgController.allowJetpack))
                {
                    this.jetpackStateMachine.SetNextState(new Idle());
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            inJetpackState = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);

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

        public override void OnExit()
        {
            if (crosshairOverrideRequest != null) crosshairOverrideRequest.Dispose();
            base.OnExit();
        }
    }
}