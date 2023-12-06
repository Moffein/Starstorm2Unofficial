using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;

namespace Starstorm2Unofficial.Modules
{
    public static class BazaarChecker
    {
        private static bool inBazaar = false;
        private static SceneDef bazaarScene = Addressables.LoadAssetAsync<SceneDef>("RoR2/Base/bazaar/bazaar.asset").WaitForCompletion();

        internal static void Stage_onStageStartGlobal(Stage obj)
        {
            inBazaar = false;
            if (RoR2.SceneCatalog.GetSceneDefForCurrentScene() == bazaarScene)
            {
                inBazaar = true;
            }
        }

        public static bool InBazaar()
        {
            return inBazaar;
        }
    }
}
