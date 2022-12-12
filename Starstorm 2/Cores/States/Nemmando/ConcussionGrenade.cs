using EntityStates;
using EntityStates.Huntress;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando
{
    class ConcussionGrenade : BaseSkillState
    {
        private float baseDuration = 0.4f;
        private float baseExplosionFuse = 0.4f;
        private float explosionHeigth = 5f;
        private float explosionRadius = 10f;
        private Vector3 initialPosition;
        private EffectData concussionEffect;
        private GameObject concussionEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/omnieffect/OmniExplosionVFX");

        public override void OnEnter()
        {
            base.OnEnter();
            this.initialPosition = this.characterBody.transform.position + Vector3.up * this.explosionHeigth;

            this.concussionEffect = new EffectData()
            {
                color = new Color32(242, 217, 216, 255),
                scale = explosionRadius,
                origin = initialPosition
            };
        }

        public override void OnExit()
        {
            EffectManager.SpawnEffect(this.concussionEffectPrefab, concussionEffect, false);

            BlastAttack blast = new BlastAttack()
            {
                radius = explosionRadius,
                procCoefficient = 0.1f,
                position = this.initialPosition,
                attacker = base.gameObject,
                teamIndex = TeamIndex.Player,
                crit = RollCrit(),
                baseDamage = (base.characterBody.damage * 0.1f),
                damageColorIndex = DamageColorIndex.Default,
                falloffModel = BlastAttack.FalloffModel.None,
                damageType = DamageType.Generic | DamageType.Stun1s               
            };
            blast.teamIndex = TeamComponent.GetObjectTeam(blast.attacker);
            blast.Fire();

            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.isAuthority && this.fixedAge > this.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }                
        }


        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}

