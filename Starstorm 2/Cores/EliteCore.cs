using R2API;
using R2API.Utils;
using RoR2;
using RoR2.ContentManagement;
using Starstorm2.Cores.Elites;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Starstorm2.Cores
{
    [R2APISubmoduleDependency(nameof(EliteAPI))]
    class EliteCore
    {
        public static List<EliteDef> eliteDefs = new List<EliteDef>();
        public List<SS2Elite> elites = new List<SS2Elite>();
        internal bool esoInstalled = (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.jarlyk.eso"));

        //private static Color32 voidColor = new Color32(165, 52, 175, 255);
        //private static Color32 ethColor = new Color32(18, 93, 74, 255);
        //public static GameObject voidBubble;

        public EliteCore()
        {
            elites.Add(new VoidElite());
            //elites.Add(new EtherealElite());
            //elites.Add(new DazingElite());

            foreach (var elite in elites)
            {
                elite.Init();
            }
        }
    }

    abstract class SS2Elite
    {
        public EliteDef eliteDef;

        public abstract string eliteName { get; }
        public virtual int TierIndex { get; } = 1;
        public abstract SS2Equipment AffixEquip { get; }
        public abstract Color32 EliteColor { get; }
        public virtual Sprite affixIconSprite { get; } = null;
        public virtual CombatDirector.EliteTierDef customTier { get; } = null;
        public virtual int desiredTierIndex { get; } = -1;

        public BuffDef eliteBuffDef;

        public virtual void Init()
        {
            RegisterElite();
            RegisterHooks();
            RegisterAdditional();
        }

        public void RegisterElite()
        {
            string upperName = this.eliteName.ToUpper(CultureInfo.InvariantCulture);

            BuffDef affixDef = ScriptableObject.CreateInstance<BuffDef>();
            affixDef.name = $"Affix{eliteName}";
            affixDef.buffColor = Color.white; //so the icon appears in its proper color
            affixDef.canStack = false;
            affixDef.iconSprite = affixIconSprite;
            //affixDef.startSfx

            eliteDef = ScriptableObject.CreateInstance<EliteDef>();
            eliteDef.color = EliteColor;
            eliteDef.modifierToken = $"ELITE_{upperName}";
            eliteDef.name = eliteName;

            affixDef.eliteDef = eliteDef;
            eliteBuffDef = affixDef;
            eliteDef.eliteEquipmentDef = AffixEquip.Init();

            if (customTier != null)
            {
                /*
                CombatDirector.EliteTierDef eliteTier = new CombatDirector.EliteTierDef();
                eliteTier.costMultiplier = CostMult;
                eliteTier.damageBoostCoefficient = DamageMult;
                eliteTier.healthBoostCoefficient = HealthMult;
                eliteTier.eliteTypes = new EliteDef[0];
                eliteTier.isAvailable = CanSpawn;
                */

                if (customTier.eliteTypes == null)
                    customTier.eliteTypes = Array.Empty<EliteDef>();
                if (customTier.eliteTypes == null)
                    Debug.Log("****************** not working!!");
                int newTierIdx = EliteAPI.AddCustomEliteTier(customTier, desiredTierIndex);
                EliteAPI.Add(new CustomElite(eliteDef, new List<CombatDirector.EliteTierDef>() { customTier }));
            }
            else
            {
                EliteAPI.Add(new CustomElite(eliteDef, new List<CombatDirector.EliteTierDef>() { CombatDirector.eliteTiers[TierIndex] }));
            }

            EliteCore.eliteDefs.Add(eliteDef);
            EquipmentCore.instance.equipmentDefs.Add(eliteDef.eliteEquipmentDef);
            BuffCore.buffDefs.Add(affixDef);
            //CreateESOEliteCard(CostMult, DamageMult, HealthMult, CanSpawn);
            On.RoR2.Run.Start += (orig, self) =>
            {
                Debug.Log(CombatDirector.eliteTiers[3].eliteTypes.Length);
                orig(self);
            };
        }

        public virtual void RegisterHooks()
        {
            /*
            On.RoR2.CharacterModel.UpdateMaterials += (On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel self) =>
            {
            };
            */
        }

        protected void SetUpEliteMaterialAlternate(On.RoR2.CharacterModel.orig_UpdateMaterials orig, CharacterModel mdl)
        {
            orig(mdl);
            if (mdl.myEliteIndex == eliteDef.eliteIndex)
            {
                CharacterModel.RendererInfo[] rendererInfos = mdl.baseRendererInfos;
                MaterialPropertyBlock prop = mdl.propertyStorage;
                foreach (CharacterModel.RendererInfo i in rendererInfos)
                {
                    Renderer renderer = i.renderer;
                    renderer.GetPropertyBlock(prop);
                    prop.SetInt(RoR2.CommonShaderProperties._EliteIndex, -1);
                    prop.SetColor("_Color", EliteColor);
                    renderer.SetPropertyBlock(prop);
                }
            }
        }

        public abstract void RegisterAdditional();

        //ESO-dependant code encapsulated for convenience if we ever drop the dependency
        private void CreateESOEliteCard(float costMult, float damageMult, float healthMult, Func<bool> canSpawn)
        {
            EliteSpawningOverhaul.EliteAffixCard card = new EliteSpawningOverhaul.EliteAffixCard();
            card.costMultiplier = costMult;
            card.damageBoostCoeff = damageMult;
            card.healthBoostCoeff = healthMult;
            card.isAvailable = canSpawn;
            card.eliteType = eliteDef.eliteIndex;
            EliteSpawningOverhaul.EsoLib.Cards.Add(card);
        }
    }

    abstract class SS2Elite<T> : SS2Elite where T : SS2Elite<T>
    {
        public static T instance { get; private set; }

        public SS2Elite()
        {
            instance = this as T;
        }
    }
}
