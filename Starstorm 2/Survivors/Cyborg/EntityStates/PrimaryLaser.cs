using R2API;
using RoR2;
using RoR2.Skills;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Cyborg;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Cyborg
{
    public class PrimaryLaser : BaseSkillState, SteppedSkillDef.IStepSetter
    {
        public static float damageCoefficient = 3f;
        public static float baseDuration = 0.5f;
        public static float recoil = 2f;
        public static GameObject tracerEffectPrefab;
        public static GameObject tracerEffectColossusPrefab;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightning.prefab").WaitForCompletion();

        int step = 0;
        public string muzzleString;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            duration = PrimaryLaser.baseDuration / this.attackSpeedStat;
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

            FireLaser();
        }

        private void FireLaser()
        {
            Ray aimRay = base.GetAimRay();
            EffectManager.SimpleMuzzleFlash(PrimaryLaser.muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
            Util.PlaySound("SS2UCyborgPrimary", base.gameObject);

            if (base.isAuthority)
            {
                GameObject tracer = tracerEffectPrefab;
                if (CyborgCore.IsColossusSkin(base.characterBody) && tracerEffectColossusPrefab) tracer = tracerEffectColossusPrefab;
                BulletAttack bullet = new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = aimRay.direction,
                    minSpread = 0,
                    maxSpread = 0,
                    damage = PrimaryLaser.damageCoefficient * this.damageStat,
                    force = 1000f,
                    radius = 1f,
                    smartCollision = true,
                    tracerEffectPrefab = tracer,
                    muzzleName = muzzleString,
                    hitEffectPrefab = hitEffectPrefab,
                    isCrit = base.RollCrit(),
                    falloffModel = BulletAttack.FalloffModel.None,
                    damageType = DamageType.Generic,
                    maxDistance = 1000f,
                    procCoefficient = 1f
                };
                bullet.damageType.damageSource = DamageSource.Primary;
                bullet.damageType.damageTypeExtended |= DamageTypeExtended.Electrical;
                bullet.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.CyborgCanDetonateShockCore);
                bullet.Fire();
                base.AddRecoil(-0.5f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
                base.characterBody.AddSpreadBloom(0.6f/(Mathf.Sqrt(this.attackSpeedStat)));
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.fixedAge >= this.duration)
            {
                if (base.isAuthority)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
