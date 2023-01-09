using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace Starstorm2Unofficial.Modules.Orbs
{
    public class NemmandoDashOrb : Orb
    {
        private const float speed = 80f;

        public override void Begin()
        {
            base.duration = base.distanceToTarget / speed;

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            EffectManager.SpawnEffect(Modules.Assets.nemDashEffect, effectData, true);
        }

        public override void OnArrival()
        {
            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            EffectManager.SpawnEffect(Modules.Assets.nemPreImpactFX, effectData, true);

            HurtBox hurtBox = this.target.GetComponent<HurtBox>();
            if (hurtBox)
            {
                GameObject bodyObject = hurtBox.healthComponent.gameObject;
                if (bodyObject)
                {
                    SetStateOnHurt setState = bodyObject.GetComponent<SetStateOnHurt>();
                    if (setState)
                    {
                        setState.CallRpcSetStun(5f);
                    }
                }
            }
        }
    }
}