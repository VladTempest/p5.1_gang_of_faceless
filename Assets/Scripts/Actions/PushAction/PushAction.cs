using System;
using System.Collections;
using System.Numerics;
using DefaultNamespace;
using GridSystems;
using Scripts.Unit;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Actions
{
    public class PushAction : BaseAction
    {
        public static event EventHandler<OnAnyPushActionEventArgs> OnAnyUnitPushed;
        public event EventHandler<OnPushActionEventArgs> OnUnitPushed;
        
        private PushActionState _currentState = PushActionState.Idle;
        private float _moveSpeed = 10f;

        public PushActionState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }
        
        [SerializeField] private WarriorAnimationEvents _warriorAnimationEvents;
        private Unit _enemyUnit;
        private GridPosition _sourceOfPushGridPosition;
        private float _timeToRotateToEnemy = 0.3f;
        private float _timeForEnemyToRotate = 0.3f;

        private void Start()
        {
            _warriorAnimationEvents.PushingCallback += PushingCallback;
            _warriorAnimationEvents.ActionFinishCallback += ActionFinishCallback;
        }

        private void ActionFinishCallback()
        {
            TryToChangeState(PushActionState.Idle);
        }

        private void PushingCallback()
        {
            TryToChangeState(PushActionState.Pushing);
        }

        public override string GetActionName()
        {
            return "Push";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            _currentState = PushActionState.Idle;
            _enemyUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            _sourceOfPushGridPosition = _unit.GetGridPosition();
            TryToChangeState(PushActionState.GettingReady);
            ActionStart(onActionComplete);
        }

        private void PushEnemyAway(Unit unit, GridPosition sourceOfPushGridPosition)
        {
            var coroutine = MoveUnit(unit, sourceOfPushGridPosition);
            StartCoroutine(coroutine);
        }

        private void TryToChangeState(PushActionState state)
        {
        
            switch (state)
            {
                case PushActionState.Idle:
                    if (CurrentState == PushActionState.Pushing)
                    {
                        CurrentState = state;
                    }
                    break;
                case PushActionState.Pushing:
                    if (CurrentState == PushActionState.GettingReady)
                    {
                        CurrentState = state;
                        OnUnitPushed?.Invoke(this, new OnPushActionEventArgs(){pushedUnitAnimator = _enemyUnit.GetComponentInChildren<Animator>()});
                        PushEnemyAway(_enemyUnit, _sourceOfPushGridPosition);
                        StartCoroutine(UnitRotator.RotateUnitToDirection(_enemyUnit, _unit.WorldPosition, _timeForEnemyToRotate));
                    }
                    break;
                case PushActionState.GettingReady:
                    if (CurrentState == PushActionState.Idle)
                    {
                        CurrentState = state;
                        InvokeOnActionStart(this, EventArgs.Empty);
                        StartCoroutine(UnitRotator.RotateToDirection(_unit.transform, _enemyUnit.WorldPosition, _timeToRotateToEnemy));
                    }
                    break;
            }
        }
        
        protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
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
            
            if (!GridPositionValidator.IsPositionInsideActionCircleRange(ActionRange, testGridPosition, unitGridPosition)) return false;

            return true;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction(){actionValue = 0, gridPosition = new GridPosition()};
        }

        private IEnumerator MoveUnit(Unit pushedUnit, GridPosition sourceOfPush)
        {
            var pushedFromPosition = pushedUnit.WorldPosition;
            var pushDirection = pushedFromPosition - transform.position;
            Vector3 targetPosition = pushedUnit.transform.position + pushDirection;
            if (GridPositionValidator.IsGridPositionOpenToMoveTo(LevelGrid.Instance.GetGridPosition(targetPosition),
                    pushedUnit.GetGridPosition()))
            {
                while (Vector3.Distance(targetPosition, pushedUnit.transform.position) >= _moveSpeed * Time.deltaTime)
                {
                    pushedUnit.transform.position = Vector3.MoveTowards(pushedUnit.transform.position, targetPosition,
                        _moveSpeed * Time.deltaTime);
                    yield return 0;
                }
                OnAnyUnitPushed?.Invoke(this, new OnAnyPushActionEventArgs(){ pushedFromGridPosition = LevelGrid.Instance.GetGridPosition(pushedFromPosition)});
                pushedUnit.transform.position = targetPosition;
            }
            TryToChangeState(PushActionState.Idle);
            ActionComplete();
        }
    }
}