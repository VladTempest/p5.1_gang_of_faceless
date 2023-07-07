using System;
using System.Linq;
using UnityEngine;

namespace Editor.Scripts.HubLocation.ResourcesSO
{
	[CreateAssetMenu(fileName = "ResourceCraftPropertySO", menuName = "ScriptableObjects/ResourceCraftPropertySO", order = 1)]
	public class ResourceCraftPropertySO : ScriptableObject
	{
		public SerializableDictionary<ResourceTypes, ResourceCraftProperty> ResourceCraftPropertyDictionary;

		private void OnValidate()
		{
			//insert in name field nameof(ResourceType) Key
			var list = ResourceCraftPropertyDictionary.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				var keyValuePair = list[i];
				var craftProperty = keyValuePair.Value;
				craftProperty.Name = keyValuePair.Key.ToString();
				craftProperty.ResourceType = keyValuePair.Key;
				ResourceCraftPropertyDictionary[keyValuePair.Key] = craftProperty;
			}
		}
	}
}