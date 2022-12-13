using RoR2;
using RoR2.Orbs;
using Starstorm2.Survivors.Executioner.Components;

namespace Starstorm2.Modules.Orbs
{
    public class ExecutionerIonOrb : Orb
    {
        private const float speed = 60f;
        private IonGunChargeComponent chargeComponent;

        public override void Begin()
        {
            base.duration = base.distanceToTarget / speed;

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            EffectManager.SpawnEffect(Modules.Assets.exeIonOrb, effectData, true);

            HurtBox hurtBox = this.target.GetComponent<HurtBox>();
            if (hurtBox)
            {
                this.chargeComponent = hurtBox.healthComponent.GetComponent<IonGunChargeComponent>();
            }
        }

        public override void OnArrival()
        {
            if (this.chargeComponent)
            {
                this.chargeComponent.RpcAddIonCharge();
                EffectManager.SimpleSoundEffect(Modules.Assets.exeChargeGainSoundDef.index, this.chargeComponent.transform.position, true);
            }
        }
    }
}