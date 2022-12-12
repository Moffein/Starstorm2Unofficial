using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace Starstorm2.Cores
{
    [R2APISubmoduleDependency((nameof(DifficultyAPI)), (nameof(ResourcesAPI)))]
    public class TyphoonCore
    {
        public static DifficultyIndex diffIdxTyphoon;
        public const float difScale = 3.5f;
        public const string difName = "Typhoon";
        public string difDesc =
            @"Maximum challenge. The world is a nightmare, and survival is merely an illusion. Nobody has what it takes.<style=cStack>

>Player health regeneration: <style=cIsHealth>-40%</style>
>Difficulty scaling: <style=cIsHealth>+75%</style>";
        public const string iconPath = "Assets/AssetBundle/GeneralAssets/TyphoonIcon";
        public Sprite iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("TyphoonIcon");
        public const string difServerTag = "ty";
        public Color difColor = new Color(0.71f, 0.25f, 0.50f);
        private static int defMonsterCap;

        public TyphoonCore()
        {
            string nameToken = "DIFFICULTY_TYPHOON_NAME";
            string descToken = "DIFFICULTY_TYPHOON_DESCRIPTION";

            if (Modules.Config.TyphoonIncreaseSpawnCap.Value)
                difDesc += "\n>Monster spawn limit: <style=cIsHealth>+100%</style></style>";



            //FIXME: typhoon icon doesn't work without R2API AssetBundleResourcesProvider, game uses Resources.Load which can't find assets in our asset bundle
            DifficultyDef typhoon = new DifficultyDef(difScale, nameToken, iconPath, descToken, difColor, difServerTag, true);
            //can't even load it ourselves to bypass the resource lookup
            typhoon.iconSprite = iconSprite;
            typhoon.foundIconSprite = true;

            LanguageAPI.Add(nameToken, difName);
            LanguageAPI.Add(descToken, difDesc);

            diffIdxTyphoon = DifficultyAPI.AddDifficulty(typhoon);

            Run.onRunStartGlobal += Run_onRunStartGlobal;
            Run.onRunDestroyGlobal += Run_onRunDestroyGlobal;
        }

        private void Run_onRunStartGlobal(Run run)
        {
            defMonsterCap = TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit;
            if (run.selectedDifficulty == diffIdxTyphoon)
            {
                foreach (CharacterMaster cm in run.userMasters.Values)
                {
                    cm.inventory.GiveItem(RoR2Content.Items.MonsoonPlayerHelper.itemIndex);
                }
                if (Modules.Config.TyphoonIncreaseSpawnCap.Value)
                {
                    TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit *= 2;
                }
            }
        }

        private void Run_onRunDestroyGlobal(Run run)
        {
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = defMonsterCap;
        }
    }
}
