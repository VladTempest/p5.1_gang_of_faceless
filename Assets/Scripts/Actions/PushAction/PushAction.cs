using System;
using System.Collections;
using System.Numerics;
using DefaultNamespace;
using GridSystems;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace Actions
{
    public class PushAction : BaseAction
    {
        public static event EventHandler<OnPushActionEventArgs> OnAnyUnitPushed;
        
        private PushActionState _currentState = PushActionState.Idle;
        private float _moveSpeed = 10f;

        public PushActionState CurrentState
        {
            get => _currentState;
            set => _currentState = value;
        }

        public override string GetActionName()
        {
            return "Push";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            TryToChangeState(PushActionState.Pushing);
            var enemyUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            var sourceOfPushGridPosition = _unit.GetGridPosition();
            ActionStart(onActionComplete);
            PushEnemyAway(enemyUnit, sourceOfPushGridPosition);
            
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
                    if (CurrentState == PushActionState.Pushing) CurrentState = state;
                    break;
                case PushActionState.Pushing:
                    if (CurrentState == PushActionState.Idle) CurrentState = state;
                    break;
            }
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
            
            if (!GridPositionValidator.IsPositionInsideActionCircleRange(ActionRange, testGridPosition, unitGridPosition)) return false;;

            return true;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            throw new NotImplementedException();
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
                OnAnyUnitPushed?.Invoke(this, new OnPushActionEventArgs(){ pushedFromGridPosition = LevelGrid.Instance.GetGridPosition(pushedFromPosition)});
                pushedUnit.transform.position = targetPosition;
            }
            TryToChangeState(PushActionState.Idle);
            ActionComplete();
        }
    }
}