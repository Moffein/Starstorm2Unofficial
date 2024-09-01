using R2API;
using RoR2;
using Starstorm2Unofficial.Cores;
using System;
using System.Globalization;
using UnityEngine;

public abstract class SS2Item
{
    public ItemDef itemDef;
    public abstract string NameInternal { get; }
    public virtual bool CanRemove { get; } = true;
    public abstract ItemTier Tier { get; }
    public abstract ItemTag[] Tags { get; }
    public abstract string PickupIconPath { get; }
    public abstract string PickupModelPath { get; }
    public static GameObject displayPrefab;
    public virtual bool DropInMultiBlacklist { get; } = false;

    public virtual void Init()
    {
        RegisterItem();
        RegisterHooks();
    }

    protected virtual void RegisterItem()
    {
        string upperName = this.NameInternal.ToUpper(CultureInfo.InvariantCulture);
        itemDef = ScriptableObject.CreateInstance<ItemDef>();
        itemDef.name = NameInternal;
        itemDef.nameToken = $"ITEM_{upperName}_NAME";
        itemDef.pickupToken = $"ITEM_{upperName}_PICKUP";
        itemDef.descriptionToken = $"ITEM_{upperName}_DESC";
        itemDef.loreToken = $"ITEM_{upperName}_LORE";
        itemDef.canRemove = CanRemove;
        itemDef.pickupIconSprite = PickupIconPath != "" ? Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<Sprite>(PickupIconPath) : null;
        itemDef.pickupModelPrefab = PickupModelPath != "" ? Starstorm2Unofficial.Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(PickupModelPath) : null;
        itemDef.tags = Tags;
        itemDef.tier = Tier;
        itemDef.deprecatedTier = Tier;
        Starstorm2Unofficial.Modules.Items.itemDefs.Add(itemDef);

        //LanguageAPI.Add(itemDef.nameToken, Name);
        //LanguageAPI.Add(itemDef.pickupToken, Pickup);
        //LanguageAPI.Add(itemDef.descriptionToken, Description);
        //LanguageAPI.Add(itemDef.loreToken, Lore);

        if (PickupModelPath != "")
        {
            GameObject modelPrefab = itemDef.pickupModelPrefab;
            if (modelPrefab)
            {
                SetupMaterials(modelPrefab);
            }
            itemDef.pickupModelPrefab = modelPrefab;
        }

        //ItemDisplayRuleDict displayRules = CreateDisplayRules();
        //CustomItem customItem = new CustomItem(itemDef, displayRules);
    }

    protected virtual void SetupMaterials(GameObject modelPrefab) {
        Starstorm2Unofficial.Modules.Assets.ConvertAllRenderersToHopooShader(modelPrefab);
    }

    public abstract void RegisterHooks();

    public virtual ItemDisplayRuleDict CreateDisplayRules()
    {
        return new ItemDisplayRuleDict(new ItemDisplayRule[0]);
    }

    public int GetCount(CharacterBody body)
    {
        if (!(body && body.inventory)) return 0;
        return body.inventory.GetItemCount(itemDef);
    }

}

abstract class SS2Item<T> : SS2Item where T : SS2Item<T>
{
    public static T instance { get; private set; }

    public SS2Item()
    {
        instance = this as T;
    }
}