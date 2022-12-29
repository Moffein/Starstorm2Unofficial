using RoR2;
using RoR2.Achievements;
using Starstorm2.Survivors.Nemmando.Components;

namespace Starstorm2.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoKillSelf", "Skins.SS2UNemmando.Commando", null, null)]
    public class NemmandoKillSelfAchievement : BaseAchievement
    {
        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            NemmandoUnlockComponent.OnDeath += NemmandoUnlockComponent_OnDeath;
        }

        public override void OnBodyRequirementBroken()
        {
            NemmandoUnlockComponent.OnDeath -= NemmandoUnlockComponent_OnDeath;
            base.OnBodyRequirementBroken();
        }

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Survivors.Nemmando.NemmandoCore.bodyIndex;
        }

        private void NemmandoUnlockComponent_OnDeath(Run obj)
        {
            base.Grant();
        }
    }
}
