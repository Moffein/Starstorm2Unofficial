using R2API;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores
{
    public class DamageTypeCore
    {
        private UInt32 currentMax = 0u;

        public static DamageTypeCore instance;

        public static class ModdedDamageTypes
        {
            public static DamageAPI.ModdedDamageType GougeOnHit;
            public static DamageAPI.ModdedDamageType ExtendFear;
        }

        //public static DamageType
        //OnHit;

        public static float gougeDamageCoefficient = 1.5f;    //gouge DPS

        public DamageTypeCore()
        {
            instance = this;

            ModdedDamageTypes.GougeOnHit = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ExtendFear = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.GougeOnHit))
                {
                    //Supposed to have a 0.25 mult for enemy Nemmando.
                    var dotInfo = new InflictDotInfo()
                    {
                        attackerObject = damageInfo.attacker,
                        victimObject = victim,
                        dotIndex = DoTCore.gougeIndex,
                        duration = 2,
                        damageMultiplier = DamageTypeCore.gougeDamageCoefficient
                    };
                    DotController.InflictDot(ref dotInfo);
                }

                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.ExtendFear))
                {
                    CharacterBody victimBody;
                    if (victim)
                    {
                        victimBody = victim.GetComponent<CharacterBody>();
                        if (victimBody && victimBody.HasBuff(BuffCore.fearDebuff))
                        {
                            victimBody.AddTimedBuff(BuffCore.fearDebuff, EntityStates.Starstorm2States.Executioner.ExecutionerDash.debuffDuration);
                        }
                    }
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool triggerGougeProc = false;
            if (damageInfo.dotIndex == DoTCore.gougeIndex && damageInfo.procCoefficient == 0f)
            {
                if (damageInfo.attacker)
                {
                    CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                    }
                }
                damageInfo.procCoefficient = 0.5f;
                triggerGougeProc = true;
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (triggerGougeProc)
                {
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, self.gameObject);
                }
            }
        }
    }
}