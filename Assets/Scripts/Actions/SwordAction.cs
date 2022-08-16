using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class SwordAction : BaseAction
{
    private enum State
    {
        SwingSwordBeforeHit = 0,
        SwingSwordAfterHit = 1,
    }

    public static event EventHandler OnAnySwordHit; 

    public event EventHandler OnSwordActionStarted;
    public event EventHandler OnSwordActionCompleted;

    [SerializeField] private Transform _swordDamageSource;

    private State _state;
    private float _stateTimer;

    private Unit _targetUnit;
    private float _rotateSpeed = 30f;
    

    private void Update()
    {
        if (!_isActive) return;
        
        _stateTimer -= Time.deltaTime;
        
        switch (_state)
        {
            case State.SwingSwordBeforeHit:
                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - _unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward,aimDirection, Time.deltaTime*_rotateSpeed);
                break;
            case State.SwingSwordAfterHit:
                break;
        }
        
        if (_stateTimer <= 0f)
        {
            NextState();
        }
    }
    
    private void NextState()
    {
        switch (_state)
        {
            case State.SwingSwordBeforeHit:
                _state = State.SwingSwordAfterHit;
                float afterHitStateTime = 0.1f;
                _stateTimer = afterHitStateTime;
                _targetUnit.Damage(100, _swordDamageSource.position);
                OnAnySwordHit?.Invoke(this, EventArgs.Empty);
                break;
            case State.SwingSwordAfterHit:
                OnSwordActionCompleted?.Invoke(this, EventArgs.Empty);
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
        
        _state = State.SwingSwordBeforeHit;
        float beforeHitStateTime = 0.7f;
        _stateTimer = beforeHitStateTime;
        
        OnSwordActionStarted?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
        
    }

    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
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

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            gridPosition = gridPosition,
            actionValue = 20000

        };
    }
}
