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
        public override ItemTier Tier => ItemTier.Tier3;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Damage, ItemTag.Utility, ItemTag.AIBlacklist,
            ItemTag.FoodRelated,
            ItemTag.CanBeTemporary
        };
        public override string PickupIconPath => "GreenChocolate_Icon";
        public override string PickupModelPath => "MDLGreenChocolate";

        public float damageThreshold = 0.2f;

        public override void RegisterHooks()
        {
            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
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

