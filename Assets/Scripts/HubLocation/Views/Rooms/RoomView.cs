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


			UpdateUI();
			UpdateVisuals(roomController.RoomState);
			
			return this;
		}

		private void OnDestroy()
		{
			RoomController.OnRoomFocusedEvent -= OnRoomFocused;
			RoomController.OnRoomUnfocusedEvent -= OnRoomUnfocused;
		}

		public void ChangeRoomState(RoomState roomState)
		{
			UpdateUI();
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
					uiDocument.Value.rootVisualElement.visible = RoomController.RoomState == RoomState.Unlocked;
					break;
				case RoomViewUIType.ForFunctionality:
					uiDocument.Value.rootVisualElement.visible = RoomController.RoomState == RoomState.Built;
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