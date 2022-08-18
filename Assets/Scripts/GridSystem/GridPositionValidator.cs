using GridSystems;
using UnityEngine;

namespace DefaultNamespace
{
    public static class GridPositionValidator
    {
        private static float _unitShoulderHeight = 1.7f;

        public static bool IsPositionInsideBoundaries(GridPosition testGridPosition)
        {
            return LevelGrid.Instance.IsValidGridPosition(testGridPosition);
        }

        public static bool HasAnyUnitOnGridPosition(GridPosition testGridPosition)
        {
            return LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition);
        }

        public static bool IsGridPositionWithEnemy(GridPosition testGridPosition, Unit sourceUnit)
        {
            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
            return targetUnit.IsUnitAnEnemy != sourceUnit.IsUnitAnEnemy;
        }

        public static bool IsPositionInsideActionRange(int actionRange, GridPosition testGridPosition, GridPosition unitGridPosition)
        {
            int x = testGridPosition.x - unitGridPosition.x;
            int z = testGridPosition.z - unitGridPosition.z;
            int testDistance = Mathf.RoundToInt(Mathf.Abs(x) + Mathf.Abs(z));
            return (testDistance <= actionRange);
        }

        public static bool IsGridPositionOnLineOfSight(GridPosition testGridPosition, GridPosition sourceGridPosition, LayerMask obstaclesLayerMask)
        {
            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(sourceGridPosition);
            Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
            Vector3 lineOfSightDirection = (targetWorldPosition - unitWorldPosition).normalized;

            if (Physics.Raycast(unitWorldPosition + Vector3.up * _unitShoulderHeight, lineOfSightDirection,
                    Vector3.Distance(unitWorldPosition, targetWorldPosition), obstaclesLayerMask))
            {
                return false;
            }

            return true;

        }

        public static bool IsTargetGridPositionSameAsSourceGridPosition(object sourceGridPosition,
            object testGridPosition)
        {
            return sourceGridPosition == testGridPosition;
        }

        public static bool IsGridPositionReachable(GridPosition testGridPosition, GridPosition sourceGridPosition, int actionRange)
        {
            if (!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
            {
                return false;
            }
                
            if (!Pathfinding.Instance.HasPath( sourceGridPosition, testGridPosition)) {
                return false;
            }
                
            int pathFindingDistanceMultiplier = 10;
            if (Pathfinding.Instance.GetPathLength( sourceGridPosition, testGridPosition) > actionRange * pathFindingDistanceMultiplier)
            {
                return false;
            }
            
            return true;
        }

        public static bool IsDoorOnGridPosition(GridPosition testGridPosition)
        {
            IInteractable interactableObject = LevelGrid.Instance.GetInteractableAtGridPosition(testGridPosition);
            if (interactableObject == null)
            {
                return false;
            }

            return true;
        }
        
    }
}