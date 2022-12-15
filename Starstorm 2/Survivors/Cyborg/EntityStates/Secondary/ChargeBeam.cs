using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class ChargeBeam : BaseState
    {
        public static string muzzleString = "Lowerarm.L_end";
        public static string fullChargeSound = "Play_item_proc_crit_cooldown";
        public static string beginChargeSound = "Play_MULT_m1_snipe_charge";
        public static string endChargeSound = "Play_MULT_m1_snipe_charge_end";
        public static GameObject chargeCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshairSmall.prefab").WaitForCompletion();
        public static GameObject chargeupVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Toolbot/SpearChargeUpVFX.prefab").WaitForCompletion();
        public static GameObject holdChargeVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/SpearChargedVFX.prefab").WaitForCompletion();

        public static GameObject fullChargeEffectPrefab;
        public static float baseDuration = 1f;
        public static float perfectChargeDuration = 0.3f;   //dont scale this with attack speed

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private float duration;
        public float charge;
        private GameObject chargeupVfxGameObject;
        private GameObject holdChargeVfxGameObject;
        private Transform muzzleTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = ChargeBeam.baseDuration / this.attackSpeedStat;
            charge = 0f;

            Util.PlaySound(ChargeBeam.beginChargeSound, base.gameObject);
            if (ChargeBeam.chargeCrosshairPrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, ChargeBeam.chargeCrosshairPrefab, CrosshairUtils.OverridePriority.Skill);
            }
            base.characterBody.SetAimTimer(3f);

            ChildLocator cl = base.GetModelChildLocator();
            if (cl)
            {
                muzzleTransform = cl.FindChild(muzzleString);
                if (muzzleTransform)
                {
                    this.chargeupVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(ChargeBeam.chargeupVfxPrefab, muzzleTransform);
                    this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;
                }
            }
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

                    if (this.chargeupVfxGameObject)
                    {
                        EntityState.Destroy(this.chargeupVfxGameObject);
                        this.chargeupVfxGameObject = null;
                    }
                    if (!this.holdChargeVfxGameObject && muzzleTransform)
                    {
                        this.holdChargeVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(ChargeBeam.holdChargeVfxPrefab, muzzleTransform);
                    }
                }
            }


            if (base.isAuthority)
            {
                if (!base.inputBank || (base.inputBank && !base.inputBank.skill2.down))
                {
                    FireBeam fireBeam = new FireBeam()
                    {
                        perfectCharge = base.fixedAge >= this.duration && base.fixedAge <= this.duration + ChargeBeam.perfectChargeDuration,
                        charge = this.charge,
                        crosshairPrefab = null
                    };
                    if (this.crosshairOverrideRequest != null)
                    {
                        fireBeam.crosshairPrefab = this.crosshairOverrideRequest.prefab;
                    }
                    this.outer.SetNextState(fireBeam);
                    return;
                }
            }

            base.characterBody.SetAimTimer(3f);
            base.characterBody.SetSpreadBloom(1f - charge, false);
        }

        public override void OnExit()
        {
            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            if (this.chargeupVfxGameObject)
            {
                EntityState.Destroy(this.chargeupVfxGameObject);
                this.chargeupVfxGameObject = null;
            }
            if (this.holdChargeVfxGameObject)
            {
                EntityState.Destroy(this.holdChargeVfxGameObject);
                this.holdChargeVfxGameObject = null;
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}