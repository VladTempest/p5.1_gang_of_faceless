using System;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using UnityEngine;

namespace Editor.Scripts.HubLocation.Rooms
{
	public static class RoomControllerFactory
	{
		public static RoomControllerBase CreateRoomControllerByRoomType(RoomType roomType, RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController)
		{
			switch (roomType)
			{
				case RoomType.Workshop:
					return new Workshop(roomData, transformForBuilding, hubCameraController);
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