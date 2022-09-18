using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;

public class SpinAction : BaseAction
{
   
    private float _totalSpinAmount;
    

    private void Update()
    {
        if (!enabled) return;
        if (!IsActive) return;
        
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);

        _totalSpinAmount += spinAddAmount;
        if (_totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _totalSpinAmount = 0;
        //InvokeOnActionStart(this, EventArgs.Empty);
        ActionStart(onActionComplete);

    }

    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (GridPositionValidator.HasAnyUnitOnGridPosition(testGridPosition))
        {
            return true;
        }

        return false;
    }

    public override string GetActionName()
    {
        return "Spin";
    }

    public override int GetActionPointCost()
    {
        return 1;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }
}
