using RoR2;
using UnityEngine;
using Starstorm2Unofficial.Survivors.Nucleator;
using UnityEngine.UI;
using RoR2.UI;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components.Crosshair
{
    public class NucleatorCrosshairController : MonoBehaviour
    {
        public static Color32 chargeColor = new Color32(224, 248, 44, 255);
        public static Color32 overchargeColor = new Color32(255, 0, 0, 255);

        private HudElement hudElement;
        private NucleatorChargeComponent chargeComponent;

        private Image chargeBar;
        private Image chargeBackground;
        private CharacterBody savedCharacterBody;

        private void Awake()
        {
            hudElement = base.GetComponent<HudElement>();
            ChildLocator cl = base.GetComponent<ChildLocator>();
            if (cl)
            {
                Transform transform = cl.FindChild("ChargeBar");
                if (transform)
                {
                    chargeBar = transform.GetComponent<Image>();
                }

                Transform transform2 = cl.FindChild("BarBackground");
                if (transform2)
                {
                    chargeBackground = transform2.GetComponent<Image>();
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
                if (savedCharacterBody) chargeComponent = savedCharacterBody.GetComponent<NucleatorChargeComponent>();
            }
            else
            {
                if (chargeComponent.shouldShowCharge)
                {
                    chargeBackground.color = Color.white;
                    chargeBar.color = chargeComponent.isOvercharge ? overchargeColor : chargeColor;
                    chargeBar.fillAmount = chargeComponent.chargeFraction;
                }
                else
                {
                    chargeBar.fillAmount = 0f;
                    if (chargeBar) chargeBar.color = Color.clear;
                    if (chargeBackground) chargeBackground.color = Color.clear;
                }
            }
        }
    }
}
