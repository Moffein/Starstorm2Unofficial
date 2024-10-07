using RoR2;
using RoR2.Projectile;
using RoR2.UI;
using Starstorm2Unofficial.Survivors.Cyborg;
using Starstorm2Unofficial.Survivors.Cyborg.Components;
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
        private Transform laserVisuals;
        private float tickDuration;
        private float tickStopwatch;
        private float blinkStopwatch;
        private float blinkToggleDuration;
        private TeamIndex inputTeamIndex;
        private DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo defenseMatrixInfo;
        private CyborgEnergyComponent energyComponent;

        public static float shieldDuration = 1f / 0.15f;
        public static float minDuration = 0.5f;
        public static string attackSoundString = "SS2UCyborgSpecialTeleport";
        public static GameObject projectileDeletionEffectPrefab;
        public static GameObject matrixPrefab;
        public static float ticksPerSecond = 30;
        public static float energyFractionPerProjectile = 0f;   //Leaving this in just in-case

        public static float blinkTime = 0.5f;
        public static float blinkFrequency = 20f;

        private float lastUpdateTime;

        public override void OnEnter()
        {
            base.OnEnter();
            lastUpdateTime = Time.time;
            energyComponent = base.GetComponent<CyborgEnergyComponent>();
            if (energyComponent)
            {
                energyComponent.energySkillsActive++;
            }

            Util.PlaySound(DefenseMatrix.attackSoundString, base.gameObject);
            base.StartAimMode(2f);

            tickDuration = 1f / DefenseMatrix.ticksPerSecond;
            tickStopwatch = 0f;

            blinkStopwatch = 0f;
            blinkToggleDuration = 1f / DefenseMatrix.blinkFrequency;

            if (DefenseMatrix.matrixPrefab)
            {
                ChildLocator cl = base.GetModelChildLocator();
                if (cl)
                {
                    matrixRootTransform = cl.FindChild("DefenseMatrixRoot");
                    if (matrixRootTransform)
                    {
                        matrixInstance = UnityEngine.Object.Instantiate(matrixPrefab);//, matrixRootTransform   //causes it to jerk around
                        matrixInstance.transform.position = matrixRootTransform.position;
                        matrixInstance.transform.rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);

                        ChildLocator laserCL = matrixInstance.GetComponent<ChildLocator>();
                        if (laserCL)
                        {
                            Transform hitboxTransform = laserCL.FindChild("Hitbox");
                            if (hitboxTransform)
                            {
                                matrixCollider = hitboxTransform.GetComponent<BoxCollider>();
                                inputTeamIndex = base.GetTeam();
                                Collider[] colliders = hitboxTransform.GetComponentsInChildren<Collider>();
                                foreach (Collider c in colliders)
                                {
                                    c.gameObject.layer = LayerIndex.world.mask;
                                }
                                defenseMatrixInfo = new DefenseMatrixManager.DefenseMatrixManager.DefenseMatrixInfo(colliders, inputTeamIndex);
                                this.defenseMatrixInfo = DefenseMatrixManager.DefenseMatrixManager.AddMatrix(defenseMatrixInfo);
                            }

                            laserVisuals = laserCL.FindChild("LaserVisuals");
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

                Vector3 sizeVector = matrixCollider.size * 0.5f;
                sizeVector.x *= matrixInstance.transform.localScale.x;
                sizeVector.y *= matrixInstance.transform.localScale.y;
                sizeVector.z *= matrixInstance.transform.localScale.z;

                Collider[] colliders = Physics.OverlapBox(matrixCollider.transform.position, sizeVector, matrixCollider.transform.rotation, LayerIndex.projectile.mask);
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

                int projectilesDeleted = deletionList.Count;
                for (int i = 0; i < projectilesDeleted; i++)
                {
                    GameObject toDelete = deletionList[i].gameObject;
                    if (toDelete)
                    {
                        if (toDelete.transform) EffectManager.SimpleEffect(DefenseMatrix.projectileDeletionEffectPrefab, toDelete.transform.position, default, true);
                        EntityState.Destroy(toDelete);
                    }
                }
                if (energyComponent && projectilesDeleted > 0 && DefenseMatrix.energyFractionPerProjectile > 0f)
                {
                    energyComponent.RestoreEnergyServer(projectilesDeleted * DefenseMatrix.energyFractionPerProjectile);
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            float deltaTime = Time.time - lastUpdateTime;
            lastUpdateTime = Time.time;
            base.StartAimMode(2f);

            if (base.characterBody && base.characterBody.isSprinting) base.characterBody.isSprinting = false;

            if (NetworkServer.active)
            {
                tickStopwatch += deltaTime;
                if (tickStopwatch >= tickDuration)
                {
                    tickStopwatch -= tickDuration;
                    DeleteProjectilesServer();
                }
            }

            bool shieldDepleted = false;
            if (this.energyComponent)
            {
                float toConsume = deltaTime / DefenseMatrix.shieldDuration;
                if (!CyborgCore.useEnergyRework.Value) toConsume *= 2f;
                this.energyComponent.ConsumeEnergy(toConsume);
                shieldDepleted = this.energyComponent.energyDepleted;
            }

            if (base.isAuthority)
            {
                if (this.energyComponent)
                {
                    if (this.energyComponent.remainingEnergyFraction <= blinkTime/DefenseMatrix.shieldDuration)
                    {
                        blinkStopwatch += deltaTime;
                        if (blinkStopwatch >= blinkToggleDuration)
                        {
                            blinkStopwatch -= blinkToggleDuration;
                            if (laserVisuals)
                            {
                                laserVisuals.gameObject.SetActive(!laserVisuals.gameObject.activeSelf);
                            }
                        }
                    }
                }

                bool keyIsDown = base.inputBank && base.inputBank.skill2.down;
                if ((shieldDepleted || (!keyIsDown && base.fixedAge >= DefenseMatrix.minDuration)))
                {
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        public override void Update()
        {
            base.Update();

            if (matrixInstance)
            {
                matrixInstance.transform.rotation = Util.QuaternionSafeLookRotation(base.GetAimRay().direction);
                if (matrixRootTransform) matrixInstance.transform.position = matrixRootTransform.position;
            }
        }

        public override void OnExit()
        {
            if (energyComponent)
            {
                energyComponent.energySkillsActive--;
            }

            if (this.defenseMatrixInfo != null)
            {
                DefenseMatrixManager.DefenseMatrixManager.RemoveMatrix(this.defenseMatrixInfo);
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
