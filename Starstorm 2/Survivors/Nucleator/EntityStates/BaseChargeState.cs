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

        public float baseDuration = 1f;
        public float overchargeHealthFraction = 0.2f;
        public float overchargeDamageFrequency = 10f;

        private float overchargeDamageDuration;
        private float overchargeDamageStopwatch;
        private float duration;
        private NucleatorChargeComponent chargeComponent;

        public float chargeFraction;

        public override void OnEnter()
        {
            base.OnEnter();
            chargeFraction = 0f;
            duration = baseDuration / this.attackSpeedStat;

            overchargeDamageStopwatch = 0f;
            overchargeDamageDuration = (1f / overchargeDamageFrequency) / this.attackSpeedStat;

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
                if (NetworkServer.active)
                {
                    if (base.characterBody && base.healthComponent && !base.characterBody.HasBuff(BuffCore.nucleatorSpecialBuff))
                    {
                        overchargeDamageStopwatch += Time.fixedDeltaTime;
                        if (overchargeDamageStopwatch >= overchargeDamageDuration)
                        {
                            overchargeDamageStopwatch -= overchargeDamageDuration;

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
                            base.healthComponent.TakeDamage(damageInfo);
                        }
                    }
                }
            }

            if (base.isAuthority)
            {
                if (chargeFraction >= 1f || !GetInputPressed())
                {
                    if (chargeFraction >= overchargeFraction)
                    {
                        SetNextStateOvercharged();
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

        protected virtual bool GetInputPressed()
        {
            return false;
        }

        protected virtual void SetNextState() { this.outer.SetNextStateToMain(); }
        protected virtual void SetNextStateOvercharged() { this.outer.SetNextStateToMain(); }
    }
}
