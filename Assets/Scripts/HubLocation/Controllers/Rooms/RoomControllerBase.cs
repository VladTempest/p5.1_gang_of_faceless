using System;
using System.Collections.Generic;
using System.Text;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	[Serializable]
	public abstract class RoomControllerBase
	{
		public event Action<Transform> OnRoomFocusedEvent;
		public event Action OnRoomUnfocusedEvent;

		public RoomState RoomState
		{
			get => _roomState;
			set => ChangeRoomState(value);
		}

		private RoomState _roomState = RoomState.Locked;
		private RoomView RoomView { get; set; }
		public string RoomName { get; }
		public SerializableDictionary<ResourceTypes, int> Cost { get; set; }
		public bool IsBuilt { get; private set; }
		
		const string RETURN_BUTTON_KEY = "ReturnButton";
		const string BUILD_BUTTON_KEY = "BuildButton";
		const string RESOURCE_COST_KEY = "ResourceCost";

		private const string resourceCountKey = "ResourceCount";


		public bool IsFocused => !_hubCameraController.IsFreeLook;
		private HubCameraController _hubCameraController;
		
		protected RoomControllerBase(RoomType roomType, RoomData roomData, RoomState roomState, Transform transformForBuilding, HubCameraController hubCameraController)
		{
			OnRoomFocusedEvent += hubCameraController.FocusOnRoom;
			OnRoomUnfocusedEvent += hubCameraController.UnfocusOnRoom;
			_hubCameraController = hubCameraController;
			_roomState = roomState;
			Cost = ResourceController.Instance.GetRoomCost(roomType);
			RoomName = roomData.RoomName;
			RoomView = GameObject.Instantiate(roomData.RoomViewPrefab, transformForBuilding);
			RoomView.Initialize(this, transformForBuilding, roomData);
			IsBuilt = false;
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

		private bool TryUpgradeRoomState()
		{
			foreach (var pair in Cost)
			{
				ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled,
					$"Checking for decreasing {pair.Key} by {pair.Value}");
				if (!ResourceController.Instance.HasEnoughResource(pair.Key, pair.Value))
				{
					ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled,
						$"Player can't afford room {RoomName}");
					return false;
				}
			}
			foreach (var pair in Cost)
			{
				ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled,
					$"Decreasing {pair.Key} by {pair.Value}");
				ResourceController.Instance.DecreaseResource(pair.Key, pair.Value);
			}
            
			return true;
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

			var resourceReactiveData = ResourceController.Instance.GetResourceReactiveData(ResourceTypes.Gold);
			var label = commonUIRootVisualElement.Q<Label>(resourceCountKey);
			label.text = resourceReactiveData?.Amount.Value.ToString();
			if (resourceReactiveData?.Amount != null)
			{
				resourceReactiveData.Amount.onValueChanged += (value) => label.text = value.ToString();
			}
			else
			{
				ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"resourceReactiveData {resourceCountKey} is null");
			}
		}

		private void SetUpBuildUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			

			var rootVisualElement = uiDocumentDictionary[RoomViewUIType.ForBuilding].rootVisualElement;
			
			var buildButton = rootVisualElement.Q<Button>(BUILD_BUTTON_KEY);
			buildButton.clicked += () => RoomState = RoomState.Built ;
			
			var labelResourceCost = rootVisualElement.Q<Label>(RESOURCE_COST_KEY);
			
			StringBuilder costsStringBuilder = new StringBuilder();
			costsStringBuilder.Append("Cost: ");
        
			foreach (var pair in Cost)
			{
				costsStringBuilder.Append($"{pair.Value} {pair.Key}, ");
			}

			// Remove the last comma and space
			if (costsStringBuilder.Length > 2)
			{
				costsStringBuilder.Remove(costsStringBuilder.Length - 2, 2);
			}

			labelResourceCost.text = costsStringBuilder.ToString();
		}

		private void ChangeRoomState(RoomState roomState)
		{
			switch (roomState)
			{
				case RoomState.Locked:
					ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} can't be locked yet");
					return;
				case RoomState.Unlocked:
					if (_roomState == RoomState.Locked)
					{
						ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is unlocked");
						_roomState = roomState;
						RoomView.ChangeRoomState(roomState);
					}
					break;
				case RoomState.Built:
					if (_roomState == RoomState.Unlocked && TryUpgradeRoomState())
					{
						ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is built");
						_roomState = roomState;
						RoomView.ChangeRoomState(roomState);
						OnRoomBuilt();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(roomState), roomState, null);
			}
		}
	}

}