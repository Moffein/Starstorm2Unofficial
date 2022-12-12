using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

//FIXME: void sphere materials broken
//FIXME: void slow debuff wears off too quickly to stack and doesn't slow

namespace Starstorm2.Cores.Elites
{
    class VoidElite : SS2Elite<VoidElite>
    {
        public override string eliteName => "Void";
        //public override float CostMult => CombatDirector.baseEliteCostMultiplier * 4;
        //public override float DamageMult => CombatDirector.baseEliteDamageBoostCoefficient * 1.5f;
        //public override float HealthMult => CombatDirector.baseEliteHealthBoostCoefficient * 3;
        //public override Func<SpawnCard.EliteRules, bool> CanSpawn => (rules) =>
        //{
        //    if (rules == SpawnCard.EliteRules.Default)
        //    {
        //        if (Run.instance.loopClearCount > 0 || SceneManager.GetActiveScene().name == "arena")
        //        {
        //            return true;
        //        }
        //        else if (VoidCore.instance != null && VoidCore.instance.voidCleared)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //};
        public override int TierIndex => -1;
        public override SS2Equipment AffixEquip => new Equipment.AffixVoid();
        public override Color32 EliteColor => new Color32(165, 52, 175, 255);
        public override Sprite affixIconSprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("buffAffixVoid.png");
        public override CombatDirector.EliteTierDef customTier => new CombatDirector.EliteTierDef()
        {
            costMultiplier = CombatDirector.baseEliteCostMultiplier * 4,
            damageBoostCoefficient = CombatDirector.baseEliteDamageBoostCoefficient * 1.5f,
            healthBoostCoefficient = CombatDirector.baseEliteHealthBoostCoefficient * 3,
            isAvailable = (rules) =>
            {
                if (rules == SpawnCard.EliteRules.Default)
                {
                    if (Run.instance.loopClearCount > 0 || SceneManager.GetActiveScene().name == "arena")
                    {
                        return true;
                    }
                    else if (VoidCore.instance != null && VoidCore.instance.voidCleared)
                    {
                        return true;
                    }
                }
                return false;
            }
        };
        public override int desiredTierIndex => 3;

        static internal GameObject voidBubble;
        static internal BuffDef voidSlow;

        public override void RegisterHooks()
        {
            base.RegisterHooks();
            On.RoR2.CharacterModel.UpdateMaterials += SetUpEliteMaterialAlternate;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.BuffWard.BuffTeam += BuffWard_BuffTeam;
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            On.RoR2.CharacterBody.AddTimedBuff_BuffDef_float += CharacterBody_AddTimedBuff_BuffDef_float;
        }

        public override void RegisterAdditional()
        {
            /*
            CombatDirector.EliteTierDef tierDef = new CombatDirector.EliteTierDef()
            {
                costMultiplier = CombatDirector.baseEliteCostMultiplier * 3,
                damageBoostCoefficient = CombatDirector.baseEliteDamageBoostCoefficient * 2,
                healthBoostCoefficient = CombatDirector.baseEliteHealthBoostCoefficient * 3,
                isAvailable = CanSpawn,
                eliteTypes = new EliteDef[] { this.eliteDef }
            };
            EliteCore.RegisterEliteTier(tierDef);
            */

            voidSlow = ScriptableObject.CreateInstance<BuffDef>();
            voidSlow.buffColor = EliteColor;
            voidSlow.canStack = true;
            voidSlow.iconSprite = Resources.Load<Sprite>("Textures/BuffIcons/texBuffNullifiedIcon");
            voidSlow.name = "VoidSlow";

            BuffCore.buffDefs.Add(voidSlow);

            voidBubble = Resources.Load<GameObject>("Prefabs/NetworkedObjects/AffixHauntedWard").InstantiateClone("VoidWard", true);
            BuffWard ward = voidBubble.GetComponent<BuffWard>();
            ward.buffDuration = 3.1f;
            ward.buffDef = voidSlow;
            ward.interval = 2.0f;
            ward.invertTeamFilter = true;
            NetworkedBodyAttachment a = voidBubble.AddOrGetComponent<NetworkedBodyAttachment>();
            a.shouldParentToAttachedBody = true;

            //someone, anyone, please figure out a better way to change materials
            var mats = Resources.FindObjectsOfTypeAll<Material>();

            GameObject indicator = ward.transform.Find("Indicator").gameObject;
            MeshRenderer sphereRenderer = indicator.transform.Find("IndicatorSphere").gameObject.GetComponent<MeshRenderer>();
            //FIXME: no longer works
            var newMat = mats.Where(m => m.name == "matNullifierExplosionAreaIndicatorHard").FirstOrDefault();
            sphereRenderer.material = newMat;

            ParticleSystemRenderer particleRenderer = indicator.transform.Find("Spores").gameObject.GetComponent<ParticleSystemRenderer>();
            //FIXME: no longer works
            newMat = mats.Where(m => m.name == "matNullifierStarParticle").FirstOrDefault();
            particleRenderer.material = newMat;
            //Material wardMat = ward.GetComponentInChildren<Material>();
            //wardMat.color = new Color(1.0f, 1.0f, 0f, 1.0f);
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);

