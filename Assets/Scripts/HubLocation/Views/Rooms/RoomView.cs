using UnityEngine;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.Rooms;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;


namespace Editor.Scripts.HubLocation.Views.Rooms
{
	public enum RoomViewUIType
	{
		Common,
		ForBuilding,
		ForFunctionality,
	}
	public class RoomView : MonoBehaviour
	{
		[SerializeField]
		private SerializableDictionary<RoomState, GameObject> _roomStateDictionary;
		private RoomState _roomState = RoomState.Unlocked;
		[SerializeField]
		private SerializableDictionary<RoomViewUIType,UIDocument> _uiDocumentDictionary;
		private RoomControllerBase RoomControllerController { get; set; }
		public Button BuildButton;

		public RoomView Initialize(RoomControllerBase roomController, Transform roomViewTransform)
		{
			ConvenientLogger.Log(nameof(RoomView), GlobalLogConstant.IsHubRoomControllLogEnabled, $"roomController {roomController.RoomName} initialized");
			RoomControllerController = roomController;
			RoomControllerController.SetUpRoomViewUI(_uiDocumentDictionary);
			return Instantiate(this, roomViewTransform);
		}
		
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