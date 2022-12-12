using RoR2;
using UnityEngine;

namespace Starstorm2.Components
{
    public class SS2CharacterAnimationEvents : MonoBehaviour
    {
        private ChildLocator childLocator;

        private void Awake()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        public void PlayEffect(string effectString)
        {
            if (this.childLocator)
            {
                Transform effect = this.childLocator.FindChild(effectString);
                if (effect)
                {
                    effect.gameObject.SetActive(false);
                    effect.gameObject.SetActive(true);
                }
            }
        }

        public void StopEffect(string effectString)
        {
            if (this.childLocator)
            {
                Transform effect = this.childLocator.FindChild(effectString);
                if (effect)
                {
                    effect.gameObject.SetActive(false);
                }
            }
        }

        public void PlaySound(string soundString)
        {
            Util.PlaySound(soundString, this.gameObject);
        }
    }
}