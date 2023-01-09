using UnityEngine;

namespace Starstorm2Unofficial.Cores
{
    public class SpooderCore
    {
        public static GameObject bodyPrefab;

        public SpooderCore() => Setup();

        private void Setup()
        {
            bodyPrefab = PrefabCore.spooderPrefab;
            //insert custom spawn state here
            //bodyPrefab.GetComponent<EntityStateMachine>().initialStateType = new SerializableEntityStateType(typeof(SpooderMain));
        }
    }
}