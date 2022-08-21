using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    public event EventHandler<OnShootEventArgs> OnShoot;
    public event EventHandler OnAiming;

    public class OnShootEventArgs : EventArgs
    {
        public Unit targetUnit;
        public Unit shootUnit;
    }
    
    private enum State
    {
        Idle = 0,
        Aiming = 1,
        Shooting = 2,
        Cooloff = 3
    }

    [SerializeField]
    private LayerMask _obstaclesLayerMask;

    private State _state;
    private float _totalSpinAmount;
   
    private float _stateTimer;
    private Unit _targetUnit;
    private bool _canShootBullet;
    private bool _alreadyAimed;
    private float _rotateSpeed = 10f;
    private float _unitShoulderHeight = 1.7f;
    
    [SerializeField] private float _shootingStateTime = 5f;
    [SerializeField] private float _coolStateTime = 0.5f;
    [SerializeField] private float _aimingStateTime = 2f;
    [SerializeField] private float _idleStateTime = 2f;
    

    private void Update()
    {
        if (!_isActive) return;

        _stateTimer -= Time.deltaTime;
        
        switch (_state)
        {
         case State.Idle:
             Vector3 aimDirection = (_targetUnit.WorldPosition - _unit.WorldPosition).normalized;
             transform.forward = Vector3.Lerp(transform.forward,aimDirection, Time.deltaTime*_rotateSpeed);
             break;
         case State.Aiming:
             if (!_alreadyAimed)
             {
                 OnAiming?.Invoke(this, EventArgs.Empty);
                 _alreadyAimed = true;
             }
                break;
         case State.Shooting:
             if (_canShootBullet)
             {
                 Shoot();
                 _canShootBullet = false;
             }
             break;
         case State.Cooloff:
             break;
        }
        
        if (_stateTimer <= 0f)
        {
            NextState();
        }
    }

    private void Shoot()
    {
        OnShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootUnit = _unit
        });
        
        OnAnyShoot?.Invoke(this, new OnShootEventArgs
        {
            targetUnit = _targetUnit,
            shootUnit = _unit
        });
        _targetUnit.Damage(40, transform.position + Vector3.up * _unitShoulderHeight);
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.Idle:
                _state = State.Aiming;
                _stateTimer = _shootingStateTime;
                break;
            case State.Aiming:
                _state = State.Shooting;
                _stateTimer = _aimingStateTime;
                break;
            case State.Shooting:
                _state = State.Cooloff;
                _stateTimer = _coolStateTime;
                break;
            case State.Cooloff:
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

        _state = State.Idle;
        _stateTimer = _idleStateTime;

        _alreadyAimed = false;
        _canShootBullet = true;
        
        ActionStart(onActionComplete);
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


    public Unit GetTargetUnit()
    {
        return _targetUnit;
    }

    public Unit GetActiveUnit()
    {
        return _unit;
    }

    public int GetMaxShootDistance()
    {
        return ActionRange;
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
