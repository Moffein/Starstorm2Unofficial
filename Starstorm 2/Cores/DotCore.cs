using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;
using static RoR2.DotController;

//FIXME: it's totally fucking fucked mate, big time

namespace Starstorm2.Cores
{
    [R2APISubmoduleDependency(nameof(DotAPI))]
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

        static internal DotIndex detritive;
        static internal DotIndex strangeCanPoison;
        static internal DotIndex gougeIndex;

        public DoTCore()
        {
            RegisterDoTs();
            /*
            IL.RoR2.DotController.Awake += DotController_Awake;
            On.RoR2.DotController.Awake += DotController_Awake1;
            On.RoR2.DotController.OnDestroy += DotController_OnDestroy;
            //IL.RoR2.DotController.GetDotDef += DotController_GetDotDef;
            On.RoR2.DotController.GetDotDef += DotController_GetDotDef1;
            //IL.RoR2.DotController.FixedUpdate += DotController_FixedUpdate;
            On.RoR2.DotController.FixedUpdate += DotController_FixedUpdate1;
            On.RoR2.DotController.HasDotActive += DotController_HasDotActive;
            IL.RoR2.DotController.InflictDot_refInflictDotInfo += DotController_InflictDot_refInflictDotInfo;
            */
        }

        //private bool DotController_HasDotActive(On.RoR2.DotController.orig_HasDotActive orig, DotController self, DotIndex dotIndex)
        //{
        //    if ((int)dotIndex >= VanillaDotCount)
        //    {
        //        if (ActiveCustomDots.TryGetValue(self, out var activeDots))
        //        {
        //            return activeDots[(int)dotIndex - VanillaDotCount];
        //        }

        //        return false;
        //    }

        //    return orig(self, dotIndex);
        //}

        //private void DotController_Awake(ILContext il)
        //{
        //    ILCursor c = new ILCursor(il);
        //    if (c.TryGotoNext(
        //                   i => i.MatchLdcI4(VanillaDotCount),
        //                   i => i.MatchNewarr<float>()))
        //    {
        //        c.Index++;
        //        c.EmitDelegate<Func<int, int>>(i => VanillaDotCount + dotDefs.Length);
        //    }
        //    else
        //        Debug.LogWarning("Failed to modify dot timer array in DotController.Awake");


        //}

        //private void DotController_Awake1(On.RoR2.DotController.orig_Awake orig, DotController self)
        //{
        //    orig(self);

        //    ActiveCustomDots.Add(self, new bool[CustomDotCount]);
            
        //}

        //private void DotController_OnDestroy(On.RoR2.DotController.orig_OnDestroy orig, DotController self)
        //{
        //    orig(self);

        //    ActiveCustomDots.Remove(self);
        //}

        //private object DotController_GetDotDef1(On.RoR2.DotController.orig_GetDotDef orig, DotController self, DotIndex dotIndex)
        //{
        //    return dotDefs[(int)dotIndex];
        //}

        //private void DotController_FixedUpdate1(On.RoR2.DotController.orig_FixedUpdate orig, DotController self)
        //{
        //    orig(self);

        //    if (NetworkServer.active)
        //    {
        //        for (var i = VanillaDotCount; i < dotDefs.Length; i++)
        //        {
        //            var dotDef = dotDefs[i];
        //            var dotTimers = self.GetFieldValue<float[]>("dotTimers");

        //            float dotProcTimer = dotTimers[i] - Time.fixedDeltaTime;
        //            if (dotProcTimer <= 0f)
        //            {
        //                dotProcTimer += dotDef.interval;

        //                var parameters = new object[] {
        //                    (DotController.DotIndex)i, dotDef.interval, -1
        //                };
        //                var mi = typeof(DotController).GetMethodCached("EvaluateDotStacksForType");
        //                mi.Invoke(self, parameters);
        //                var remainingActive = (int)parameters[2];

