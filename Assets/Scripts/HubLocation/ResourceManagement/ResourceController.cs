using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.ResourcesSO;
using SaveSystem;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class ResourceController : MonoBehaviour, ISaveable
	{
		private Dictionary<ResourceTypes, ResourceReactiveData> _resourcesDictionary;
		
		public static ResourceController Instance { get; private set; }

		[SerializeField]
		private ResourceCraftPropertySO ResourceCraftPropertySo;
		[SerializeField]
		private ResourceRoomBuildPropertySO _resourceRoomBuildPropertySo;

		private SaveData _persistentData;

		private void Awake()
		{
			if (Instance != null)
			{
				ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(Instance);
		}

		private void Start()
		{
			DataPersistenceManager.Instance.RegisterDataPersistence(this);
			
			InitializeResourcesDictionary();
			
			InitAndLogResources();
		}

		private void InitializeResourcesDictionary()
		{
			
			_persistentData = (SaveData)DataPersistenceManager.Instance.GetState(GetType());
			if (_persistentData == null)
			{
				_persistentData = new SaveData(new Dictionary<ResourceTypes, ResourceReactiveData>());
			}

			_resourcesDictionary = new Dictionary<ResourceTypes, ResourceReactiveData>();
			foreach (var pair in _persistentData.ResourcesDictionary.ToDictionary())
			{
				_resourcesDictionary.Add(pair.Key, new ResourceReactiveData(pair.Value));
			}

		}
		
		private void InitAndLogResources()
		{
			if (_resourcesDictionary == null || _resourcesDictionary.Count == 0)
			{
				ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled,
					$"_resourcesDictionary is null");
				_resourcesDictionary = new Dictionary<ResourceTypes, ResourceReactiveData>
				{
					{ResourceTypes.Gold, new ResourceReactiveData(InputResourceData.Gold)},
					{ResourceTypes.ParalyzingArrows, new ResourceReactiveData(InputResourceData.ParalyzingArrows)},
					{ResourceTypes.ExplosionPotion, new ResourceReactiveData(InputResourceData.ExplosionPotion)},
					{ResourceTypes.Metal, new ResourceReactiveData(InputResourceData.Metal)},
					{ResourceTypes.Wood, new ResourceReactiveData(InputResourceData.Wood)},
					{ResourceTypes.GoldenOre, new ResourceReactiveData(InputResourceData.GoldenOre)},
					{ResourceTypes.Stone, new ResourceReactiveData(InputResourceData.Stone)}
				};
			}

			LogResourceDictionaryContent(_resourcesDictionary);
		}

		private void LogResourceDictionaryContent(Dictionary<ResourceTypes, ResourceReactiveData> resourcesDictionary)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Start log resource dictionary content");
			foreach (var pair in resourcesDictionary)
			{
				ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"{pair.Key} : {pair.Value.Amount.Value}");
			}
		}

		public bool HasEnoughResource(ResourceTypes resourceType, int resourceCost)
		{
			if (_resourcesDictionary.ContainsKey(resourceType))
			{
				ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled,
					$"Has enough {resourceType} resource: {resourceCost} against {_resourcesDictionary[resourceType].Amount.Value}: {_resourcesDictionary[resourceType].Amount.Value >= resourceCost}");
				return _resourcesDictionary[resourceType].Amount.Value >= resourceCost;
			}

			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled,
				$"There is no {resourceType} resource in dictionary");
			return false;
		}

		public void DecreaseResource(ResourceTypes resourceType, int resourceAmount)
		{
			if (_resourcesDictionary.ContainsKey(resourceType))
			{
				ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled,
					$"Decrease {resourceType} amount: {resourceAmount}. Now: {_resourcesDictionary[resourceType].Amount.Value - resourceAmount}");
				_resourcesDictionary[resourceType].Amount.Value -= resourceAmount;
				return;
			}

			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled,
				$"There is no {resourceType} resource in dictionary");

		}

		public ResourceReactiveData GetResourceReactiveData(ResourceTypes resourceType)
         {
             ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Get {resourceType} resource reactive data");
             _resourcesDictionary.TryGetValue(resourceType, out ResourceReactiveData resourceReactiveData);
             return resourceReactiveData;
         }

		public void IncreaseResource(ResourceTypes resourceType, int resourceAmount)
		{
			if (_resourcesDictionary.ContainsKey(resourceType))
			{
				ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Increase {resourceType} resource amount: {resourceAmount}. Now: {_resourcesDictionary[resourceType].Amount.Value + resourceAmount}");
				_resourcesDictionary[resourceType].Amount.Value += resourceAmount;
				return;
			}

            ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"There is no {resourceType} resource in dictionary");
		}

		public SerializableDictionary<ResourceTypes, int> GetRoomCost(RoomType roomType)
		{
			return _resourceRoomBuildPropertySo.ResourceRoomBuildPropertyDictionary[roomType].Cost;
		}
		
		public List<ResourceCraftProperty> GetCraftCostList()
		{
			return ResourceCraftPropertySo.ResourceCraftPropertyDictionary.Values.ToList();
		}
        
		public (Type, object) SaveData()
		{
			return (GetType(), new SaveData(_resourcesDictionary));
		}
	}

	[Serializable]
	class SaveData
	{
		public BinarySerializableDictionary<ResourceTypes,int> ResourcesDictionary;
		
		public SaveData(Dictionary<ResourceTypes,ResourceReactiveData> resourcesDictionary)
		{
			ResourcesDictionary = new BinarySerializableDictionary<ResourceTypes, int>();
			foreach (var pair in resourcesDictionary)
			{
				ResourcesDictionary.Add((pair.Key, pair.Value.Amount.Value));
			}
		}
	}
	
	[System.Serializable]
	public class BinarySerializableDictionary<TKey, TValue>
	{
		public List<TKey> keys=new List<TKey>();
		public List<TValue> values=new List<TValue>();
    
		public Dictionary<TKey, TValue> ToDictionary()
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			for (var i = 0; i!= Math.Min(keys.Count, values.Count); i++)
				dictionary.Add(keys[i], values[i]);
            
			return dictionary;
		}
		
		public void Add((TKey,TValue) item)
		{
			keys.Add(item.Item1);
			values.Add(item.Item2);
		}
	}
}