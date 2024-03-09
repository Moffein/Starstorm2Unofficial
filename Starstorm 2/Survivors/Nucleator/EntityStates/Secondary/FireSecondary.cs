namespace EntityStates.SS2UStates.Nucleator.Secondary
{
    public class FireSecondary : BaseState
    {
        public static float baseDuration = 0.4f;

        public float charge;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;// this.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "SecondaryRelease", "Secondary.playbackRate", duration * 2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
