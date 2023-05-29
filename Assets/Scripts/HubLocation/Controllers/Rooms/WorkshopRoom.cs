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

		public override void SetUpRoomViewUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			base.SetUpRoomViewUI(uiDocumentDictionary);
        
			if (uiDocumentDictionary == null)
			{
				ConvenientLogger.Log(nameof(Workshop), GlobalLogConstant.IsHubRoomControllLogEnabled, $"uiDocumentDictionary is null");
				return;
			}
			
			var rootVisualElement = uiDocumentDictionary[RoomViewUIType.ForBuilding].rootVisualElement;
			var buildButton = rootVisualElement.Q<Button>("BuildButton");
			buildButton.clicked += UpgradeRoomState;

		} //ToDo: move it to the base class
	}
}