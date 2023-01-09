using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RoR2;

namespace Starstorm2Unofficial.Cores.Items
{
    class BabyToys : SS2Item<BabyToys>
    {
        public override string NameInternal => "LevelDownOnPickup";
        public override ItemTier Tier => ItemTier.Tier1;
        public override ItemTag[] Tags => new ItemTag[]
        {
            ItemTag.Utility
        };
        public override string PickupIconPath => "CoffeeBag_Icon";
        public override string PickupModelPath => "MDLCoffeeBag";

        public override void RegisterHooks()
        {
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
 
            int toysCount = GetCount(self);
            self.level-=3*toysCount;
        }
    }
}
