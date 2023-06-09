using System;

namespace Editor.Scripts.HubLocation
{
	public static class ResourceController
	{
		private static int _gold;
		
		public static event Action OnGoldChanged;
		
		public static int Gold
		{
			get => _gold;
			set
			{
				_gold = value;
				OnGoldChanged?.Invoke();
			}
		}
		
		//method to check if player has enough gold to build a room
		public static bool HasEnoughGold(int goldCost)
		{
			return _gold >= goldCost;
		}
		
		//method to deduct gold from player
		public static void DeductGold(int goldCost)
		{
			_gold -= goldCost;
		}
		
		//method to add gold to player
		public static void AddGold(int gold)
		{
			_gold += gold;
		}
	}
}