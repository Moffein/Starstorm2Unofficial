using RoR2;
using RoR2.Achievements;
using Starstorm2Unofficial.Survivors.Nemmando.Components;

namespace Starstorm2Unofficial.Survivors.Nemmando.Achievements
{
    [RegisterAchievement("SS2UNemmandoUnlock", "Characters.SS2UNemmando", null, 3, null)]
    public class NemmandoUnlockAchievement : BaseAchievement
    {
		public override void OnInstall()
		{
			base.OnInstall();

            NemmandoUnlockComponent.OnDeath += NemmandoUnlockComponent_OnDeath;
		}

        private void NemmandoUnlockComponent_OnDeath(Run obj)
        {
            base.Grant();
        }

        public override void OnUninstall()
        {
            NemmandoUnlockComponent.OnDeath -= NemmandoUnlockComponent_OnDeath;
            base.OnUninstall();
        }
    }
}
