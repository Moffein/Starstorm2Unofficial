using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;

namespace Starstorm2.Survivors.Cyborg.Components.Crosshair
{
    public class CyborgCrosshairChargeController : MonoBehaviour
    {
        //These are the min/max fill on the sprite in Unity
        private const float minFill = 0f;
        private const float maxFill = 1f;

        public static Color chargeColor = Color.white;
        public static Color perfectChargeColor = new Color32(139, 237, 227, 255);

        private CyborgChargeComponent chargeComponent;
        private HudElement hudElement;
        private Image image;

        private void Awake()
        {
            this.hudElement = base.GetComponent<HudElement>();
            this.image = base.GetComponent<Image>();
        }

        private void FixedUpdate()
        {
            if (!this.chargeComponent)
            {
                if (this.hudElement && this.hudElement.targetCharacterBody)
                {
                    chargeComponent = this.hudElement.targetCharacterBody.GetComponent<CyborgChargeComponent>();
                }
            }
            else
            {
                if (this.image)
                {
                    float targetFill = Mathf.Lerp(minFill, maxFill, chargeComponent.chargeFraction);
                    this.image.fillAmount = targetFill;
                    this.image.color = this.chargeComponent.perfectCharge ? perfectChargeColor : chargeColor;
                }
            }
        }
    }
}
