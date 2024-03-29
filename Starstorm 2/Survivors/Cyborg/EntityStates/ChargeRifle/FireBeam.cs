using RoR2.UI;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
using RoR2.Skills;
using R2API;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Cyborg.ChargeRifle
{
    public class FireBeam : BaseState
    {
        public static float perfectChargeDamageMultiplier = 1.333334f;
        public static float minDamageCoefficient = 3f;
        public static float maxDamageCoefficient = 9f;
        public static float minForce = 1000f;
        public static float maxForce = 2000f;
        public static float baseDuration = 0.5f;
        public static float recoil = 4f;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject tracerEffectPrefab;
        public static GameObject perfectTracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Huntress/TracerHuntressSnipe.prefab").WaitForCompletion();
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightning.prefab").WaitForCompletion();

        public static string attackSoundString = "SS2UCyborgPrimary";
        public static string fullSoundString = "SS2UCyborgSecondary";
        public static string perfectSoundString = "SS2UCyborgUtility";

        public GameObject crosshairPrefab;
        public float charge;
        public bool perfectCharge;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private CyborgEnergyComponent chargeComponent;
        private string muzzleString;

        public int step = 0;

        private float duration;
        
        public override void OnEnter()
        {
            base.OnEnter();
            chargeComponent = base.GetComponent<CyborgEnergyComponent>();
            if (chargeComponent)
            {
                chargeComponent.rifleChargeFraction = this.charge;
                chargeComponent.riflePerfectCharge = this.perfectCharge;
            }
            duration = FireBeam.baseDuration / this.attackSpeedStat;

            //base.PlayAnimation("Gesture, Override", "FireM2", "FireArrow.playbackRate", this.duration);
            if (step == 1)
            {
                this.muzzleString = "Lowerarm.L_end";
                base.PlayCrossfade("Gesture, Override", "FireM1Alt", "FireM1.playbackRate", this.duration, 0.1f);
            }
            else
            {
                this.muzzleString = "Lowerarm.R_end";
                base.PlayCrossfade("Gesture, Override", "FireM1", "FireM1.playbackRate", this.duration, 0.1f);
            }

            if (crosshairPrefab)
            {
                this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, crosshairPrefab, CrosshairUtils.OverridePriority.Sprint);
            }

            string sound = FireBeam.attackSoundString;
            if (perfectCharge)
            {
                sound = FireBeam.perfectSoundString;
            }
            else if (charge >= 1f)
            {
                sound = FireBeam.fullSoundString;
            }
            Util.PlaySound(sound, base.gameObject);

            if (base.isAuthority)
            {
                float dmg = Mathf.Lerp(FireBeam.minDamageCoefficient, FireBeam.maxDamageCoefficient, charge) * this.damageStat * (perfectCharge ? FireBeam.perfectChargeDamageMultiplier : 1f);
                float force = Mathf.Lerp(FireBeam.minForce, FireBeam.maxForce, charge);

                Ray r = base.GetAimRay();
                BulletAttack bullet = new BulletAttack
                {
                    aimVector = r.direction,
                    origin = r.origin,
                    damage = dmg,
                    damageType = DamageType.Generic,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = force,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = Mathf.Lerp(1f, 2f, charge),
                    weapon = base.gameObject,
                    tracerEffectPrefab = perfectCharge ? FireBeam.perfectTracerEffectPrefab : FireBeam.tracerEffectPrefab,
                    hitEffectPrefab = FireBeam.hitEffectPrefab,
                    maxDistance = 1000f
                };
                bullet.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.CyborgCanDetonateShockCore);
                if (perfectCharge || charge >= 1f) bullet.stopperMask = LayerIndex.world.mask;
                bullet.Fire();
            }
            base.AddRecoil(-0.5f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            base.characterBody.AddSpreadBloom(2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge > this.duration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (this.chargeComponent)
            {
                chargeComponent.ResetCharge();
            }
            if (this.crosshairOverrideRequest != null)
            {
                this.crosshairOverrideRequest.Dispose();
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
