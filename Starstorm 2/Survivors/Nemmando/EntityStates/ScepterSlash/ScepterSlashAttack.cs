using EntityStates;
using EntityStates.SS2UStates.Common;
using RoR2;
using RoR2.Orbs;
using R2API;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Survivors.Nemmando.Components;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using Starstorm2Unofficial.Cores;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ScepterSlashAttack : BaseCustomSkillState
    {
        public float charge;

        public static float baseDuration = 2.5f;
        public static int baseHitCount = 12;
        public static float damageCoefficient = 3.6f;
        public static float radius = 64f;
        public static float radiusEnemy = 32f;  //Nerf this on NPCs
        public static float swordEmission = 350f;

        private float hitStopwatch;
        private int hitCount;
        private int hitsFired;
        private float duration;
        private float emission;
        private EffectData attackEffect;
        private Material swordMat;
        private NemmandoController nemmandoController;
        private CharacterModel characterModel;
        private float minimumEmission;
        private bool isCrit;
        private bool hidden;
        public CameraTargetParams.CameraParamsOverrideHandle camOverrideHandle;
        private CharacterCameraParamsData decisiveCameraParams = new CharacterCameraParamsData
        {
            maxPitch = 70f,
            minPitch = -70f,
            pivotVerticalOffset = 1f, //how far up should the camera go?
            idealLocalCameraPos = zoomCameraPosition,
            wallCushion = 0.1f
        };
        public static Vector3 zoomCameraPosition = new Vector3(0f, 0f, -5.3f); // how far back should the camera go?

        private List<HurtBox> targetList;

        private float lastUpdateTime;

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            base.characterBody.isSprinting = false;
            this.hitsFired = 0;
            this.hitCount = (int)(ScepterSlashAttack.baseHitCount * this.attackSpeedStat);
            this.duration = ScepterSlashAttack.baseDuration;// this.attackSpeedStat;
            this.nemmandoController = base.GetComponent<NemmandoController>();
            this.emission = ScepterSlashAttack.swordEmission;
            this.isCrit = base.RollCrit();
            this.hidden = true;
            this.characterModel = base.modelLocator.modelTransform.GetComponent<CharacterModel>();
            base.characterBody.hideCrosshair = true;

            if (base.characterBody.skinIndex == 2) this.minimumEmission = 70f;
            else this.minimumEmission = 0f;

            this.attackEffect = new EffectData()
            {
                scale = 0.5f * ScepterSlashAttack.radius,
                origin = base.characterBody.corePosition
            };

            base.characterMotor.rootMotion = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;

            EffectManager.SpawnEffect(Starstorm2Unofficial.Modules.Assets.nemChargedSlashStartFX, this.attackEffect, true);

            base.PlayAnimation("FullBody, Override", "DecisiveStrike", "DecisiveStrike.playbackRate", this.duration);
            Util.PlaySound("SS2UNemmandoDecisiveStrikeFire", base.gameObject);

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);

            this.swordMat = base.GetModelTransform().GetComponent<ModelSkinController>().skins[base.characterBody.skinIndex].rendererInfos[1].defaultMaterial;

            this.GetTargets();

            if (this.characterModel) this.characterModel.invisibilityCount++;

            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle, .25f);
                CameraTargetParams.CameraParamsOverrideRequest request = new CameraTargetParams.CameraParamsOverrideRequest
                {
                    cameraParamsData = decisiveCameraParams,
                    priority = 0f
                };
                camOverrideHandle = cameraTargetParams.AddParamsOverride(request, duration);
            }
        }

        private void FireAttack()
        {
            this.hitsFired++;

            if (this.targetList == null) return;

            this.hitStopwatch = 0.05f;

            foreach (HurtBox i in this.targetList)
            {
                if (i)
                {
                    HurtBoxGroup hurtboxGroup = i.hurtBoxGroup;
                    HurtBox hurtbox = hurtboxGroup.hurtBoxes[Random.Range(0, hurtboxGroup.hurtBoxes.Length - 1)];
                    if (hurtbox && hurtbox.healthComponent.alive)
                    {
                        Util.PlaySound(this.effectComponent.impactSoundDef.eventName, i.gameObject);


                        DamageInfo damageInfo = new DamageInfo();
                        damageInfo.damage = ScepterSlashAttack.damageCoefficient * this.damageStat;
                        damageInfo.attacker = base.gameObject;
                        damageInfo.procCoefficient = 1f;
                        damageInfo.position = hurtbox.transform.position;
                        damageInfo.crit = this.isCrit;
                        damageInfo.damageType = DamageType.Generic;
                        damageInfo.damageType.damageSource = DamageSource.Special;
                        damageInfo.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.GougeOnHit);

                        hurtbox.healthComponent.TakeDamage(damageInfo);

                        GlobalEventManager.instance.OnHitEnemy(damageInfo, hurtbox.healthComponent.gameObject);
                        GlobalEventManager.instance.OnHitAll(damageInfo, hurtbox.healthComponent.gameObject);

                        EffectData effect = new EffectData
                        {
                            scale = 4f,
                            origin = hurtbox.transform.position,
                            rotation = hurtbox.transform.rotation
                        };

                        EffectManager.SpawnEffect(Starstorm2Unofficial.Modules.Assets.nemScepterImpactFX, effect, false);
                    }
                }
            }
        }

        private void GetTargets()
        {
            this.targetList = new List<HurtBox>();

            Ray aimRay = base.GetAimRay();

            SphereSearch search = new SphereSearch();
            search.mask = LayerIndex.entityPrecise.mask;

            search.ClearCandidates();
            search.origin = base.transform.position;
            search.radius = ScepterSlashAttack.radius;
            search.RefreshCandidates();
            search.FilterCandidatesByDistinctHurtBoxEntities();
            search.FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.teamComponent.teamIndex));
            search.GetHurtBoxes(this.targetList);

            if (base.GetTeam() != TeamIndex.Player && !(base.characterBody && base.characterBody.isPlayerControlled))
            {
                search.radius = ScepterSlashAttack.radiusEnemy;
            }

            if (NetworkServer.active)
            {
                foreach (HurtBox i in this.targetList)
                {
                    if (i.healthComponent != this.healthComponent)
                    {
                        if (i.healthComponent.alive)
                        {
                            Starstorm2Unofficial.Modules.Orbs.NemmandoDashOrb dashOrb = new Starstorm2Unofficial.Modules.Orbs.NemmandoDashOrb();
                            dashOrb.origin = base.transform.position;
                            dashOrb.target = i;
                            OrbManager.instance.AddOrb(dashOrb);
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            this.hitStopwatch -= deltaTime;
            this.emission -= 10f * deltaTime;
            if (this.emission < 0f) this.emission = 0f;

            if (this.swordMat) this.swordMat.SetFloat("_EmPower", Util.Remap(base.fixedAge, 0, this.duration, this.emission, this.minimumEmission));

            base.characterMotor.rootMotion = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;

            if (base.fixedAge >= 0.4f * this.duration && this.hidden)
            {
                this.hidden = false;
                if (this.characterModel) this.characterModel.invisibilityCount--;
                base.PlayAnimation("FullBody, Override", "ScepterSpecial", "DecisiveStrike.playbackRate", 1f);
            }

            if (base.fixedAge >= this.duration && this.hitsFired < this.hitCount && this.hitStopwatch <= 0f)
            {
                if (this.nemmandoController) this.nemmandoController.UncoverScreen();

                if (this.hitsFired == 0) base.PlayAnimation("FullBody, Override", "ScepterSpecialEnd", "DecisiveStrike.playbackRate", 1.4f);

                this.FireAttack();
            }

            if (base.isAuthority && base.fixedAge >= this.duration && this.hitsFired >= this.hitCount)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (cameraTargetParams)
            {
                cameraTargetParams.RemoveParamsOverride(camOverrideHandle, duration / 1.5f);
            }

            if (this.swordMat) this.swordMat.SetFloat("_EmPower", this.minimumEmission);
            if (this.nemmandoController) this.nemmandoController.UncoverScreen();
            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);

            if (!this.outer.destroying) base.PlayAnimation("FullBody, Override", "BufferEmpty");

            base.characterBody.hideCrosshair = false;
            base.OnExit();
        }
    }
}