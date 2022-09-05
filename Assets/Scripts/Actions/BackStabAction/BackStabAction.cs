using System;
using System.Collections.Generic;
using DefaultNamespace;
using GridSystems;
using UnityEngine;

namespace Actions
{
    public class BackStabAction: BaseAction
    {
        [SerializeField] private int _attackDamage = 100;
        private Dictionary<GridPosition, GridPosition> _firstValidTeleportGridPositionsAndEnemyGridPositions = new Dictionary<GridPosition, GridPosition>();

        private BackStabActionState CurrentState { get; set; } = BackStabActionState.Idle;

        public override string GetActionName()
        {
            return "Back Stab";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            ActionStart(onActionComplete);
            BackStab(gridPosition);
        }

        private void BackStab(GridPosition gridPosition)
        {
            TeleportWithRotate(gridPosition);
            Attack(gridPosition);
        }

        private void Attack(GridPosition gridPosition)
        {
            TryToChangeState(BackStabActionState.Stabbing);
            TryToChangeState(BackStabActionState.Idle);
            var targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            targetUnit.Damage(_attackDamage, _unit.WorldPosition);
            ActionComplete();
        }

        private void TeleportWithRotate(GridPosition gridPosition)
        {
            TryToChangeState(BackStabActionState.Teleporting);
            gameObject.transform.position = LevelGrid.Instance.GetWorldPosition(_firstValidTeleportGridPositionsAndEnemyGridPositions[gridPosition]);
            gameObject.transform.LookAt(LevelGrid.Instance.GetWorldPosition(gridPosition));
            _firstValidTeleportGridPositionsAndEnemyGridPositions.Clear();
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

            if (!GridPositionValidator.HasTestGridPositionAvailableNeighbours(testGridPosition,
                   _firstValidTeleportGridPositionsAndEnemyGridPositions))
            {
                return false;
            }

            return true;
        }

        public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            throw new NotImplementedException();
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
                    break;
                case BackStabActionState.Stabbing:
                    if (CurrentState == BackStabActionState.Teleporting) CurrentState = state;
                    break;
            }
        }
    }
}