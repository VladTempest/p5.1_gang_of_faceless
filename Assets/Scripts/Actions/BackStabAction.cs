using System;
using GridSystems;

namespace Actions
{
    public class BackStabAction: BaseAction
    {
        public override string GetActionName()
        {
            return "Back Stab";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            throw new NotImplementedException();
        }

        protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
        {
            throw new NotImplementedException();
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            throw new NotImplementedException();
        }
    }
}