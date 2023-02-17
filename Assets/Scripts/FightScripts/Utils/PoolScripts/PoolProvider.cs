using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using UnityEngine;

namespace Editor.Scripts.Utils.PoolScripts
{
	public class PoolProvider : MonoBehaviour
	{
		
		
		public Dictionary<PoolsEnum, Queue<GameObject>> poolDictionary = new Dictionary<PoolsEnum, Queue<GameObject>>();
		public List<Pool> pools;
		
		public static PoolProvider Instance { get; private set; }
		
		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("There are many singletonss");
				Destroy(gameObject);
				return;
			}
			Instance = this;
		}
		
		private void InitiatePoolForEnumPool(PoolsEnum poolEnum)
		{
			Queue<GameObject> objectPool = new Queue<GameObject>();
			Pool pool = pools.Find(x => x.poolEnum == poolEnum);

			if (pool == null)
			{
				ConvenientLogger.LogError(nameof(PoolProvider), GlobalLogConstant.IsPoolLogEnabled,
					$"Pool with enum {poolEnum} not found");
				return;
			}

			for (int i = 0; i < pool.size; i++)
			{
				GameObject obj = Instantiate(pool.prefab);
				obj.SetActive(false);
				objectPool.Enqueue(obj);
			}
			poolDictionary.Add(poolEnum, objectPool);
		}
		
		public GameObject SpawnFromPool(PoolsEnum poolEnum, Vector3 position, Quaternion rotation)
		{
			if (!poolDictionary.ContainsKey(poolEnum)) InitiatePoolForEnumPool(poolEnum);
			
			if (!poolDictionary.ContainsKey(poolEnum))
			{
				return new GameObject("Error");
			}
			
			if (poolDictionary[poolEnum].Count == 0)
			{
				ConvenientLogger.LogError(nameof(PoolProvider), GlobalLogConstant.IsPoolLogEnabled,
					$"Pool with enum {poolEnum} is empty");
				return new GameObject("Error");
			}
			
			GameObject objectToSpawn = poolDictionary[poolEnum].Dequeue();

			objectToSpawn.SetActive(true);
			objectToSpawn.transform.position = position;
			objectToSpawn.transform.rotation = rotation;
			
			return objectToSpawn;
		}

		public void DespawnToPool(PoolsEnum poolEnum, GameObject obj)
		{
			obj.SetActive(false);
			poolDictionary[poolEnum].Enqueue(obj);
		}
	}
}