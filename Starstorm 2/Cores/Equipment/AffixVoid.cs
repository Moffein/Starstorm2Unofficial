using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.Equipment
{
    class AffixVoid : SS2Equipment<AffixVoid>
    {
        public override string NameInternal => "AffixVoid";
        public override string Name => "Condemnation's Lament";
        public override string Pickup => "Become an aspect of Void.";
        public override string Description => "Become an aspect of Void.";
        public override string Lore => "";
        public override float Cooldown => 10;
        public override bool CanDrop => false;
        public override BuffDef PassiveBuffDef => Elites.VoidElite.instance.eliteBuffDef;
        public override string PickupIconPath => "affixVoidIcon";
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
