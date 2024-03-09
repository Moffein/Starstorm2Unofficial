using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using System.Collections.Generic;
using EntityStates.SS2UStates.Nucleator.Utility;
using UnityEngine.SocialPlatforms;
using RoR2.Orbs;
using Starstorm2Unofficial.Cores;

namespace Starstorm2Unofficial.Survivors.Nucleator.Components
{
    public class NucleatorNetworkComponent : NetworkBehaviour
    {
        private HealthComponent healthComponent;
        private CharacterBody characterBody;
        private void Awake()
        {
            healthComponent = base.GetComponent<HealthComponent>();
            characterBody = base.GetComponent<CharacterBody>();
        }

        public void HealFractionAuthority(float healFraction)
        {
            if (this.hasAuthority) CmdHealFraction(healFraction);
        }

        [Command]
        private void CmdHealFraction(float healFraction)
        {
            if (!NetworkServer.active || !healthComponent) return;

            healthComponent.HealFraction(healFraction, default);
        }

        public void UtilityShockAuthority(Vector3 position, bool crit)
        {
            if (this.hasAuthority) CmdUtilityShockAuthority(position, crit);
        }

        [Command]
        private void CmdUtilityShockAuthority(Vector3 position, bool crit)
        {
            if (!NetworkServer.active || !characterBody) return;
            //todo
            TeamIndex myTeam = characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.None;
            List<HealthComponent> targetList = new List<HealthComponent>();
            Collider[] colliders = Physics.OverlapSphere(position, FireLeapOvercharge.shockRadius, LayerIndex.entityPrecise.mask);
            foreach (Collider c in colliders)
            {
                HurtBox hb = c.GetComponent<HurtBox>();
                if (hb && hb.healthComponent && !targetList.Contains(hb.healthComponent))
                {
                    TeamIndex targetTeam = TeamIndex.None;
                    if (hb.healthComponent.body.teamComponent) targetTeam = hb.healthComponent.body.teamComponent.teamIndex;
                    if (TeamManager.IsTeamEnemy(myTeam, targetTeam))  targetList.Add(hb.healthComponent);
                }
            }

            foreach (HealthComponent hc in targetList)
            {
                if (hc.body.mainHurtBox == null || !hc.body.mainHurtBox.isActiveAndEnabled) continue;
                HurtBox targetHurtbox = hc.body.mainHurtBox;
                LightningOrb lightning = new LightningOrb
                {
                    bouncedObjects = null,
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    damageValue = characterBody.damage * FireLeapOvercharge.shockDamageCoefficient,
                    procCoefficient = 0.5f,
                    teamIndex = characterBody.teamComponent ? characterBody.teamComponent.teamIndex : TeamIndex.None,
                    isCrit = crit,
                    procChainMask = default,
                    lightningType = LightningOrb.LightningType.Loader,
                    damageColorIndex = DamageColorIndex.Default,
                    bouncesRemaining = 0,
                    targetsToFindPerBounce = 1,
                    range = FireLeapOvercharge.shockRadius,
                    origin = base.transform.position,
                    damageType = DamageType.Shock5s,
                    speed = 30f
                };
                if (characterBody.HasBuff(BuffCore.nucleatorSpecialBuff)) lightning.damageType |= DamageType.PoisonOnHit;
                lightning.target = targetHurtbox;
                OrbManager.instance.AddOrb(lightning);
            }
        }
    }
}
