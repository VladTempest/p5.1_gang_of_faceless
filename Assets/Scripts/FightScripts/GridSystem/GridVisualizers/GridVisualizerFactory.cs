using System;
using UnityEngine;

namespace FightScripts.GridSystem
{
	public class GridVisualizerFactory : MonoBehaviour
	{
		[SerializeField]
		private PerimeterGridVisualizer _perimeterGridVisualizerPrefab;
		[SerializeField]
		private SeparateGridsVisualizer _separateGridsVisualizerPrefab;
		public IGridVisualizer GetGridVisualizer(GridSystemVisual.TypeOfGridVisual typeOfGridVisual)
		{
			switch (typeOfGridVisual)
			{
				case GridSystemVisual.TypeOfGridVisual.SeparateGrids:
					_separateGridsVisualizerPrefab.gameObject.SetActive(true);
					return _separateGridsVisualizerPrefab;
				case GridSystemVisual.TypeOfGridVisual.OnlyPerimeter:
					_perimeterGridVisualizerPrefab.gameObject.SetActive(true);
					return _perimeterGridVisualizerPrefab;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}