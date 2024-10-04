using R2API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Starstorm2Unofficial.Modules
{
    public class SoundBanks
    {
        private static bool initialized = false;
        public static string SoundBankDirectory
        {
            get
            {
                return Path.Combine(Files.assemblyDir, "Assets");
            }
        }

        public static void Init()
        {
            if (initialized) return;
            initialized = true;

            using (Stream manifestResourceStream = new FileStream(SoundBankDirectory + "\\Starstorm2Unofficial.bnk", FileMode.Open))
            {
                byte[] array = new byte[manifestResourceStream.Length];
                manifestResourceStream.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            //This check needs to be done because there's a mystery conflict with SS2O music.
            if (!ModCompat.SS2OCompat.pluginLoaded)
            {
                using (Stream manifestResourceStream = new FileStream(SoundBankDirectory + "\\SS2UMusic.bnk", FileMode.Open))
                {
                    byte[] array = new byte[manifestResourceStream.Length];
                    manifestResourceStream.Read(array, 0, array.Length);
                    SoundAPI.SoundBanks.Add(array);
                }
            }
        }
    }
}
