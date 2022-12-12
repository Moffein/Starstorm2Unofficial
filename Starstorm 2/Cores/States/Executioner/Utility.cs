using RoR2;
using Starstorm2.Components;
using Starstorm2.Cores;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace EntityStates.Executioner
{
    public class ExecutionerDash : BaseSkillState
    {
        public static float baseDuration = 0.5f;
        public static float speedMultiplier = 4.0f;
        public static float debuffRadius = 20f;
        public static float debuffDuration = 2.0f;
        public static float debuffCheckInterval = 0.1f;

        private float debuffCheckStopwatch;
        private Vector3 initialDirection;
        private float initialSpeed;
        private float duration;
        private SphereSearch fearSearch;
        private List<HurtBox> hits;
        private Animator animator;
        private ExecutionerController exeController;

        public override void OnEnter()
        {
            base.OnEnter();
            debuffCheckStopwatch = 0f;
            initialSpeed = base.moveSpeedStat;
            if (base.inputBank)
            {
                initialDirection = base.inputBank.moveVector;
            }
            else
            {
                initialDirection = base.GetAimRay().direction;
            }
            initialDirection.y = 0f;

            if (base.characterMotor)
            {
                if (base.characterMotor.velocity.y < 0f)
                {
                    base.characterMotor.velocity.y = 0f;
                }
            }
            this.exeController = base.GetComponent<ExecutionerController>();
            this.animator = base.GetModelAnimator();

            this.duration = baseDuration;
            Util.PlayAttackSpeedSound("ExecutionerUtility", base.gameObject, 1.0f);
            base.PlayAnimation("FullBody, Override", "Utility", "Utility.playbackRate", this.duration);

            if (this.exeController) this.exeController.PlayDashEffect();

            //create dash aoe
            if (base.isAuthority)
            {
                Vector3 orig = base.characterBody.corePosition;
                BlastAttack blast = new BlastAttack()
                {
                    baseDamage = 0,
                    damageType = DamageType.Stun1s | DamageType.NonLethal | DamageType.Silent | DamageType.BypassBlock,
                    radius = debuffRadius,
                    falloffModel = BlastAttack.FalloffModel.None,
                    baseForce = 0f,
                    teamIndex = TeamComponent.GetObjectTeam(base.gameObject),
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    position = orig,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    procCoefficient = 1f
                };
                blast.Fire();
            }

            hits = new List<HurtBox>();
            fearSearch = new SphereSearch();
            fearSearch.mask = LayerIndex.entityPrecise.mask;
            fearSearch.radius = debuffRadius;

            Transform modelTransform = base.GetModelTransform();
            if (modelTransform)
            {
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1.5f * ExecutionerDash.baseDuration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = LegacyResourcesAPI.Load<Material>("Materials/matHuntressFlashBright");
                temporaryOverlay.AddToCharacerModel(modelTransform.GetComponent<CharacterModel>());
            }
        }

        private void CreateFearAoe()
        {
            hits.Clear();
            fearSearch.ClearCandidates();
            fearSearch.origin = base.characterBody.corePosition;
            fearSearch.RefreshCandidates();
            fearSearch.FilterCandidatesByDistinctHurtBoxEntities();
            fearSearch.FilterCandidatesByHurtBoxTeam(TeamMask.GetUnprotectedTeams(base.teamComponent.teamIndex));
            fearSearch.GetHurtBoxes(hits);
            foreach (HurtBox h in hits)
            {
                HealthComponent hp = h.healthComponent;
                if (hp)
                {
                    //This ends up insta executing anything below the execute threshold.
                    /*DamageInfo damage = new DamageInfo
                    {
                        attacker = base.gameObject,
                        inflictor = base.gameObject,
                        damage = 0f,
                        procCoefficient = 1f,
                        crit = false,
                        force = Vector3.zero,
                        position = h.transform.position,
                        procChainMask = default,
                        canRejectForce = true,
                        damageColorIndex = DamageColorIndex.Default,
                        damageType = DamageType.Stun1s | DamageType.NonLethal | DamageType.Silent | DamageType.BypassBlock
                    };
                    hp.TakeDamage(damage);*/

                    CharacterBody body = hp.body;
                    if (body && body != base.characterBody)
                    {
                        body.AddTimedBuff(BuffCore.fearDebuff, debuffDuration);
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterBody.isSprinting = true;

            if (base.characterMotor)
            {
                base.characterMotor.rootMotion += (initialDirection * initialSpeed * speedMultiplier) * Time.fixedDeltaTime;
                if (base.characterMotor.velocity.y < 0f)
                {
                    base.characterMotor.velocity.y = 0f;
                }
            }

            if (NetworkServer.active)
            {
                debuffCheckStopwatch += Time.fixedDeltaTime;
                if (debuffCheckStopwatch >= ExecutionerDash.debuffCheckInterval)
                {
                    debuffCheckStopwatch -= ExecutionerDash.debuffCheckInterval;
                    CreateFearAoe();
                }
            }

            if (base.fixedAge >= this.duration)
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