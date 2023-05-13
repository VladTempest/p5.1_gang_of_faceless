using System;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation
{
	public enum RoomType
	{
		Workshop,
		Library,
		TrainingGround,
		Armory,
		AlchemyLab,
		Storage,
		Stable,
		Infirmary,
		Market,
		Chapel,
		ThroneRoom,
		TrainingRoom,
	}

	public class HubController : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomType, Transform> _roomTransformDictionary;
		
		[SerializeField]
		private RoomDataSO.RoomDataSO _roomDataSo;
		
		/*private RoomBase _roomController;
		[SerializeField] private RoomView _roomViewPrefab; //Later create Scriptable Object for this
		[SerializeField] private Transform _roomViewTransform;
		private List<RoomBase> _builtRoomList;
		
		
		//intstantiate a room after button b is clicked
		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.B))
			{
				ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start instantiate room");


				var testWorkshopRoom = new Workshop();
				if (ResourceController.HasEnoughGold(testWorkshopRoom.Cost))
				{
					ResourceController.DeductGold(testWorkshopRoom.Cost);
					InstantiateRoom(testWorkshopRoom, _roomViewPrefab, _roomViewTransform);
				}
				else
				{
					ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Not enough gold to build room");
				}
			}
		}

		public void InstantiateRoom(RoomBase room, RoomView roomViewPrefab, Transform roomViewTransform)
		{
			_roomController = room;
			room.RoomView = Instantiate(roomViewPrefab, roomViewTransform);
			room.RoomView .Initialize(room, roomViewTransform);
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {room.RoomName} instantiated");
		}*/
		
		//method to instantiate rooms that already built
		private void InstantiateBuiltRooms()
		{
			//instantiate rooms that already built
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start instantiate built rooms");
		}
		
		//method to build room of certain type - certain type here we got from touch that processed by Input Controller
		public void BuildRoom(RoomType roomType)
		{
			ConvenientLogger.Log(nameof(HubController), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start build room");
			//Get roomData from SO based on roomType
			var roomData = _roomDataSo.RoomDataDictionary[roomType];
			//Check if player can afford the room
			if (ResourceController.HasEnoughGold(roomData.Cost))
			{
				//Build the room
				//Create controller for room based on roomType with roomVIew from RoomData
				var roomController = RoomControllerFactory.GetRoomControllerByRoomType(roomType, roomData);

				//Instantiate roomView with transfrom from HubController Dictionary with roomType as key
				//Add built room to builtRoomList
			}
		}
	}
}