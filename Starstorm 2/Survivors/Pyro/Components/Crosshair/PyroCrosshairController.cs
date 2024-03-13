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
        private CharacterBody savedCharacterBody;

        private void Awake()
        {
            hudElement = base.GetComponent<HudElement>();
            heatBar = base.GetComponent<Image>();
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
                this.heatController = null;
            }

            if (!this.heatController)
            {
                if (savedCharacterBody) heatController = savedCharacterBody.GetComponent<HeatController>();
            }
            else
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
