using System;
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
			
			_resourceReactiveData = new ResourceReactiveData(InputResourceData.GoldCount);
		}
		
		public bool HasEnoughGold(int roomDataCost)
		{
			return _resourceReactiveData.GoldCount.Value >= roomDataCost;
		}

		public void DecreaseGold(int roomDataCost)
		{
			_resourceReactiveData.GoldCount.Value -= roomDataCost;
		}

		public ResourceReactiveData GetResourceReactiveData()
		{
			return _resourceReactiveData;
		}
	}
}