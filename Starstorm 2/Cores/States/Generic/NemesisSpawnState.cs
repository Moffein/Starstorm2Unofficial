using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2.Cores.States.Generic
{
    public class NemmandoSpawnState : NemesisSpawnState
    {
        public override void SpawnEffect()
        {
            this.portalMuzzle = "PortalSpawn";
            base.SpawnEffect();
        }
    }

    public class NemesisSpawnState : BaseState
    {
        public static float duration = 3f;
        protected string portalMuzzle = "Chest";

        private CameraRigController cameraController;
        private bool initCamera;

        public override void OnEnter()
        {
            base.OnEnter();
            this.initCamera = false;
            base.PlayAnimation("FullBody, Override", "Spawn");
            Util.PlaySound(EntityStates.NullifierMonster.SpawnState.spawnSoundString, base.gameObject);

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
            this.SpawnEffect();
        }

        public virtual void SpawnEffect()
        {
            if (EntityStates.NullifierMonster.SpawnState.spawnEffectPrefab)
            {
                EffectManager.SimpleMuzzleFlash(EntityStates.NullifierMonster.SpawnState.spawnEffectPrefab, base.gameObject, portalMuzzle, false);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            // i don't know if all this null checking is necessary but i'd rather play it safe than spend time testing
            if (!this.cameraController)
            {
                if (base.characterBody && base.characterBody.master)
                {
                    if (base.characterBody.master.playerCharacterMasterController)
                    {
                        if (base.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = base.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                if (!this.initCamera)
                {
                    this.initCamera = true;
                    this.cameraController.SetPitchYawFromLookVector(-base.characterDirection.forward);
                }
            }

            if (base.fixedAge >= NemesisSpawnState.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}