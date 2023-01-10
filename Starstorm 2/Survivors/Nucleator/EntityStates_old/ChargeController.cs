using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Starstorm2Unofficial.Cores.States.Nucleator
{
    [RequireComponent(typeof(HudElement))]
    public class ChargeController : MonoBehaviour
    {
        private GameObject sourceGameObject;
        private HudElement hudElement;

        public Image image;

        private void Awake()
        {
            this.hudElement = base.GetComponent<HudElement>();
        }
        
        private void FixedUpdate()
        {
            float fillAmount = 0f;
            if (this.hudElement.targetCharacterBody)
            {
                SkillLocator component = this.hudElement.targetCharacterBody.GetComponent<SkillLocator>();
                if (component && component.primary && component.secondary && component.utility)
                {
                    EntityStateMachine stateMachine = component.secondary.stateMachine;
                    if (stateMachine)
                    {
                        NucleatorSkillStateBase nucleatorChargeState = stateMachine.state as NucleatorSkillStateBase;
                        if (nucleatorChargeState != null)
                        {
                            fillAmount = nucleatorChargeState.charge;
                        }
                    }
                }
            }
            if (this.image)
            {
                this.image.fillAmount = fillAmount;
            }
        }
    }
}