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

		private GridSystemVisualSingle[,] GridSystemVisualSingleArray
		{
			get
			{
				if (_gridSystemVisualSingleArray == null)
				{
					_gridSystemVisualSingleArray = SetUpSeparateGrids();
				}

				return _gridSystemVisualSingleArray;
			}
		}

		public void UpdateGridVisuals(Dictionary<GridVisualType, List<GridPosition>> gridVisualDict)
		{
			if (GridSystemVisualSingleArray == null) SetUpSeparateGrids();
			foreach (var keyValuePair in gridVisualDict)
			{
				ShowGridPositionList(keyValuePair.Value,keyValuePair.Key);
			}
		}

		public void HideGridVisuals()
		{
			if (GridSystemVisualSingleArray == null) return;
			foreach (var gridSystemVisualSingle in GridSystemVisualSingleArray)
			{
				gridSystemVisualSingle.Hide();
			}
			
		}
		
		private GridSystemVisualSingle[,] SetUpSeparateGrids()
		{
			GridSystemVisualSingle[,] gridSystemVisualSingleArray =
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

					gridSystemVisualSingleArray[x, z] =
						gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
				}
			}

			return gridSystemVisualSingleArray;
		}
		
		public void ShowGridPositionList(List<GridPosition> gridPositions, GridVisualType gridVisualType)
		{
			if (gridPositions == null) return;
			foreach (var gridPosition in gridPositions)
			{
				//_gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].gameObject.SetActive(true);
				GridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
			}
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