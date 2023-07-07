using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Scripts.HubLocation.ResourcesSO
{
	[Serializable]
	public struct ResourceCraftProperty
	{
		[HideInInspector]
		public string Name;
		[HideInInspector]
		public ResourceTypes ResourceType;
		public SerializableDictionary<ResourceTypes, int> Cost;

		public ResourceCraftProperty(string name, ResourceTypes resourceType, SerializableDictionary<ResourceTypes, int> cost)
		{
			this.Name = name;
			this.Cost = cost;
			this.ResourceType = resourceType;
		}
	}
}