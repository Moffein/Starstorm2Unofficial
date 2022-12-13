using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.Starstorm2States.Executioner
{
    public class ExecutionerBurstPistol : BaseState
    {
        public static float damageCoefficient = 1.5f;
        public static float force = 100f;
        public static float baseDuration = 0.5f;
        public static float baseShotDuration = 0.12f;
        public static string attackSoundString = "ExecutionerPrimaryClassic";//"ExecutionerPrimary";
        public static string critSoundString = "ExecutionerPrimaryClassic";//"ExecutionerPrimaryCrit";
        public static int baseShotCount = 2;
        public static string muzzleString = "Muzzle";
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
            duration = ExecutionerBurstPistol.baseDuration / this.attackSpeedStat;
            shotDuration = ExecutionerBurstPistol.baseShotDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            FireBullet();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (shotCount < ExecutionerBurstPistol.baseShotCount)
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
            Util.PlaySound(crit ? ExecutionerBurstPistol.critSoundString : ExecutionerBurstPistol.attackSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, muzzleString, false);
            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);

            if (base.isAuthority)
            {
                float dmg = ExecutionerBurstPistol.damageCoefficient * this.damageStat;
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
                    force = ExecutionerBurstPistol.force,
                    isCrit = this.crit,
                    owner = base.gameObject,
                    muzzleName = muzzleString,
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
