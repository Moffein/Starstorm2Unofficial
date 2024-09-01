﻿using RoR2;
using RoR2.Achievements;
using Starstorm2Unofficial.Survivors.Nemmando.Components;

namespace Starstorm2Unofficial.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoKillSelf", "Skins.SS2UNemmando.Commando", null, 3, null)]
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
