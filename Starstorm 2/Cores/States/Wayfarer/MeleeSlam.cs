﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using RoR2;
using EntityStates;

namespace Starstorm2Unofficial.Cores.States.Wayfarer
{
    class MeleeSlam : BaseSkillState
    {
        public static float baseDuration = 2.0f;
        //***
        public static float damageCoefficient = 4.0f;
        public static float force = 10.0f;
        public static float radius = 15.0f;
        public static GameObject explosionPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");

        private Animator animator;
        private float duration;
        private EffectData effectData;
        private BlastAttack attack;
        private ChildLocator locator;
        private bool hasAttackedL;
        private bool hasAttackedR;

        public override void OnEnter()
        {
            base.OnEnter();

            animator = base.GetModelAnimator();
            this.duration = baseDuration / attackSpeedStat;
            effectData = new EffectData();
            effectData.scale = radius;

            base.PlayCrossfade("FullBody, Override", "Melee", "Melee.playbackRate", duration, 0.2f);

            attack = new BlastAttack();
            attack.attacker = base.gameObject;
            attack.inflictor = base.gameObject;
            attack.baseDamage = this.damageStat * damageCoefficient;
            attack.baseForce = force;
            attack.radius = radius;
            attack.teamIndex = TeamComponent.GetObjectTeam(base.gameObject);

            locator = base.GetModelTransform().GetComponent<ChildLocator>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
            {
                if (!hasAttackedL && this.animator.GetFloat("MeleeL.active") > 0.5)
                {
                    hasAttackedL = true;
                    DoAttack("LanternL");
                }
                else if (!hasAttackedR && this.animator.GetFloat("MeleeR.active") > 0.5)
                {
                    hasAttackedR = true;
                    DoAttack("LanternR");
                }
            }

            if (base.fixedAge >= duration)
            {
                outer.SetNextStateToMain();
            }
        }

        private void DoAttack(string childName)
        {
            Vector3 orig = locator.FindChild(childName).position;
            effectData.origin = orig;
            EffectManager.SpawnEffect(explosionPrefab, effectData, true);
            if (base.isAuthority)
            {
                Debug.Log(orig);
                attack.position = orig;
                attack.Fire();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