        //                ActiveCustomDots[self][i - VanillaDotCount] = remainingActive != 0;
        //            }

        //            dotTimers[i] = dotProcTimer;

        //            /*
        //            if (ActiveCustomDots[self][i])
        //            {
        //                _customDotVisuals[i]?.Invoke(self);
        //            }
        //            */
        //        }
        //    }
        //}

        protected internal void RegisterDoTs()
        {
            //LogCore.LogInfo("Initializing Core: " + base.ToString());

            //detritive = RegisterDot(1, 0.5f, DamageColorIndex.Item, BuffCore.detritiveBuff);
            //strangeCanPoison = RegisterDot(.5f, 0.5f, DamageColorIndex.Item, BuffCore.strangeCanPoisonBuff);
            //gougeIndex = RegisterDot(0.5f, 0.33f, DamageColorIndex.Bleed, BuffCore.gougeBuff);

            detritive = DotAPI.RegisterDotDef(1, 0.5f, DamageColorIndex.Item, BuffCore.detritiveBuff);
            strangeCanPoison = DotAPI.RegisterDotDef(.5f, 0.5f, DamageColorIndex.Item, BuffCore.strangeCanPoisonBuff);
            gougeIndex = DotAPI.RegisterDotDef(0.5f, 0.33f, DamageColorIndex.Bleed, BuffCore.gougeBuff);
        }

        //private DotIndex RegisterDot(float interval, float damageCoefficient, DamageColorIndex colorIndex, BuffDef associatedBuff)
        //{
        //    DotDef newDotDef = new DotDef();
        //    newDotDef.interval = interval;
        //    newDotDef.damageCoefficient = damageCoefficient;
        //    newDotDef.damageColorIndex = colorIndex;
        //    newDotDef.associatedBuff = associatedBuff;

        //    int last = DotController.dotDefs.Length;
        //    Array.Resize(ref DotController.dotDefs, last + 1);
        //    DotController.dotDefs[last] = newDotDef;

        //    return (DotIndex)last;
        //}

        //private void DotController_GetDotDef(ILContext il)
        //{
        //    ILCursor c = new ILCursor(il);
        //    if (c.TryGotoNext(x => x.MatchLdcI4(VanillaDotCount)))
        //    {
        //        c.Remove();
        //        c.Emit(OpCodes.Ldsfld, typeof(DotController).GetField(nameof(DotController.dotDefs), BindingFlags.NonPublic | BindingFlags.Static));
        //        c.Emit(OpCodes.Ldlen);
        //    }
        //    else
        //        Debug.LogWarning("Failed to modify bounds check in DotController.GetDotDef");
        //}

        //private void DotController_FixedUpdate(ILContext il)
        //{
        //    ILCursor c = new ILCursor(il);
        //    if (c.TryGotoNext(x => x.MatchLdcI4(VanillaDotCount)))
        //    {
        //        c.Remove();
        //        c.Emit(OpCodes.Ldsfld, typeof(DotController).GetField(nameof(DotController.dotDefs), BindingFlags.NonPublic | BindingFlags.Static));
        //        c.Emit(OpCodes.Ldlen);
        //    }
        //    else
        //        Debug.LogWarning("Failed to modify bounds check in DotController.FixedUpdate");
        //}

        //private void DotController_InflictDot_refInflictDotInfo(ILContext il)
        //{
        //    ILCursor c = new ILCursor(il);
        //    if (c.TryGotoNext(x => x.MatchLdcI4(VanillaDotCount)))
        //    {
        //        c.Remove();
        //        c.Emit(OpCodes.Ldsfld, typeof(DotController).GetField(nameof(DotController.dotDefs), BindingFlags.NonPublic | BindingFlags.Static));
        //    c.Emit(OpCodes.Ldlen);
        //    }
        //    else
        //        Debug.LogWarning("Failed to modify bounds check in DotController.FixedUpdate");
        //}
    }
}
