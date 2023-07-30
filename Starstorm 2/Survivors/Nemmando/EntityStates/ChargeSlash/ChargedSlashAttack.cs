using EntityStates;
using EntityStates.SS2UStates.Common;
using R2API;
using RoR2;
using Starstorm2Unofficial.Components;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Survivors.Nemmando.Components;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Nemmando
{
    public class ChargedSlashAttack : BaseCustomSkillState
    {
        public float charge;

        public static float baseDuration = 0.6f;
        public static int maxHits = 6;
        public static int minHits = 1;
        public static float maxDamageCoefficient = 3f;
        public static float minDamageCoefficient = 3f;
        public static float maxRadius = 20f;
        public static float minRadius = 8f;
        public static float maxEmission = 150f;
        public static float minEmission = 25f;

        private float hitStopwatch;
        private int hitsFired;
        private float duration;
        private int hitCount;
        private float damageCoefficient;
        private float radius;
        private float emission;
        private BlastAttack blastAttack;
        private EffectData attackEffect;
        private Material swordMat;
        private NemmandoController nemmandoController;
        private float minimumEmission;
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

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = ChargedSlashAttack.baseDuration / this.attackSpeedStat;
            if (base.characterBody)
            {
                base.characterBody.isSprinting = false;
                base.characterBody.hideCrosshair = false;

                if (NetworkServer.active)
                {
                    base.characterBody.AddTimedBuff(RoR2Content.Buffs.ArmorBoost, this.duration + 0.5f);
                }
            }

            this.hitsFired = 0;
            this.hitCount = Mathf.RoundToInt(Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minHits, ChargedSlashAttack.maxHits));
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minDamageCoefficient, ChargedSlashAttack.maxDamageCoefficient);
            this.radius = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minRadius, ChargedSlashAttack.maxRadius);
            this.emission = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minEmission, ChargedSlashAttack.maxEmission);
            this.nemmandoController = base.GetComponent<NemmandoController>();

            this.minimumEmission = this.effectComponent.defaultSwordEmission;

            this.blastAttack = new BlastAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHitSelf,
                baseDamage = this.damageCoefficient * this.damageStat,
                baseForce = -500f,
                bonusForce = Vector3.zero,
                crit = base.RollCrit(),
                damageColorIndex = DamageColorIndex.Default,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = this.gameObject,
                losType = BlastAttack.LoSType.None,
                position = base.characterBody.corePosition,
                procChainMask = default,
                procCoefficient = 1f,
                radius = this.radius,
                teamIndex = base.GetTeam()
            };
            this.blastAttack.AddModdedDamageType(DamageTypeCore.ModdedDamageTypes.GougeOnHit);

            this.attackEffect = new EffectData()
            {
                scale = 0.75f * this.radius,
                origin = base.characterBody.corePosition
            };

            base.characterMotor.rootMotion = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;

            if (this.charge >= 0.4f) EffectManager.SpawnEffect(this.effectComponent.chargeAttackEffect, this.attackEffect, true);

            this.FireAttack();

            if (this.charge >= 0.6f)
            {
                base.PlayAnimation("FullBody, Override", "DecisiveStrikeMax", "DecisiveStrike.playbackRate", this.duration);
                Util.PlaySound("SS2UNemmandoDecisiveStrikeFire", base.gameObject);
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "DecisiveStrike", "DecisiveStrike.playbackRate", this.duration);
                Util.PlaySound(this.effectComponent.swingSound, base.gameObject);
            }

            this.swordMat = base.GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;

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

            this.hitStopwatch = this.duration / this.hitCount;

            if (base.isAuthority)
            {
                this.blastAttack.position = base.characterBody.corePosition;
                this.attackEffect.origin = base.characterBody.corePosition;

                int hitcount = this.blastAttack.Fire().hitCount;
                if (this.hitCount > 0) Util.PlaySound(EntityStates.Merc.GroundLight.hitSoundString, gameObject);

                EffectManager.SpawnEffect(this.effectComponent.chargeAttackLoopEffect, this.attackEffect, true);

                Util.PlayAttackSpeedSound(this.effectComponent.swingSound, base.gameObject, 2f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.hitStopwatch -= Time.fixedDeltaTime;
            this.emission -= 2f * Time.fixedDeltaTime;
            if (this.emission < 0f) this.emission = 0f;

            if (this.swordMat) this.swordMat.SetFloat("_EmPower", Util.Remap(base.fixedAge, 0, this.duration, this.emission, this.minimumEmission));

            base.characterMotor.rootMotion = Vector3.zero;
            base.characterMotor.velocity = Vector3.zero;

            if (this.hitStopwatch <= 0f && this.hitsFired < this.hitCount)
            {
                this.FireAttack();
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
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
            this.swordMat.SetFloat("_EmPower", this.minimumEmission);
            //if (this.nemmandoController) this.nemmandoController.UncoverScreen();
            if (!this.outer.destroying) base.PlayAnimation("FullBody, Override", "BufferEmpty");
            base.OnExit();
        }
    }
}