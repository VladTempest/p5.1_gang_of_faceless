using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractDistance;
    
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactableObject = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactableObject.Interact(OnInteractComplete);
        ActionStart(onActionComplete, 0);
    }

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
        
        if (!GridPositionValidator.IsDoorOnGridPosition(testGridPosition))
        {
            return false;
        }

        return true;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
    }

    private void OnInteractComplete()
    {
        ActionComplete();
    }
}
