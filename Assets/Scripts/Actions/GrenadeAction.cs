using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform _grenadeProjectilePrefab;
    [SerializeField] private LayerMask _obstaclesLayerMask;
    

    private void Update()
    {
        if (!_isActive) return;
    }

    public override string GetActionName()
    {
        return "Grenade";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(_grenadeProjectilePrefab, _unit
            .GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition, OnGrenadeBehaviourComplete);
        
        Debug.Log("Grenade Action");
        ActionStart(onActionComplete);
    }

    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
        {
            return false;
        }
        
        if (!GridPositionValidator.IsPositionInsideActionRange(_actionRange, testGridPosition, unitGridPosition)) return false;;

        if (!GridPositionValidator.IsGridPositionOnLineOfSight(testGridPosition, unitGridPosition,
                _obstaclesLayerMask))
        {
            return false;;
        }

        return true;
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();   
    }
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 0
        };
        
    }
}
