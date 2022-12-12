using EntityStates;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando.ScepterBarrage
{
    public class ScepterBarrageCharge : BaseSkillState
    {
        public static float baseChargeDuration = 1.5f;

        private float chargeDuration;
        private ChildLocator childLocator;
        private Animator animator;
        private Transform modelBaseTransform;
        private uint chargePlayID;
        private bool hasFinishedCharging;
        private GameObject chargeEffect;

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = ScepterBarrageCharge.baseChargeDuration;// / this.attackSpeedStat;
            this.childLocator = base.GetModelChildLocator();
            this.modelBaseTransform = base.GetModelBaseTransform();
            this.animator = base.GetModelAnimator();
            this.hasFinishedCharging = false;

            this.chargePlayID = Util.PlaySound("NemmandoSubmissionCharge", base.gameObject);

            this.chargeEffect = this.childLocator.FindChild("GunChargeEffect").gameObject;

            if (this.chargeEffect) this.chargeEffect.SetActive(true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float charge = this.CalcCharge();

            if (charge >= 1f)
            {
                if (!this.hasFinishedCharging)
                {
                    this.hasFinishedCharging = true;
                    Util.PlaySound("NemmandoSubmissionReady", base.gameObject);
                }
            }

            if (base.isAuthority && ((!base.IsKeyDownAuthority() && base.fixedAge >= 0.1f)))
            {
                ScepterBarrageFire nextState = new ScepterBarrageFire();
                nextState.charge = charge;
                this.outer.SetNextState(nextState);
            }
        }

        protected float CalcCharge()
        {
            return Mathf.Clamp01(base.fixedAge / this.chargeDuration);
        }

        public override void OnExit()
        {
            AkSoundEngine.StopPlayingID(this.chargePlayID);
            base.OnExit();

            if (this.chargeEffect) this.chargeEffect.SetActive(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}