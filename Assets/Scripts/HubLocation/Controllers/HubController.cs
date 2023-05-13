using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public class HubController : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomType, Transform> _roomTransformDictionary;
		
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

		public void TestFunctions()
		{
			UpgradeRoomState(RoomType.Workshop);
		}
		
		//method to instantiate rooms that already built
		private void InstantiateBuiltRooms()
		{
			//instantiate rooms that already built
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start instantiate built rooms");
		}
		
		//method to build room of certain type - certain type here we got from touch that processed by Input Controller
		private void InitializeRoom(RoomType roomType)
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start build room");
			//Get roomData from SO based on roomType
			var roomData = _roomDataSo.RoomDataDictionary[roomType];
			//Check if player can afford the room
			if (ResourceController.HasEnoughGold(roomData.Cost))
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Player can afford room");
				ResourceController.DeductGold(roomData.Cost);
				//Initialize the room
				//Create controller for room based on roomType with roomVIew from RoomData
				var roomController = RoomControllerFactory.GetRoomControllerByRoomType(roomType, roomData, _roomTransformDictionary[roomType]);
				roomController.Initialize();
				//Add built room to builtRoomList
				_roomControllerDictionary.Add(roomType, roomController);
				//Turnoff placeholder room
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