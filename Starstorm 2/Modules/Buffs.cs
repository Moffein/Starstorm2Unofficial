using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace Starstorm2.Modules
{
    public static class Buffs
    {
        internal static BuffDef fearDebuff;
        internal static BuffDef gougeDebuff;
        internal static BuffDef exeAssistBuff;
        internal static BuffDef exeSuperchargedBuff;

        internal static BuffDef nucleatorSpecialBuff;

        internal static List<BuffDef> buffDefs = new List<BuffDef>();

        internal static void Initialize()
        {
            // ill finish this later
            //fearDebuff = AddNewBuff("ExecutionerFearDebuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, false, true);
            //gougeDebuff = AddNewBuff("GougeDebuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffGenericShield"), Color.white, true, true);
            exeAssistBuff = AddNewBuff("ExecutionerAssistBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffPowerIcon"), Color.white, false, false);
            exeSuperchargedBuff = AddNewBuff("ExecutionerSuperchargedBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffNullifiedIcon"), new Color(72 / 255, 1, 1), false, false);
            nucleatorSpecialBuff = AddNewBuff("NucleatorSpecialBuff", LegacyResourcesAPI.Load<Sprite>("Textures/BuffIcons/texBuffOverheat"), Color.green, false, false);
        }

        // simple helper method
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack, bool isDebuff)
        {
            BuffDef buffDef = ScriptableObject.CreateInstance<BuffDef>();
            buffDef.name = buffName;
            buffDef.buffColor = buffColor;
            buffDef.canStack = canStack;
            buffDef.isDebuff = isDebuff;
            buffDef.eliteDef = null;
            buffDef.iconSprite = buffIcon;

            buffDefs.Add(buffDef);

            return buffDef;
        }
    }
}