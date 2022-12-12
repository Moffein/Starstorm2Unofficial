using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.Items
{
    class GreenChocolate : SS2Item<GreenChocolate>
    {
        public override string NameInternal => "GreenChoc";
        public override string Name => "Green Chocolate";
        public override string Pickup => "Reduce damage taken and gain a huge damage and critical chance bonus upon taking heavy damage.";
        public override string Description => $"When damaged for at least <style=cIsHealth>{StaticValues.choccyThreshold * 100}%</style> of your max HP, gain a stacking buff that grants <style=cIsDamage>50%</style> base damage and <style=cIsDamage>20%</style> critical chance for <style=cIsDamage>{StaticValues.choccyStackTime + StaticValues.choccyBaseTime}s</style> <style=cStack>(+{StaticValues.choccyStackTime}s per stack)</style>. Damage in excess of the 20% max HP threshold is reduced by <style=cIsUtility>50%</style>.";
        public override string Lore => "<style=cMono>Audio transcription complete from signal echoes. Assigning generic tokens.</style>\n\n[Hissing, loud clank]\n\nMAN 1: Uh... Hey, weird question?\n\nMAN 2: [Audible sigh] What?\n\nMAN 1: Do you know of any weird, exotic chocolates that are bright green in color?\n\nMAN 2: What? What on Earth are you on about?\n\nMAN 1: I found some green chocolate in this security chest, and I'm asking if that's normal or not.\n\nMAN 2: Uh, no, it's not. I don't think so, anyways. I wouldn't know.\n\n[Crinkling]\n\nMAN 1: So then what's it supposed to be? Do you think this is poisonous, too?\n\nMAN 2: Probably not. And, ugh, for the last time, the meat wasn't poisonous, you've just got a weak stomach.\n\nMAN 1: Alright, I guess I'll keep it, for if things get desperate.\n\nMAN 2: That's probably not a great idea, but alright.\n\n<style=cMono> End of requested transcript.</style>\n";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage
        };
        public override string PickupIconPath => "GreenChocolate_Icon";
        public override string PickupModelPath => "MDLGreenChocolate";

        public override void RegisterHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        // no item displays for this. fuck you

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (damageInfo.attacker && self)
            {
                var victimBody = self.body;
                if (victimBody)
                {
                    //green choc
                    int greenChocCount = GetCount(victimBody);
                    if (greenChocCount > 0 && damageInfo.damage >= self.fullCombinedHealth * StaticValues.choccyThreshold)
                    {
                        damageInfo.damage = damageInfo.damage / 2f + (self.fullCombinedHealth * 0.1f);
                        self.body.AddTimedBuff(BuffCore.greenChocBuff, StaticValues.choccyBaseTime + (StaticValues.choccyStackTime * greenChocCount));
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}

