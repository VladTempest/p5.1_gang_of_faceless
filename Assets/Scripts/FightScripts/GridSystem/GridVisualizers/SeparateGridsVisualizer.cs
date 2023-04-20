using System;
using System.Collections.Generic;
using Actions;
using Editor.Scripts.Actions;
using GridSystems;
using UnityEngine;

namespace FightScripts.GridSystem
{
	[Serializable]
	public struct GridVisualTypeMaterial
	{
		public GridVisualType type;
		public Material material;
	}
	

	
	class SeparateGridsVisualizer : MonoBehaviour, IGridVisualizer
	{
		[SerializeField] private Transform _gridSystemVisualSinglePrefab;
		[SerializeField] private List<GridVisualTypeMaterial> _gridVisualTypeMaterialList;
		
		private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;
		public void VisualizeGrid()
		{
			throw new System.NotImplementedException();
		}

		public void UpdateGridVisuals(Unit selectedUnit)
		{
			UpdateSeparateGridPositions(selectedUnit);
		}

		public void HideGridVisuals()
		{
			foreach (var gridSystemVisualSingle in _gridSystemVisualSingleArray)
			{
				gridSystemVisualSingle.Hide();
			}
			
		}
		
		private void SetUpSeparateGrids()
		{
			_gridSystemVisualSingleArray =
				new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];

			for (int x = (int) LevelGrid.Instance.transform.position.x;
			     x < (int) LevelGrid.Instance.transform.position.x + LevelGrid.Instance.GetWidth();
			     x++)
			{
				for (int z = (int) LevelGrid.Instance.transform.position.z;
				     z < (int) LevelGrid.Instance.transform.position.z + LevelGrid.Instance.GetHeight();
				     z++)
				{
					GridPosition gridPosition = new GridPosition(x, z);
					Transform gridSystemVisualSingleTransform = Instantiate(_gridSystemVisualSinglePrefab,
						LevelGrid.Instance.GetWorldPosition(gridPosition),
						Quaternion.identity);
					gridSystemVisualSingleTransform.parent = transform;

					_gridSystemVisualSingleArray[x, z] =
						gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
				}
			}
		}
		
		public void ShowGridPositionRangeCircle(GridPosition gridPosition, int maxRange, GridVisualType gridVisualType, int minRange = 0)
		{
			List<GridPosition> gridPositionList = new List<GridPosition>();

			for (int x = -maxRange; x <= maxRange; x++)
			{
				for (int z = -maxRange; z <= maxRange; z++)
				{
					GridPosition offsetGridPosition = new GridPosition(x, z);
					GridPosition testGridPosition = gridPosition + offsetGridPosition;

					if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
					{
						continue;
					}

					int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
					if (testDistance > maxRange || testDistance < minRange) continue;

					gridPositionList.Add(testGridPosition);
				}
			}
        
			ShowGridPositionList(gridPositionList, gridVisualType);
		}
		
		public void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
		{
			List<GridPosition> gridPositionList = new List<GridPosition>();
        
			for (int x = -range; x <= range; x++)
			{
				for (int z = -range; z <= range; z++)
				{
					GridPosition offsetGridPosition = new GridPosition(x, z);
					GridPosition testGridPosition = gridPosition + offsetGridPosition;
                
					if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
					{
						continue;
					}

					gridPositionList.Add(testGridPosition);
				}
			}

			ShowGridPositionList(gridPositionList, gridVisualType);
		}
		
		public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
		{
			if (gridPositions == null) return;
			foreach (var gridPosition in gridPositions)
			{
				//_gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].gameObject.SetActive(true);
				_gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
			}
		}
		
		private void UpdateSeparateGridPositions(Unit selectedUnit)
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Red;
                break;
            case BackStabAction backstab:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), backstab.MaxActionRange,
                    GridVisualType.RedSoft);
                break;
            case GreatSwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.MaxActionRange,
                    GridVisualType.RedSoft);
                break;
            case DualSwordsAction dualSwordAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), dualSwordAction.MaxActionRange,
                    GridVisualType.RedSoft);
                break;
            case KnockDownAction knockDownAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), knockDownAction.MaxActionRange,
                    GridVisualType.RedSoft);
                break;
            case BaseShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), shootAction.MaxActionRange,
                    GridVisualType.RedSoft, shootAction.MinActionRange);
                break;
            case PushAction pushAction:
                gridVisualType = GridVisualType.Red;
                ShowGridPositionRangeCircle(selectedUnit.GetGridPosition(), pushAction.MaxActionRange,
                    GridVisualType.RedSoft);
                break;
        }

        ShowGridPositionList(selectedAction.GetValidGridPositions(), gridVisualType);
    }
		
		public Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
		{
			foreach (var visualTypeMaterial in _gridVisualTypeMaterialList)
			{
				if (visualTypeMaterial.type == gridVisualType)
				{
					return visualTypeMaterial.material;
				}
			}
        
			Debug.LogError("Couldn't find material " + gridVisualType);
			return null;
		}
	}
}