using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrPrimary : BaseState
    {
        public static float damageCoefficient = 1.5f;
        public static float force = 100f;
        public static float baseDuration = 0.5f;
        public static float baseShotDuration = 0.1f;
        public static string attackSoundString = "ChirrPrimary";
        public static int baseShotCount = 3;
        public static float spreadBloom = 0.4f;
        public static float recoil = 1f;

        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoDefault.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommando.prefab").WaitForCompletion();

        private int shotCount;
        private float duration;
        private float shotDuration;
        private float shotStopwatch;
        private bool crit;

        public override void OnEnter()
        {
            base.OnEnter();

            crit = base.RollCrit();
            shotCount = 0;
            shotStopwatch = 0f;
            duration = ChirrPrimary.baseDuration / this.attackSpeedStat;
            shotDuration = ChirrPrimary.baseShotDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Primary", "Primary.playbackRate", this.duration);

            FireBullet();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (shotCount < ChirrPrimary.baseShotCount)
            {
                shotStopwatch += Time.fixedDeltaTime;
                if (shotStopwatch >= shotDuration)
                {
                    FireBullet();
                }
            }
            else
            {
                if (base.isAuthority && base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void OnExit()
        {
            if (base.characterBody)
            {
                base.characterBody.SetSpreadBloom(0f, false);
            }
            base.OnExit();
        }

        private void FireBullet()
        {
            shotStopwatch = 0f;
            shotCount++;
            Util.PlaySound(ChirrPrimary.attackSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "WingLEnd", false);
            EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "WingREnd", false);

            if (base.isAuthority)
            {
                float dmg = ChirrPrimary.damageCoefficient * this.damageStat;
                Ray r = base.GetAimRay();
                BulletAttack bullet = new BulletAttack
                {
                    aimVector = r.direction,
                    origin = r.origin,
                    damage = damageCoefficient * damageStat,
                    damageType = DamageType.Generic,
                    damageColorIndex = DamageColorIndex.Default,
                    minSpread = 0f,
                    maxSpread = 0f,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    force = ChirrPrimary.force,
                    isCrit = this.crit,
                    owner = base.gameObject,
                    muzzleName = (shotCount % 2 == 0 ? "WingLEnd" : "WingREnd"),
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = 1f,
                    radius = 0.3f,
                    weapon = base.gameObject,
                    tracerEffectPrefab = tracerEffectPrefab,
                    hitEffectPrefab = hitEffectPrefab,
                    maxDistance = 200f
                };
                bullet.Fire();
            }
            base.AddRecoil(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            if (base.characterBody) base.characterBody.AddSpreadBloom(spreadBloom); //Spread is cosmetic. Skill is always perfectly accurate.
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
