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
		private RoomState _roomState = RoomState.Unlocked;
		private Dictionary<RoomViewUIType,UIDocument> _uiDocumentDictionary = new();
		private RoomControllerBase RoomController { get; set; }

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform, RoomData roomData)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomController = roomController;
			roomController.OnRoomFocusedEvent += OnRoomFocused;
			foreach (var uiDocument in roomData.UIDocumentDictionary)
			{
				_uiDocumentDictionary.Add(uiDocument.Key, Instantiate(uiDocument.Value, roomViewTransform));
			}
			StartSetUpUI();
			RoomController.SetUpRoomViewUI(_uiDocumentDictionary);
			UpdateUI();
			return this;
		}
		
		public GameObject GetRoomPrefab()
		{
			return _roomStateDictionary[RoomState.Built];
		}

		public void ChangeRoomState(RoomState roomState)
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
					uiDocument.Value.gameObject.SetActive(true);
					break;
				case RoomViewUIType.ForBuilding:
					uiDocument.Value.gameObject.SetActive(_roomState == RoomState.Unlocked);
					break;
				case RoomViewUIType.ForFunctionality:
					uiDocument.Value.gameObject.SetActive(_roomState == RoomState.Built);
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
				uiDocument.Value.gameObject.SetActive(uiDocument.Value.gameObject.activeSelf && RoomController.IsFocused);
			}
		}

		public void HandleTouch()
		{
			RoomController.OnRoomFocused();
		}
	}
	
	public enum RoomState
	{
		Locked,
		Unlocked,
		Built
	}
}