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
        

        private void Start()
        {
            SetUpEnemyAIUnit();
        }

        private void SetUpEnemyAIUnit()
        {
            _availableAttackActions = _unit.BaseActions.Where(action => action.isActiveAndEnabled).ToList();
            _availableAttackActions.Remove(_moveAction);
        }

        public void MakeAIAction(Action onActionComplete)
        {
            StartMovePhase(onActionComplete); 
            //StartAttackPhase();
        }

        private void StartMovePhase(Action onActionComplete)
        {
            var maxGridsToMove = _unit.ActionPoints / GameGlobalConstants.ONE_GRID_MOVEMENT_COST;
            

            
            for (int gridsOffsetNumber = 0; gridsOffsetNumber <= maxGridsToMove; gridsOffsetNumber++)
            {
                if (TryToMoveWithAnOffset(onActionComplete, gridsOffsetNumber))
                    return;
            }
        }

        private bool TryToMoveWithAnOffset(Action onActionComplete, int gridsOffsetNumber)
        {
            AIMovementActionData aiBestActionData = new AIMovementActionData(new GridPosition(0,0), 0);
            Tuple<float, GridPosition> currentTuple;
            if (gridsOffsetNumber == 0)
            {
                aiBestActionData = UpdateBestTuple(aiBestActionData);

                if (aiBestActionData.MovementRating > 0) return true;
                return false;
            }

            if (gridsOffsetNumber > 1)
            {
                var previousOffsetNumber = gridsOffsetNumber - 1;
                aiBestActionData = UpdateBestTuple(aiBestActionData, previousOffsetNumber , previousOffsetNumber);
                aiBestActionData = UpdateBestTuple(aiBestActionData,-previousOffsetNumber, -previousOffsetNumber);
                aiBestActionData = UpdateBestTuple(aiBestActionData,previousOffsetNumber, -previousOffsetNumber);
                aiBestActionData = UpdateBestTuple(aiBestActionData,-previousOffsetNumber, previousOffsetNumber);
            }

            aiBestActionData = UpdateBestTuple(aiBestActionData, 0 , gridsOffsetNumber);
            aiBestActionData = UpdateBestTuple(aiBestActionData,gridsOffsetNumber, 0);
            aiBestActionData = UpdateBestTuple(aiBestActionData,0, -gridsOffsetNumber);
            aiBestActionData = UpdateBestTuple(aiBestActionData,-gridsOffsetNumber, 0);

            if (aiBestActionData.MovementRating > 0)
            {
                _moveAction.TakeAction(aiBestActionData.TargetGridPosition, onActionComplete);
                return true;
            }

            return false;
        }

        private AIMovementActionData UpdateBestTuple(AIMovementActionData aiBestMovementActionData,int xGridOffset = 0, int zGridOffset = 0)
        {
            AIMovementActionData currentAiData;
            currentAiData = RateGridPositionWithOffset(xGridOffset, zGridOffset);
            if (currentAiData.MovementRating != 0 && currentAiData.MovementRating >= aiBestMovementActionData.MovementRating)
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
            const float enemyPresenceWeight = 1;
            const float enemyPathLenghtWeight = 1;
            float gridPositionRating = 0;
            var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;
            foreach (var playerUnit in friendlyUnitList)
            {
                foreach (var action in _availableAttackActions)
                {
                    Debug.Log("Checking " + action.name + "action");
                    if (!CheckIfGridPositionReachable(testGridPosition, _unit.GetGridPosition())) continue;
                    if (!CheckIfUnitInAttackRange(testGridPosition, playerUnit, action)) continue;
                    var playerGridPosition = playerUnit.GetGridPosition();
                    var enemyToPlayerLenghtPath =
                        Pathfinding.Instance.GetPathLengthToUnwalkableGridPosition(testGridPosition, playerGridPosition);
                    gridPositionRating += enemyPresenceWeight + 1/(float)enemyToPlayerLenghtPath*enemyPathLenghtWeight;
                }
            }

            Debug.Log($"Check gridposition {testGridPosition} with rating {gridPositionRating}");
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

        private void StartAttackPhase()
        {
            throw new NotImplementedException();
        }
    }
}