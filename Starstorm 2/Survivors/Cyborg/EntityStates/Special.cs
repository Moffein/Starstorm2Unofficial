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
using Starstorm2.Survivors.Cyborg.Components;

namespace EntityStates.Starstorm2States.Cyborg
{
    public class CyborgTeleport : BaseSkillState
    {
        public float damageCoefficient = 16f;
        public float baseDuration = 0.5f;
        public float recoil = 1f;

        private float duration;
        private float fireDuration;
        private bool hasTpd;
        private Animator animator;
        private string muzzleString;
        private CyborgInfoComponent TPInfo;


        public override void OnEnter()
        {
            base.OnEnter();
            TPInfo = base.GetComponent<CyborgInfoComponent>();

            if (!TPInfo.isHooked)
            {
                On.RoR2.Stage.Start += (orig, self) =>
                {
                    orig(self);
                    TPInfo.tpReady = false;
                };
                TPInfo.isHooked = true;
            }

            //TeleportInfoComponent.tpPos;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            this.animator = base.GetModelAnimator();
            if (TPInfo.tpReady)
            {
                base.PlayAnimation("Gesture, Override", "UseTP", "FireM1.playbackRate", this.duration);
            }
            else
            {
                base.PlayAnimation("Gesture, Override", "CreateTP", "FireM1.playbackRate", this.duration);
            }
        }

        private void SetTp()
        {
            //Chat.AddMessage("TP set!");
            //TPInfo.tpPos = base.characterBody.transform.position + new Vector3(0, 1, 0);
            string soundString = "CyborgSpecialPlace";//base.effectComponent.shootSound;
            Util.PlaySound(soundString, base.gameObject);
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    crit = base.RollCrit(),
                    damage = this.damageStat * damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    force = 0f,
                    owner = base.gameObject,
                    position = aimRay.origin,
                    procChainMask = default(ProcChainMask),
                    projectilePrefab = CyborgCore.cyborgPylon,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    target = null
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }
        }

        private void UseTp()
        {
            //Chat.AddMessage("Teleported!");
            string soundString = "CyborgSpecialTeleport";//base.effectComponent.shootSound;
            Util.PlaySound(soundString, base.gameObject);
            if (base.characterMotor)
            {
                //base.characterMotor.Motor.SetPositionAndRotation(TPInfo.tpPos, Quaternion.identity, true);
            }
            List<ProjectileController> instancesList = InstanceTracker.GetInstancesList<ProjectileController>();
            int count = instancesList.Count;
            for (int i = 0; i < count; i++)
            {
                if ((instancesList[i].owner == base.gameObject) && (instancesList[i].name == "Prefabs/Projectiles/CyborgTPPylon(Clone)"))
                {
                    base.characterMotor.Motor.SetPositionAndRotation(instancesList[i].transform.position, Quaternion.identity, true);
                    instancesList[i].GetComponent<ProjectileImpactExplosion>().lifetime = 0;
                }
                    //Chat.AddMessage(instancesList[i].name);
            }

            base.skillLocator.primary.RunRecharge(4);
            base.skillLocator.secondary.RunRecharge(4);
            base.skillLocator.utility.RunRecharge(4);
        }

        public override void OnExit()
        {
            base.OnExit();
        }



        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.fireDuration) && !hasTpd)
            {
                if (!TPInfo.tpReady)
                    SetTp();
                else
                    UseTp();
                hasTpd = true;
                TPInfo.tpReady = !TPInfo.tpReady;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}