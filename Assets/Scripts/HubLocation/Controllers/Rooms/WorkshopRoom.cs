using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class Workshop : RoomControllerBase
	{
		public Workshop(RoomData roomData, Transform transformForBuilding) : base(roomData, transformForBuilding)
		{
			
		}

		public override void SetUpRoomViewUI(SerializableDictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			base.SetUpRoomViewUI(uiDocumentDictionary);
        
			if (uiDocumentDictionary == null)
			{
				ConvenientLogger.Log(nameof(Workshop), GlobalLogConstant.IsHubRoomControllLogEnabled, $"uiDocumentDictionary is null");
				return;
			}
			var root = uiDocumentDictionary[RoomViewUIType.ForBuilding].rootVisualElement;
			var button = root.Q<Button>("YourButton");
			button.clicked += YourButtonClicked;
		}

		private void YourButtonClicked()
		{
			// Handle button click here
		}
	}
}