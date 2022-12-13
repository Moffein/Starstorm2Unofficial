using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Audio;

//TODO: should check that secondary is ion gun before attempting to store/load charges

namespace Starstorm2.Cores.States.Cyborg
{
    public class CyborgMain : GenericCharacterMain
    {
        private float hoverVelocity = -1f;    //was -1.1
        private float hoverAcceleration = 60f;  //was 25f
        private bool wasHovering = false;

        public static NetworkSoundEventDef jetpackOnNetworkSound;

        public override void OnEnter()
        {
            base.OnEnter();
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
                bool hoverInput = base.inputBank.jump.down && base.characterMotor.velocity.y < 0f && !base.characterMotor.isGrounded;

                if (base.isAuthority)
                {
                    if (hoverInput)
                    {
                        if (!wasHovering)
                        {
                            wasHovering = true;
                            if (jetpackOnNetworkSound) EntitySoundManager.EmitSoundServer(jetpackOnNetworkSound.index, base.gameObject);
                        }
                        float num = base.characterMotor.velocity.y;
                        num = Mathf.MoveTowards(num, hoverVelocity, hoverAcceleration * Time.fixedDeltaTime);
                        base.characterMotor.velocity = new Vector3(base.characterMotor.velocity.x, num, base.characterMotor.velocity.z);
                    }
                    else
                    {
                        if (wasHovering)
                        {
                            wasHovering = false;
                        }
                    }
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