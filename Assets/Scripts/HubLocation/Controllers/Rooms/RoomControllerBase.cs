using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public abstract class RoomControllerBase
	{
		public event Action<Transform> OnRoomFocusedEvent;
		public RoomView RoomView { get; set; }
		public string RoomName { get; }
		public int Cost { get; set; }
		public bool IsBuilt { get; private set; }

		public bool IsFocused => !_hubCameraController.IsFreeLook;
		private HubCameraController _hubCameraController;
		
		protected RoomControllerBase(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController)
		{
			OnRoomFocusedEvent += hubCameraController.FocusOnRoom;
			_hubCameraController = hubCameraController;
			Cost = roomData.Cost;
			RoomName = roomData.RoomName;
			RoomView = GameObject.Instantiate(roomData.RoomViewPrefab, transformForBuilding);
			RoomView.Initialize(this, transformForBuilding, roomData);
			IsBuilt = false;
		}
		
		public void Initialize()
		{
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start building room {RoomName}");
			
			IsBuilt = true;
			OnRoomBuilt();
		}
		private bool CanAffordRoom()
		{
			// Check if the player can afford the room
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Check if player can afford room {RoomName}");
			return true; // for now, just return true
		}
		protected virtual void OnRoomBuilt()
		{
			// Do something when the room is built
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is built");
		}
		public virtual void OnRoomFocused()
		{
			OnRoomFocusedEvent?.Invoke(RoomView.transform);
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is focused");
		}

		public void UpgradeRoomState()
		{
			RoomView.ChangeRoomState(RoomState.Built);
		}

		public virtual void SetUpRoomViewUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			
		}
	}

}