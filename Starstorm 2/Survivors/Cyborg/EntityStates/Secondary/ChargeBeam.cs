using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class ChargeBeam : BaseState
    {
        public static string muzzleString = "Lowerarm.L_end";
        public static string fullChargeSound;
        public static string beginChargeSound = "Play_MULT_m1_snipe_charge";
        public static string endChargeSound = "Play_MULT_m1_snipe_charge_end";
        public static GameObject chargeCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/TiltedBracketCrosshair.prefab").WaitForCompletion();
        public static GameObject perfectCrosshairPrefabPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/TiltedBracketCrosshair.prefab").WaitForCompletion();

        public static GameObject fullChargeEffectPrefab;
        public static float baseDuration = 1f;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private float duration;
        public float charge;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = ChargeBeam.baseDuration / this.attackSpeedStat;
            charge = 0f;

            Util.PlaySound(ChargeBeam.beginChargeSound, base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (charge < 1f)
            {
                charge = base.fixedAge / duration;
                if (charge >= 1f)
                {
                    Util.PlaySound(ChargeBeam.endChargeSound, base.gameObject);
                    if (base.isAuthority) Util.PlaySound(ChargeBeam.fullChargeSound, base.gameObject);
                }
            }

            if (base.isAuthority)
            {
                if (!base.inputBank || (base.inputBank && !base.inputBank.skill2.down))
                {
                    FireBeam fireBeam = new FireBeam()
                    {
                        charge = this.charge
                    };
                    this.outer.SetNextState(fireBeam);
                    return;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}