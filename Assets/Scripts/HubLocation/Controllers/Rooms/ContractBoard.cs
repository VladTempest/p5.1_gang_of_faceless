using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using Editor.Scripts.SceneLoopScripts;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor.Scripts.HubLocation.Rooms
{
	[Serializable]
	public class ContractBoard : RoomControllerBase
	{
		private const string CONTRACTS_CONTAINER_KEY = "ContarctsContainer";
		public ContractBoard(RoomType roomType, RoomData roomData, RoomState roomState, Transform transformForBuilding, HubCameraController hubCameraController) : base(roomType, roomData, roomState, transformForBuilding, hubCameraController)
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
			buttonElement.text = GetLocalisationKeyFromSceneEnum(scenesEnum);
		}

		private string GetLocalisationKeyFromSceneEnum(ScenesEnum scenesEnum)
		{
			switch (scenesEnum)
			{
				case ScenesEnum.FightScene_Ambush:
					return "#room_contract_board_ambush";
				case ScenesEnum.FightScene_Bridge:
					return "#room_contract_board_bridge";
				case ScenesEnum.MainMenu:
				case ScenesEnum.Loading:
				case ScenesEnum.HubScene:
				default:
					throw new ArgumentOutOfRangeException(nameof(scenesEnum), scenesEnum, null);
			}
		}
	}
	
}