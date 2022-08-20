using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveAction : BaseAction
{
    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;

    private int ONE_GRID_MOVEMENT_COST = 2;
    private int PATH_TO_POINT_MULTIPLIER = 10;
    
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");

    private List<Vector3> _positionList;
    private int _currentPositionIndex;
    private float _stoppingDistance = 0.1f;
    private float _rotateSpeed = 30f;
    


    private void Update()
    {
        if (!_isActive) return;

        Vector3 targetPosition = _positionList[_currentPositionIndex];
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward,moveDirection, Time.deltaTime*_rotateSpeed);
        
        if (Vector3.Distance(targetPosition, transform.position) >= _stoppingDistance)
        {
            var moveSpeed = 4f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            _currentPositionIndex++;
            if (_currentPositionIndex >= _positionList.Count)
            {
                OnStopMoving?.Invoke(this, EventArgs.Empty);
                
                ActionComplete(); 
            }
            
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositions = Pathfinding.Instance.FindPath(_unit.GetGridPosition(), gridPosition, out var pathLength);
        
        _currentPositionIndex = 0;
        _positionList = new List<Vector3>();

        foreach (var pathGridPosition in pathGridPositions)
        {
            _positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }

        OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }


    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
        {
            return false;
        }
        
        if (GridPositionValidator.IsTargetGridPositionSameAsSourceGridPosition(unitGridPosition, testGridPosition))
        {
            return false;
        }

        if (GridPositionValidator.HasAnyUnitOnGridPosition(testGridPosition))
        {
            return false;
        }

        if (!GridPositionValidator.IsGridPositionReachable(testGridPosition, unitGridPosition, Mathf.FloorToInt(_unit.ActionPoints/2)))
        {
            return false;
        }
                
        return true;
    }

    public override string GetActionName()
    {
        return "Move";
    }
    
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        var actionValueForMovingToNearest = GetActionValueForMovingToNearest(gridPosition);

        var actionValueForMovingToMostTarget = GetActionValueForMovingToMostTargets(gridPosition);


        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = Mathf.Max(actionValueForMovingToNearest, actionValueForMovingToMostTarget)
        };
    }

    private int GetActionValueForMovingToMostTargets(GridPosition gridPosition)
    {
        var shootAction = _unit.GetAction<ShootAction>();
        var targetCountAtGridPosition = shootAction.GetTargetCountAtPosition(gridPosition);
        var actionValueForMovingToMostTarget = targetCountAtGridPosition * 1000;
        return actionValueForMovingToMostTarget;
    }

    private static int GetActionValueForMovingToNearest(GridPosition gridPosition)
    {
        var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;

        var nearestFriendlyUnitDistance = float.MaxValue;
        var currentDistanceFromEnemyToFriendlyUnit = int.MaxValue;
        foreach (var friendlyUnit in friendlyUnitList)
        {
            Pathfinding.Instance.FindPath(friendlyUnit.GetGridPosition(), gridPosition,
                out currentDistanceFromEnemyToFriendlyUnit);
            if (currentDistanceFromEnemyToFriendlyUnit < nearestFriendlyUnitDistance)
            {
                nearestFriendlyUnitDistance = currentDistanceFromEnemyToFriendlyUnit;
            }
        }

        var actionValueForMovingToNearest = Mathf.RoundToInt(1000 - currentDistanceFromEnemyToFriendlyUnit);
        return actionValueForMovingToNearest;
    }
    public override int GetActionPointCost()
    {
        var mousePosition = MouseWorld.GetPointerInWorldPosition();
        var targetGridPosition = LevelGrid.Instance.GetGridPosition(mousePosition);
        var currentPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        var pathLength = Pathfinding.Instance.GetPathLength(currentPosition, targetGridPosition);
        return pathLength * ONE_GRID_MOVEMENT_COST / PATH_TO_POINT_MULTIPLIER;
    }
}
