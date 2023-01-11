using EntityStates;
using EntityStates.Captain.Weapon;
using RoR2;
using RoR2.UI;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.States.Nucleator
{
    class NucleatorSkillStateBase : BaseSkillState
    {
        public static float baseMaxChargeTime = 2F;
        private static GameObject chargeupVfxPrefab = ChargeCaptainShotgun.chargeupVfxPrefab;
        public static float overchargeThreshold = 0.66F;
        public static float maxOverchargePlayerDamageDealt = 0.5F;


        private Transform muzzleTransform;
        private GameObject chargeupVfxGameObject;

        public float charge;
        public float lastCharge;
        public float lastDamage;
        public float maxChargeTime;

        private float playerHealthFinal;
        private float playerHealth;
        private float damageDealt;
        private float nextDamageInstance = 0.05f;
        private bool isCrosshairInitialized = false;
        private GameObject defaultCrosshair;

        public override void OnEnter()
        {
            base.OnEnter();
            this.maxChargeTime = NucleatorSkillStateBase.baseMaxChargeTime / this.attackSpeedStat;
            this.playerHealth = this.characterBody.healthComponent.combinedHealth;
            this.playerHealthFinal = playerHealth - playerHealth * maxOverchargePlayerDamageDealt;
            this.defaultCrosshair = this.characterBody._defaultCrosshairPrefab;
            this.characterBody._defaultCrosshairPrefab = Survivors.Nucleator.NucleatorCore.chargeCrosshair;

            if(!isCrosshairInitialized)
            {
                foreach (Transform child in Survivors.Nucleator.NucleatorCore.chargeCrosshair.transform)
                {
                    if (child.name == "Zoom Amount") Destroy(child.gameObject);
                }
                isCrosshairInitialized = true;
            }
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                this.muzzleTransform = base.FindModelChild("MuzzleCenter");
                if (this.muzzleTransform)
                {
                    this.chargeupVfxGameObject = UnityEngine.Object.Instantiate<GameObject>(NucleatorSkillStateBase.chargeupVfxPrefab, this.muzzleTransform);
                    this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.maxChargeTime;
                }
            }
        }
        public override void OnExit()
        {
            if(damageDealt > 0 && this.playerHealth - this.playerHealthFinal > damageDealt)
            {
                TakeDamage();
            }

            if (this.chargeupVfxGameObject)
            {
                EntityState.Destroy(this.chargeupVfxGameObject);
                this.chargeupVfxGameObject = null;
            }

            this.characterBody._defaultCrosshairPrefab = this.defaultCrosshair;
            base.OnExit();
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.SetAimTimer(1f);
            this.lastCharge = this.charge;
            this.charge = base.fixedAge / this.maxChargeTime;
            if (this.charge <= 1.0f
                   && this.charge > overchargeThreshold
                   && charge - overchargeThreshold > this.nextDamageInstance)
            {                
                damageDealt += TakeDamage();
                this.nextDamageInstance += (1 - overchargeThreshold) / 5;
            }

        }

        private float CalculateDamageInstance()
        {
            var chargeCoef = (this.charge - overchargeThreshold) / (1 - overchargeThreshold);
            var damageCoef = chargeCoef * maxOverchargePlayerDamageDealt;
            var damage = Mathf.Abs(chargeCoef * (playerHealthFinal - playerHealth)) - damageDealt;
            return damage;
        }

        private float TakeDamage()
        {
            var damage = CalculateDamageInstance();
            if (!this.characterBody.HasBuff(Starstorm2Unofficial.Cores.BuffCore.nucleatorSpecialBuff))
            {
                this.characterBody.healthComponent.TakeDamage(
                    new DamageInfo()
                    {
                        damageType = DamageType.BypassOneShotProtection | DamageType.BypassArmor | DamageType.NonLethal,
                        crit = false,
                        damage = damage,
                        position = this.characterBody.transform.position
                    });
            }
            return damage;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

