using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine.Serialization;

namespace Editor.Scripts.HubLocation.RoomDataSO
{
[System.Serializable]
	public struct RoomData
	{
		public string RoomName;
		[FormerlySerializedAs("RoomView")] public RoomView RoomViewPrefab;
		public int Cost;
	}
}