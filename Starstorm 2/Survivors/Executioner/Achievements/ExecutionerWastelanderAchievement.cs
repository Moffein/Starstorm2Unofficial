using RoR2;
using RoR2.Achievements;

namespace Starstorm2Unofficial.Survivors.Executioner.Achievements
{
	[RegisterAchievement("SS2UExecutionerWastelander", "Skins.SS2UExecutioner.Wastelander", null, null)]
	public class ExecutionerWastelanderAchievement : BaseAchievement
	{
		public override BodyIndex LookUpRequiredBodyIndex()
		{
			return Survivors.Executioner.ExecutionerCore.bodyIndex;
		}

		public override void OnBodyRequirementMet()
		{
			base.OnBodyRequirementMet();
            On.EntityStates.Interactables.StoneGate.Opening.OnEnter += StoneGateOpen;
		}

        private void StoneGateOpen(On.EntityStates.Interactables.StoneGate.Opening.orig_OnEnter orig, EntityStates.Interactables.StoneGate.Opening self)
		{
			orig(self);
			base.Grant();
        }

        public override void OnBodyRequirementBroken()
		{
			On.EntityStates.Interactables.StoneGate.Opening.OnEnter -= StoneGateOpen;
			base.OnBodyRequirementBroken();
		}

	}
}
