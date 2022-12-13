using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Audio;
using Starstorm2.Components;

//TODO: should check that secondary is ion gun before attempting to store/load charges

namespace Starstorm2.Cores.States.Cyborg
{
    public class CyborgMain : GenericCharacterMain
    {
        private float hoverVelocity = -1f;    //was -1.1
        private float hoverAcceleration = 60f;  //was 25f
        private CyborgController cyborgController;

        private EntityStateMachine jetpackStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();
            jetpackStateMachine = EntityStateMachine.FindByCustomName(base.gameObject, "Jetpack");
            cyborgController = base.GetComponent<CyborgController>();
        }

        public override void OnExit()
        {
            
            base.OnExit();
        }

        public override void ProcessJump()
        {
            base.ProcessJump();
            if (this.hasCharacterMotor && this.hasInputBank && base.isAuthority)
            {
                bool inputPressed = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;
                bool inJetpackState = this.jetpackStateMachine.state.GetType() == typeof(JetpackOn);
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