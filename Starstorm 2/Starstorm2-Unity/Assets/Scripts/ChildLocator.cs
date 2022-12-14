using UnityEngine;
using System;

public class ChildLocator : MonoBehaviour
{
	[Serializable]
	public struct NameTransformPair
	{
		public string name;
		public Transform transform;
	}

	[SerializeField]
	public NameTransformPair[] transformPairs;

	//for editor purposes. will not affect the game
	public NameTransformPair[] TransformPairs { get => transformPairs; set => transformPairs = value; }
}
