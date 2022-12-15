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

//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.SS2UStates.Chirr
{
    public class ChirrHeadbutt : BasicMeleeAttack, SteppedSkillDef.IStepSetter
    {
        public static float damageCoefficient = 5f;
        public float baseDuration = 0.6f;
        public float recoil = 1f;
        public int step;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");
        public GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public GameObject critEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/critspark");

        //private float duration;
        //private float fireDuration;
        //private bool firstShot = false;
        //private bool secondShot = false;
        //private bool thirdShot = false;
        private Animator animator;
        //private string muzzleString;
        private BullseyeSearch search = new BullseyeSearch();
        private float forceMagnitude = 7000;


        public override void OnEnter()
        {
            this.hitBoxGroupName = "HeadbuttHitbox";
            //base.baseDuration = BladeOfCessation.baseDurationNormal;
            base.duration = base.baseDuration / base.attackSpeedStat;
            base.hitPauseDuration = 0f;
            base.damageCoefficient = damageCoefficient;
            base.procCoefficient = 1.0f;

            //base.mecanimHitboxActiveParameter = "Primary.Hitbox";
            //this.effectTime = duration * baseEffectTime;
            base.PlayAnimation("Gesture, Override", "Secondary", "Secondary.playbackRate", this.baseDuration / base.attackSpeedStat);
            base.PlayAnimation("Gesture, Additive", "Secondary", "Secondary.playbackRate", this.baseDuration / base.attackSpeedStat);
            //Util.PlaySound(EntityStates.BeetleMonster.HeadbuttState.attackSoundString, base.gameObject);
            Util.PlaySound(EntityStates.Merc.Weapon.GroundLight2.slash1Sound, base.gameObject);
            //base.characterDirection.forward = base.GetAimRay().direction;

            base.OnEnter();

            base.overlapAttack.forceVector = (base.characterDirection ? (base.characterDirection.forward * forceMagnitude) : Vector3.zero);
            base.overlapAttack.damageType = DamageType.Stun1s;
            //base.overlapAttack.hitEffectPrefab = EntityStates.Bison.Charge.hitEffectPrefab;
            base.overlapAttack.hitEffectPrefab = Starstorm2.Modules.Assets.nemImpactFX;
            //base.characterDirection.forward = base.GetAimRay().direction;
        }

        void SteppedSkillDef.IStepSetter.SetStep(int i)
        {
            this.step = i;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireTrackshot()
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode();

            /*if ((base.fixedAge >= this.fireDuration) && !firstShot)
            {
                FireTrackshot();
                firstShot = true;
            }
            if ((base.fixedAge >= this.fireDuration * 2) && !secondShot)
            {
                FireTrackshot();
                secondShot = true;
            }
            if ((base.fixedAge >= this.fireDuration * 3) && !thirdShot)
            {
                FireTrackshot();
                thirdShot = true;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }*/
        }
        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write((byte)this.step);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.step = (int)reader.ReadByte();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}