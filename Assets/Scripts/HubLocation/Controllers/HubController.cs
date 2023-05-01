using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class HubController : MonoBehaviour
	{
		private RoomBase _roomController;
		[SerializeField] private RoomView _roomViewPrefab; //Later create Scriptable Object for this
		[SerializeField] private Transform _roomViewTransform;
		private List<RoomBase> _builtRoomList;
		
		
		//intstantiate a room after button b is clicked
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start instantiate room");
				InstantiateRoom(new Workshop(), _roomViewPrefab, _roomViewTransform);
			}
		}

		public void InstantiateRoom(RoomBase room, RoomView roomViewPrefab, Transform roomViewTransform)
		{
			_roomController = room;
			var roomView = Instantiate(roomViewPrefab, roomViewTransform);
			roomView.Initialize(room);
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Room {room.RoomName} instantiated");
		}
		
		
	}
}