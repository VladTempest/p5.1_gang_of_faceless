using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Editor.Scripts.Actions;
using GridSystems;
using Scripts.Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class KnockDownAction : MeleeAttackAction
{
    public static event EventHandler OnAnyKnockDownHappened;
    
    
    public override string GetActionName()
    {
        return "Knock down";
    }
    protected override void TryToChangeState(MeleeAttackState state)
    {
        switch (state)
        {
            case MeleeAttackState.Swinging:
                if (_currentState != MeleeAttackState.Idle) break;
                _currentState = state;
                StartCoroutine(UnitRotator.RotateToDirection(transform, _targetUnit.WorldPosition, _timeToRotateToEnemy));
                break;
            case MeleeAttackState.Attacking:
                if (_currentState != MeleeAttackState.Swinging) break;
                _currentState = state;
                StartCoroutine(UnitRotator.RotateUnitToDirection(_targetUnit, _unit.WorldPosition, _timeForEnemyToRotate));
                _targetUnit.Damage(_damage, transform.position);
                _targetUnit.EffectSystem.KnockDownUnit();   
                OnAnyKnockDownHappened?.Invoke(this, EventArgs.Empty);
                break;
            case MeleeAttackState.Idle:
                if (_currentState != MeleeAttackState.Attacking) break;
                _currentState = state;
                ActionComplete();
                break;
        }
    }
    
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        TryToChangeState(MeleeAttackState.Swinging);
        ActionStart(onActionComplete);
        
    }

    public override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (!base.IsGridPositionValid(testGridPosition, unitGridPosition))
        {
            return false;
        }
        
        if (!GridPositionValidator.IsPositionInsideActionCircleRange(MaxActionRange, testGridPosition, unitGridPosition))
        {
            return false;
        }

        var unitAtGridPosition = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
        if (unitAtGridPosition !=null && unitAtGridPosition.EffectSystem.IsKnockedDown(out var duration))
        {
            return false;
        }

        return true;
    }
}
