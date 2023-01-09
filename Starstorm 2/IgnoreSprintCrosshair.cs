using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial
{
    //This wouldn't be needed if the crosshair override system actually worked.
    public static class IgnoreSprintCrosshair
    {
        public static HashSet<BodyIndex> bodies = new HashSet<BodyIndex>();

        private static bool initialized = false;
        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            IL.RoR2.UI.CrosshairManager.UpdateCrosshair += SuppressSprintCrosshair;
        }

        private static void SuppressSprintCrosshair(MonoMod.Cil.ILContext il)
        {
            ILCursor c = new ILCursor(il);
            if (c.TryGotoNext(MoveType.After,
                x => x.MatchCallvirt<CharacterBody>("get_isSprinting")
                ))
            {
                c.Emit(OpCodes.Ldarg_1);    //targetBody
                c.EmitDelegate<Func<bool, CharacterBody, bool>>((isSprinting, body) =>
                {
                    return isSprinting && !bodies.Contains(body.bodyIndex);
                });
            }
        }
    }
}
