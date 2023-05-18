using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public abstract class RoomControllerBase
	{
		public RoomView RoomView { get; set; }
		public string RoomName { get; }
		public int Cost { get; set; }
		public bool IsBuilt { get; private set; }
		
		protected RoomControllerBase(RoomData roomData, Transform transformForBuilding)
		{
			Cost = roomData.Cost;
			RoomName = roomData.RoomName;
			RoomView = roomData.RoomViewPrefab.Initialize(this, transformForBuilding, roomData);
			IsBuilt = false;
		}
		
		public void Initialize()
		{
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start building room {RoomName}");
			
			IsBuilt = true;
			OnRoomBuilt();
		}
		private bool CanAffordRoom()
		{
			// Check if the player can afford the room
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Check if player can afford room {RoomName}");
			return true; // for now, just return true
		}
		protected virtual void OnRoomBuilt()
		{
			// Do something when the room is built
			ConvenientLogger.Log(nameof(RoomControllerBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {RoomName} is built");
		}

		public void UpgradeRoomState()
		{
			RoomView.ChangeRoomState(RoomState.Built);
		}

		public virtual void SetUpRoomViewUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			
		}
	}

}