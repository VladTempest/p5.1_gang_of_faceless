using Editor.Scripts.GlobalUtils;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class ResourceController : MonoBehaviour
	{
		private ResourceReactiveData _resourceReactiveData;
		public static ResourceController Instance { get; private set; }

		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogError("There are many singletonss");
				Destroy(gameObject);
				return;
			}

			Instance = this;
			DontDestroyOnLoad(Instance);
			
			_resourceReactiveData = new ResourceReactiveData(InputResourceData.GoldCount);
		}
		
		public bool HasEnoughGold(int roomDataCost)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Has enough gold: {roomDataCost} against {_resourceReactiveData.GoldCount.Value}: {_resourceReactiveData.GoldCount.Value >= roomDataCost}");
			return _resourceReactiveData.GoldCount.Value >= roomDataCost;
		}

		public void DecreaseGold(int goldAmount)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Decrease gold amount: {goldAmount}. Now: {_resourceReactiveData.GoldCount.Value - goldAmount}");
			_resourceReactiveData.GoldCount.Value -= goldAmount;
		}

		public ResourceReactiveData GetResourceReactiveData()
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Get resource reactive data");
			return _resourceReactiveData;
		}

		public void IncreaseGold(int goldAmount)
		{
			ConvenientLogger.Log(nameof(ResourceController), GlobalLogConstant.IsResourceControllerLogEnabled, $"Increase gold amount: {goldAmount}. Now: {_resourceReactiveData.GoldCount.Value + goldAmount}");
			_resourceReactiveData.GoldCount.Value += goldAmount;
		}
	}
}