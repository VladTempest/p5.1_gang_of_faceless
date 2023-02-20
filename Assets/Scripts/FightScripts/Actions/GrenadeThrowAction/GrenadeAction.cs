using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Editor.Scripts.Actions;
using Editor.Scripts.Animation;
using GridSystems;
using Scripts.Unit;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Transform _grenadeProjectilePrefab;
    [SerializeField] private LayerMask[] _obstaclesLayerMask;
    [SerializeField] private LightWarriorAnimationEvents _lightWarriorAnimationEvents;
    [SerializeField] private Transform _throwStartPoint;
    private GrenadeThrowState _currentState;
    private GridPosition _targetPosition;
    private float _timeToRotateToEnemy = 0.4f;

    private new void Start()
    {
        base.Start();
        if (!enabled) return;
        _lightWarriorAnimationEvents.OnReleaseBomb += LightWarriorAnimationEvents_OnReleaseBomb;
        _lightWarriorAnimationEvents.OnEquipDagger += LightWarriorAnimationEvents_OnEquipDagger;
    }

    private void LightWarriorAnimationEvents_OnEquipDagger()
    {
        TryToChangeState(GrenadeThrowState.Idle);
    }

    private void LightWarriorAnimationEvents_OnReleaseBomb()
    {
        TryToChangeState(GrenadeThrowState.Attacking);
    }
    
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        _targetPosition = gridPosition;
        _currentState = GrenadeThrowState.Idle;
        TryToChangeState(GrenadeThrowState.Swinging);
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
        
        if (!GridPositionValidator.IsPositionInsideActionCircleRange(MaxActionRange, testGridPosition, unitGridPosition)) return false;;

        if (!GridPositionValidator.IsGridPositionOnLineOfSight(testGridPosition, unitGridPosition,
                _obstaclesLayerMask))
        {
            return false;;
        }

        return true;
    }

    private void ActionFinishCallback()
    {
        TryToChangeState(GrenadeThrowState.Idle);
    }

    private void ActionEffectCallback()
    {
        TryToChangeState(GrenadeThrowState.Attacking);
    }

    protected virtual void TryToChangeState(GrenadeThrowState state)
    {
        
        switch (state)
        {
            case GrenadeThrowState.Swinging:
                if (_currentState != GrenadeThrowState.Idle) break;
                _currentState = state;
                StartCoroutine(UnitRotator.RotateToDirection(transform, LevelGrid.Instance.GetWorldPosition(_targetPosition), _timeToRotateToEnemy));
                break;
            case GrenadeThrowState.Attacking:
                if (_currentState != GrenadeThrowState.Swinging) break;
                _currentState = state;
                Transform grenadeProjectileTransform = Instantiate(_grenadeProjectilePrefab, _throwStartPoint.position, Quaternion.identity);
                GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
                grenadeProjectile.Setup(_throwStartPoint, _targetPosition, OnGrenadeBehaviourComplete, _damage);
                break;
            case GrenadeThrowState.Idle:
                if (_currentState != GrenadeThrowState.Attacking) break;
                _currentState = state;
                ActionComplete();
                break;
        }
    }
    
    private void OnGrenadeBehaviourComplete()
    {
        TryToChangeState(GrenadeThrowState.Idle);
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
