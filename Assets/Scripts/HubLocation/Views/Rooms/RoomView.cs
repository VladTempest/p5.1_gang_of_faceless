using System;
using System.Collections.Generic;
using UnityEngine;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Rooms;
using Unity.VisualScripting;
using UnityEditor;
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
		private RoomControllerBase RoomControllerController { get; set; }

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform, RoomData roomData)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomControllerController = roomController;
			foreach (var uiDocument in roomData.UIDocumentDictionary)
			{
				_uiDocumentDictionary.Add(uiDocument.Key, Instantiate(uiDocument.Value, roomViewTransform));
			}
			SetUpUI();
			RoomControllerController.SetUpRoomViewUI(_uiDocumentDictionary);
			return Instantiate(this, roomViewTransform);
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
		
		private void OnRoomFocused()
		{
			SetUpUI();
			SetUpCamera();
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomControllerController.RoomName} is focused");
		}

		private void SetUpCamera()
		{
			
		}

		private void SetUpUI()
		{
			foreach (var uiDocument in _uiDocumentDictionary)
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
		}
	}

	public enum RoomState
	{
		Locked,
		Unlocked,
		Built
	}
}