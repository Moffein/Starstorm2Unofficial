using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores.Items
{
    class GreenChocolate : SS2Item<GreenChocolate>
    {
        public override string NameInternal => "SS2U_GreenChoc";
        public override string Name => "Green Chocolate";
        public override string Pickup => "Upon taking heavy damage, reduce damage taken and gain a massive damage boost.";
        public override string Description => "When receiving <style=cIsHealth>20%</style> or more of your <style=cIsHealth>max health</style> as damage, any damage over the threshold is <style=cIsHealing>reduced by 50%</style>. Gain <style=cIsDamage>50% damage</style> for <style=cIsUtility>7s</style> when triggering this effect, stacks up to <style=cIsDamage>1</style> <style=cStack>(+1 per stack)</style> time.";
        public override string Lore => "<style=cMono>Audio transcription complete from signal echoes. Assigning generic tokens.</style>\n\n[Hissing, loud clank]\n\nMAN 1: Uh... Hey, weird question?\n\nMAN 2: [Audible sigh] What?\n\nMAN 1: Do you know of any weird, exotic chocolates that are bright green in color?\n\nMAN 2: What? What on Earth are you on about?\n\nMAN 1: I found some green chocolate in this security chest, and I'm asking if that's normal or not.\n\nMAN 2: Uh, no, it's not. I don't think so, anyways. I wouldn't know.\n\n[Crinkling]\n\nMAN 1: So then what's it supposed to be? Do you think this is poisonous, too?\n\nMAN 2: Probably not. And, ugh, for the last time, the meat wasn't poisonous, you've just got a weak stomach.\n\nMAN 1: Alright, I guess I'll keep it, for if things get desperate.\n\nMAN 2: That's probably not a great idea, but alright.\n\n<style=cMono> End of requested transcript.</style>\n";
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist
        };
        public override string PickupIconPath => "GreenChocolate_Icon";
        public override string PickupModelPath => "MDLGreenChocolate";

        public float damageThreshold = 0.2f;

        public override void RegisterHooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (NetworkServer.active && damageInfo.attacker)
            {
                int greenChocCount = GetCount(self.body);
                if (greenChocCount > 0)
                {
                    float threshold = self.fullCombinedHealth * damageThreshold;
                    if (damageInfo.damage >= threshold)
                    {
                        float overThreshold = damageInfo.damage - threshold;
                        damageInfo.damage = threshold + overThreshold * 0.5f;

                        int currentChocCount = self.body.GetBuffCount(BuffCore.greenChocBuff);
                        int newChocCount = Mathf.Min(currentChocCount + 1, greenChocCount);

                        self.body.ClearTimedBuffs(BuffCore.greenChocBuff);

                        for (int i = 0; i < newChocCount; i++)
                        {
                            self.body.AddTimedBuff(BuffCore.greenChocBuff, 7f);
                        }
                    }
                }
            }
            orig(self, damageInfo);
        }
    }
}

