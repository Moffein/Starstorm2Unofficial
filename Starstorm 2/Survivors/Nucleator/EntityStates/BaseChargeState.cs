using RoR2;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Nucleator.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nucleator
{
    public abstract class BaseChargeState : BaseState
    {
        public static float overchargeFraction = 2f / 3f;
        public static string overchargeSoundString = "SS2UNucleatorAlarm";

        public float baseDuration = 1f;
        public float overchargeHealthFraction = 0.15f;  //Damage per tick when overcharging. Scales off of current health at the time of the tick.
        public float overchargeDamageFrequency = 10f;
        public float overchargeHealingFraction = 0.1f/3f; //Healing per tick when overcharging. Scales off of total health.

        private bool playedOverchargeSound = false;
        private float overchargeDamageDuration;
        private float overchargeDamageStopwatch;
        protected float duration;
        private NucleatorChargeComponent chargeComponent;
        private NucleatorNetworkComponent networkComponent;
        private ShakeEmitter shakeEmitter;

        public float chargeFraction;

        public override void OnEnter()
        {
            base.OnEnter();
            chargeFraction = 0f;
            duration = baseDuration / this.attackSpeedStat;

            overchargeDamageStopwatch = 0f;
            overchargeDamageDuration = 1f / (overchargeDamageFrequency * this.attackSpeedStat);

            networkComponent = base.GetComponent<NucleatorNetworkComponent>();
            chargeComponent = base.GetComponent<NucleatorChargeComponent>();
            if (chargeComponent)
            {
                chargeComponent.StartCharge();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            chargeFraction = Mathf.Min(1f, base.fixedAge / duration);
            if (chargeComponent)
            {
                chargeComponent.SetCharge(chargeFraction, overchargeFraction);
            }

            if (chargeFraction >= overchargeFraction)
            {
                if (!playedOverchargeSound)
                {
                    playedOverchargeSound = true;
                    Util.PlaySound(overchargeSoundString, base.gameObject);

                    if (base.isAuthority)
                    {
                        HurtSelfAuthority();
                        shakeEmitter = ShakeEmitter.CreateSimpleShakeEmitter(base.transform.position, new Wave() { amplitude = 0.5f , cycleOffset = 0f, frequency = 10f }, 0.4f, 20f, false);
                        shakeEmitter.transform.parent = base.transform;
                    }
                }
                else if (base.isAuthority && base.healthComponent)
                {
                    overchargeDamageStopwatch += Time.fixedDeltaTime;
                    if (overchargeDamageStopwatch >= overchargeDamageDuration)
                    {
                        overchargeDamageStopwatch -= overchargeDamageDuration;

                        HurtSelfAuthority();
                    }
                }
            }

            if (base.isAuthority)
            {

                if (chargeFraction >= 1f || !GetInputPressed())
                {
                    if (chargeFraction >= overchargeFraction)
                    {
                        SetNextStateOvercharge();
                        return;
                    }
                    else
                    {
                        SetNextState();
                        return;
                    }
                }
            }
        }

        public override void OnExit()
        {
            if (shakeEmitter)
            {
                Destroy(shakeEmitter);
            }

            if (chargeComponent)
            {
                chargeComponent.Reset();
            }
            base.OnExit();
        }

        private void HurtSelfAuthority()
        {
            if (!base.isAuthority) return;
            bool specialBuffActive = base.characterBody && base.characterBody.HasBuff(BuffCore.nucleatorSpecialBuff);

            if (!specialBuffActive)
            {
                DamageInfo damageInfo = new DamageInfo();
                damageInfo.damage = base.healthComponent.combinedHealth * overchargeHealthFraction;
                damageInfo.position = base.characterBody.corePosition;
                damageInfo.force = Vector3.zero;
                damageInfo.damageColorIndex = DamageColorIndex.Default;
                damageInfo.crit = false;
                damageInfo.attacker = null;
                damageInfo.inflictor = null;
                damageInfo.damageType = (DamageType.NonLethal | DamageType.BypassArmor);
                damageInfo.procCoefficient = 0f;
                damageInfo.procChainMask = default(ProcChainMask);

                if (base.characterBody && base.characterBody.mainHurtBox)
                {
                    R2API.Networking.NetworkingHelpers.DealDamage(damageInfo, base.characterBody.mainHurtBox, true, false, false);
                }
            }
            else
            {
                if (!base.healthComponent) return;
                if (NetworkServer.active)
                {
                    base.healthComponent.HealFraction(overchargeHealingFraction, default);
                }
                else if (networkComponent)
                {
                    networkComponent.HealFractionAuthority(overchargeHealingFraction);
                }
            }
        }

        protected virtual bool GetInputPressed()
        {
            return false;
        }

        protected virtual void SetNextState() { this.outer.SetNextStateToMain(); }
        protected virtual void SetNextStateOvercharge() { this.outer.SetNextStateToMain(); }
    }
}
