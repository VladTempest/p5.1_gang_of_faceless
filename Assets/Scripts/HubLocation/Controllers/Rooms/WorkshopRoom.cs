using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using UnityEngine;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class Workshop : RoomControllerBase
	{
		public Workshop(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController) : base(roomData, transformForBuilding, hubCameraController)
		{
			
		}
	}
	
}