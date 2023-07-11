using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.ResourcesSO;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class ResourceController : MonoBehaviour
	{
		private Dictionary<ResourceTypes, ResourceReactiveData> _resourcesDictionary;
		
		public static ResourceController Instance { get; private set; }

		[SerializeField]
		private ResourceCraftPropertySO ResourceCraftPropertySo;
		[SerializeField]
		private ResourceRoomBuildPropertySO _resourceRoomBuildPropertySo; 

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
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Has enough {resourceType} resource: {resourceCost} against {_resourcesDictionary[resourceType].Amount.Value}: {_resourcesDictionary[resourceType].Amount.Value >= resourceCost}");
			return _resourcesDictionary[resourceType].Amount.Value >= resourceCost;
		}

		public void DecreaseResource(ResourceTypes resourceType, int resourceAmount)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Decrease {resourceType} amount: {resourceAmount}. Now: {_resourcesDictionary[resourceType].Amount.Value - resourceAmount}");
			_resourcesDictionary[resourceType].Amount.Value -= resourceAmount;
		}

		public ResourceReactiveData GetResourceReactiveData(ResourceTypes resourceType)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Get {resourceType} resource reactive data");
			return _resourcesDictionary[resourceType];
		}

		public void IncreaseResource(ResourceTypes resourceType, int resourceAmount)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Increase {resourceType} resource amount: {resourceAmount}. Now: {_resourcesDictionary[resourceType].Amount.Value + resourceAmount}");
			_resourcesDictionary[resourceType].Amount.Value += resourceAmount;
		}

		public SerializableDictionary<ResourceTypes, int> GetRoomCost(RoomType roomType)
		{
			return _resourceRoomBuildPropertySo.ResourceRoomBuildPropertyDictionary[roomType].Cost;
		}
		
		public List<ResourceCraftProperty> GetCraftCostList()
		{
			return ResourceCraftPropertySo.ResourceCraftPropertyDictionary.Values.ToList();
		}
	}
}