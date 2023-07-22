using RoR2;
using RoR2.CharacterAI;
using RoR2.Orbs;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Chirr.Components
{
    public class MinionDistractComponent : MonoBehaviour
    {
        public float delayBetweenPulses = 2f;
        public float distractDuration = 5f;
        public float baseLifetime = 8f;
        public float distractRadius = 30f;

        private float lifetime;
        private float stopwatch = 0f;
        private float pulseStopwatch = 0f;

        private CharacterBody ownerBody;
        private TeamIndex teamIndex = TeamIndex.None;

        private void Awake()
        {
            lifetime = baseLifetime;
            ownerBody = base.GetComponent<CharacterBody>();
            if (ownerBody)
            {
                DistractEnemiesServer();
            }
            else
            {
                Destroy(this);
            }
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            if (!ownerBody.HasBuff(Cores.BuffCore.chirrFriendDistractBuff)) ownerBody.AddBuff(Cores.BuffCore.chirrFriendDistractBuff);

            lifetime -= Time.fixedDeltaTime;
            pulseStopwatch += Time.fixedDeltaTime;
            if (pulseStopwatch >= delayBetweenPulses)
            {
                DistractEnemiesServer();
                pulseStopwatch -= delayBetweenPulses;
            }
            if (lifetime <= 0f)
            {
                Destroy(this);
            }
        }

        public void DistractEnemiesServer()
        {
            if (!NetworkServer.active || !base.transform) return;

            //Refresh team in case it gets changed
            if (ownerBody && ownerBody.teamComponent) teamIndex = ownerBody.teamComponent.teamIndex;

            //Find enemies.
            List<CharacterBody> enemyBodyList = new List<CharacterBody>();
            Collider[] array = Physics.OverlapSphere(base.transform.position, distractRadius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    CharacterBody enemyBody = null;
                    if (healthComponent) enemyBody = healthComponent.body;
                    if (healthComponent && enemyBody && enemyBody.teamComponent && enemyBody.teamComponent.teamIndex != teamIndex && !enemyBodyList.Contains(enemyBody))
                    {
                        enemyBodyList.Add(healthComponent.body);
                    }
                }
            }

            //Redirect AI
            foreach (CharacterBody cb in enemyBodyList)
            {
                if (cb.master && !cb.isPlayerControlled && !cb.isChampion)
                {
                    if (cb.master.aiComponents != null && cb.master.aiComponents.Length > 0)
                    {
                        foreach (BaseAI ai in cb.master.aiComponents)
                        {
                            if (ai.currentEnemy.gameObject != base.gameObject)
                            {
                                ai.currentEnemy.gameObject = base.gameObject;
                                ai.currentEnemy.bestHurtBox = ownerBody.mainHurtBox;
                                ai.enemyAttention = distractDuration;
                                ai.targetRefreshTimer = distractDuration;
                                ai.BeginSkillDriver(ai.EvaluateSkillDrivers());
                            }
                        }

                        //Give a small heal orb for each enemy distracted, mainly so that you can see which enemies are being affected.
                        if (OrbManager.instance)
                        {
                            HealOrb healOrb = new HealOrb
                            {
                                healValue = ownerBody.healthComponent ? ownerBody.healthComponent.fullCombinedHealth * 0.01f : 10f,
                                target = ownerBody.mainHurtBox,
                                origin = cb.mainHurtBox && cb.mainHurtBox.transform ? cb.mainHurtBox.transform.position : cb.corePosition
                            };
                            OrbManager.instance.AddOrb(healOrb);
                        }
                    }
                }
            }
        }

        public void ResetLifetime()
        {
            lifetime = baseLifetime;
        }

        private void OnDestroy()
        {
            if (ownerBody && ownerBody.HasBuff(Cores.BuffCore.chirrFriendDistractBuff)) ownerBody.RemoveBuff(Cores.BuffCore.chirrFriendDistractBuff);
        }
    }
}
