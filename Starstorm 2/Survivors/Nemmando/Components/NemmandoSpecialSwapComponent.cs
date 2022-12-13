using RoR2;
using UnityEngine;

namespace Starstorm2.Survivors.Nemmando.Components
{
    // this is such a hack but if it works it works(for now)
    public class NemmandoSpecialSwapComponent : MonoBehaviour
    {
        private CharacterBody body;

        private void Awake()
        {
            this.body = this.GetComponent<CharacterBody>();
            Invoke("OverrideSpecial", 1f);
        }

        private void OverrideSpecial()
        {
            if (this.body)
            {
                if (this.body.teamComponent && this.body.teamComponent.teamIndex != TeamIndex.Player) this.body.skillLocator.special.SetBaseSkill(Survivors.Nemmando.NemmandoCore.decisiveStrikeSkillDef);
            }

            Destroy(this);
        }
    }
}