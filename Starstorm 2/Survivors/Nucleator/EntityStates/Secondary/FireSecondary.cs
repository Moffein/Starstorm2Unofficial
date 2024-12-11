using UnityEngine.AddressableAssets;
using UnityEngine;
using RoR2;
using R2API;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nucleator.Secondary
{
    public class FireSecondary : BaseState
    {
        public static float force = 3000f;  //Knockback feels bad if force is not consistent
        public static float selfKnockbackForce = 1600f; //This is meant to be a really small amount, just a subtle thing for feel.
        public static float range = 40f;
        public static float baseDuration = 0.4f;
        public static GameObject coneEffectPrefab;
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleAcidImpact.prefab").WaitForCompletion();

        public float charge;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration;// this.attackSpeedStat;
            this.damageStat *= this.attackSpeedStat;

            Util.PlaySound("Play_acrid_shift_land", base.gameObject);

            Ray aimRay = base.GetAimRay();
            if (coneEffectPrefab)
            {
                //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleR", false);
                //EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, "MuzzleL", false);

                Transform muzzleTransform = null;
                ChildLocator cl = base.GetModelChildLocator();
                if (cl) muzzleTransform = cl.FindChild("MuzzleCenter");

                if (muzzleTransform)
                {
                    GameObject effectPrefab = GetEffectPrefab();
                    Quaternion rot = Quaternion.LookRotation(aimRay.direction) * Quaternion.Euler(90f, 0f, 0f);
                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = rot,
                        origin = muzzleTransform.position + aimRay.direction * 3.2f
                    }, false);

                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = rot * Quaternion.Euler(0f, 60f, 0f),
                        origin = muzzleTransform.position + aimRay.direction * 9f
                    }, false);

                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = rot * Quaternion.Euler(0f, 120f, 0f),
                        origin = muzzleTransform.position + aimRay.direction * 15f
                    }, false);

                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = rot * Quaternion.Euler(0f, 180f, 0f),
                        origin = muzzleTransform.position + aimRay.direction * 21f
                    }, false);

                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        scale = 5f,
                        rotation = rot * Quaternion.Euler(0f, 240f, 0f),
                        origin = muzzleTransform.position + aimRay.direction * 27f
                    }, false);
                }
            }
            if (base.isAuthority)
            {
                BulletAttack ba = new BulletAttack
                {
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = this.damageStat * GetDamageCoefficient(),
                    damageType = DamageType.Stun1s,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.None,
                    force = 4000f,
                    isCrit = base.RollCrit(),
                    owner = base.gameObject,
                    muzzleName = "MuzzleCenter",
                    smartCollision = false,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = 7f,
                    weapon = base.gameObject,
                    tracerEffectPrefab = null,
                    hitEffectPrefab = hitEffectPrefab,
                    maxDistance = range,
                    stopperMask = LayerIndex.noCollision.mask
                };
                ba.damageType.damageSource = DamageSource.Secondary;
                ba.AddModdedDamageType(Starstorm2Unofficial.Cores.DamageTypeCore.ModdedDamageTypes.ScaleForceToMass);
                ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.ResetVictimForce);
                ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.GroundedForceCorrection);
                if (base.characterBody && base.characterBody.HasBuff(Starstorm2Unofficial.Cores.BuffCore.nucleatorSpecialBuff))
                {
                    ba.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.NucleatorRadiationOnHit);
                }
                ModifyBulletAttack(ba);
                ba.Fire();

                if (base.characterMotor)
                {
                    if (base.characterMotor.velocity.y < 0f) base.characterMotor.velocity.y = 0f;
                    if (!base.characterMotor.isGrounded && base.characterMotor.velocity != Vector3.zero)
                        base.characterMotor.ApplyForce(-selfKnockbackForce * aimRay.direction, false, false);
                }
            }

            float recoil = 8f;
            base.AddRecoil(-0.5f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            base.PlayAnimation("Gesture, Override", "SecondaryRelease", "Secondary.playbackRate", 1.2f);
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

        protected virtual float GetDamageCoefficient()
        {
            float chargeScaled = charge / BaseChargeState.overchargeFraction;
            return Mathf.Lerp(2f, 7.2f, chargeScaled);
        }

        protected virtual GameObject GetEffectPrefab()
        {
            return coneEffectPrefab;
        }

        protected virtual void ModifyBulletAttack(BulletAttack ba)
        {

        }
    }
}
