using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.ResourcesSO;
using SaveSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Editor.Scripts.HubLocation
{
	public class ResourceController : MonoBehaviour, ISaveable
	{
		private Dictionary<ResourceTypes, ResourceReactiveData> _resourcesDictionary;
		
		public static ResourceController Instance { get; private set; }

		[FormerlySerializedAs("ResourceCraftPropertySo")] [SerializeField]
		private ResourceCraftPropertySO _resourceCraftPropertySo;
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
			
			RestoreData();
			
			InitAndLogResources();
		}

		public void RestoreData()
		{
			
			var _persistentDataDict = DataPersistenceManager.Instance.GetState(GetType());

			_persistentData = (SaveData) _persistentDataDict;
			if (_persistentData == null)
			{
				_persistentData = new SaveData(new Dictionary<ResourceTypes, ResourceReactiveData>());
			}

			var convertedDictionary = _persistentData.ResourcesDictionary.ToDictionary(pair => pair.Key, pair => new ResourceReactiveData(pair.Value));
            
			if (_resourcesDictionary != null && _resourcesDictionary.Count != 0)
			{
				foreach (var pair in convertedDictionary)
				{
					if (_resourcesDictionary.TryGetValue(pair.Key, out var value))
					{
						value.Amount.Value = pair.Value.Amount.Value;
					}
					else
					{
						_resourcesDictionary.Add(pair.Key, pair.Value);
					}
				}
				return;
			}
			
			_resourcesDictionary = new Dictionary<ResourceTypes, ResourceReactiveData>(convertedDictionary);
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
			return _resourceCraftPropertySo.ResourceCraftPropertyDictionary.Values.ToList();
		}
        
		public (Type, object) CaptureData()
		{
			return (GetType(), new SaveData(_resourcesDictionary));
		}
		
		[Serializable]
		class SaveData
		{
			public Dictionary<ResourceTypes,int> ResourcesDictionary;
		
			public SaveData(Dictionary<ResourceTypes,ResourceReactiveData> resourcesDictionary)
			{
				var convertedDictionary = resourcesDictionary.ToDictionary(pair => pair.Key, pair => pair.Value.Amount.Value);
			
				ResourcesDictionary = new Dictionary<ResourceTypes, int>(convertedDictionary);
			}
		}
	}
}