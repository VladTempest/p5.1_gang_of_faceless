using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using Editor.Scripts.SceneLoopScripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class ContractBoard : RoomControllerBase
	{
		private const string CONTRACTS_CONTAINER_KEY = "ContarctsContainer";
		private const string CONTRACT_BUTTON_TEMPLATE_KEY = "ContractButton";
		public ContractBoard(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController) : base(roomData, transformForBuilding, hubCameraController)
		{
			
		}

		protected override void SetUpRoomFunctionalityUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			base.SetUpRoomFunctionalityUI(uiDocumentDictionary);
			var contractsContainer = uiDocumentDictionary[RoomViewUIType.ForFunctionality].rootVisualElement.Q<VisualElement>(CONTRACTS_CONTAINER_KEY);
			var containerButtons = contractsContainer.Children();
			var childrenList = containerButtons.ToList();
			
			//ToDo: more flexible system for adding buttons
			SetUpContractButton(childrenList, ScenesEnum.FightScene_Ambush, 0);
			SetUpContractButton(childrenList, ScenesEnum.FightScene_Bridge, 1);
			 
			 for (int i = 2; i < childrenList.Count; i++)
			 {
				 contractsContainer.Remove(childrenList[i]);
			 }
		}

		private void SetUpContractButton(List<VisualElement> childrenList, ScenesEnum scenesEnum, int indexOfButton)
		{
			var buttonElement = childrenList[indexOfButton].Q<Button>();
			buttonElement.clicked += () => ScenesController.Instance.LoadScene(scenesEnum);
			buttonElement.text = scenesEnum.ToString().Split("_")[1];
		}
	}
	
}