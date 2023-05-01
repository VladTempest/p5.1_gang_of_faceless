using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Views.Rooms;

namespace Editor.Scripts.HubLocation.Rooms
{
	public abstract class RoomBase
	{
		public abstract RoomView RoomView { get; set; }
		public abstract string RoomName { get; }
		public abstract int Cost { get; }
		public bool IsBuilt { get; private set; }
		public void Build()
		{
			ConvenientLogger.Log(nameof(RoomBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Start building room {RoomName}");
			if (CanAffordRoom())
			{
				IsBuilt = true;
				OnRoomBuilt();
			}
		}
		private bool CanAffordRoom()
		{
			// Check if the player can afford the room
			ConvenientLogger.Log(nameof(RoomBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Check if player can afford room {RoomName}");
			return true; // for now, just return true
		}
		protected virtual void OnRoomBuilt()
		{
			// Do something when the room is built
			ConvenientLogger.Log(nameof(RoomBase), GlobalLogConstant.IsHubRoomControllLogEnabled, $"Room {RoomName} is built");
		}
	}

}