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
		public event Action OnRoomUnfocusedEvent;
		public RoomView RoomView { get; set; }
		public string RoomName { get; }
		public int Cost { get; set; }
		public bool IsBuilt { get; private set; }
		
		const string RETURN_BUTTON_KEY = "ReturnButton";
		const string BUILD_BUTTON_KEY = "BuildButton";
		

		public bool IsFocused => !_hubCameraController.IsFreeLook;
		private HubCameraController _hubCameraController;
		
		protected RoomControllerBase(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController)
		{
			OnRoomFocusedEvent += hubCameraController.FocusOnRoom;
			OnRoomUnfocusedEvent += hubCameraController.UnfocusOnRoom;
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
		
		public virtual void OnRoomUnfocused()
		{
			OnRoomUnfocusedEvent?.Invoke();
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is unfocused");
		}

		public void UpgradeRoomState()
		{
			RoomView.ChangeRoomState(RoomState.Built);
		}

		public virtual void SetUpRoomViewUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			if (uiDocumentDictionary == null)
			{
				ConvenientLogger.Log(nameof(Workshop), GlobalLogConstant.IsHubRoomControllLogEnabled, $"uiDocumentDictionary is null");
				return;
			}
			
			SetUpBuildUI(uiDocumentDictionary);
			SetUpCommonUI(uiDocumentDictionary);
		}

		private void SetUpCommonUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			var returnButton = uiDocumentDictionary[RoomViewUIType.Common].rootVisualElement.Q<Button>(RETURN_BUTTON_KEY);
			returnButton.clicked += OnRoomUnfocused;
		}

		private void SetUpBuildUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			var rootVisualElement = uiDocumentDictionary[RoomViewUIType.ForBuilding].rootVisualElement;
			var buildButton = rootVisualElement.Q<Button>(BUILD_BUTTON_KEY);
			buildButton.clicked += UpgradeRoomState;
		}
	}

}