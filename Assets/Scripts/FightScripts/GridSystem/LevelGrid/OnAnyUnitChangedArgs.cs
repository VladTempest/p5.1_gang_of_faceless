namespace GridSystems
{
    public class OnAnyUnitChangedArgs
    {
        public GridPosition gridPositionMovedFrom;
        public GridPosition gridPositionMovedTo;
        public Unit unit;

        public OnAnyUnitChangedArgs(GridPosition gridPositionMovedFrom, GridPosition gridPositionMovedTo, Unit unit = null)
        {
            this.gridPositionMovedFrom = gridPositionMovedFrom;
            this.gridPositionMovedTo = gridPositionMovedTo;
            this.unit = unit;
        }
    }
    
}