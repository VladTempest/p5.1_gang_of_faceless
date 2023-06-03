using System;
using System.Collections.Generic;
using UnityEngine;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Rooms;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


namespace Editor.Scripts.HubLocation.Views.Rooms
{
	public enum RoomViewUIType
	{
		Common,
		ForBuilding,
		ForFunctionality,
	}
	public class RoomView : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomState, GameObject> _roomStateDictionary;
		private RoomState _roomState = RoomState.Locked;
		private Dictionary<RoomViewUIType,UIDocument> _uiDocumentDictionary = new();
		private RoomControllerBase RoomController { get; set; }

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform, RoomData roomData)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomController = roomController;
			roomController.OnRoomFocusedEvent += OnRoomFocused;
			roomController.OnRoomUnfocusedEvent += OnRoomUnfocused;
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
	
	public enum RoomState
	{
		Locked,
		Unlocked,
		Built
	}
}