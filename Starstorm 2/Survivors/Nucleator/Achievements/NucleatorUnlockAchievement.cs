using RoR2;
using RoR2.Achievements;

namespace Starstorm2Unofficial.Survivors.Nucleator.Achievements
{
    [RegisterAchievement("SS2UNucleatorUnlock", "Characters.SS2UNucleator", null, 3, null)]
    public class NucleatorUnlockAchievement : BaseAchievement
    {
        public override void OnInstall()
        {
            base.OnInstall();
            Inventory.onInventoryChangedGlobal += OnInventoryChangedGlobal;
        }

        private void OnInventoryChangedGlobal(Inventory inventory)
        {
            CharacterMaster master = inventory.GetComponent<CharacterMaster>();
            if (master && localUser.cachedMaster == master)
            {
                if (inventory.HasAtLeastXTotalRemovablePermanentItemsOfTier(ItemTier.Tier3, 5))
                {
                    Grant();
                }
            }
        }
        public override void OnUninstall()
        {
            Inventory.onInventoryChangedGlobal -= OnInventoryChangedGlobal;
            base.OnUninstall();
        }
    }
}
