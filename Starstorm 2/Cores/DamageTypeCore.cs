using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Cores
{
    public class DamageTypeCore
    {
        private UInt32 currentMax = 0u;

        public static DamageTypeCore instance;

        public static class ModdedDamageTypes
        {
            public static DamageAPI.ModdedDamageType CyborgPrimary; //Used for Cyborg Shock Rifle combo
            public static DamageAPI.ModdedDamageType ScaleForceToMass;
            public static DamageAPI.ModdedDamageType GroundedForceCorrection;   //Used to fix scaled knockback force attacks on grounded enemies.
            public static DamageAPI.ModdedDamageType GougeOnHit;
            public static DamageAPI.ModdedDamageType ExtendFear;
            public static DamageAPI.ModdedDamageType GuaranteedFearOnHit;   //Used for Exe Scepter
            public static DamageAPI.ModdedDamageType ResetVictimForce;
            public static DamageAPI.ModdedDamageType ErraticGadget;
            public static DamageAPI.ModdedDamageType SlayerExceptItActuallyWorks;
            public static DamageAPI.ModdedDamageType AntiFlyingForce;
            public static DamageAPI.ModdedDamageType Root3s;
        }

        //public static DamageType
        //OnHit;

        public static float gougeDamageCoefficient = 2f;    //gouge DPS

        public DamageTypeCore()
        {
            instance = this;

            ModdedDamageTypes.GroundedForceCorrection = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.AntiFlyingForce = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.SlayerExceptItActuallyWorks = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ResetVictimForce = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.CyborgPrimary = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ScaleForceToMass = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.GougeOnHit = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ExtendFear = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.GuaranteedFearOnHit = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ErraticGadget = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.Root3s = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            if (NetworkServer.active && !damageInfo.rejected)
            {

                if (victim)
                {
                    if (damageInfo.HasModdedDamageType(ModdedDamageTypes.GougeOnHit))
                    {
                        //Supposed to have a 0.25 mult for enemy Nemmando.
                        var dotInfo = new InflictDotInfo()
                        {
                            attackerObject = damageInfo.attacker,
                            victimObject = victim,
                            dotIndex = DoTCore.NemmandoGouge,
                            duration = 2,
                            damageMultiplier = DamageTypeCore.gougeDamageCoefficient
                        };
                        DotController.InflictDot(ref dotInfo);
                    }

                    CharacterBody victimBody = victim.GetComponent<CharacterBody>();
                    if (victimBody)
                    {
                        if (damageInfo.HasModdedDamageType(ModdedDamageTypes.ExtendFear))
                        {
                            if (victimBody && victimBody.HasBuff(BuffCore.fearDebuff))
                            {
                                victimBody.AddTimedBuff(BuffCore.fearDebuff, EntityStates.SS2UStates.Executioner.ExecutionerDash.debuffDuration);
                            }
                        }

                        if (damageInfo.HasModdedDamageType(ModdedDamageTypes.Root3s))
                        {
                            victimBody.AddTimedBuff(RoR2Content.Buffs.LunarSecondaryRoot, 3f);
                        }
                    }
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool triggerGougeProc = false;
            if (NetworkServer.active)
            {
                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.SlayerExceptItActuallyWorks))
                {
                    damageInfo.RemoveModdedDamageType(ModdedDamageTypes.SlayerExceptItActuallyWorks);
                    damageInfo.damage *= Mathf.Lerp(3f, 1f, self.combinedHealthFraction);
                }

                CharacterBody cb = self.body;
                //This will only work on things that are run on the server.
                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.ResetVictimForce))
                {
                    if (cb.rigidbody)
                    {
                        cb.rigidbody.velocity = new Vector3(0f, cb.rigidbody.velocity.y, 0f);
                        cb.rigidbody.angularVelocity = new Vector3(0f, cb.rigidbody.angularVelocity.y, 0f);
                    }
                    if (cb.characterMotor != null)
                    {
                        cb.characterMotor.velocity.x = 0f;
                        cb.characterMotor.velocity.z = 0f;
                        cb.characterMotor.rootMotion.x = 0f;
                        cb.characterMotor.rootMotion.z = 0f;
                    }
                }

                if (damageInfo.dotIndex == DoTCore.NemmandoGouge && damageInfo.procCoefficient == 0f)
                {
                    CharacterBody attackerBody = null;
                    if (damageInfo.attacker) attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (attackerBody)
                    {
                        damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                    }
                    damageInfo.procCoefficient = 0.7f;
                    triggerGougeProc = true;
                }

                if (damageInfo.dotIndex == DoTCore.StrangeCanPoison)
                {
                    damageInfo.damage = self.combinedHealth * 0.03f;
                    EffectManager.SimpleEffect(DoTCore.StrangeCanHitEffect, damageInfo.position, self.transform.rotation, true);
                }

                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.GuaranteedFearOnHit))
                {
                    self.body.AddTimedBuff(BuffCore.fearDebuff, EntityStates.SS2UStates.Executioner.ExecutionerDash.debuffDuration);
                }

                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.GroundedForceCorrection))
                {
                    if (cb.characterMotor != null && cb.characterMotor.isGrounded)
                    {
                        if (damageInfo.force.y <= 1200f) damageInfo.force.y = 1200f;
                    }
                }

                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.ScaleForceToMass))
                {
                    if (!cb.isFlying && cb.characterMotor != null)
                    {
                        if (!cb.characterMotor.isGrounded)    //Multiply launched enemy force
                        {
                            if (cb.isChampion)
                            {
                                damageInfo.force.x *= 0.7f;
                                damageInfo.force.z *= 0.7f;
                            }
                        }
                        else
                        {
                            if (cb.isChampion) //deal less knockback against bosses if they're on the ground
                            {
                                damageInfo.force.x *= 0.5f;
                                damageInfo.force.z *= 0.5f;
                            }
                        }
                    }

                    if (cb.rigidbody)
                    {
                        float forceMult = Mathf.Max(cb.rigidbody.mass / 100f, 1f);
                        damageInfo.force *= forceMult;
                    }
                }

                //Based off of RiskyMod
                if (damageInfo.HasModdedDamageType(ModdedDamageTypes.AntiFlyingForce))
                {
                    float downwardsForce = -1600f;
                    if (cb && cb.isFlying)
                    {
                        //Scale force to match mass
                        Rigidbody rb = cb.rigidbody;
                        if (rb)
                        {
                            downwardsForce *= Mathf.Min(4f, Mathf.Max(rb.mass / 100f, 1f));  //Greater Wisp 300f, SCU 1000f
                            if (damageInfo.force.y > downwardsForce) damageInfo.force.y = downwardsForce;
                        }
                    }
                }
            }

            orig(self, damageInfo);

            if (NetworkServer.active && !damageInfo.rejected)
            {
                if (triggerGougeProc)
                {
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, self.gameObject);
                    //GlobalEventManager.instance.OnHitAll(damageInfo, self.gameObject);
                }
            }
        }
    }
}