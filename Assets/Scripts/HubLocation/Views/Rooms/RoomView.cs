using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using UnityEngine;

namespace Editor.Scripts.HubLocation.Views.Rooms
{
	using UnityEngine;
	using UnityEngine.UI;
	public class RoomView : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomState, GameObject> _roomStateDictionary;
		private RoomState _roomState = RoomState.Unlocked;
		private RoomControllerBase RoomControllerController { get; set; }
		public Button BuildButton;

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomControllerController = roomController;
			return Instantiate(this, roomViewTransform);
		}
		/*public void UpdateView()
		{

		}
		*/

		public GameObject GetRoomPrefab()
		{
			return _roomStateDictionary[RoomState.Built];
		}

		public void ChangeRoomState(RoomState roomState)
		{
			foreach (var roomStateGameObject in _roomStateDictionary)
			{
				roomStateGameObject.Value.SetActive(false);
			}
			_roomStateDictionary[roomState].SetActive(true);
		}
	}

	public enum RoomState
	{
		Locked,
		Unlocked,
		Built
	}
}