using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.ObjectModel;
using UnityEngine.AddressableAssets;

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrHeal : BaseSkillState
    {
        public static GameObject healEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/AffixEarthHealExplosion.prefab").WaitForCompletion();
        public static float baseDuration = 1.5f;
        public static float baseFireDuration = 1.3f;
        public static float recoil = 1f;
        public static float radius = 30f;
        public static float healFraction = .25f;
        public static float regenDuration = 3f;

        private float duration;
        private float fireDuration;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            this.fireDuration = baseFireDuration / this.attackSpeedStat;
            Util.PlayAttackSpeedSound("ChirrHealStart", base.gameObject, this.attackSpeedStat);

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
                Util.PlaySound("ChirrHealTrigger", base.gameObject);
                hasFired = true;
                if (NetworkServer.active)
                {
                    EffectManager.SpawnEffect(healEffectPrefab, new EffectData
                    {
                        origin = base.transform.position,
                        scale = ChirrHeal.radius,
                        rootObject = base.gameObject
                    }, true);
                    ReadOnlyCollection<TeamComponent> teamMembers = TeamComponent.GetTeamMembers(teamComponent.teamIndex);
                    float num = radius * radius;
                    Vector3 position = base.transform.position;
                    for (int i = 0; i < teamMembers.Count; i++)
                    {
                        if ((teamMembers[i].transform.position - position).sqrMagnitude <= num)
                        {
                            HealthComponent component = teamMembers[i].GetComponent<HealthComponent>();
                            if (component)
                            {
                                float num2 = component.fullHealth * healFraction;
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
            return InterruptPriority.PrioritySkill;
        }
    }
}