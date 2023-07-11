using UnityEngine.Serialization;

namespace SaveSystem
{
	[System.Serializable]
	public class GameData
	{
		public int goldCount;
		
		public GameData()
		{
			goldCount = 666;
		}
	}
}