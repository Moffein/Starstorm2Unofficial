using EntityStates;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2Unofficial.Cores.States.Wayfarer
{
    class CreateBuffWard : BaseSkillState
    {
        public static float baseDuration = 3.5f;
        public static float radius = 30f;

        private Animator animator;
        private GameObject buffPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            animator = base.GetModelAnimator();
            base.PlayCrossfade("FullBody, Override", "Ward", "Ward.playbackRate", baseDuration / attackSpeedStat, 0.2f);

        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!buffPrefab && animator.GetFloat("Ward.active") > 0.5)
            {
                buffPrefab = UnityEngine.Object.Instantiate(EnemyCore.wayfarerBuffWardPrefab);
                buffPrefab.GetComponent<TeamFilter>().teamIndex = base.characterBody.teamComponent.teamIndex;
                buffPrefab.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(base.gameObject);
            }

            if (base.fixedAge >= baseDuration)
            {
                outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            if (buffPrefab)
            {
                UnityEngine.Object.Destroy(buffPrefab);
            }
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
