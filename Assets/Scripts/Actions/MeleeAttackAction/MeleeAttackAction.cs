using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Editor.Scripts.Actions;
using GridSystems;
using Scripts.Unit;
using UnityEngine;
using UnityEngine.Serialization;

public class MeleeAttackAction : BaseAction
{
    public static event EventHandler OnAnyMeleeHit;

    [SerializeField] private WarriorAnimationEvents _warriorAnimationEvents;
    [SerializeField] private Transform _swordDamageSource;

    protected MeleeAttackState _currentState;
    protected Unit _targetUnit;
    protected float _timeToRotateToEnemy = 0.5f;
    protected float _timeForEnemyToRotate = 0.3f;

    private void Start()
    {
        if (!enabled) return;
        base.Start();
        _warriorAnimationEvents.ActionEffectCallback += ActionEffectCallback;
        _warriorAnimationEvents.ActionFinishCallback += ActionFinishCallback;
        _warriorAnimationEvents.DualSwordCutWasMadeCallback += DualSwordCutWasMadeCallback;
    }

    private void DualSwordCutWasMadeCallback()
    {
        _targetUnit.Damage(0, _swordDamageSource.position);
    }

    private void ActionFinishCallback()
    {
        TryToChangeState(MeleeAttackState.Idle);
    }

    private void ActionEffectCallback()
    {
        TryToChangeState(MeleeAttackState.Attacking);
    }

    protected virtual void TryToChangeState(MeleeAttackState state)
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
                _targetUnit.Damage(_damage, _swordDamageSource.position);
                OnAnyMeleeHit?.Invoke(this, EventArgs.Empty);
                break;
            case MeleeAttackState.Idle:
                if (_currentState != MeleeAttackState.Attacking) break;
                _currentState = state;
                ActionComplete();
                break;
        }
    }

    public override string GetActionName()
    {
        return "Sword";
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
        if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
        {
            return false;
        }

        if (!GridPositionValidator.HasAnyUnitOnGridPosition(testGridPosition))
        {
            return false;
        }

        if (!GridPositionValidator.IsGridPositionWithEnemy(testGridPosition, _unit))
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
            actionValue = 20000

        };
    }
}
