using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

namespace Starstorm2Unofficial.Cores
{
    [R2APISubmoduleDependency((nameof(DifficultyAPI)))]
    public class TyphoonCore
    {
        public static DifficultyIndex diffIdxTyphoon;
        public const float difScale = 3.5f;
        public const string iconPath = "Assets/AssetBundle/GeneralAssets/TyphoonIcon";
        public Sprite iconSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("TyphoonIcon");
        public const string difServerTag = "ty";
        public Color difColor = new Color(0.71f, 0.25f, 0.50f);
        private static int defMonsterCap;

        public TyphoonCore()
        {
            string nameToken = "DIFFICULTY_TYPHOON_NAME";
            string descToken = "DIFFICULTY_TYPHOON_DESCRIPTION";

            if (Modules.Config.TyphoonIncreaseSpawnCap.Value) descToken = "DIFFICULTY_TYPHOON_INCREASEDSPAWNCAP_DESCRIPTION";

            //FIXME: typhoon icon doesn't work without R2API AssetBundleResourcesProvider, game uses Resources.Load which can't find assets in our asset bundle
            DifficultyDef typhoon = new DifficultyDef(difScale, nameToken, iconPath, descToken, difColor, difServerTag, true);
            //can't even load it ourselves to bypass the resource lookup
            typhoon.iconSprite = iconSprite;
            typhoon.foundIconSprite = true;

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
                    TeamDef monsterTeam = TeamCatalog.GetTeamDef(TeamIndex.Monster);
                    monsterTeam.softCharacterLimit = Mathf.FloorToInt(monsterTeam.softCharacterLimit * 1.5f);   //was 2
                }
            }
        }

        private void Run_onRunDestroyGlobal(Run run)
        {
            TeamCatalog.GetTeamDef(TeamIndex.Monster).softCharacterLimit = defMonsterCap;
        }
    }
}
