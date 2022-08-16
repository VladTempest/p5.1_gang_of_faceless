using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;

    [SerializeField] public int _actionRange = 0;
    
    protected Unit _unit;
    protected bool _isActive;

    protected Action _onActionComplete;

    protected virtual void Awake()
    {
        _unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validGridPositions = GetValidGridPositions();
        return validGridPositions.Contains(gridPosition);
    }
    
    protected abstract bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition);

    public virtual int GetActionPointCost()
    {
        return 1;
    }

    public List<GridPosition> GetValidGridPositions()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        
        for (int x = -_actionRange; x <= _actionRange; x++)
        {
            for (int z = -_actionRange; z <= _actionRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (IsGridPositionValid(testGridPosition, unitGridPosition))
                {
                    validGridPositionList.Add(testGridPosition);
                }
            }
        }

        return validGridPositionList;
    }

    protected void ActionStart(Action onActionComplete)
    {
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
        _isActive = true;
        _onActionComplete = onActionComplete;
    }

    protected void ActionComplete()
    {
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        _isActive = false;
        _onActionComplete?.Invoke();
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidGridPositions();

        foreach (var gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }
        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort(((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue));
            return enemyAIActionList[0];
        }
        else
        {
            Debug.LogError("No EnemyActions");
            return null;
        }

    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
}
