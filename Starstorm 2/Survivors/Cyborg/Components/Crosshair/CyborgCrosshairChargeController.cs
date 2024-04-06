using RoR2.UI;
using UnityEngine.UI;
using UnityEngine;
using Starstorm2Unofficial.Survivors.Nucleator.Components;
using RoR2;
using BepInEx.Configuration;
using TMPro;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Survivors.Cyborg.Components.Crosshair
{
    public class CyborgCrosshairChargeController : MonoBehaviour
    {
        public static ConfigEntry<bool> useSimpleEnergyBar;
        public static ConfigEntry<float> fontSize;

        public static Color chargeColor = Color.white;
        public static Color perfectChargeColor = new Color32(197, 246, 241, 255);

        public static Color shieldColor = new Color32(139, 237, 227, 255);
        public static Color shieldDepleteColor = new Color32(250, 100, 100, 255);
        public static Color shieldConsumeColor = new Color32(255, 255, 255, 255);

        private static Color emptyColor = new Color(1f, 1f, 1f, 0f);

        private CyborgEnergyComponent chargeComponent;
        private HudElement hudElement;
        private Image chargeBar;
        private Image chargeBarBackground;
        private Image shieldBar;

        private Image energyBackground;
        private Image energyBar;
        private TextMeshProUGUI energyText;
        private TextMeshProUGUI shieldText;

        private Image rightDot;
        private Image leftDot;

        private CharacterBody savedCharacterBody;

        //Save this to apply settings mid-game.
        public static ConfigEntry<float> energyBarScale;
        public static ConfigEntry<float> energyBarXPos;
        public static ConfigEntry<float> energyBarYPos;
        private RectTransform energyRect;

        private void Awake()
        {
            this.hudElement = base.GetComponent<HudElement>();
            this.chargeBar = base.GetComponent<Image>();

            ChildLocator cl = base.GetComponent<ChildLocator>();
            if (cl)
            {
                Transform shieldBarTransform = cl.FindChild("ShieldBar");
                if (shieldBarTransform) shieldBar = shieldBarTransform.GetComponent<Image>();

                Transform chargeBackgroundTransform = cl.FindChild("BackgroundImage");
                if (chargeBackgroundTransform) chargeBarBackground = chargeBackgroundTransform.GetComponent<Image>();

                Transform rightDotTransform = cl.FindChild("RightDot");
                if (rightDotTransform) rightDot = rightDotTransform.GetComponent<Image>();

                Transform leftDotTransform = cl.FindChild("LeftDot");
                if (leftDotTransform) leftDot = leftDotTransform.GetComponent<Image>();

                Transform energyBarTransform = cl.FindChild("EnergyBar");
                if (energyBarTransform) energyBar = energyBarTransform.GetComponent<Image>();

                Transform energyBackgroundTransform = cl.FindChild("EnergyBackground");
                if (energyBackgroundTransform)
                {
                    energyBackground = energyBackgroundTransform.GetComponent<Image>();
                    energyRect = energyBackgroundTransform.GetComponent<RectTransform>();
                }

                TMP_FontAsset font = Addressables.LoadAssetAsync<TMPro.TMP_FontAsset>("RoR2/Base/Common/Fonts/Bombardier/tmpBombDropshadow.asset").WaitForCompletion();
                Transform energyTextTransform = cl.FindChild("EnergyText");
                if (energyTextTransform) energyText = energyTextTransform.GetComponent<TextMeshProUGUI>();
                if (energyText) energyText.font = font;

                Transform shieldTextTransform = cl.FindChild("ShieldText");
                if (shieldTextTransform) shieldText = shieldTextTransform.GetComponent<TextMeshProUGUI>();
                if (shieldText) shieldText.font = font;
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
                    if (this.chargeComponent.skillLocator && this.chargeComponent.skillLocator.primary.skillDef == CyborgCore.Skills.ChargeRifle)
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

                //Energy Bar
                {
                    if (energyRect != null)
                    {
                        energyRect.localScale = energyBarScale.Value * Vector3.one;

                        if (energyRect.localPosition.y != energyBarYPos.Value || energyRect.localPosition.x != energyBarXPos.Value)
                        {
                            Vector3 newPos = new Vector3(energyBarXPos.Value, energyBarYPos.Value, energyRect.localPosition.z);
                            energyRect.localPosition = newPos;
                        }
                    }

                    float targetFill = Mathf.Lerp(0f, 1f, chargeComponent.remainingEnergyFraction / chargeComponent.GetMaxEnergyFraction());
                    Color targetColor = shieldColor;
                    if (chargeComponent.energyDepleted)
                    {
                        targetColor = shieldDepleteColor;
                    }
                    else if (chargeComponent.energySkillsActive > 0 || chargeComponent.energyRechargeDelayStopwatch > 0f)
                    {
                        targetColor = shieldConsumeColor;
                    }

                    if (useSimpleEnergyBar.Value || !CyborgCore.useEnergyRework.Value)
                    {
                        if (shieldBar)
                        {
                            shieldBar.fillAmount = targetFill;
                            shieldBar.color = targetColor;
                        }

                        if (shieldText)
                        {
                            shieldText.text = Mathf.FloorToInt(100f * chargeComponent.remainingEnergyFraction) + "%";
                            shieldText.color = targetColor;
                            shieldText.fontSize = fontSize.Value;
                        }

                        if (energyBackground) energyBackground.color = emptyColor;
                        if (energyBar) energyBar.color = emptyColor;
                        if (energyText && energyText.color != emptyColor)
                        {
                            energyText.color = emptyColor;
                        }
                    }
                    else
                    {
                        if (energyBar)
                        {
                            energyBar.fillAmount = targetFill;
                            energyBar.color = targetColor;
                        }

                        if (energyBackground) energyBackground.color = shieldColor;

                        if (energyText)
                        {
                            energyText.text = Mathf.FloorToInt(100f * chargeComponent.remainingEnergyFraction) + "%";
                            energyText.color = targetColor;
                            energyText.fontSize = fontSize.Value / energyBarScale.Value;
                        }

                        if (shieldBar) shieldBar.color = emptyColor;
                        if (shieldText && shieldText.color != emptyColor)
                        {
                            shieldText.color = emptyColor;
                        }
                    }
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
