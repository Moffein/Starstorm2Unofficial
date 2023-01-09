using R2API;
using R2API.Utils;
using RoR2;
using RoR2.Skills;
using Starstorm2Unofficial.Cores;
using System.Collections;
using System.Linq;
using UnityEngine;

public static class Utils
{
    #region Skills
    /// <summary>
    /// LoadoutApi needs to add your skilldef, as well as any EntityState types your skill might use
    /// </summary>
    /// <param name="skillDef"></param>
    /// <param name="skillTypes"></param>
    public static void RegisterSkillDef(SkillDef skillDef, params System.Type[] skillTypes) {
        for (int i = 0; i < skillTypes.Length; i++) {
            Starstorm2Unofficial.Modules.States.AddState(skillTypes[i]);
        }

        Starstorm2Unofficial.Modules.Skills.skillDefs.Add(skillDef);
    }

    /// <summary>
    /// </summary>
    /// <param name="skillDef"></param>
    /// <param name="unlockableDef"></param>
    /// <returns>Returns a new skill variant to add to your character's skill families</returns>
    public static SkillFamily.Variant RegisterSkillVariant(SkillDef skillDef, UnlockableDef unlockableDef = null) {

        return new SkillFamily.Variant {
            skillDef = skillDef,
            unlockableDef = unlockableDef,
            viewableNode = new ViewablesCatalog.Node(skillDef.skillNameToken, false, null)
        };

    }

    /// <summary>
    /// Takes care of adding your character's skills. Adds a GenericSkill component to your CharacterBody, sets up a Skillfamily, and adds all your skill variants 
    /// </summary>
    /// <param name="characterBodyObject"></param>
    /// <param name="skillVariants"></param>
    /// <returns>Returns a SkillFamily with your skill variants. Set this to the corresponding SkillFamily in your character's SkillLocator (for example skillLocator.primary, etc)</returns>
    public static GenericSkill RegisterSkillsToFamily(GameObject characterBodyObject, params SkillFamily.Variant[] skillVariants) {
        return RegisterSkillsToFamily(characterBodyObject, "", skillVariants);

    }
    /// <summary>
    /// Takes care of adding your character's skills. Adds a GenericSkill component to your characterBody, sets up a skillfamily, and adds all your skill variants 
    /// </summary>
    /// <param name="characterBodyObject"></param>
    /// <param name="skillname">A label to the GenericSkill component. Useful for when a skill needs to reference your currently equipped skills</param>
    /// <param name="skillVariants"></param>
    /// <returns>Returns a SkillFamily with your skill variants. Set this to the corresponding SkillFamily in your character's SkillLocator (for example skillLocator.primary, etc)</returns>
    public static GenericSkill RegisterSkillsToFamily(GameObject characterBodyObject, string skillname, params SkillFamily.Variant[] skillVariants) {
        GenericSkill genericSkill = characterBodyObject.AddComponent<GenericSkill>();

        SkillFamily newFamily = ScriptableObject.CreateInstance<SkillFamily>();
        Starstorm2Unofficial.Modules.Skills.skillFamilies.Add(newFamily);

        genericSkill.SetFieldValue("_skillFamily", newFamily);
        genericSkill.SetFieldValue("skillName", skillname);

        newFamily.variants = skillVariants;

        return genericSkill;
    }

    /// <summary>
    /// if you're simply setting up all your variants at once, use RegisterSkillsToFamily. Use this if you want to add additional skills to an already set up family (for example in a config)
    /// </summary>
    /// <param name="genericSkill"></param>
    /// <param name="skillVariants"></param>
    public static void RegisterAdditionalSkills(GenericSkill genericSkill, params SkillFamily.Variant[] skillVariants) {

        SkillFamily skillfamily = genericSkill.skillFamily;

        skillfamily.variants = skillfamily.variants.Concat(skillVariants).ToArray();
    }

    #endregion
    #region Items/Equipment
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
    #endregion
    #region Misc
    internal static GameObject LoadCrosshair(string crosshairName)
    {
        return LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
    }

    public static IEnumerator BroadcastChat(string token)
    {
        yield return new WaitForSeconds(1);
        Chat.SendBroadcastChat(new Chat.SimpleChatMessage() { baseToken = token });
        yield break;
    }

    internal static string ScepterDescription(string desc)
    {
        return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
    }
    #endregion
}