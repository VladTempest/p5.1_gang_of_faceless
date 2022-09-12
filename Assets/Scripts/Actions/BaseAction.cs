using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    
    public event EventHandler OnActionStart;

    public int ActionRange = 0;
    
    protected Unit _unit;
    private bool _isActive;

    protected bool IsActive
    {
        get => _isActive;
        private set => _isActive = value;
    }

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

    protected virtual bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (_unit.EffectSystem.IsKnockedDown(out int durationLeft))
        {
            return false;
        }

        return true;
    }

    public virtual int GetActionPointCost()
    {
        return 1;
    }

    public List<GridPosition> GetValidGridPositions()
    {
        if (IsActive) return new List<GridPosition>();
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        
        for (int x = -ActionRange; x <= ActionRange; x++)
        {
            for (int z = -ActionRange; z <= ActionRange; z++)
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
        IsActive = true;
        _onActionComplete = onActionComplete;
    }

    protected void ActionComplete()
    {
        IsActive = false;
        _onActionComplete?.Invoke();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
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

    protected void InvokeOnActionStart(object sender, EventArgs eventArgs)
    {
        OnActionStart?.Invoke(sender, eventArgs);
    }
}
