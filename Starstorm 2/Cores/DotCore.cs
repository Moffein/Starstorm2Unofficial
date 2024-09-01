using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using static RoR2.DotController;

//FIXME: it's totally fucking fucked mate, big time

namespace Starstorm2Unofficial.Cores
{
    class DoTCore
    {
        //private static DotController.DotDef[] DotDefs
        //{
        //    //get => typeof(DotController).GetFieldValue<DotController.DotDef[]>("dotDefs");
        //    //set => typeof(DotController).SetFieldValue("dotDefs", value);
        //    get => DotController.dotDefs;
        //    set { DotController.dotDefs = value; }
        //}

        //private static void ResizeDotDefs(int newSize)
        //{
        //    //var dotDefs = DotDefs;
        //    Array.Resize(ref dotDefs, newSize);
        //    //DotDefs = dotDefs;
        //}

        //private static readonly int VanillaDotCount = dotDefs.Length;
        //private static int CustomDotCount => dotDefs.Length - VanillaDotCount;
        //private static readonly Dictionary<DotController, bool[]> ActiveCustomDots = new Dictionary<DotController, bool[]>();

        public static DotIndex DetritiveTrematodeInfection;
        public static DotIndex StrangeCanPoison;
        public static DotIndex NemmandoGouge;
        public static DotIndex NucleatorRadiation;

        public static GameObject TrematodeHitEffect;
        public static GameObject StrangeCanHitEffect;

        public DoTCore()
        {
            RegisterDoTs();

            StrangeCanHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Croco/CrocoDiseaseImpactEffect.prefab").WaitForCompletion().InstantiateClone("SS2UStrangeCanHitEffect", false);
            Modules.Assets.AddEffect(StrangeCanHitEffect, "SS2UStrangeCan");

            TrematodeHitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleSpitExplosion.prefab").WaitForCompletion().InstantiateClone("SS2UTrematodeHitEffect", false);
            Modules.Assets.AddEffect(TrematodeHitEffect);   //This removes the sound from it
        }

        protected internal void RegisterDoTs()
        {
            DetritiveTrematodeInfection = DotAPI.RegisterDotDef(1f, 1f, DamageColorIndex.Item, BuffCore.detritiveBuff);
            StrangeCanPoison = DotAPI.RegisterDotDef(1f, 1f, DamageColorIndex.Poison, BuffCore.strangeCanPoisonBuff);
            NemmandoGouge = DotAPI.RegisterDotDef(0.25f, 0.25f, DamageColorIndex.Bleed, BuffCore.gougeBuff);
            NucleatorRadiation = DotAPI.RegisterDotDef(1f, 1f, DamageColorIndex.Poison, BuffCore.nucleatorSpecialDebuff);
        }
    }
}
