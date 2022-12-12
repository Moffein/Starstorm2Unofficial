using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;

namespace Starstorm2.Cores.Equipment
{
    class AffixDazing : SS2Equipment<AffixDazing>
    {
        public override string NameInternal => "AffixDazing";
        public override string Name => "Focal Obfuscator";
        public override string Pickup => "Become a dazing aspect.";
        public override string Description => "Become a dazing aspect.";
        public override float Cooldown => 10;
        public override bool CanDrop => false;
        public override BuffDef PassiveBuffDef => Elites.DazingElite.instance.eliteBuffDef;
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
