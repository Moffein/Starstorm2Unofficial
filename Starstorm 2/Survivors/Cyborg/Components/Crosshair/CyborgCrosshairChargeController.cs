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

        private CyborgChargeComponent chargeComponent;
        private HudElement hudElement;
        private Image chargeBar;
        private Image chargeBarBackground;
        private Image shieldBar;
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
                if (savedCharacterBody) chargeComponent = savedCharacterBody.GetComponent<CyborgChargeComponent>();
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

                if (this.chargeBar)
                {
                    float targetFill = Mathf.Lerp(0f, 1f, chargeComponent.chargeFraction);
                    this.chargeBar.fillAmount = targetFill;
                    this.chargeBar.color = this.chargeComponent.perfectCharge ? perfectChargeColor : chargeColor;
                }

                if (this.shieldBar)
                {
                    if (this.chargeComponent.skillLocator && this.chargeComponent.skillLocator.secondary.skillDef == CyborgCore.defenseMatrixDef)
                    {
                        float targetFill = Mathf.Lerp(0f, 1f, chargeComponent.remainingShieldDuration / chargeComponent.GetMaxShieldDuration());
                        Color targetColor = chargeComponent.shieldDepleted ? shieldDepleteColor : shieldColor;
                        targetColor.a = chargeComponent.shieldActive ? 1f : 0.5f;

                        shieldBar.color = targetColor;
                        shieldBar.fillAmount = targetFill;
                    }
                    else
                    {
                        shieldBar.fillAmount = 0f;
                    }
                }
            }
        }
    }
}
