using Editor.Scripts.HubLocation.Views.Rooms;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class Workshop : RoomBase
	{
		public override RoomView RoomView { get; set; }
		public override string RoomName => "Workshop";
		public override int Cost => 100;
	}
}