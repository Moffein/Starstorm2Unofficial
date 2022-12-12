using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using R2API;
using R2API.Utils;
using EntityStates;
using RoR2;
using RoR2.Skills;
using RoR2.Projectile;
using UnityEngine;
using Starstorm2.Cores;
using UnityEngine.Networking;
using KinematicCharacterController;
using JetBrains.Annotations;
using RoR2.Networking;
using System.Collections.ObjectModel;

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Chirr
{
    public class ChirrHeal : BaseSkillState
    {
        public float damageCoefficient = 1.85f;
        public float baseDuration = 1f;
        public float recoil = 1f;
        public float radius = 30f;
        public float healFraction = .15f;

        private float duration;
        private float fireDuration;
        private bool hasFired;


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.5f * this.duration;
            base.characterBody.SetAimTimer(2f);


            base.PlayAnimation("Gesture, Override", "Utility", "Utility.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Utility", "Utility.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireBFG()
        {
            if (!this.hasFired)
            {
                GameObject ptr = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/TPHealNovaEffect"), base.transform);
                //ptr.GetComponent<TeamFilter>().teamIndex = teamIndex;
                NetworkServer.Spawn(ptr);
                hasFired = true;
                if (NetworkServer.active)
                {
                    ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamComponent.teamIndex);
                    float num = this.radius * this.radius;
                    Vector3 position = base.transform.position;
                    for (int i = 0; i < teamMembers.Count; i++)
                    {
                        if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
                        {
                            HealthComponent component = teamMembers[i].GetComponent<HealthComponent>();
                            if (component)
                            {
                                float num2 = component.fullHealth * this.healFraction;
                                if (num2 > 0f)
                                {
                                    component.Heal(num2, default(ProcChainMask), true);
                                    if (component.body)
                                        component.body.AddTimedBuff(RoR2Content.Buffs.CrocoRegen, 1);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireBFG();
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}