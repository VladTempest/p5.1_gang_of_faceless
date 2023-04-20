namespace FightScripts.GridSystem
{
	public interface IGridVisualizer
	{
		public void VisualizeGrid();
		public void UpdateGridVisuals(Unit unit = null);
		public void HideGridVisuals();
	}
}