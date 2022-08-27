using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using Scripts.Unit;
using UnityEngine;
using UnityEngine.Serialization;

public class DefaultShootAction : BaseAction

{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    

    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Action HitCallback;
    }

    public Unit ActiveUnit => _unit;
    public Unit TargetUnit => _targetUnit;
    
    private enum State
    {
        Aiming = 0,
        Shooting = 1,
        Idle = 2
    }

    [SerializeField]
    private LayerMask _obstaclesLayerMask;

    private State _currentState;
    
    private Unit _targetUnit;
    private float _unitShoulderHeight = 1.7f;
    
    [SerializeField] private ArcherAnimationsEvents _archerAnimationEvents;
    
    [SerializeField] private float _rotationTime = 0.5f;

    private void Start()
    {
        _archerAnimationEvents.ActionEffectCallback = () => TryToChangeState(State.Shooting);
        _archerAnimationEvents.ActionFinishCallback = () => TryToChangeState(State.Idle);
    }
    
    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            TargetUnit = _targetUnit,
            HitCallback = () => Hit()
            
        });
        
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            TargetUnit = _targetUnit
        });
        
    }

    private void Hit()
    {
        _targetUnit.Damage(40, transform.position + Vector3.up * _unitShoulderHeight);
    }

    private void TryToChangeState(State state)
    {
        
        switch (state)
        {
            case State.Aiming:
                if (_currentState != State.Idle) break;
                _currentState = state;
                StartCoroutine(UnitRotator.RotateToDirection(transform, _targetUnit.WorldPosition, _rotationTime));
                InvokeOnActionStart(this, EventArgs.Empty);
                break;
            case State.Shooting:
                if (_currentState != State.Aiming) break;
                _currentState = state;
                Shoot();
                break;
            case State.Idle:
                if (_currentState != State.Shooting) break;
                _currentState = state;
                ActionComplete();
                break;
        }
    }
    
    
    public override string GetActionName()
    {
        return "Shoot";
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);

        _currentState = State.Idle;
        
        ActionStart(onActionComplete);
        TryToChangeState(State.Aiming);
    }

    protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
        {
            return false;
        }

        if (!GridPositionValidator.IsPositionInsideActionRange(ActionRange, testGridPosition, unitGridPosition))
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

        if (!GridPositionValidator.IsGridPositionOnLineOfSight(testGridPosition, unitGridPosition,
                _obstaclesLayerMask))
        {
            return false;
        }

        return true;
    }
    
    
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        return new EnemyAIAction
        {
            gridPosition = gridPosition,
            actionValue = 10000 + Mathf.RoundToInt((1 - targetUnit.HealthNormalised)*100f)
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidGridPositions().Count;
    }
    
}
