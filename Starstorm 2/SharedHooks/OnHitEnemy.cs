using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.SharedHooks
{
    internal static class OnHitEnemy
    {
        public delegate void OnHitNoAttacker(DamageInfo damageInfo, CharacterBody victimBody);
        public static OnHitNoAttacker OnHitNoAttackerActions;

        public delegate void OnHitAttacker(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody);
        public static OnHitAttacker OnHitAttackerActions;

        public delegate void OnHitAttackerInventory(DamageInfo damageInfo, CharacterBody victimBody, CharacterBody attackerBody, Inventory attackerInventory);
        public static OnHitAttackerInventory OnHitAttackerInventoryActions;

        public static void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, UnityEngine.GameObject victim)
        {

            orig(self, damageInfo, victim);

            bool validDamage = NetworkServer.active && damageInfo.procCoefficient > 0f && !damageInfo.rejected;
            if (!validDamage) return;

            CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
            if (!victimBody) return;

            OnHitNoAttackerActions?.Invoke(damageInfo, victimBody);

            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
            if (!attackerBody) return;

            OnHitAttackerActions?.Invoke(damageInfo, victimBody, attackerBody);
            if (attackerBody.inventory) OnHitAttackerInventoryActions?.Invoke(damageInfo, victimBody, attackerBody, attackerBody.inventory);
        }
    }
}
