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
    class CloakingHeadband : SS2Equipment<CloakingHeadband>
    {
        public override string NameInternal => "SS2U_CloakingHeaband";
        public override float Cooldown => 45;
        public override string PickupIconPath => "CloakingHeadband_Icon";
        public override string PickupModelPath => "MDLCloakingHeadband";

        public override void RegisterHooks()
        {
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            equip.characterBody.AddTimedBuff(RoR2Content.Buffs.Cloak, 10f);
            return true;
        }
    }
}
