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

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrHeal : BaseSkillState
    {
        public float damageCoefficient = 1.85f;
        public float baseDuration = 1f;
        public float recoil = 1f;
        public float radius = 30f;
        public float healFraction = .25f;
        public float regenDuration = 6f;

        private float duration;
        private float fireDuration;
        private bool hasFired;


        public override void OnEnter()
        {
            base.OnEnter();
            Util.PlaySound("Play_voidman_R_activate", base.gameObject);
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.5f * this.duration;

            base.PlayAnimation("Gesture, Override", "Utility", "Utility.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Utility", "Utility.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireHeal()
        {
            if (!this.hasFired)
            {
                Util.PlaySound("Play_voidman_R_pop", base.gameObject);
                hasFired = true;
                if (NetworkServer.active)
                {
                    GameObject vfx = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("prefabs/effects/TPHealNovaEffect"), base.transform);
                    NetworkServer.Spawn(vfx);
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
                                    if (component.body && component.body != base.characterBody)
                                        component.body.AddTimedBuff(RoR2Content.Buffs.CrocoRegen, regenDuration);
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
                FireHeal();
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
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