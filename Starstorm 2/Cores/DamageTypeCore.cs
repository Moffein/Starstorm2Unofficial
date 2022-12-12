using RoR2;
using System;
using UnityEngine;

namespace Starstorm2.Cores
{
    class DamageTypeCore
    {
        private UInt32 currentMax = 0u;

        public static DamageTypeCore instance;

        //public static DamageType gougeOnHit;

        public static float gougeDamageCoefficient = 1.2f;

        public DamageTypeCore()
        {
            instance = this;
            //LogCore.LogInfo("Initializing Core: " + base.ToString());

            //maxPossible = 0b_1ul << 32;

            var damageValues = Enum.GetValues(typeof(DamageType)) as UInt32[];
            for (int i = 0; i < damageValues.Length; i++)
            {
                var value = damageValues[i];
                if (value > currentMax)
                {
                    currentMax = value;
                }
            }
            AddDamageTypes();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.DotController.AddDot += DotController_AddDot;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool gouging = false;
            float gougeDamage = 0f;
            // this is a fucking ugly block of code jesus
            if (damageInfo.damageType.HasFlag(DamageType.BlightOnHit) && self && self.body && damageInfo.attacker &&
    (damageInfo.attacker.name == "NemmandoBody(Clone)" || damageInfo.attacker.name == "NemmandoMonsterBody(Clone)"))
            {
                gouging = true;
                damageInfo.damageType &= ~DamageType.BlightOnHit;

                CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                gougeDamage = (damageInfo.crit ? 2f : 1f) * gougeDamageCoefficient;
                /*if (attacker && attacker.HasBuff(BuffCore.awarenessBuff))
                {
                    damageCoef *= 2f;
                    attacker.RemoveBuff(BuffCore.awarenessBuff);
                }*/
            }

            orig(self, damageInfo);

            if (gouging && !damageInfo.rejected)
            {
                if (damageInfo.attacker)
                {
                    TeamComponent attackerTeam = damageInfo.attacker.GetComponent<TeamComponent>();
                    if (attackerTeam)
                    {
                        if (attackerTeam.teamIndex.HasFlag(TeamIndex.Monster))
                        {
                            gougeDamage *= 0.25f;
                        }
                    }
                }

                var dotInfo = new InflictDotInfo()
                {
                    attackerObject = damageInfo.attacker,
                    victimObject = self.gameObject,
                    dotIndex = DoTCore.gougeIndex,
                    duration = 3,
                    damageMultiplier = gougeDamage
                };

                DotController.InflictDot(ref dotInfo);
            }
        }

        private void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject,
            float duration, DotController.DotIndex dotIndex, float damageMultiplier, uint? maxStacksFromAttacker, float? totalDamage, DotController.DotIndex? preUpgradeDotIndex)
        {
            orig(self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage, preUpgradeDotIndex);
            if (dotIndex == DoTCore.gougeIndex)
            {
                int i = 0;
                int count = self.dotStackList.Count;
                while (i < count)
                {
                    if (self.dotStackList[i].dotIndex == DoTCore.gougeIndex)
                    {
                        self.dotStackList[i].timer = Mathf.Max(self.dotStackList[i].timer, duration);
                    }
                    i++;
                }
            }
        }

        protected void AddDamageTypes()
        {
            //gougeOnHit = AddDamageType();
            //LogCore.LogInfo(gougeOnHit);
        }

        /*
        // :^]]
        protected DamageType AddDamageType()
        {
            currentMax *= 2;
            var damageType = (DamageType)currentMax;
            return damageType;
        }
        */
    }
}