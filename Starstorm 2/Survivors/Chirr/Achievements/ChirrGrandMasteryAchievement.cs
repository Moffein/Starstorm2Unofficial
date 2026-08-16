using RoR2;
using Starstorm2Unofficial.Modules.Achievements;
using Starstorm2Unofficial.Survivors.Cyborg;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial.Survivors.Chirr.Achievements
{
    [RegisterAchievement("SS2UChirrClearGameTyphoon", "Skins.SS2UChirr.GrandMastery", null, 15, null)]
    public class ChirrGrandMasteryAchievement : BaseGrandMasteryAchievement
    {
        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return ChirrCore.bodyIndex;
        }
    }
}
