using Editor.Scripts.Utils;
using UnityEngine;

namespace Editor.Scripts.HubLocation.RoomDataSO
{
	[CreateAssetMenu(fileName = "RoomDataSO", menuName = "ScriptableObjects/RoomDataSO", order = 1)]
	public class RoomDataSO  : ScriptableObject
	{
		public SerializableDictionary<RoomType, RoomData> RoomDataDictionary;
	}
}