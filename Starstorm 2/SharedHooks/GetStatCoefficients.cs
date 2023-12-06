using R2API;
using RoR2;

namespace Starstorm2Unofficial.SharedHooks
{
    internal static class GetStatCoefficients
    {
        public delegate void HandleStatsInventory(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args, Inventory inventory);
        public static HandleStatsInventory HandleStatsInventoryActions;

        internal static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.inventory && HandleStatsInventoryActions != null)
            {
                HandleStatsInventoryActions.Invoke(sender, args, sender.inventory);
            }
        }
    }
}
