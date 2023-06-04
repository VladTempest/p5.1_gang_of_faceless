using System;
using System.Collections.Generic;
using UnityEngine;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Rooms;
using UnityEngine.UIElements;


namespace Editor.Scripts.HubLocation.Views.Rooms
{
	public class RoomView : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomState, GameObject> _roomStateDictionary;
		private RoomState _roomState = RoomState.Locked;
		private Dictionary<RoomViewUIType,UIDocument> _uiDocumentDictionary = new();
		private RoomControllerBase RoomController { get; set; }
		
		private const string ROOM_NAME_CONTAINER_NAME = "RoomName";
		private const string ROOM_DESCRIPTION_CONTAINER_NAME = "RoomDescription";
		private const string ROOM_KEY_MASK = "{ROOMKEY}";
		private string _roomKey;

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform, RoomData roomData)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomController = roomController;
			roomController.OnRoomFocusedEvent += OnRoomFocused;
			roomController.OnRoomUnfocusedEvent += OnRoomUnfocused;
			
			_roomKey = roomData.RoomKey;
			foreach (var uiDocument in roomData.UIDocumentDictionary)
			{
				_uiDocumentDictionary.Add(uiDocument.Key, Instantiate(uiDocument.Value, roomViewTransform));
			}
			StartSetUpUI();
			RoomController.SetUpRoomViewUI(_uiDocumentDictionary);

			//ToDo: this is a temporary solution, need to be changed with adding Save/Load system and Conditional Room Availability
			_roomState = RoomState.Unlocked;
			UpdateUI();
			UpdateVisuals(_roomState);
			
			return this;
		}

		private void OnDestroy()
		{
			RoomController.OnRoomFocusedEvent -= OnRoomFocused;
			RoomController.OnRoomUnfocusedEvent -= OnRoomUnfocused;
		}

		public void ChangeRoomState(RoomState roomState)
		{
			switch (roomState)
			{
				case RoomState.Locked:
					ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomController.RoomName} can't be locked yet");
					return;
				case RoomState.Unlocked:
					if (_roomState == RoomState.Locked)
					{
						ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomController.RoomName} is unlocked");
						_roomState = roomState;
						UpdateUI();
						break;
					}
					return;
				case RoomState.Built:
					if (_roomState == RoomState.Unlocked)
					{
						ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomController.RoomName} is built");
						_roomState = roomState;
						UpdateUI();
						break;
					}
					return;
				default:
					throw new ArgumentOutOfRangeException(nameof(roomState), roomState, null);
			}

			UpdateVisuals(roomState);
		}

		private void UpdateVisuals(RoomState roomState)
		{
			foreach (var roomStateGameObject in _roomStateDictionary)
			{
				roomStateGameObject.Value.SetActive(false);
			}

			_roomStateDictionary[roomState].SetActive(true);
		}

		private void OnRoomFocused(Transform _)
		{
			UpdateUI();
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomController.RoomName} is focused");
		}
		
		private void OnRoomUnfocused()
		{
			UpdateUI();
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomController.RoomName} is unfocused");
		}

		private void StartSetUpUI()
		{
			foreach (var uiDocument in _uiDocumentDictionary)
			{
				SetUpSpecificUIDocument(uiDocument);
			}
		}

		private void SetUpSpecificUIDocument(KeyValuePair<RoomViewUIType, UIDocument> uiDocument)
		{
			switch (uiDocument.Key)
			{
				case RoomViewUIType.Common:
					
					ReplaceRommKeyMaskInLocaleKey(uiDocument, ROOM_NAME_CONTAINER_NAME);
					ReplaceRommKeyMaskInLocaleKey(uiDocument, ROOM_DESCRIPTION_CONTAINER_NAME);

					uiDocument.Value.rootVisualElement.visible = true;
					break;
				case RoomViewUIType.ForBuilding:
					uiDocument.Value.rootVisualElement.visible = _roomState == RoomState.Unlocked;
					break;
				case RoomViewUIType.ForFunctionality:
					uiDocument.Value.rootVisualElement.visible = _roomState == RoomState.Built;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ReplaceRommKeyMaskInLocaleKey(KeyValuePair<RoomViewUIType, UIDocument> uiDocument, string ROOM_NAME_CONTAINER_NAME)
		{
			var roomNameContainer = uiDocument.Value.rootVisualElement.Q<Label>(ROOM_NAME_CONTAINER_NAME);
			roomNameContainer.text = roomNameContainer.text.Replace(ROOM_KEY_MASK, _roomKey);
		}

		private void UpdateUI()
		{
			foreach (var uiDocument in _uiDocumentDictionary)
			{
				SetUpSpecificUIDocument(uiDocument);
				uiDocument.Value.rootVisualElement.visible = uiDocument.Value.rootVisualElement.visible && RoomController.IsFocused;
			}
		}

		public void HandleTouch()
		{
			RoomController.OnRoomFocused();
		}

		public void HandlePinch()
		{
			RoomController.OnRoomUnfocused();
		}
	}
}