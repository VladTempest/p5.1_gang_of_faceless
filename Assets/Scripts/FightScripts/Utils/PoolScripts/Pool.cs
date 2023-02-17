using System;
using UnityEngine;

namespace Editor.Scripts.Utils.PoolScripts
{
	[Serializable]
	public class Pool
	{
		public PoolsEnum poolEnum;
		public GameObject prefab;
		public int size;
	}
}