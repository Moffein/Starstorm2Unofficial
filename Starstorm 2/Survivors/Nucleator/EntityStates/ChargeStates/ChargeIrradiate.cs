using EntityStates;
using RoR2;

namespace Starstorm2Unofficial.Cores.States.Nucleator
{
    class ChargeIrradiate : NucleatorSkillStateBase
    {
        private uint chargePlayID;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Gesture, Override", "PrimaryCharge", "Primary.playbackRate", 0.8f * this.maxChargeTime);
            this.chargePlayID = Util.PlaySound("NucleatorChargePrimary", this.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(this.chargePlayID);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.maxChargeTime || !base.inputBank || !base.inputBank.skill1.down) && base.isAuthority)
            {
                FireIrradiate fireIrradiate = new FireIrradiate();
                fireIrradiate.charge = this.charge;

                this.outer.SetNextState(fireIrradiate);
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}