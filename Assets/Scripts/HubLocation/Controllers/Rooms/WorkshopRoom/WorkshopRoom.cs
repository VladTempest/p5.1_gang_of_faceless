using System.Collections.Generic;
using System.Linq;
using System.Text;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.HubLocation.CameraController;
using Editor.Scripts.HubLocation.RoomDataSO;
using Editor.Scripts.HubLocation.Views.Rooms;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

namespace Editor.Scripts.HubLocation.Rooms
{
	public class Workshop : RoomControllerBase
	{
		private const string DROPDOWN_KEY = "ResourceDropdown";
		private const string RESOURCES_COSTS_KEY = "ResourcesCosts";
		private const string CRAFT_BUTTON_KEY = "CraftButton";

		//ToDo: Create a scriptable object for this
		public List<CraftableItem> _craftableItems = new()
		{
			new($"{ResourceTypes.ParalyzingArrows}", ResourceTypes.ParalyzingArrows,
				new Dictionary<ResourceTypes, int>() {{ResourceTypes.Wood, 1}, {ResourceTypes.Metal, 2}}),
			new ($"{ResourceTypes.ExplosionPotion}", ResourceTypes.ExplosionPotion,
				new Dictionary<ResourceTypes, int>() {{ResourceTypes.Wood, 3}, {ResourceTypes.Metal, 4}})
		};

		private VisualElement _root;
		private DropdownField _stuffDropdown;
		private Label _resourcesCosts;
		private Button _craftButton;
		
		public Workshop(RoomData roomData, Transform transformForBuilding, HubCameraController hubCameraController) :
			base(roomData, transformForBuilding, hubCameraController)
		{
		}

		protected override void SetUpRoomFunctionalityUI(Dictionary<RoomViewUIType, UIDocument> uiDocumentDictionary)
		{
			base.SetUpRoomFunctionalityUI(uiDocumentDictionary);
			_root = uiDocumentDictionary[RoomViewUIType.ForFunctionality].rootVisualElement;
			_resourcesCosts = _root.Q<Label>(RESOURCES_COSTS_KEY);
			_stuffDropdown = _root.Q<DropdownField>(DROPDOWN_KEY);
			_craftButton = _root.Q<Button>(CRAFT_BUTTON_KEY);
			
			PopulateDropdown(_stuffDropdown, _craftableItems);

			SetUpCraftButton(_craftButton);
		}

		private void SetUpCraftButton(Button craftButton)
		{
			craftButton.clicked += TryToCraftItem;
		}

		private void TryToCraftItem()
		{
			var item = _craftableItems.Find(i => i.Name == _stuffDropdown.value);
			if (item != null)
			{
				foreach (var pair in item.Cost)
				{
					ConvenientLogger.Log(nameof(Workshop), GlobalLogConstant.IsResourceControllerLogEnabled,
						$"Trying to decreasing {pair.Key} by {pair.Value}");
					if (!ResourceController.Instance.HasEnoughResource(pair.Key, pair.Value)) return;
					ResourceController.Instance.DecreaseResource(pair.Key, pair.Value);
					ResourceController.Instance.IncreaseResource(item.ResourceType, 1);
				}
			}
		}

		private void PopulateDropdown(DropdownField stuffDropdown, List<CraftableItem> craftableItems)
		{
			var choices = new List<string>();
			foreach (var item in craftableItems)
			{
				choices.Add(item.Name);
			}
			stuffDropdown.choices = choices;
			stuffDropdown.RegisterValueChangedCallback(evt => UpdateCosts(evt.newValue));
		}
		
		private void UpdateCosts(string itemName)
		{
			var item = _craftableItems.Find(i => i.Name == itemName);
			if (item != null)
			{
				StringBuilder costsStringBuilder = new StringBuilder();
				costsStringBuilder.Append("Cost: ");
        
				foreach (var pair in item.Cost)
				{
					costsStringBuilder.Append($"{pair.Value} {pair.Key}, ");
				}

				// Remove the last comma and space
				if (costsStringBuilder.Length > 2)
				{
					costsStringBuilder.Remove(costsStringBuilder.Length - 2, 2);
				}

				_resourcesCosts.text = costsStringBuilder.ToString();
			}
		}
		}
}