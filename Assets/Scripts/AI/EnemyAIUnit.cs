using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.Utils;
using GridSystems;
using UnityEngine;

namespace Editor.Scripts.AI
{
    
    struct AIMovementActionData
    {
        public GridPosition TargetGridPosition;
        public float MovementRating;

        public AIMovementActionData(GridPosition targetGridPosition, float movementRating)
        {
            TargetGridPosition = targetGridPosition;
            MovementRating = movementRating;
        }
    }
    
    public class EnemyAIUnit : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        [SerializeField]
        private MoveAction _moveAction;

        private List<BaseAction> _availableAttackActions;
        private Action _onAiActionComplete;

        const float enemyPresenceWeight = 1;
        
        private void Start()
        {
            SetUpEnemyAIUnit();
        }

        private void SetUpEnemyAIUnit()
        {
            _availableAttackActions = _unit.BaseActions.Where(action => action.isActiveAndEnabled).ToList();
            _availableAttackActions.Remove(_moveAction);
        }

        public void TryMakeAIAction(Action onActionComplete)
        {
            _onAiActionComplete = onActionComplete;
            TryStartMovePhase(() => TryStartAttackPhase(() => TryMakeAIAction(onActionComplete)));

        }
        

        private bool TryStartAttackPhase(Action onActionComplete)
        {
            var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;
            var unitActionPoint = _unit.ActionPoints;
            if (_availableAttackActions.Any(action => action.ActionPointCost <= unitActionPoint))
            {
                foreach (var playerUnit in friendlyUnitList)
                {
                    foreach (var action in _availableAttackActions)
                    {
                        if (TryMakeAttackAction(playerUnit, action, onActionComplete))
                        {
                            return true;
                        }
                    }
                }
            }
            _onAiActionComplete?.Invoke();
            return false;
        }

        private bool TryMakeAttackAction(Unit playerUnit, BaseAction action, Action onActionComplete)
        {
            var playerUnitGridPosition = playerUnit.GetGridPosition();
            if (action.IsGridPositionValid(playerUnitGridPosition, _unit.GetGridPosition()))
            {
                if (_unit.TrySpendActionPointsToTakeAction(action))
                {
                    action.TakeAction(playerUnitGridPosition, onActionComplete);
                    return true;
                }
            }

            return false;
        }

        private bool TryStartMovePhase(Action onActionComplete)
        {
            
            var maxGridsToMove = _unit.ActionPoints / GameGlobalConstants.ONE_GRID_MOVEMENT_COST;
            

            AIMovementActionData aiBestActionData = new AIMovementActionData(new GridPosition(0, 0), 0);
            for (int gridsOffsetNumber = 0; gridsOffsetNumber <= maxGridsToMove; gridsOffsetNumber++)
            {
                aiBestActionData = GetBestActionDataForOffset(gridsOffsetNumber, aiBestActionData);
                if (aiBestActionData.MovementRating >= enemyPresenceWeight) {break;}
            }

            if (aiBestActionData.TargetGridPosition != _unit.GetGridPosition())
            {
                if (_unit.TrySpendActionPointsToTakeAction(_moveAction, aiBestActionData.TargetGridPosition))
                {
                    _moveAction.TakeAction(aiBestActionData.TargetGridPosition, onActionComplete);
                    return true;
                }
            }
            onActionComplete?.Invoke();
  
            return false;
        }

        private AIMovementActionData GetBestActionDataForOffset(int gridsOffsetNumber, AIMovementActionData aiBestActionData)
        {
            if (gridsOffsetNumber == 0)
            {
                return UpdateBestActionData(aiBestActionData);
            }

            if (gridsOffsetNumber > 1)
            {
                var previousOffsetNumber = gridsOffsetNumber - 1;
                aiBestActionData = UpdateBestActionData(aiBestActionData, previousOffsetNumber , previousOffsetNumber);
                aiBestActionData = UpdateBestActionData(aiBestActionData,-previousOffsetNumber, -previousOffsetNumber);
                aiBestActionData = UpdateBestActionData(aiBestActionData,previousOffsetNumber, -previousOffsetNumber);
                aiBestActionData = UpdateBestActionData(aiBestActionData,-previousOffsetNumber, previousOffsetNumber);
            }

            aiBestActionData = UpdateBestActionData(aiBestActionData, 0 , gridsOffsetNumber);
            aiBestActionData = UpdateBestActionData(aiBestActionData,gridsOffsetNumber, 0);
            aiBestActionData = UpdateBestActionData(aiBestActionData,0, -gridsOffsetNumber);
            aiBestActionData = UpdateBestActionData(aiBestActionData,-gridsOffsetNumber, 0);

            return aiBestActionData;
        }

        private AIMovementActionData UpdateBestActionData(AIMovementActionData aiBestMovementActionData,int xGridOffset = 0, int zGridOffset = 0)
        {
            AIMovementActionData currentAiData;
            currentAiData = RateGridPositionWithOffset(xGridOffset, zGridOffset);
            if (currentAiData.MovementRating != 0 && currentAiData.MovementRating > aiBestMovementActionData.MovementRating)
            {
                aiBestMovementActionData = currentAiData;
            }

            return aiBestMovementActionData;
        }
        
        private AIMovementActionData RateGridPositionWithOffset(int xOffset = 0, int zOffset = 0)
        {
            var testGridPosition = _unit.GetGridPosition() + new GridPosition(xOffset, zOffset);
            var rating = RateGridPosition(testGridPosition);
            return new AIMovementActionData(testGridPosition, rating);

        }
        private float RateGridPosition(GridPosition testGridPosition)
        {
            const float enemyPathLenghtWeight = 1;
            float gridPositionRating = 0;
            var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;
            foreach (var playerUnit in friendlyUnitList)
            {
                foreach (var action in _availableAttackActions)
                {
                    //Debug.Log("Checking " + action.name + "action");
                    var enemiesInRangeNumber = 0;
                    if (testGridPosition != _unit.GetGridPosition() && !CheckIfGridPositionReachable(testGridPosition, _unit.GetGridPosition())) continue;

                    if (CheckIfUnitInAttackRange(testGridPosition, playerUnit, action))
                    {
                        enemiesInRangeNumber = 1;
                    }
                    
                    var playerGridPosition = playerUnit.GetGridPosition();
                    var enemyToPlayerLenghtPath = Pathfinding.Instance.GetPathLengthToUnwalkableGridPosition(testGridPosition, playerGridPosition, _unit.GetGridPosition());
                    if (enemyToPlayerLenghtPath == 0) continue;
                    
                    gridPositionRating += enemiesInRangeNumber * enemyPresenceWeight + 1/(float)enemyToPlayerLenghtPath*enemyPathLenghtWeight;
                }
            }

            //Debug.Log($"Check gridposition {testGridPosition} with rating {gridPositionRating}");
            return gridPositionRating;
        }

        private bool CheckIfUnitInAttackRange(GridPosition enemyUnitGridPosition, Unit friendlyUnit, BaseAction action)
        {
            var enemyGridPosition = friendlyUnit.GetGridPosition();
            return action.IsGridPositionValid(enemyGridPosition, enemyUnitGridPosition);
        }
        
        private bool CheckIfGridPositionReachable(GridPosition targetGridPosition, GridPosition enemyGridPosition)
        {
            return _moveAction.IsGridPositionValid(targetGridPosition, enemyGridPosition);
        }
        
    }
}