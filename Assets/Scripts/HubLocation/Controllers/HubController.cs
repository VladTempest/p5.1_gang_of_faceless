using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class HubController : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomType, Transform> _roomTransformDictionary;
		[SerializeField] HubCameraController _hubCameraController;
		
		[SerializeField]
		private RoomDataSO.RoomDataSO _roomDataSo;

		private Dictionary<RoomType, RoomControllerBase> _roomControllerDictionary;

		private void Awake()
		{
			_roomControllerDictionary = new Dictionary<RoomType, RoomControllerBase>();
		}
		
		private void Start()
		{
			InstantiateBuiltRooms();

			foreach (var pair in _roomTransformDictionary)
			{
				InitializeRoom(pair.Key);
			}
		}

		private void InstantiateBuiltRooms()
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start instantiate built rooms");
		}
		
		private void InitializeRoom(RoomType roomType)
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start build room");
			var roomData = _roomDataSo.RoomDataDictionary[roomType];
			if (ResourceController.HasEnoughGold(roomData.Cost))
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Player can afford room");
				ResourceController.DeductGold(roomData.Cost);
				var roomController = RoomControllerFactory.CreateRoomControllerByRoomType(roomType, roomData, _roomTransformDictionary[roomType], _hubCameraController);
				roomController.Initialize();
				_roomControllerDictionary.Add(roomType, roomController);
				return;
			}
			
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Player can't afford room");
		}

		private void UpgradeRoomState(RoomType roomType)
		{
			_roomControllerDictionary[roomType].UpgradeRoomState();
		}
	}
}