using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Starstorm2Unofficial
{
    internal class SneedUtils
    {
        public static bool IsEnemyInSphere(float radius, Vector3 position, TeamIndex team, bool airborneOnly = false)
        {
            List<HealthComponent> hcList = new List<HealthComponent>();
            Collider[] array = Physics.OverlapSphere(position, radius, LayerIndex.entityPrecise.mask);
            for (int i = 0; i < array.Length; i++)
            {
                HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent && !hcList.Contains(healthComponent))
                    {
                        hcList.Add(healthComponent);
                        if (healthComponent.body && healthComponent.body.teamComponent && healthComponent.body.teamComponent.teamIndex != team)
                        {
                            if (!airborneOnly ||
                                (healthComponent.body.isFlying ||
                                (healthComponent.body.characterMotor && !healthComponent.body.characterMotor.isGrounded)))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
}
