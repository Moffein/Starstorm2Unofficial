using System;
using System.Reflection;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.ObjectModel;
using UnityEngine.AddressableAssets;
using RoR2.Orbs;

namespace EntityStates.SS2UStates.Chirr
{
    //On stage start there seems to be a nullref related to RoR2.Indicator.SetVisibleInternal (called by RoR2.Indicator+IndicatorManager.OnPreRenderUI)
    //Seems to be harmless, but would be good to fix.
    public class ChirrHeal : BaseSkillState
    {
        public static GameObject healEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/EliteEarth/AffixEarthHealExplosion.prefab").WaitForCompletion();
        public static float baseDuration = 1.5f;
        public static float baseFireDuration = 1.3f;
        public static float recoil = 1f;
        public static float radius = 30f;
        public static float healFraction = .25f;
        public static float regenDuration = 3f;

        private float duration;
        private float fireDuration;
        private bool hasFired;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            this.fireDuration = baseFireDuration / this.attackSpeedStat;
            Util.PlayAttackSpeedSound("SS2UChirrHealStart", base.gameObject, this.attackSpeedStat);

            base.PlayAnimation("Gesture, Additive", "Utility", "Utility.playbackRate", this.duration);
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void FireHeal()
        {
            if (!this.hasFired)
            {
                Util.PlaySound("SS2UChirrHealTrigger", base.gameObject);
                hasFired = true;
                if (NetworkServer.active)
                {
                    EffectManager.SpawnEffect(healEffectPrefab, new EffectData
                    {
                        origin = base.transform.position,
                        scale = ChirrHeal.radius,
                        rootObject = base.gameObject
                    }, true);


                    List<HealthComponent> hcList = new List<HealthComponent>();
                    Collider[] array = Physics.OverlapSphere(base.transform.position, radius, LayerIndex.entityPrecise.mask);
                    for (int i = 0; i < array.Length; i++)
                    {
                        HurtBox hurtBox = array[i].GetComponent<HurtBox>();
                        if (hurtBox && hurtBox.healthComponent && !hcList.Contains(hurtBox.healthComponent))
                        {
                            hcList.Add(hurtBox.healthComponent);
                            if (hurtBox.healthComponent.body.teamComponent && hurtBox.healthComponent.body.teamComponent.teamIndex == base.GetTeam())
                            {
                                float healAmount = hurtBox.healthComponent.fullCombinedHealth * healFraction;   //was fullHealth, makes it bad when healing Vanilla Overloading
                                /*if (component.body.mainHurtBox && !component.body.disablingHurtBoxes)
                                {
                                    HealOrb healOrb = new HealOrb();
                                    healOrb.origin = base.transform.position;
                                    healOrb.target = component.body.mainHurtBox;
                                    healOrb.healValue = healAmount;
                                    healOrb.overrideDuration = 0.3f;
                                    OrbManager.instance.AddOrb(healOrb);
                                }*/
                                hurtBox.healthComponent.HealFraction(healFraction, default);
                                if (hurtBox.healthComponent.body != base.characterBody) hurtBox.healthComponent.body.AddTimedBuff(RoR2Content.Buffs.CrocoRegen, regenDuration);

                                Util.CleanseBody(hurtBox.healthComponent.body, true, false, false, true, true, false);
                            }
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                FireHeal();
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}