            //Inventory inv = self.inventory;
            self.AddItemBehavior<AffixVoidBehavior>(self.HasBuff(eliteBuffDef) ? 1 : 0);
        }

        private void CharacterModel_UpdateOverlays(On.RoR2.CharacterModel.orig_UpdateOverlays orig, CharacterModel self)
        {
            orig(self);
            if (self.body.HasBuff(eliteBuffDef))
            {
            }
        }

        private void BuffWard_BuffTeam(On.RoR2.BuffWard.orig_BuffTeam orig, BuffWard self, IEnumerable<TeamComponent> recipients, float radiusSqr, Vector3 currentPosition)
        {
            orig(self, recipients, radiusSqr, currentPosition);
            if (self.buffDef == eliteBuffDef)
            {
                //create visual effect for void ward pulse
            }
        }

        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self.HasBuff(voidSlow))
            {
                int debuffStack = self.GetBuffCount(voidSlow);
                self.moveSpeed *= (float)Math.Pow(0.9, debuffStack);
                self.jumpPower *= (float)Math.Pow(0.9, debuffStack);
            }
        }

        private void CharacterBody_AddTimedBuff_BuffDef_float(On.RoR2.CharacterBody.orig_AddTimedBuff_BuffDef_float orig, CharacterBody self, BuffDef buffDef, float duration)
        {
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.CharacterBody::AddTimedBuff(RoR2.BuffIndex,System.Single)' called on client");
                return;
            }

            if (buffDef == voidSlow)
            {
                int count = 0;
                //reset slowdown debuff when adding a new stack
                foreach (var buff in self.timedBuffs)
                {
                    if (buff.buffIndex == voidSlow.buffIndex && buff.timer < duration)
                    {
                        buff.timer = duration;
                        count++;
                    }
                }
                // Don't add anymore stacks if cap has been reached, the current stacks still get reset. 
                if (count < 5)
                    orig(self, buffDef, duration);
            }
            else
                orig(self, buffDef, duration);
        }

        public class AffixVoidBehavior : CharacterBody.ItemBehavior
        {
            private GameObject voidWard;
            //private const float voidPulseInterval = 6f;

            //private float pulseStopwatch = 0f;

            public void Awake()
            {
            }

            public void FixedUpdate()
            {
                if (!NetworkServer.active)
                    return;
                bool enabled = stack > 0;
                if (voidWard != enabled)
                {
                    if (enabled)
                    {
                        voidWard = UnityEngine.Object.Instantiate(voidBubble);
                        voidWard.GetComponent<BuffWard>().Networkradius = 25f + body.radius;
                        voidWard.GetComponent<TeamFilter>().teamIndex = body.teamComponent.teamIndex;
                        voidWard.GetComponent<NetworkedBodyAttachment>().AttachToGameObjectAndSpawn(body.gameObject);
                        Util.PlaySound("VoidEliteSpawn", voidWard);
                    }
                    else
                    {
                        UnityEngine.Object.Destroy(voidWard);
                        voidWard = null;
                    }
                    /*
                    pulseStopwatch += Time.fixedDeltaTime;
                    if (pulseStopwatch >= voidPulseInterval)
                    {
                        pulseStopwatch = 0;
                    }
                    */
                }
            }

            private void OnDisable()
            {
                if (voidWard) UnityEngine.Object.Destroy(voidWard);
            }
        }
    }

}
