using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public abstract class RoomControllerBase
	{
		public event Action<Transform> OnRoomFocusedEvent;
		public event Action OnRoomUnfocusedEvent;
		private RoomView RoomView { get; set; }
		public string RoomName { get; }
		public int Cost { get; set; }
		public bool IsBuilt { get; private set; }
		
		const string RETURN_BUTTON_KEY = "ReturnButton";
		const string BUILD_BUTTON_KEY = "BuildButton";
		const string RESOURCE_COST_KEY = "ResourceCost";

		private const string resourceCountKey = "ResourceCount";


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
		
		private bool CanAffordRoom()
		{
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Check if player can afford room {RoomName}");
			return ResourceController.Instance.HasEnoughGold(Cost);
		}
		protected virtual void OnRoomBuilt()
		{
			// Do something when the room is built
			IsBuilt = true;
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

		private void TryUpgradeRoomState()
		{
			if (!ResourceController.Instance.HasEnoughGold(Cost))
			{
				ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled,
					$"Player can't afford room {RoomName}");
				return;
			}
			
			ResourceController.Instance.DecreaseGold(Cost);
			RoomView.ChangeRoomState(RoomState.Built);
			OnRoomBuilt();
		}

		public void SetUpRoomViewUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Setting Up Room View UI for {RoomName}");
			
			if (uiDocumentDictionary == null)
			{
				ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"uiDocumentDictionary is null");
				return;
			}
			
			SetUpBuildUI(uiDocumentDictionary);
			SetUpCommonUI(uiDocumentDictionary);
			SetUpRoomFunctionalityUI(uiDocumentDictionary);
		}

		protected virtual void SetUpRoomFunctionalityUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			// Do nothing
		}

		private void SetUpCommonUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			var commonUIRootVisualElement = uiDocumentDictionary[RoomViewUIType.Common].rootVisualElement;
			
			var returnButton = commonUIRootVisualElement.Q<Button>(RETURN_BUTTON_KEY);
			returnButton.clicked += OnRoomUnfocused;

			var resourceReactiveData = ResourceController.Instance.GetResourceReactiveData();
			var label = commonUIRootVisualElement.Q<Label>(resourceCountKey);
			label.text = resourceReactiveData.GoldCount.Value.ToString();
			resourceReactiveData.GoldCount.onValueChanged += (value) => label.text = value.ToString();
		}

		private void SetUpBuildUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			

			var rootVisualElement = uiDocumentDictionary[RoomViewUIType.ForBuilding].rootVisualElement;
			
			var buildButton = rootVisualElement.Q<Button>(BUILD_BUTTON_KEY);
			buildButton.clicked += TryUpgradeRoomState;
			
			var labelResourceCost = rootVisualElement.Q<Label>(RESOURCE_COST_KEY);
			labelResourceCost.text = Cost.ToString();
		}
	}

}