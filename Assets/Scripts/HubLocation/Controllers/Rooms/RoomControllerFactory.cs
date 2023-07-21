using System;
using System.Linq;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation.Rooms
{
	public static class RoomControllerFactory
	{
		public static RoomControllerBase CreateRoomControllerByRoomType(RoomType roomType, RoomData roomData, RoomState roomState, Transform transformForBuilding, HubCameraController hubCameraController)
		{
			switch (roomType)
			{
				case RoomType.Workshop:
					var resourcesProperties = ResourceController.Instance.GetCraftCostList();
					return new Workshop(roomType, roomData, roomState, transformForBuilding, hubCameraController, resourcesProperties);
				case RoomType.ContractBoard:
					return new ContractBoard(roomType, roomData, roomState, transformForBuilding, hubCameraController);
				/*case RoomType.Library:
				case RoomType.TrainingGround:
				case RoomType.Armory:
				case RoomType.AlchemyLab:
				case RoomType.Storage:
				case RoomType.Stable:
				case RoomType.Infirmary:
				case RoomType.Market:
				case RoomType.Chapel:
				case RoomType.ThroneRoom:
				case RoomType.TrainingRoom:*/
				default:
					throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null);
			}
		}
	}
}