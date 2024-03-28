using RoR2;
using Starstorm2Unofficial.Cores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine;
using R2API;
using Starstorm2Unofficial.Survivors.Cyborg.Components;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class FireTriShot : BaseState
    {
        public static float chargeConsumptionPerShot = 0.2f / 3f;
        public static float damageCoefficient = 1f;
        public static float baseDuration = 0.2f;
        public static float recoil = 0.5f;
        public static GameObject tracerEffectPrefab;//Prefabs/Effects/Tracers/TracerHuntressSnipe
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommandoShotgun.prefab").WaitForCompletion();
        public static GameObject muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightning.prefab").WaitForCompletion();

        private CyborgChargeComponent chargeComponent;
        int step = 0;
        public string muzzleString;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            chargeComponent = base.GetComponent<CyborgChargeComponent>();
            if (chargeComponent)
            {
                chargeComponent.showTriShotCrosshair = true;
                chargeComponent.shieldActive = true;
                chargeComponent.ConsumeShield(chargeConsumptionPerShot);

                step = chargeComponent.armToFireFrom;
                
                if (step == 0)
                {
                    chargeComponent.armToFireFrom = 1;
                }
                else
                {
                    chargeComponent.armToFireFrom = 0;
                }
            }

            duration = baseDuration / this.attackSpeedStat;
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
            EffectManager.SimpleMuzzleFlash(muzzleflashEffectPrefab, base.gameObject, this.muzzleString, false);
            Util.PlaySound("SS2UCyborgSecondary", base.gameObject);

            if (base.isAuthority)
            {
                bool isCrit = base.RollCrit();
                Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                float currentSpread = 0f;
                float angle = 0f;
                float num2 = 0f;
                num2 = UnityEngine.Random.Range(1f + currentSpread, 1f + currentSpread) * 3f;   //Bandit is x2
                angle = num2 / 2f;  //3 - 1 shots

                Vector3 direction = Quaternion.AngleAxis(-num2 * 0.5f, axis) * aimRay.direction;
                Quaternion rotation = Quaternion.AngleAxis(angle, axis);
                Ray aimRay2 = new Ray(aimRay.origin, direction);
                for (int i = 0; i < 3; i++)
                {
                    BulletAttack bullet = new BulletAttack
                    {
                        owner = base.gameObject,
                        weapon = base.gameObject,
                        origin = aimRay2.origin,
                        aimVector = aimRay2.direction,
                        minSpread = 0,
                        maxSpread = 0,
                        damage = damageCoefficient * this.damageStat,
                        force = 200f,
                        radius = 1f,
                        smartCollision = true,
                        tracerEffectPrefab = tracerEffectPrefab,
                        muzzleName = muzzleString,
                        hitEffectPrefab = hitEffectPrefab,
                        isCrit = isCrit,
                        falloffModel = BulletAttack.FalloffModel.None,
                        damageType = DamageType.SlowOnHit,
                        maxDistance = 1000f,
                        procCoefficient = 0.5f
                    };
                    bullet.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.CyborgCanDetonateShockCore);
                    bullet.Fire();
                    aimRay2.direction = rotation * aimRay2.direction;
                }
                base.AddRecoil(-0.5f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            }
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
                base.characterBody.AddSpreadBloom(0.6f / (Mathf.Sqrt(this.attackSpeedStat)));
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
            return InterruptPriority.PrioritySkill;
        }

        public override void OnExit()
        {
            if (chargeComponent)
            {
                chargeComponent.shieldActive = false;
                chargeComponent.showTriShotCrosshair = false;
            }
            base.OnExit();
        }

        public void SetStep(int i)
        {
            step = i;
        }
    }
}
