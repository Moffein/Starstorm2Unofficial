using EntityStates;
using RoR2;
using Starstorm2.Components;
using UnityEngine;

namespace Starstorm2.Cores.States.Nemmando
{
    public class ChargedSlashAttack : BaseCustomSkillState
    {
        public float charge;

        public static float baseDuration = 0.6f;
        public static int maxHits = 6;
        public static int minHits = 1;
        public static float maxDamageCoefficient = 3.8f;
        public static float minDamageCoefficient = 2.5f;
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

        public override void OnEnter()
        {
            base.OnEnter();
            base.characterBody.isSprinting = false;
            this.hitsFired = 0;
            this.duration = ChargedSlashAttack.baseDuration / this.attackSpeedStat;
            this.hitCount = Mathf.RoundToInt(Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minHits, ChargedSlashAttack.maxHits));
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minDamageCoefficient, ChargedSlashAttack.maxDamageCoefficient);
            this.radius = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minRadius, ChargedSlashAttack.maxRadius);
            this.emission = Util.Remap(this.charge, 0f, 1f, ChargedSlashAttack.minEmission, ChargedSlashAttack.maxEmission);
            this.nemmandoController = base.GetComponent<NemmandoController>();
            base.characterBody.hideCrosshair = false;

            this.minimumEmission = this.effectComponent.defaultSwordEmission;

            this.blastAttack = new BlastAttack()
            {
                attacker = base.gameObject,
                attackerFiltering = AttackerFiltering.NeverHit,
                baseDamage = this.damageCoefficient * this.damageStat,
                baseForce = -500f,
                bonusForce = Vector3.zero,
                crit = base.RollCrit(),
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.BlightOnHit,
                falloffModel = BlastAttack.FalloffModel.None,
                inflictor = this.gameObject,
                losType = BlastAttack.LoSType.None,
                position = base.characterBody.corePosition,
                procChainMask = default,
                procCoefficient = 1f,
                radius = this.radius,
                teamIndex = base.GetTeam()
            };

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
                Util.PlaySound("NemmandoDecisiveStrikeFire", base.gameObject);
            }
            else
            {
                base.PlayAnimation("FullBody, Override", "DecisiveStrike", "DecisiveStrike.playbackRate", this.duration);
                Util.PlaySound(this.effectComponent.swingSound, base.gameObject);
            }

            this.swordMat = base.GetModelTransform().GetComponent<CharacterModel>().baseRendererInfos[1].defaultMaterial;
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
            base.OnExit();
            if (base.cameraTargetParams) base.cameraTargetParams.aimMode = CameraTargetParams.AimType.Standard;
            this.swordMat.SetFloat("_EmPower", this.minimumEmission);
            //if (this.nemmandoController) this.nemmandoController.UncoverScreen();
        }
    }
}