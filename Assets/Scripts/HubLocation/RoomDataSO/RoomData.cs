using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.RoomDataSO
{
[System.Serializable]
	public struct RoomData
	{
		public string RoomName;
		[FormerlySerializedAs("RoomView")] public RoomView RoomViewPrefab;
		public SerializableDictionary<RoomViewUIType,UIDocument> UIDocumentDictionary;
		public string RoomKey;
	}
}