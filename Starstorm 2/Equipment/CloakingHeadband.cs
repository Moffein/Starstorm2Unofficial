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
        public override string Name => "Cloaking Headband";
        public override string Pickup => "Become temporarily invisible.";
        public override string Description => "Become <style=cIsUtility>invisible</style> for 10 seconds.";
        public override string Lore => "Order: Cloaking Headband\nTracking Number: 554*****\nEstimated Delivery: 08/01/2056\nShipping Method: Standard\nShipping Address: 202 W. Calbury Ln, Adora, Mercury\nShipping Details:\n\nI was finally able to get ahold of a personal cloaking device for you. Sneaking around in places you shouldn't be should be a cinch with this puppy.\n\nNow, there's a couple a' things you should understand about this thing. First, the cloaking only lasts a short time since the device is so small, so keep yourself hidden while it cools down again. Secondly, you need to hit the button on the headband to activate it. Not exactly a very clandestine action, I know, but it's what I could find for you.\n\nLast, if the box you get seems like it's completely empty, just feel around in the bottom until you find the button. It saves time, money, and privacy to ship it without the Volatile treatment. tell me when this gets to you, and have fun with your new toy.\n";
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
