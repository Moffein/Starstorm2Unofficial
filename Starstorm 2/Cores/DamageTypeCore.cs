﻿using R2API;
using R2API.Utils;
using RoR2;
using Starstorm2Unofficial.Cores.Items;
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
            public static DamageAPI.ModdedDamageType CyborgCanDetonateShockCore; //Used for Cyborg Shock Rifle combo
            public static DamageAPI.ModdedDamageType ScaleForceToMass;
            public static DamageAPI.ModdedDamageType GroundedForceCorrection;   //Used to fix scaled knockback force attacks on grounded enemies.
            public static DamageAPI.ModdedDamageType GougeOnHit;
            public static DamageAPI.ModdedDamageType ExtendFear;
            public static DamageAPI.ModdedDamageType GuaranteedFearOnHit;   //Used for Exe Scepter
            public static DamageAPI.ModdedDamageType ResetVictimForce;
            public static DamageAPI.ModdedDamageType ErraticGadget;
            public static DamageAPI.ModdedDamageType SlayerExceptItActuallyWorks;
            public static DamageAPI.ModdedDamageType AntiFlyingForce;
            public static DamageAPI.ModdedDamageType Root5s;
            public static DamageAPI.ModdedDamageType NucleatorRadiationOnHit;
        }

        //public static DamageType
        //OnHit;

        public static float gougeDamageCoefficient = 1.5f;    //gouge DPS

        public DamageTypeCore()
        {
            instance = this;

            ModdedDamageTypes.GroundedForceCorrection = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.AntiFlyingForce = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.SlayerExceptItActuallyWorks = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ResetVictimForce = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.CyborgCanDetonateShockCore = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ScaleForceToMass = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.GougeOnHit = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ExtendFear = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.GuaranteedFearOnHit = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.ErraticGadget = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.Root5s = DamageAPI.ReserveDamageType();
            ModdedDamageTypes.NucleatorRadiationOnHit = DamageAPI.ReserveDamageType();

            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
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

                        if (damageInfo.HasModdedDamageType(ModdedDamageTypes.Root5s))
                        {
                            victimBody.AddTimedBuff(RoR2Content.Buffs.LunarSecondaryRoot, 5f);
                        }

                        if (damageInfo.HasModdedDamageType(ModdedDamageTypes.NucleatorRadiationOnHit) && victimBody.bodyIndex != Survivors.Nucleator.NucleatorCore.bodyIndex)
                        {
                            //Remove existing Radiation DoT

                            var dotInfo = new InflictDotInfo()
                            {
                                attackerObject = damageInfo.attacker,
                                victimObject = victim,
                                dotIndex = DoTCore.NucleatorRadiation,
                                duration = 5,
                                damageMultiplier = 1,
                                maxStacksFromAttacker = 1
                            };
                            DotController.InflictDot(ref dotInfo);
                        }
                    }
                }
            }
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
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

                CharacterBody attackerBody = null;
                if (damageInfo.attacker) attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
                if (damageInfo.dotIndex == DoTCore.NemmandoGouge && damageInfo.procCoefficient == 0f)
                {
                    if (attackerBody)
                    {
                        damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                    }
                    damageInfo.procCoefficient = 0.5f;
                    damageInfo.procChainMask.AddProc(ProcType.Rings);   //Don't let this proc bands.
                    triggerGougeProc = true;
                }
                else if (damageInfo.dotIndex == DoTCore.StrangeCanPoison)
                {
                    float minDamage = 1f;
                    float percentDamage = self.combinedHealth * 0.025f;
                    if (attackerBody)
                    {
                        minDamage = 100f * (percentDamage / self.fullCombinedHealth) * attackerBody.damage;
                    }

                    damageInfo.damage = Mathf.Max(percentDamage, minDamage);
                    EffectManager.SimpleEffect(DoTCore.StrangeCanHitEffect, damageInfo.position, self.transform.rotation, true);
                }
                else if (damageInfo.dotIndex == DoTCore.DetritiveTrematodeInfection)
                {
                    //DoT disappears if target heals above 25% HP
                    if (self.combinedHealthFraction > 0.25f)
                    {
                        damageInfo.damage = 0f;
                        damageInfo.rejected = true;

                        DotController dot = DotController.FindDotController(self.gameObject);
                        if (dot)
                        {
                            int trematodeIndex = -1;
                            for (int i = 0; i < dot.dotStackList.Count; i++)
                            {
                                if (dot.dotStackList[i].dotIndex == DoTCore.DetritiveTrematodeInfection)
                                {
                                    trematodeIndex = i;
                                    break;
                                }
                            }
                            if (trematodeIndex >= 0) dot.RemoveDotStackAtServer(trematodeIndex);
                        }
                    }
                    else
                    {
                        EffectManager.SimpleEffect(DoTCore.TrematodeHitEffect, damageInfo.position, self.transform.rotation, true);
                    }
                }
                else if (damageInfo.dotIndex == DoTCore.NucleatorRadiation)
                {
                    damageInfo.damage = Mathf.Max(damageInfo.damage, self.fullCombinedHealth * 0.01f);

                    EffectManager.SpawnEffect(DoTCore.TrematodeHitEffect, new EffectData
                    {
                        origin = damageInfo.position,
                        rotation = self.transform.rotation,
                        scale = Mathf.Max(1f, Mathf.Min(4f, cb.radius))
                    }, true);
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
                        if (cb.isFlying) forceMult = Mathf.Min(forceMult, 7.5f);

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