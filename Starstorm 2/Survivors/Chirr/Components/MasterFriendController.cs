using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Starstorm2Unofficial.Survivors.Chirr.Components
{
    [RequireComponent(typeof(CharacterMaster))]
    public class MasterFriendController : MonoBehaviour
    {
        public uint masterNetID = NetworkInstanceId.Invalid.Value;

        public int[] masterItemStacks;
        public MasterCatalog.MasterIndex masterIndex = MasterCatalog.MasterIndex.none;
        public EquipmentIndex masterEquipmentIndex;
        public HashSet<BuffIndex> EliteBuffs = new HashSet<BuffIndex>();

        public void Clear()
        {
            masterNetID = NetworkInstanceId.Invalid.Value;
            masterItemStacks = null;
            masterIndex = MasterCatalog.MasterIndex.none;
            masterEquipmentIndex = EquipmentIndex.None;
            EliteBuffs.Clear();
        }

        private void Awake()
        {
            RoR2.Stage.onServerStageComplete += ResetNetID;
        }

        private void OnDestroy()
        {
            RoR2.Stage.onServerStageComplete -= ResetNetID;
        }

        private void ResetNetID(Stage obj)
        {
            masterNetID = NetworkInstanceId.Invalid.Value;
        }
    }
}
