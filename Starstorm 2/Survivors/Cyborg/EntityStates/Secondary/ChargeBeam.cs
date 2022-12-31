using RoR2;
using RoR2.UI;
using Starstorm2.Survivors.Cyborg.Components;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class ChargeBeam : BaseState
    {
        public static string muzzleString = "Lowerarm.L_end";
        public static string fullChargeSound = "CyborgPerfectCharge";
        public static string beginChargeSound = "Play_mage_m2_charge";
        public static GameObject chargeupVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/ChargeLoaderFist.prefab").WaitForCompletion();
        public static GameObject holdChargeVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/SpearChargedVFX.prefab").WaitForCompletion();

        public static GameObject fullChargeEffectPrefab;
        public static float baseDuration = 1f;
        public static float perfectChargeDuration = 0.3f;   //dont scale this with attack speed

        private uint loopSoundInstanceId = uint.MaxValue;
        private float duration;
        public float charge;
        private GameObject chargeupVfxGameObject;
        private GameObject holdChargeVfxGameObject;
        private Transform muzzleTransform;
        private bool setNextState = false;
        private CyborgChargeComponent chargeComponent;
        private bool isAutoFire = false;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = ChargeBeam.baseDuration / this.attackSpeedStat;
            charge = 0f;

            this.loopSoundInstanceId = Util.PlayAttackSpeedSound(ChargeBeam.beginChargeSound, base.gameObject, this.attackSpeedStat);
            base.characterBody.SetAimTimer(3f);

            chargeComponent = base.GetComponent<CyborgChargeComponent>();

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
                    if (loopSoundInstanceId != uint.MaxValue)
                    {
                        AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);
                        loopSoundInstanceId = uint.MaxValue;
                    }
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

            bool perfectCharge = base.fixedAge >= this.duration && base.fixedAge <= this.duration + ChargeBeam.perfectChargeDuration;
            if (this.chargeComponent)
            {
                chargeComponent.chargeFraction = charge;
                chargeComponent.perfectCharge = perfectCharge;
            }

            if (base.isAuthority)
            {
                if (isAutoFire || !(base.inputBank && base.inputBank.skill1.down))
                {
                    FireBeam fireBeam = new FireBeam()
                    {
                        perfectCharge = perfectCharge,
                        charge = this.charge
                    };
                    this.outer.SetNextState(fireBeam);
                    return;
                }
            }

            base.characterBody.SetAimTimer(3f);
            base.characterBody.SetSpreadBloom(1f - charge, false);
        }

        public override void OnExit()
        {
            if (loopSoundInstanceId != uint.MaxValue) AkSoundEngine.StopPlayingID(this.loopSoundInstanceId);
            if (chargeComponent)
            {
                chargeComponent.ResetCharge();
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
            return InterruptPriority.Skill;
        }
    }
}