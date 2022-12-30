using UnityEngine;

namespace Starstorm2.Survivors.Cyborg.Components
{
    //Crosshair gets charge info from this component.
    public class CyborgChargeComponent : MonoBehaviour
    {
        public float chargeFraction = 0f;
        public bool perfectCharge = false;
        
        public void ResetCharge()
        {
            chargeFraction = 0f;
            perfectCharge = false;
        }
    }
}
