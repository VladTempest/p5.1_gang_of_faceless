using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation.Views.Rooms
{
	using UnityEngine;
	using UnityEngine.UI;
	public class RoomView : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomState, GameObject> _roomStateDictionary;

		private RoomBase _roomController { get; set; }
		public Button BuildButton;
		public RoomView Initialize(RoomBase room, Transform roomViewTransform)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {room.RoomName} initialized");
			_roomController = room;
			//`BuildButton.onClick.AddListener(OnBuildButtonClicked);
			UpdateView();
			return this;
		}
		private void OnBuildButtonClicked()
		{
			if (!_roomController.IsBuilt)
			{
				// Build the room directly using the model's functionality
				_roomController.Build();
				UpdateView();
			}
		}
		public void UpdateView()
		{
			// Update the UI and visuals based on the room's state
		}

		public GameObject GetRoomPrefab()
		{
			return _roomStateDictionary[RoomState.Built];
		}
	}

	internal enum RoomState
	{
		Locked,
		Unlocked,
		Built
	}
}