using RoR2;
using UnityEngine;
using Starstorm2Unofficial.Survivors.Pyro;
using UnityEngine.UI;
using RoR2.UI;

namespace Starstorm2Unofficial.Survivors.Pyro.Components.Crosshair
{
    public class PyroCrosshairController : MonoBehaviour
    {
        public static Color32 colorLowHeat = new Color32(232, 228, 117, 255);
        public static Color colorHighHeat = new Color32(209, 86, 33, 255);

        private HudElement hudElement;
        private HeatController heatController;
        private Image heatBar;

        private void Awake()
        {
            hudElement = base.GetComponent<HudElement>();
            heatBar = base.GetComponent<Image>();
            heatController = base.GetComponent<HeatController>();
        }

        private void FixedUpdate()
        {
            if (!this.heatController)
            {
                if (hudElement && hudElement.targetCharacterBody)
                {
                    heatController = hudElement.targetCharacterBody.GetComponent<HeatController>();
                }
            }
        }

        private void Update()
        {
            if (heatController)
            {
                if (heatBar)
                {
                    float heatPercent = heatController.GetHeatPercent();
                    float targetFill = Mathf.Lerp(0f, 1f, heatPercent);
                    heatBar.fillAmount = targetFill;
                    heatBar.color = heatController.IsHighHeat() ? PyroCrosshairController.colorHighHeat : PyroCrosshairController.colorLowHeat;
                }
            }
        }
    }
}
