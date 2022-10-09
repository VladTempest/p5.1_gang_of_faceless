using System.Collections.Generic;
using Editor.Scripts.Utils;
using GridSystems;
using UnityEngine;

namespace DefaultNamespace
{
    public static class GridPositionValidator
    {

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

        public static bool IsPositionInsideActionSquareRange(int actionMaxRange, GridPosition testGridPosition, GridPosition unitGridPosition, int actionMinRange = 0)
        {
            int x = testGridPosition.x - unitGridPosition.x;
            int z = testGridPosition.z - unitGridPosition.z; ;
            return (Mathf.Abs(x) <= actionMaxRange && Mathf.Abs(x) >= actionMinRange) && (Mathf.Abs(z) <= actionMaxRange && Mathf.Abs(z) >= actionMinRange);
        }
        public static bool IsPositionInsideActionCircleRange(int actionMaxRange, GridPosition testGridPosition, GridPosition unitGridPosition, int actionMinRange = 0)
        {
            int x = testGridPosition.x - unitGridPosition.x;
            int z = testGridPosition.z - unitGridPosition.z;
            int testDistance = Mathf.RoundToInt(Mathf.Abs(x) + Mathf.Abs(z));
            return (testDistance <= actionMaxRange && testDistance >= actionMinRange);
        }
        
        public static bool IsGridPositionOnLineOfSight(GridPosition testGridPosition, GridPosition sourceGridPosition, LayerMask[] obstaclesLayerMask)
        {
            Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(sourceGridPosition);
            Vector3 targetWorldPosition = LevelGrid.Instance.GetWorldPosition(testGridPosition);
            Vector3 lineOfSightDirection = (targetWorldPosition - unitWorldPosition).normalized;

            foreach (var layerMask in obstaclesLayerMask)
            {
                if (Physics.Raycast(unitWorldPosition + Vector3.up * GameGlobalConstants.UNIT_SHOULDER_HEIGHT, lineOfSightDirection,
                        Vector3.Distance(unitWorldPosition, targetWorldPosition), layerMask))
                {
                    return false;
                }
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

        public static bool IsGridPositionOpenToMoveTo(GridPosition testGridPosition, GridPosition sourceGridPosition)
        {
            return IsGridPositionReachable(testGridPosition, sourceGridPosition, 1) &&
                   !HasAnyUnitOnGridPosition(testGridPosition);
        }

        public static bool HasTestGridPositionAvailableNeighbours(GridPosition testGridPosition, Dictionary<GridPosition, GridPosition> firstValidGridPositionsAndSourcePositions)
        {
            var unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
            const int  GridSize = 2;
            var offsetPosition = unit.transform.forward * GridSize;
            if (CheckIfOffsetIsReachable(testGridPosition, unit, offsetPosition, firstValidGridPositionsAndSourcePositions)) return true;
            offsetPosition = unit.transform.right * GridSize;
            if (CheckIfOffsetIsReachable(testGridPosition, unit, offsetPosition, firstValidGridPositionsAndSourcePositions)) return true;
            offsetPosition = -unit.transform.right * GridSize;
            if (CheckIfOffsetIsReachable(testGridPosition, unit, offsetPosition, firstValidGridPositionsAndSourcePositions)) return true;
            offsetPosition = -unit.transform.forward * GridSize;
            if (CheckIfOffsetIsReachable(testGridPosition, unit, offsetPosition, firstValidGridPositionsAndSourcePositions)) return true;
            
            return false;
        }

        private static bool CheckIfOffsetIsReachable(GridPosition testGridPosition, Unit unit, Vector3 offset, Dictionary<GridPosition, GridPosition> firstValidGridPositionsAndSourcePositions)
        {
            var worldPositionWithOffset = unit.transform.position - offset;
            var gridPositionWithOffset = LevelGrid.Instance.GetGridPosition(worldPositionWithOffset);
            gridPositionWithOffset = unit.GetGridPosition() + GetValidOffset(testGridPosition, gridPositionWithOffset);
            if (IsGridPositionOpenToMoveTo(gridPositionWithOffset,testGridPosition ))
            {
                if (!firstValidGridPositionsAndSourcePositions.TryAdd(testGridPosition, gridPositionWithOffset))
                {
                    firstValidGridPositionsAndSourcePositions.Remove(testGridPosition);
                    firstValidGridPositionsAndSourcePositions.TryAdd(testGridPosition, gridPositionWithOffset);
                }
                return true;
            }
            
            return false;
        }
        
        private static GridPosition GetValidOffset(GridPosition startPosition, GridPosition finalGridPosition)
        {
            GridPosition offset = finalGridPosition - startPosition;
            if (offset.x != 0 && offset.z != 0) offset.x = 0;
            return offset;
        }
    }
}