using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Scripts.HubLocation.ResourcesSO
{
	[Serializable]
	public struct ResourceRoomBuildProperty
	{
		[HideInInspector]
		public string Name;
		[HideInInspector]
		public RoomType RoomType;
		public SerializableDictionary<ResourceTypes, int> Cost;

		public ResourceRoomBuildProperty(string name, RoomType roomType, SerializableDictionary<ResourceTypes, int> cost)
		{
			this.Name = name;
			this.Cost = cost;
			this.RoomType = roomType;
		}
	}
}