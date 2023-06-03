using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class Workshop : RoomControllerBase
	{
		public Workshop(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController) : base(roomData, transformForBuilding, hubCameraController)
		{
			
		}
	}
	
}