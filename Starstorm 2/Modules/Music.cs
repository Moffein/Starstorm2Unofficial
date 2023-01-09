using MonoMod.Cil;
using System;

namespace Starstorm2Unofficial.Modules
{
    internal static class Music
    {
        internal static int musicSources = 0;

        internal static void Initialize()
        {
            IL.RoR2.MusicController.LateUpdate += il =>
            {
                var cursor = new ILCursor(il);

                cursor.GotoNext(i => i.MatchStloc(out _));
                cursor.EmitDelegate<Func<bool, bool>>(b =>
                {
                    if (b)
                        return true;

                    return musicSources != 0;
                });
            };
        }
    }
}