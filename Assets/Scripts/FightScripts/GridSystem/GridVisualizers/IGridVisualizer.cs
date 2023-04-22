using System.Collections.Generic;
using GridSystems;

namespace FightScripts.GridSystem
{
	public interface IGridVisualizer
	{
		public void UpdateGridVisuals(Dictionary<GridVisualType, List<GridPosition>> gridVisualDict);
		public void HideGridVisuals();
	}
}