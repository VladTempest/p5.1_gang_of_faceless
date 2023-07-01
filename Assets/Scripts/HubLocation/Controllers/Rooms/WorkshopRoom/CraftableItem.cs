using System.Collections.Generic;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class CraftableItem
	{
		public string Name;
		public ResourceTypes ResourceType;
		public Dictionary<ResourceTypes, int> Cost;

		public CraftableItem(string name, ResourceTypes resourceType, Dictionary<ResourceTypes, int> cost)
		{
			this.Name = name;
			this.Cost = cost;
			this.ResourceType = resourceType;
		}
	}
}