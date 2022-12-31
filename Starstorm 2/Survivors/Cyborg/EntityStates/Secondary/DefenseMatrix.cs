using RoR2;
using RoR2.Projectile;
using RoR2.UI;
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

        public static float baseDuration = 2f;
        public static string attackSoundString = "";
        public static GameObject projectileDeletionEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXFMJ.prefab").WaitForCompletion();
        public static GameObject matrixPrefab;
        public static float ticksPerSecond = 30;

        public override void OnEnter()
        {
            base.OnEnter();

            Util.PlaySound(DefenseMatrix.attackSoundString, base.gameObject);
            base.StartAimMode(DefenseMatrix.baseDuration + 1f);

            tickDuration = 1f / DefenseMatrix.ticksPerSecond;
            tickStopwatch = 0f;

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
                    }
                }
            }
        }
        public void DeleteProjectilesServer()
        {
            if (matrixCollider)
            {
                List<ProjectileController> deletionList = new List<ProjectileController>();
                Collider[] colliders = Physics.OverlapBox(matrixCollider.center, matrixCollider.size * 0.5f, matrixCollider.transform.rotation, LayerIndex.projectile.mask);
                foreach (Collider c in colliders)
                {
                    ProjectileController pc = c.GetComponentInParent<ProjectileController>();
                    if (pc && !pc.cannotBeDeleted)
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
