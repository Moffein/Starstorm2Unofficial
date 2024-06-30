using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using Starstorm2Unofficial.Modules;
using System;
using System.Globalization;
using UnityEngine;

public abstract class SS2Equipment
{
    public static string prefabPath = "@Starstorm2:Assets/AssetBundle/Items/";

    //public EquipmentIndex index;
    public EquipmentDef equipDef;
    public abstract string NameInternal { get; }
    public virtual float Cooldown { get; } = 60f;
    public virtual bool CanDrop { get; } = true;
    public virtual bool EnigmaCompatible { get; } = true;
    public virtual bool IsBoss { get; } = false;
    public virtual bool IsLunar { get; } = false;
    public virtual BuffDef PassiveBuffDef { get; } = null;
    public abstract string PickupIconPath { get; }
    public abstract string PickupModelPath { get; }
    public static GameObject displayPrefab;

    public virtual EquipmentDef Init()
    {
        var equipDef = RegisterEquipment();
        RegisterHooks();

        return equipDef;
    }

    protected EquipmentDef RegisterEquipment()
    {
        var upperName = this.NameInternal.ToUpper(CultureInfo.InvariantCulture);
        EquipmentDef def = ScriptableObject.CreateInstance<EquipmentDef>();
        def.name = NameInternal;
        def.nameToken = $"EQUIPMENT_{upperName}_NAME";
        def.pickupToken = $"EQUIPMENT_{upperName}_PICKUP";
        def.descriptionToken = $"EQUIPMENT_{upperName}_DESC";
        def.loreToken = $"EQUIPMENT_{upperName}_LORE";
        def.cooldown = Cooldown;
        def.canDrop = CanDrop;
        def.enigmaCompatible = EnigmaCompatible;
        def.isBoss = IsBoss;
        def.isLunar = IsLunar;
        def.appearsInSinglePlayer = true;
        def.appearsInMultiPlayer = true;
        def.passiveBuffDef = PassiveBuffDef;
        def.pickupIconSprite = PickupIconPath != "" ? Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>(PickupIconPath) : null;
        def.pickupModelPrefab = PickupModelPath != "" ? Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(PickupModelPath) : null;

        //LanguageAPI.Add(def.nameToken, Name);
        //LanguageAPI.Add(def.pickupToken, Pickup);
        //LanguageAPI.Add(def.descriptionToken, Description);
        //LanguageAPI.Add(def.loreToken, Lore);

        if (PickupModelPath != "")
        {
            var modelPrefab = def.pickupModelPrefab;
            if (modelPrefab)
            {
                Assets.ConvertAllRenderersToHopooShader(modelPrefab);
            }
        }

        var displayRules = CreateDisplayRules();

        On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;

        equipDef = def;
        Starstorm2Unofficial.Modules.Items.equipmentDefs.Add(equipDef);

        return def;
    }

    public abstract void RegisterHooks();

    public virtual ItemDisplayRuleDict CreateDisplayRules()
    {
        return new ItemDisplayRuleDict(new ItemDisplayRule[0]);
    }

    public static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
    {
        MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
        CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

        for (int i = 0; i < meshes.Length; i++)
        {
            rendererInfos[i] = new CharacterModel.RendererInfo
            {
                defaultMaterial = meshes[i].material,
                renderer = meshes[i],
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false
            };
        }

        return rendererInfos;
    }

    private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
    {
        if (equipmentDef == equipDef)
        {
            return ActivateEquipment(self);
        }
        return orig(self, equipmentDef);
    }

    protected abstract bool ActivateEquipment(EquipmentSlot equip);
}

abstract class SS2Equipment<T> : SS2Equipment where T : SS2Equipment<T>
{
    public static T instance { get; private set; }

    public SS2Equipment()
    {
        instance = this as T;
    }
}


