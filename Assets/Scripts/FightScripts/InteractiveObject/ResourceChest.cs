using System;
using Editor.Scripts.HubLocation;
using GridSystems;
using UnityEngine;

namespace FightScripts.InteractiveObject
{
	public class ResourceChest : MonoBehaviour, IInteractable
	{
		[SerializeField] private int _goldAmount;
		[SerializeField] private GameObject _chestCover;
		private GridPosition GridPosition { get; set; }
		
		
		private void Start()
		{
			GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
			LevelGrid.Instance.SetInteractableAtGridPosition(GridPosition, this);
		}
		public void Interact(Action onActionComplete)
		{
			ResourceController.Instance.IncreaseResource(ResourceTypes.Gold, _goldAmount);
			_chestCover.SetActive(false);
			LevelGrid.Instance.ClearInteractableAtGridPosition(GridPosition);
			onActionComplete.Invoke();
			
		}
		
		
	}
}