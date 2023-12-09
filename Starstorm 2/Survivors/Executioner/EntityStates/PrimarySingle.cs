using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Executioner
{
    public class ExecutionerSinglePistol : BaseState
    {
        public static float damageCoefficient = 1.8f;
        public static float force = 100f;
        public static float baseDuration = 0.25f;
        public static string attackSoundString = "SS2UExecutionerPrimaryClassic";//"ExecutionerPrimary";
        public static string critSoundString = "SS2UExecutionerPrimaryClassic";//"ExecutionerPrimaryCrit";
        public static string muzzleString = "Muzzle";
        public static float spreadBloom = 0.4f;
        public static float recoil = 1f;

        public static GameObject tracerEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/TracerCommandoDefault.prefab").WaitForCompletion();
        public static GameObject hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/HitsparkCommando.prefab").WaitForCompletion();

        private float duration;
        private bool crit;

        public override void OnEnter()
        {
            base.OnEnter();

            crit = base.RollCrit();
            duration = ExecutionerSinglePistol.baseDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.SetAimTimer(2f);
            }

            FireBullet();
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
            Util.PlaySound(crit ? ExecutionerSinglePistol.critSoundString : ExecutionerSinglePistol.attackSoundString, base.gameObject);
            EffectManager.SimpleMuzzleFlash(Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, muzzleString, false);
            base.PlayAnimation("Gesture, Override", "Primary", "Primary.playbackRate", this.duration);

            if (base.isAuthority)
            {
                float dmg = ExecutionerSinglePistol.damageCoefficient * this.damageStat;
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
                    force = ExecutionerSinglePistol.force,
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
