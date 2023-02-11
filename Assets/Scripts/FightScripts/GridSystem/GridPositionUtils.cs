using System.Collections.Generic;
using GridSystems;

namespace FightScripts.GridSystem
{
	public static class GridPositionUtils
	{
		public static void SortGridPositionByDistanceToUnit(List<GridPosition> listOfTestGridPositions, Unit unit)
		{
			listOfTestGridPositions.Sort((item1, item2) =>
				GridPosition.Distance(unit.GetGridPosition(), item1) > GridPosition.Distance(unit.GetGridPosition(), item2)
					? 1
					: -1);
		}	
	}
}