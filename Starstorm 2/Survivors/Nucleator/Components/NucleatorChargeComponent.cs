using UnityEngine;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components
{
    public class NucleatorChargeComponent : MonoBehaviour
    {
        public float chargeFraction;
        public bool shouldShowCharge;
        public bool isOvercharge;

        private void Awake()
        {
            Reset();
        }

        public void Reset()
        {
            chargeFraction = 0f;
            shouldShowCharge = false;
            isOvercharge = false;
        }

        public void StartCharge()
        {
            chargeFraction = 0f;
            shouldShowCharge = true;
            isOvercharge = false;
        }

        public void SetCharge(float charge, float overchargeFraction)
        {
            chargeFraction = Mathf.Min(1f, charge);
            isOvercharge = chargeFraction >= overchargeFraction;
        }
    }
}
