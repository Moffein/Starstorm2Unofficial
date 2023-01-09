using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Modules.Orbs
{
    public class ExecutionerIonTempSuperOrb : Orb
    {
        private const float speed = 20f;
        private CharacterBody body;

        public override void Begin()
        {
            base.duration = base.distanceToTarget / speed;

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            EffectManager.SpawnEffect(Modules.Assets.exeIonSuperOrb, effectData, true);

            HurtBox hurtBox = this.target.GetComponent<HurtBox>();
            if (hurtBox)
            {
                this.body = hurtBox.healthComponent.body;
            }
        }

        public override void OnArrival()
        {
            if (this.body && NetworkServer.active)
            {
                this.body.AddTimedBuff(Starstorm2Unofficial.Cores.BuffCore.exeSuperchargedBuff, 10f);
                EffectManager.SimpleSoundEffect(Modules.Assets.exeSuperchargeSoundDef.index, this.body.transform.position, true);
            }
        }
    }
}