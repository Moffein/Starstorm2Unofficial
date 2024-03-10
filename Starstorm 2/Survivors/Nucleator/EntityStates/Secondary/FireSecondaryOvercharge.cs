using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using UnityEngine;

namespace EntityStates.SS2UStates.Nucleator.Secondary
{
    public class FireSecondaryOvercharge : FireSecondary
    {
        public static GameObject overchargeEffectPrefab;
        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_voidman_m2_explode", base.gameObject);
        }

        protected override GameObject GetEffectPrefab()
        {
            return overchargeEffectPrefab;
        }

        protected override float GetDamageCoefficient()
        {
            float chargeScaled = (charge - BaseChargeState.overchargeFraction) / (1f - BaseChargeState.overchargeFraction);
            return Mathf.Lerp(8f, 12f, chargeScaled);
        }

        protected override void ModifyBulletAttack(BulletAttack ba)
        {
            ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.Root3s);
        }
    }
}
