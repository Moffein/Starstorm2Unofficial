using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.SharedHooks
{
    internal static class OnCharacterDeathGlobal
    {
        public delegate void OnCharacterDeathInventory(GlobalEventManager self, DamageReport damageReport, CharacterBody attackerBody, Inventory attackerInventory, CharacterBody victimBody);
        public static OnCharacterDeathInventory OnCharacterDeathInventoryActions;
        public static void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            if (!NetworkServer.active || !damageReport.attackerBody || !damageReport.attackerBody.inventory  || !damageReport.victimBody) return;
            OnCharacterDeathInventoryActions?.Invoke(self, damageReport, damageReport.attackerBody, damageReport.attackerBody.inventory, damageReport.victimBody);
        }
    }
}
