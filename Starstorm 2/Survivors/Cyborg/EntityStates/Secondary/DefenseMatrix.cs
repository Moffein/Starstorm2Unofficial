using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using Starstorm2.Survivors.Cyborg;
using Starstorm2.Survivors.Cyborg.Components;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace EntityStates.SS2UStates.Cyborg.Secondary
{
    public class DefenseMatrix : BaseState
    {
        private BoxCollider matrixCollider;
        private Transform matrixRootTransform;
        private GameObject matrixInstance;
        private float tickDuration;
        private float tickStopwatch;
        private float blinkStopwatch;
        private float blinkToggleDuration;
        private float blinkStartTime;
        private TeamIndex inputTeamIndex;

        public static float baseDuration = 3f;
        public static string attackSoundString = "CyborgSpecialTeleport";
        public static GameObject projectileDeletionEffectPrefab;
        public static GameObject matrixPrefab;
        public static float ticksPerSecond = 30;

        public static float blinkTime = 0.5f;
        public static float blinkFrequency = 16f;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(DefenseMatrix.attackSoundString, base.gameObject);
            base.StartAimMode(DefenseMatrix.baseDuration + 1f);

            tickDuration = 1f / DefenseMatrix.ticksPerSecond;
            tickStopwatch = 0f;

            blinkStopwatch = 0f;
            blinkToggleDuration = 1f / DefenseMatrix.blinkFrequency;
            blinkStartTime = DefenseMatrix.baseDuration - DefenseMatrix.blinkTime;

            if (DefenseMatrix.matrixPrefab)
            {
                ChildLocator cl = base.GetModelChildLocator();
                if (cl)
                {
                    matrixRootTransform = cl.FindChild("DefenseMatrixRoot");
                    if (matrixRootTransform)
                    {
                        matrixInstance = UnityEngine.Object.Instantiate(matrixPrefab, matrixRootTransform);
                        matrixCollider = matrixInstance.GetComponentInChildren<BoxCollider>();
                        if (matrixCollider)
                        {
                            inputTeamIndex = base.GetTeam();
                            DefenseMatrixManager.AddMatrix(matrixCollider, inputTeamIndex);
                        }
                    }
                }
            }
        }
        public void DeleteProjectilesServer()
        {
            if (matrixCollider)
            {
                List<ProjectileController> deletionList = new List<ProjectileController>();

                Collider[] colliders = Physics.OverlapBox(matrixCollider.transform.position, matrixCollider.size * 0.5f, matrixCollider.transform.rotation, LayerIndex.projectile.mask);
                foreach (Collider c in colliders)
                {
                    ProjectileController pc = c.GetComponentInParent<ProjectileController>();
                    if (pc && !pc.cannotBeDeleted)
                    {
                        if (!(pc.teamFilter && pc.teamFilter.teamIndex == base.GetTeam()))
                        {
                            bool cannotDelete = false;
                            ProjectileSimple ps = pc.gameObject.GetComponent<ProjectileSimple>();
                            ProjectileCharacterController pcc = pc.gameObject.GetComponent<ProjectileCharacterController>();

                            if ((!ps || (ps && ps.desiredForwardSpeed == 0f)) && !pcc)
                            {
                                cannotDelete = true;
                            }

                            if (!cannotDelete)
                            {
                                deletionList.Add(pc);
                            }
                        }
                    }
                }

                for (int i = 0; i < deletionList.Count; i++)
                {
                    GameObject toDelete = deletionList[i].gameObject;
                    if (toDelete)
                    {
                        if (toDelete.transform) EffectManager.SimpleEffect(DefenseMatrix.projectileDeletionEffectPrefab, toDelete.transform.position, default, true);
                        EntityState.Destroy(toDelete);
                    }
                }
            }
        }

        public void SlowEnemiesServer()
        {
            if (matrixCollider)
            {
                List<HealthComponent> slowList = new List<HealthComponent>();

                Collider[] colliders = Physics.OverlapBox(matrixCollider.transform.position, matrixCollider.size * 0.5f, matrixCollider.transform.rotation, LayerIndex.entityPrecise.mask);
                foreach (Collider c in colliders)
                {
                    HurtBox hb = c.GetComponent<HurtBox>();
                    if (hb && hb.healthComponent)
                    {
                        HealthComponent hc = hb.healthComponent;
                        if (hc != base.healthComponent  && hc.body && !(hc.body.teamComponent && hc.body.teamComponent.teamIndex == base.GetTeam()) && !slowList.Contains(hc))
                        {
                            slowList.Add(hc);
                            hc.body.AddTimedBuff(RoR2Content.Buffs.Slow50, 2f);
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (matrixRootTransform)
            {
                Ray aimRay = base.GetAimRay();
                matrixRootTransform.rotation = Util.QuaternionSafeLookRotation(aimRay.direction);
            }

            if (NetworkServer.active)
            {
                tickStopwatch += Time.fixedDeltaTime;
                if (tickStopwatch >= tickDuration)
                {
                    tickStopwatch -= tickDuration;
                    DeleteProjectilesServer();
                    SlowEnemiesServer();
                }
            }

            if (base.fixedAge >= blinkStartTime)
            {
                blinkStopwatch += Time.fixedDeltaTime;
                if (blinkStopwatch >= blinkToggleDuration)
                {
                    blinkStopwatch -= blinkToggleDuration;
                    if (matrixInstance)
                    {
                        matrixInstance.SetActive(!matrixInstance.activeSelf);
                    }
                }
            }

            if (base.isAuthority && base.fixedAge >= DefenseMatrix.baseDuration)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            if (matrixCollider)
            {
                DefenseMatrixManager.RemoveMatrix(matrixCollider, inputTeamIndex);
            }

            if (matrixInstance)
            {
                EntityState.Destroy(matrixInstance);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
