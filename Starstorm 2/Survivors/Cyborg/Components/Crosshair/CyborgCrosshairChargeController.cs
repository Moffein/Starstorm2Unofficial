using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;
using Starstorm2Unofficial.Survivors.Nucleator.Components;
using RoR2;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components.Crosshair
{
    public class CyborgCrosshairChargeController : MonoBehaviour
    {
        public static Color chargeColor = Color.white;
        public static Color perfectChargeColor = new Color32(139, 237, 227, 255);

        public static Color shieldColor = new Color32(139, 237, 227, 255);
        public static Color shieldDepleteColor = new Color32(150, 0, 0, 255);

        private CyborgEnergyComponent chargeComponent;
        private HudElement hudElement;
        private Image chargeBar;
        private Image chargeBarBackground;
        private Image shieldBar;

        private Image rightDot;
        private Image leftDot;

        private CharacterBody savedCharacterBody;

        private void Awake()
        {
            this.hudElement = base.GetComponent<HudElement>();
            this.chargeBar = base.GetComponent<Image>();

            ChildLocator cl = base.GetComponent<ChildLocator>();
            if (cl)
            {
                Transform shieldBarTransform = cl.FindChild("ShieldBar");
                if (shieldBarTransform)
                {
                    shieldBar = shieldBarTransform.GetComponent<Image>();
                }

                Transform chargeBackgroundTransform = cl.FindChild("BackgroundImage");
                if (chargeBackgroundTransform)
                {
                    chargeBarBackground = chargeBackgroundTransform.GetComponent<Image>();
                }

                Transform rightDotTransform = cl.FindChild("RightDot");
                if (rightDotTransform)
                {
                    rightDot = rightDotTransform.GetComponent<Image>();
                }

                Transform leftDotTransform = cl.FindChild("LeftDot");
                if (leftDotTransform)
                {
                    leftDot = leftDotTransform.GetComponent<Image>();
                }
            }
        }

        private void FixedUpdate()
        {
            if (!hudElement) return;

            bool changedBody = false;
            if (savedCharacterBody != hudElement.targetCharacterBody)
            {
                savedCharacterBody = hudElement.targetCharacterBody;
                changedBody = true;
            }

            if (changedBody)
            {
                this.chargeComponent = null;
            }

            if (!chargeComponent)
            {
                if (savedCharacterBody) chargeComponent = savedCharacterBody.GetComponent<CyborgEnergyComponent>();
            }
            else
            {
                if (this.chargeBarBackground)
                {
                    if (this.chargeComponent.skillLocator && this.chargeComponent.skillLocator.primary.skillDef == CyborgCore.chargeRifleDef)
                    {
                        chargeBarBackground.color = Color.white;
                    }
                    else
                    {
                        chargeBarBackground.color = Color.clear;
                    }
                }

                if (chargeBar)
                {
                    float targetFill = Mathf.Lerp(0f, 1f, chargeComponent.rifleChargeFraction);
                    chargeBar.fillAmount = targetFill;
                    chargeBar.color = chargeComponent.riflePerfectCharge ? perfectChargeColor : chargeColor;
                }

                if (shieldBar)
                {
                    float targetFill = Mathf.Lerp(0f, 1f, chargeComponent.remainingEnergyFraction / chargeComponent.GetMaxEnergyFraction());
                    Color targetColor = chargeComponent.energyDepleted ? shieldDepleteColor : shieldColor;
                    targetColor.a = chargeComponent.energySkillsActive > 0 ? 1f : 0.5f;

                    shieldBar.color = targetColor;
                    shieldBar.fillAmount = targetFill;
                }

                Color dotColor = Color.white;
                dotColor.a = chargeComponent.showTriShotCrosshair ? 1f : 0f;
                if (rightDot)
                {
                    rightDot.color = dotColor;
                }
                if (leftDot)
                {
                    leftDot.color = dotColor;
                }
            }
        }
    }
}
