using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Equipment
{
    class CloakingHeadband : SS2Equipment<CloakingHeadband>
    {
        public override string NameInternal => "SS2U_CloakingHeadband";
        public override float Cooldown => 45;
        public override string PickupIconPath => "CloakingHeadband_Icon";
        public override string PickupModelPath => "MDLCloakingHeadband";

        private static GameObject effectPrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Phasing/ProcStealthkit.prefab").WaitForCompletion();

        public override void RegisterHooks()
        {
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            equip.characterBody.AddTimedBuff(RoR2Content.Buffs.CloakSpeed, 10f);
            equip.characterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, 10f);

            if (equip.characterBody && effectPrefab)
            {
                EffectManager.SpawnEffect(effectPrefab, new EffectData
                {
                    rotation = Quaternion.identity,
                    origin = equip.characterBody.transform.position
                }, true);
            }

            return true;
        }
    }
}
