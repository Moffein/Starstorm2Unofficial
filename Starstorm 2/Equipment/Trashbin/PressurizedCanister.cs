using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Equipment
{
    class PressurizedCanister : SS2Equipment<PressurizedCanister>
    {
        public override string NameInternal => "SS2U_Canister";
        public override float Cooldown => 15;
        public override string PickupIconPath => "PressurizedCanister_Icon";
        public override string PickupModelPath => "MDLPressurizedCanister";

        public override void RegisterHooks()
        {
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            CanisterBehavior canister = equip.gameObject.AddComponent<CanisterBehavior>();
            canister.body = equip.characterBody;
            return true;
        }
    }

    public class CanisterBehavior : NetworkBehaviour
    {
        public CharacterBody body;
        private const float duration = 1.0f;
        private const float thrustForce = 110f;
        private float timer;
        private static GameObject thrustStartEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/SmokescreenEffect");
        private static GameObject thrustEffect = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/CharacterLandImpact");

        private void Awake()
        {
            this.timer = 0f;
        }

        private void FixedUpdate()
        {
            if (!body) return;

            if (this.timer == 0f)
            {
                var motor = body.characterMotor;
                if (motor) motor.Motor.ForceUnground();

                EffectData effectData = new EffectData();
                effectData.origin = body.footPosition;
                EffectManager.SpawnEffect(thrustStartEffect, effectData, true);
            }
            this.timer += Time.fixedDeltaTime;
            if (this.timer < duration && body)
            {
                bool funnyCanister = Modules.Config.EnableFunnyCanister.Value;
                var motor = body.characterMotor;
                if (motor)
                {
                    if (NetworkServer.active && (funnyCanister || body.inputBank.jump.down))
                    {
                        motor.ApplyForce(Vector3.up * thrustForce * (body.rigidbody.mass / 100f));
                    }
                    EffectData effectData = new EffectData();
                    effectData.origin = body.footPosition;
                    effectData.scale = 0.5f;
                    EffectManager.SpawnEffect(thrustEffect, effectData, true);
                }
            }
            else
            {
                UnityEngine.Object.Destroy(this);
            }
        }

        private void OnDestroy()
        {
        }
    }
}
