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
		private List<GameObject> _roomStateList;
		
		public RoomBase Room { get; private set; }
		public Button BuildButton;
		public void Initialize(RoomBase room)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Room {room.RoomName} initialized");
			Room = room;
			//`BuildButton.onClick.AddListener(OnBuildButtonClicked);
			UpdateView();
		}
		private void OnBuildButtonClicked()
		{
			if (!Room.IsBuilt)
			{
				// Build the room directly using the model's functionality
				Room.Build();
				UpdateView();
			}
		}
		public void UpdateView()
		{
			// Update the UI and visuals based on the room's state
		}
	}

}