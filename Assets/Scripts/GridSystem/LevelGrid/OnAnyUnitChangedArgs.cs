namespace GridSystems
{
    public class OnAnyUnitChangedArgs
    {
        public GridPosition gridPositionMovedFrom;
        public GridPosition gridPositionMovedTo;

        public OnAnyUnitChangedArgs(GridPosition gridPositionMovedFrom, GridPosition gridPositionMovedTo)
        {
            this.gridPositionMovedFrom = gridPositionMovedFrom;
            this.gridPositionMovedTo = gridPositionMovedTo;
        }
    }
    
}