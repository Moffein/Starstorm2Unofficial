using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;

namespace Starstorm2.Cores.Equipment
{
    class AffixEthereal : SS2Equipment<AffixEthereal>
    {
        public override string NameInternal => "AffixEthereal";
        public override string Name => "Unstable Remnant";
        public override string Pickup => "Become an ethereal aspect.";
        public override string Description => "Become an ethereal aspect.";
        public override float Cooldown => 10;
        public override bool CanDrop => false;
        public override BuffDef PassiveBuffDef => Elites.EtherealElite.instance.eliteBuffDef;
        public override string PickupIconPath => "";
        public override string PickupModelPath => "";

        public override void RegisterHooks()
        {
        }

        protected override bool ActivateEquipment(EquipmentSlot equip)
        {
            return false;
        }
    }
}
