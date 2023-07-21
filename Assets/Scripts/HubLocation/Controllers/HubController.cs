using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.Rooms;
using Editor.Scripts.HubLocation.Views.Rooms;
using SaveSystem;
using UnityEngine;
using UnityEngine.Serialization;

namespace Editor.Scripts.HubLocation
{
	public class HubController : MonoBehaviour, ISaveable
	{
		[SerializeField] private SerializableDictionary<RoomType, Transform> _roomTransformDictionary;
		[SerializeField] HubCameraController _hubCameraController;

		[SerializeField] private RoomDataSO.RoomDataSO _roomDataSo;

		private Dictionary<RoomType, RoomControllerBase> _roomControllerDictionary;
		private Dictionary<RoomType, RoomState> _roomStateDictionary;

		private void Awake()
		{
			_roomControllerDictionary = new Dictionary<RoomType, RoomControllerBase>();
		}

		private void Start()
		{
			DataPersistenceManager.Instance.RegisterDataPersistence(this);
			RestoreData();
			
			InstantiateBuiltRooms();

			foreach (var pair in _roomTransformDictionary)
			{
				InitializeRoom(pair.Key);
			}
		}

		private void InstantiateBuiltRooms()
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled,
				$"Start instantiate built rooms");
		}

		private void InitializeRoom(RoomType roomType)
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled,
				$"Start build room");
			var roomData = _roomDataSo.RoomDataDictionary[roomType];
            
			var roomState = (_roomStateDictionary != null && _roomStateDictionary.TryGetValue(roomType, out var state))
				? state 
				: RoomState.Unlocked;
			
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsSaveLoadLogEnabled,
				$"_roomStateDictionary is null: {_roomStateDictionary == null} \n" + 
				$"is roomStateDictionary contains roomType: {_roomStateDictionary?.ContainsKey(roomType)} \n" +
				$"roomState: {roomState}");
			
			if (_roomStateDictionary != null && _roomStateDictionary.TryGetValue(roomType, out var value))
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsSaveLoadLogEnabled,
					$"Room state is {value}");
			}
			
			var roomController = RoomControllerFactory.CreateRoomControllerByRoomType(roomType, roomData, roomState, _roomTransformDictionary[roomType], _hubCameraController);
			_roomControllerDictionary.Add(roomType, roomController);
		}
		
		[Serializable]
		private class SaveData
		{
			public Dictionary<RoomType,RoomState> RoomStateDictionary;
		
			public SaveData(Dictionary<RoomType,RoomState> roomStateDictionary)
			{
				RoomStateDictionary = new Dictionary<RoomType, RoomState>(roomStateDictionary);
			}
		}

		public (Type, object) CaptureData()
		{
			Dictionary<RoomType, RoomState> roomStateDictionary = new Dictionary<RoomType, RoomState>();
			foreach (var pair in _roomControllerDictionary)
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsSaveLoadLogEnabled,
					$"Saved Room {pair.Key} state is {pair.Value.RoomState}");
				roomStateDictionary.Add(pair.Key, pair.Value.RoomState);
			}

			return (GetType(),new SaveData(roomStateDictionary));
		}

		public void RestoreData()
		{
            var persistantData = DataPersistenceManager.Instance.GetState(GetType());
            
            if (persistantData == null) return;
            
            var saveData = (SaveData)persistantData;
            
			_roomStateDictionary = saveData.RoomStateDictionary;
		}
	}
}