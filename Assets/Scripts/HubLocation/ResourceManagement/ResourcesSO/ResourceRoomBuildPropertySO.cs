using System;
using System.Linq;
using UnityEngine;

namespace Editor.Scripts.HubLocation.ResourcesSO
{
	[CreateAssetMenu(fileName = "ResourceRoomBuildPropertySO", menuName = "ScriptableObjects/ResourceRoomBuildPropertySO", order = 1)]
	public class ResourceRoomBuildPropertySO : ScriptableObject
	{
		public SerializableDictionary<RoomType, ResourceRoomBuildProperty> ResourceRoomBuildPropertyDictionary;

		private void OnValidate()
		{
			//insert in name field nameof(ResourceType) Key
			var list = ResourceRoomBuildPropertyDictionary.ToList();
			for (int i = 0; i < list.Count; i++)
			{
				var keyValuePair = list[i];
				var roomBuildProperty = keyValuePair.Value;
				roomBuildProperty.Name = keyValuePair.Key.ToString();
				roomBuildProperty.RoomType = keyValuePair.Key;
				ResourceRoomBuildPropertyDictionary[keyValuePair.Key] = roomBuildProperty;
			}
		}
	}
}