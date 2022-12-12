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
using RoR2.CharacterAI;
using UnityEngine;
using Starstorm2.Cores;
using Starstorm2.Cores.States.Chirr;
using UnityEngine.Networking;
using KinematicCharacterController;
using UnityEngine.Scripting;
using UnityEngineInternal;
using UnityEngine.Events;
using UnityEngine.Serialization;



//FIXME: ion gun doesn't build charges in mp if player is not host
//FIXME: ion burst stocks do not carry over between stages (may leave this as feature)

namespace EntityStates.Chirr
{
    public class ChirrBefriend : BaseSkillState
    {
        public static float damageCoefficient = 3.0f;
        public float baseDuration = 0.8f;
        public float recoil = 1f;
        public static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerHuntressSnipe");
        public GameObject effectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/HitsparkCommandoShotgun");
        public GameObject critEffectPrefab = LegacyResourcesAPI.Load<GameObject>("prefabs/effects/impacteffects/critspark");

        private float duration;
        private float fireDuration;
        private bool firstShot = false;
        private bool secondShot = false;
        private bool thirdShot = false;
        private Animator animator;
        private string muzzleString;
        private BullseyeSearch search = new BullseyeSearch();
        private HurtBox futureFriend;


        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.fireDuration = 0.25f * this.duration;
            base.characterBody.SetAimTimer(2f);
            this.animator = base.GetModelAnimator();
            this.muzzleString = "Lowerarm.L_end";


            base.PlayAnimation("Gesture, Override", "Special", "Special.playbackRate", this.duration);
            base.PlayAnimation("Gesture, Additive", "Special", "Special.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        /*public Action UpdateItems()
        {
            return new Action;
        }*/
        private void FireTrackshot()
        {
            if (!characterBody.GetComponent<ChirrInfoComponent>().friend)
            {
                futureFriend = characterBody.GetComponent<ChirrInfoComponent>().futureFriend;
                CharacterBody newFriend;  
                TeamComponent team = characterBody.GetComponent<TeamComponent>();

                if (futureFriend && NetworkServer.active)
                {
                    newFriend = futureFriend.healthComponent.body;
                    newFriend.teamComponent.teamIndex = team.teamIndex;
                    newFriend.master.teamIndex = team.teamIndex;
                    //Chat.AddMessage(newFriend.master.aiComponents.ToString());
                    if (newFriend.master.GetComponent<BaseAI>())
                    {
                        newFriend.healthComponent.Heal(newFriend.healthComponent.fullHealth, new ProcChainMask());
                        newFriend.healthComponent.RechargeShieldFull();
                        BaseAI friend = newFriend.master.GetComponent<BaseAI>();
                        friend.currentEnemy.Reset();
                        //friend.ForceAcquireNearestEnemyIfNoCurrentEnemy();
                        friend.leader.gameObject = base.gameObject;
                        newFriend.master.inventory.GetComponent<MinionOwnership>().SetOwner(base.characterBody.master);
                        characterBody.GetComponent<ChirrInfoComponent>().baseInventory = new Inventory();
                        characterBody.GetComponent<ChirrInfoComponent>().baseInventory.CopyItemsFrom(newFriend.master.inventory);
                        newFriend.master.inventory.AddItemsFrom(base.characterBody.inventory);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.WardOnLevel.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.BeetleGland.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.CrippleWardOnLevel.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.TPHealingNova.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.FocusConvergence.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.TitanGoldDuringTP.itemIndex);
                        newFriend.master.inventory.ResetItem(RoR2Content.Items.ExtraLife.itemIndex);
                        //deployable.onUndeploy.AddListener(new UnityAction(characterMaster.TrueKill));
                        //i will probably need to addlistener later on, i leave this here to remember about it

                        friend.UpdateTargets();
                        characterBody.GetComponent<ChirrInfoComponent>().friend = newFriend;
                    }
                }
                else if (futureFriend && base.isAuthority)
                {
                    newFriend = futureFriend.healthComponent.body;
                    //Chat.AddMessage(newFriend.master.aiComponents.ToString());
                    if (newFriend.master.GetComponent<BaseAI>())
                    {
                        characterBody.GetComponent<ChirrInfoComponent>().friend = newFriend;
                    }
                }
            }
            /*if ((bool)target)
            {
                targetV = target.transform.position - aimRay.origin;

            }

            Util.PlaySound(Commando.CommandoWeapon.FirePistol2.firePistolSoundString, base.gameObject);
            if (base.isAuthority)
            {
                new BulletAttack
                {
                    owner = base.gameObject,
                    weapon = base.gameObject,
                    origin = aimRay.origin,
                    aimVector = targetV.normalized,
                    minSpread = 0,
                    maxSpread = 0,
                    damage = damageCoefficient * this.damageStat,
                    force = 100,
                    tracerEffectPrefab = CyborgFireTrackshot.tracerEffectPrefab,
                    muzzleName = muzzleString,
                    hitEffectPrefab = (Util.CheckRoll(this.critStat, base.characterBody.master)) ? critEffectPrefab : effectPrefab,
                    isCrit = Util.CheckRoll(this.critStat, base.characterBody.master),
                    damageType = DamageType.Stun1s
                }.Fire();
                //ProjectileManager.instance.FireProjectile(ExampleSurvivor.ExampleSurvivor.bfgProjectile, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), base.gameObject, this.damageCoefficient * this.damageStat, 0f, Util.CheckRoll(this.critStat, base.characterBody.master), DamageColorIndex.Default, null, -1f);
            }*/
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((base.fixedAge >= this.fireDuration) && !firstShot)
            {
                FireTrackshot();
                firstShot = true;
            }
            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                //base.GetComponent<ChirrInfoComponent>().friendState = true;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }

    public class ChirrLeash : BaseSkillState
    {
        public float baseDuration = 0.5f;

        private float duration;
        private float fireDuration;
        private bool hasFired;
        private Animator animator;
        private string muzzleString;
        private DamageInfo dmg;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration;
            this.fireDuration = 0.25f * this.duration;
            this.animator = base.GetModelAnimator();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireShot()
        {
            if (!this.hasFired)
            {
                Vector3 pos = (((base.GetComponent<ChirrInfoComponent>().friend.footPosition - base.characterBody.footPosition).normalized) * 10 + base.characterBody.footPosition);
                pos.y = base.characterBody.footPosition.y + 2;

                //Chat.AddMessage(base.GetComponent<ChirrInfoComponent>().friend.ToString());
                TeleportHelper.TeleportBody(base.GetComponent<ChirrInfoComponent>().friend, pos);
                hasFired = true;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!inputBank.skill4.down)
            {
                FireShot();
            }

            if ((base.fixedAge >= this.duration && base.isAuthority))
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Any;
        }
    }
}