﻿using RoR2;
using Starstorm2Unofficial.Modules.Achievements;
namespace Starstorm2Unofficial.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoClearGameTyphoon", "Skins.SS2UNemmando.GrandMastery", null, 15, null)]
    public class NemmandoGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return Survivors.Nemmando.NemmandoCore.bodyIndex;
        }
	}
}
