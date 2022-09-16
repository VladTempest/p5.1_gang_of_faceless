using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using DefaultNamespace;
using Editor.Scripts.Utils;
using GridSystems;
using Scripts.Unit;
using UnityEngine;
using UnityEngine.Serialization;

public class BaseShootAction : BaseAction

{ 
    public static event EventHandler OnShootHit;

    public Unit ActiveUnit => _unit;
    public Unit TargetUnit => _targetUnit;

    [SerializeField] public int MinActionRange = 2;

    protected enum State
    {
        Aiming = 0,
        Shooting = 1,
        Idle = 2
    }

    [SerializeField]
    protected LayerMask[] _obstaclesLayerMask;

    private State _currentState;

    protected Unit _targetUnit;

    [SerializeField] protected ArcherAnimationsEvents _archerAnimationEvents;

    [SerializeField] private float _rotationTime = 0.5f;
    [SerializeField] private int _hitAmount = 50;
    private float _timeForEnemyToRotate = 0.3f;


    protected virtual void Shoot()
    {
    }

    protected void Hit()
    {
        _targetUnit.Damage(_hitAmount, transform.position + Vector3.up * GameGlobalConstants.UNIT_SHOULDER_HEIGHT);
        StartCoroutine(UnitRotator.RotateUnitToDirection(_targetUnit, _unit.WorldPosition, _timeForEnemyToRotate));
        OnShootHit?.Invoke(this, EventArgs.Empty);
        TryToChangeState(State.Idle);
        
    }

    protected void TryToChangeState(State state)
    {
        
        switch (state)
        {
            case State.Aiming:
                if (_currentState != State.Idle) break;
                _currentState = state;
                StartCoroutine(UnitRotator.RotateToDirection(transform, _targetUnit.WorldPosition, _rotationTime));
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
        return "Base Shoot";
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
        if (!base.IsGridPositionValid(testGridPosition, unitGridPosition))
        {
            return false;
        }

        return true;
    }

    protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
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
