using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;

public class InteractAction : BaseAction
{
    private int _maxInteractDistance;
    
    public override string GetActionName()
    {
        return "Interact";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        IInteractable interactableObject = LevelGrid.Instance.GetInteractableAtGridPosition(gridPosition);
        interactableObject.Interact(OnInteractComplete);
        ActionStart(onActionComplete);
    }

    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
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

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
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
