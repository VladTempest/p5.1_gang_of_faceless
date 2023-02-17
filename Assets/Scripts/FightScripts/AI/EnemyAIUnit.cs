using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.Utils;
using FightScripts.GridSystem;
using GridSystems;
using UnityEngine;

namespace Editor.Scripts.AI
{
    
    struct AIMovementActionData
    {
        public bool Equals(AIMovementActionData other)
        {
            return TargetGridPosition.Equals(other.TargetGridPosition) && MovementRating.Equals(other.MovementRating);
        }

        public override bool Equals(object obj)
        {
            return obj is AIMovementActionData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TargetGridPosition, MovementRating);
        }

        public GridPosition TargetGridPosition;
        public float MovementRating;

        public AIMovementActionData(GridPosition targetGridPosition, float movementRating)
        {
            TargetGridPosition = targetGridPosition;
            MovementRating = movementRating;
        }

        public override string ToString()
        {
            return $"Movement Target Grid Position {TargetGridPosition} with moveing rating {MovementRating}";
        }
        
        public static bool operator ==(AIMovementActionData a, AIMovementActionData b)
        {
            return a.TargetGridPosition == b.TargetGridPosition && Math.Abs(a.MovementRating - b.MovementRating) < 0.001f;
        }
        
        public static bool operator !=(AIMovementActionData a, AIMovementActionData b)
        {
            return !(a ==b);
        }
    }
    
    public class EnemyAIUnit : MonoBehaviour
    {
        [SerializeField]
        private Unit _unit;

        private GridRatingEstimator _ratingEstimator;
        [SerializeField]
        private MoveAction _moveAction;

        private List<BaseAction> _availableAttackActions;
        private Action _onAiActionComplete;

        const float enemyPresenceWeight = 1;
        
        private void Start()
        {
            SetUpEnemyAIUnit();
            _ratingEstimator = new GridRatingEstimator(_unit);
        }

        private void SetUpEnemyAIUnit()
        {
            _availableAttackActions = _unit.BaseActions.Where(action => action.isActiveAndEnabled).ToList();
            _availableAttackActions.Remove(_moveAction);
        }

        public void MakeAIAction(Action onActionComplete)
        {
            _onAiActionComplete = onActionComplete;
            StartMovePhase(() => StartAttackPhase(() => MakeAIAction(onActionComplete)));

        }
        

        private void StartAttackPhase(Action onActionComplete)
        {
            var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;
            var unitActionPoint = _unit.ActionPoints;
            if (_availableAttackActions.Any(action => action.ActionPointCost <= unitActionPoint))
            {
                var bestAttackActionData =
                    _ratingEstimator.GetBestAttackAction(onActionComplete, _availableAttackActions, friendlyUnitList);
                
                if (bestAttackActionData != null)
                {
                    _unit.TrySpendActionPointsToTakeAction(bestAttackActionData.AttackAction);
                    bestAttackActionData.AttackAction.TakeAction(bestAttackActionData.TargetPosition, bestAttackActionData.OnActionComplete);
                    return;
                }
            }
            _onAiActionComplete?.Invoke();
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

        private void StartMovePhase(Action onActionComplete)
        {
            
            var maxGridsToMove = _unit.ActionPoints / GameGlobalConstants.ONE_GRID_MOVEMENT_COST;
            if (maxGridsToMove == 0)
            {
                onActionComplete?.Invoke();
                return;
            }

            var emptyAiAction = new AIMovementActionData(new GridPosition(0, 0), 0);
            AIMovementActionData aiBestActionData = emptyAiAction;

            var currentGridPosition = _unit.GetGridPosition();
            var listOfTestGridPositions = new List<GridPosition>();
            for (int x = currentGridPosition.x - maxGridsToMove; x <= currentGridPosition.x + maxGridsToMove; x++)
            {
                for (int z = currentGridPosition.z - maxGridsToMove; z <= currentGridPosition.z + maxGridsToMove; z++)
                {
                    GridPosition testGridPosition = new GridPosition(x, z);
                    if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition)) continue;
                    listOfTestGridPositions.Add(testGridPosition);
                }
            }

            GridPositionUtils.SortGridPositionByDistanceToUnit(listOfTestGridPositions, _unit);
            
            foreach (var testGridPosition in listOfTestGridPositions)
            {
                if (TryGetBestAIMovementActionForGridPosition(testGridPosition, ref aiBestActionData)) break;
            }
            

            if (aiBestActionData.TargetGridPosition != _unit.GetGridPosition() && aiBestActionData != emptyAiAction)
            {
                if (_unit.TrySpendActionPointsToTakeAction(_moveAction, aiBestActionData.TargetGridPosition))
                {
                    _moveAction.TakeAction(aiBestActionData.TargetGridPosition, onActionComplete);
                    return;
                }
            }
            onActionComplete?.Invoke();
        }
        
        private bool TryGetBestAIMovementActionForGridPosition(GridPosition testGridPosition, ref AIMovementActionData aiBestActionData)
        {
            if (testGridPosition != _unit.GetGridPosition())
            {
                if (!GridPositionValidator.IsGridPositionReachable(testGridPosition, _unit.GetGridPosition(),
                        Mathf.FloorToInt(_unit.ActionPoints / 2)))
                {
                    ConvenientLogger.Log(nameof(EnemyAI), GlobalLogConstant.IsAILogEnabled, $"[Enemy AI] TOO FAR with AI Action Data for {_unit}: {testGridPosition}");
                    return false;
                }
            }

            aiBestActionData = UpdateBestActionData(aiBestActionData, testGridPosition);
            if (aiBestActionData.MovementRating >= enemyPresenceWeight)
            {
                ConvenientLogger.Log(nameof(EnemyAI), GlobalLogConstant.IsAILogEnabled, $"[Enemy AI] BREAK with AI Action Data for {_unit}: {aiBestActionData}");
                return true;
            }

            return false;
        }
        
        private AIMovementActionData UpdateBestActionData(AIMovementActionData aiBestMovementActionData,GridPosition testGridPosition,int xGridOffset = 0, int zGridOffset = 0)
        {
            AIMovementActionData currentAiData;
            currentAiData = RateGridPositionWithOffset(testGridPosition);
            if (currentAiData.MovementRating != 0 && currentAiData.MovementRating > aiBestMovementActionData.MovementRating)
            {
                aiBestMovementActionData = currentAiData;
            }

            ConvenientLogger.Log(nameof(EnemyAI), GlobalLogConstant.IsAILogEnabled,
                $" Best AI Action Data for {_unit}: {aiBestMovementActionData}");
            return aiBestMovementActionData;
        }
        
        private AIMovementActionData RateGridPositionWithOffset(GridPosition testGridPosition)
        {
            var rating = RateGridPositionToMove(testGridPosition);
            return new AIMovementActionData(testGridPosition, rating);

        }
        private float RateGridPositionToMove(GridPosition testGridPosition)
        {
            const float enemyPathLenghtWeight = 1;
            float gridPositionRating = 0;
            var friendlyUnitList = UnitManager.Instance.FriendlyUnitList;
            foreach (var playerUnit in friendlyUnitList)
            {
                foreach (var action in _availableAttackActions)
                {
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