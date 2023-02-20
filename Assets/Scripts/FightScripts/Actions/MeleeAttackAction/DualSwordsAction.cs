using DefaultNamespace;
using GridSystems;

namespace Editor.Scripts.Actions
{
    public class DualSwordsAction : MeleeAttackAction
    {
        public override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
        {
            if (!base.IsGridPositionValid(testGridPosition, unitGridPosition))
            {
                return false;
            }
            if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
            {
                return false;
            }
            if (!GridPositionValidator.IsPositionInsideActionCircleRange(MaxActionRange, testGridPosition, unitGridPosition))
            {
                return false;
            }

            if (!GridPositionValidator.HasAnyUnitOnGridPosition(testGridPosition))
            {
                return false;
            }

            if (!GridPositionValidator.IsGridPositionWithEnemy(testGridPosition, _unit))
            {
                return false;
            }

            return true;
        }
    }
}