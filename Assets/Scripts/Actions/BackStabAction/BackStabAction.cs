using System;
using System.Collections.Generic;
using DefaultNamespace;
using Editor.Scripts.Animation;
using GridSystems;
using UnityEngine;

namespace Actions
{
    public class BackStabAction: BaseAction
    {
        public event EventHandler OnStartTeleporting;
        
        [SerializeField] private LightWarriorAnimationEvents _lightWarriorAnimationEvents;
        [SerializeField] private int _attackDamage = 100;
        [SerializeField] private Transform _swordDamageSource;
        private Dictionary<GridPosition, GridPosition> _firstValidTeleportGridPositionsAndEnemyGridPositions = new Dictionary<GridPosition, GridPosition>();
        private GridPosition _taragetPosition;

        private BackStabActionState CurrentState { get; set; } = BackStabActionState.Idle;

        
        private void Start()
        {
            _lightWarriorAnimationEvents.ActionTeleportCallback += ActionTeleportCallback;
            _lightWarriorAnimationEvents.ActionEffectCallback += ActionEffectCallback;
            _lightWarriorAnimationEvents.ActionFinishCallback += ActionFinishCallback;
        }

        private void ActionFinishCallback()
        {
            TryToChangeState(BackStabActionState.Idle);
            ActionComplete();
        }

        private void ActionEffectCallback()
        {
            TryToChangeState(BackStabActionState.Stabbing);
        }

        private void ActionTeleportCallback()
        {
            TryToChangeState(BackStabActionState.Teleporting);
        }

        public override string GetActionName()
        {
            return "Back Stab";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            _taragetPosition = gridPosition;
            CurrentState = BackStabActionState.Idle;
            ActionStart(onActionComplete);
            OnStartTeleporting?.Invoke(this, EventArgs.Empty);
            
        }

        private void Attack(GridPosition gridPosition)
        {
            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            targetUnit.Damage(_attackDamage, _swordDamageSource.position);
        }

        private void TeleportWithRotate(GridPosition gridPosition)
        {
            gameObject.transform.position = LevelGrid.Instance.GetWorldPosition(_firstValidTeleportGridPositionsAndEnemyGridPositions[gridPosition]);
            gameObject.transform.LookAt(LevelGrid.Instance.GetWorldPosition(gridPosition));
            _firstValidTeleportGridPositionsAndEnemyGridPositions.Clear();
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

            if (!GridPositionValidator.HasTestGridPositionAvailableNeighbours(testGridPosition,
                   _firstValidTeleportGridPositionsAndEnemyGridPositions))
            {
                return false;
            }

            return true;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction(){actionValue = 0, gridPosition = new GridPosition()};
        }
        
        protected void TryToChangeState(BackStabActionState state)
        {
        
            switch (state)
            {
                case BackStabActionState.Idle:
                    if (CurrentState == BackStabActionState.Stabbing) CurrentState = state;
                    break;
                case BackStabActionState.Teleporting:
                    if (CurrentState == BackStabActionState.Idle) CurrentState = state;
                    TeleportWithRotate(_taragetPosition);
                    break;
                case BackStabActionState.Stabbing:
                    if (CurrentState == BackStabActionState.Teleporting) CurrentState = state;
                    Attack(_taragetPosition);
                    break;
            }
        }
    }
}