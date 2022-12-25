using System;
using System.Collections.Generic;
using System.Linq;
using Actions;
using DefaultNamespace;
using Editor.Scripts.Utils;
using GridSystems;
using Scripts.Unit;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Editor.Scripts.AI
{
    public class GridRatingEstimator
    {
        private readonly Unit _enemyUnit;
        
        private readonly float _healthLeftWeight;
        private readonly float _playerCharacterStatusWeight;
        private readonly float _pathLengthWeight;
        private readonly float _actionPointCostWeight;
        private readonly float _coolDownWeight;
        private readonly float _normalizedDamageWeight;
        private readonly float _additionalEffectsWeight;

        public AIAttackActionData GetBestAttackAction(Action onActionComplete, List<BaseAction> availableAttackActions,
            List<Unit> friendlyUnitList)
        {
            List<AIAttackActionData> listOfBestActions = new List<AIAttackActionData>();
            foreach (var friendlyUnit in friendlyUnitList)
            {
                var bestAiActionData =
                    GetBestAttackActionForUnit(friendlyUnit, availableAttackActions, onActionComplete);
                
                if (bestAiActionData != null)
                {
                    listOfBestActions.Add(bestAiActionData);
                }
            }

            if (!listOfBestActions.IsNullOrEmpty())
            {
                var randomBestAction = GetBestAIAttackActionData(listOfBestActions);
                return randomBestAction;
            }
            
            Debug.LogWarning($"[Enemy AI] Resulting best action IS NULL for {_enemyUnit.name}");
            return null;
        }

        private AIAttackActionData GetBestAIAttackActionData(List<AIAttackActionData> listOfBestActions)
        {
            var orderedList = listOfBestActions.OrderByDescending(aiActionData =>
                aiActionData.ActionRating);
            var bestActionAiData = orderedList.First();
            var randomBestAction = GetRandomBestAIActionFromList(orderedList, bestActionAiData);
            Debug.LogWarning($"[Enemy AI] Resulting {randomBestAction} for {_enemyUnit.name}");
            return randomBestAction;
        }

        public GridRatingEstimator(Unit enemyUnit)
        {
            _enemyUnit = enemyUnit; 
            
            var estimationWeights = ConstantsProvider.Instance.gridEstimationWeightsSO.GridEstimationWeightsDictionary[_enemyUnit.UnitType];
            _healthLeftWeight = estimationWeights.healthLeftWeight;
            _playerCharacterStatusWeight = estimationWeights.playerCharacterStatusWeight;
            _pathLengthWeight = estimationWeights.pathLengthWeight;
            _actionPointCostWeight = estimationWeights.actionPointCostWeight;
            _coolDownWeight = estimationWeights.coolDownWeight;
            _normalizedDamageWeight = estimationWeights.normalizedDamageWeight;
            _additionalEffectsWeight = estimationWeights.additionalEffectsWeight;
            
        }

        private AIAttackActionData GetBestAttackActionForUnit(Unit friendlyUnit, List<BaseAction> availableAttackActions, Action onActionComplete)
        {
            List<AIAttackActionData> listOfAiActionDataForUnit = new List<AIAttackActionData>();

            foreach (var attackAction in availableAttackActions)
            {
                AIAttackActionData actionData = GetAIAttackActionData(attackAction, friendlyUnit, onActionComplete);
                if (actionData != null) listOfAiActionDataForUnit.Add(actionData);
            }

            if (!listOfAiActionDataForUnit.IsNullOrEmpty())
            {
                var randomBestAction = GetBestAIAttackActionData(listOfAiActionDataForUnit);
                return randomBestAction;
            }
            Debug.LogWarning($"[Enemy AI] Rated action for friendly unit: {friendlyUnit.name} -IS NULL for {_enemyUnit.name}");
            return null;
        }

        private AIAttackActionData GetRandomBestAIActionFromList(IOrderedEnumerable<AIAttackActionData> orderedList,
            AIAttackActionData bestActionAiData)
        {
            var bestActionsWithSameRating =
                orderedList.Where(item => Math.Abs(item.ActionRating - bestActionAiData.ActionRating) < 0.01f);
            Debug.LogWarning(
                $"[Enemy AI] Rated action of {_enemyUnit.name} for friendly unit: {LevelGrid.Instance.GetUnitAtGridPosition(bestActionAiData.TargetPosition).name} - {bestActionAiData}");
            IEnumerable<AIAttackActionData> actionsWithSameRating =
                bestActionsWithSameRating as AIAttackActionData[] ?? bestActionsWithSameRating.ToArray();
            var randomIndex = Random.Range(0, actionsWithSameRating.Count());
            var randomBestAction = actionsWithSameRating.ToList()[randomIndex];
            return randomBestAction;
        }

        private AIAttackActionData GetAIAttackActionData(BaseAction attackAction, Unit friendlyUnit, Action onActionComplete)
        {
            float resultingRating = 0;

            if (_enemyUnit.CanSpendActionPointToTakeAction(attackAction) && attackAction.IsGridPositionValid(friendlyUnit.GetGridPosition(), _enemyUnit.GetGridPosition()))
            {
                float playerUnitHealthLeftRating = GetPlayerUnitHealthLeftRating(friendlyUnit);

                float normalizedDamageToHealthRating = GetNormalizedDamageToHealthRating(attackAction, friendlyUnit);
                
                float normalizedActionCostRating = GetNormalizedActionCostRating(attackAction);

                float normalizedCooldownRating = GetNormalizedCooldownRating(attackAction);
                
                float normalizedAdditionalEffectsRating = GetNormalizedAdditionalEffectsRating(attackAction, friendlyUnit);

                float normalizedPathLengthRating = GetPathLengthRating(friendlyUnit);

                float playerCharacterStatusRating = GetPlayerCharacterStatusRating(friendlyUnit);

                resultingRating = normalizedActionCostRating + normalizedDamageToHealthRating + playerUnitHealthLeftRating + normalizedCooldownRating + normalizedAdditionalEffectsRating + normalizedPathLengthRating + playerCharacterStatusRating;
                Debug.LogWarning($"[Enemy AI] RATING {attackAction.GetActionName()} for {friendlyUnit}({friendlyUnit.GetGridPosition()}): resultingRating({resultingRating}) = normalizedActionCostRating({normalizedActionCostRating})+ normalizedDamageToHealthRating({normalizedDamageToHealthRating}) + playerUnitHealthLeftRating({playerUnitHealthLeftRating}) + normalizedCooldownRating({normalizedCooldownRating}) + normalizedAdditionalEffectsRating({normalizedAdditionalEffectsRating} + normalizedPathLengthRating({normalizedPathLengthRating} + playerCharacterStatusRating({playerCharacterStatusRating})));");
                return new AIAttackActionData(attackAction, friendlyUnit.GetGridPosition(), onActionComplete,
                    resultingRating);
            }

            return null;
        }

        private float GetPlayerCharacterStatusRating(Unit friendlyUnit)
        {
            float rawPlayerCharacterStatusRating = 2f;

            if (friendlyUnit.EffectSystem.IsKnockedDown()) rawPlayerCharacterStatusRating -= 1;
            if (friendlyUnit.EffectSystem.IsParalyzed(out var duration)) rawPlayerCharacterStatusRating -= 1;
            return _playerCharacterStatusWeight * rawPlayerCharacterStatusRating;
        }

        private float GetPathLengthRating(Unit friendlyUnit)
        {
            float pathLength = Pathfinding.Instance.GetPathLengthToUnwalkableGridPosition(
                _enemyUnit.GetGridPosition(),
                friendlyUnit.GetGridPosition(),
                _enemyUnit.GetGridPosition());
            float normalizedPathLength = pathLength /
                                         (LevelGrid.Instance.GetWidth() * 1.4f *
                                          GameGlobalConstants.PATH_TO_POINT_MULTIPLIER);

            float playerCharacterPathLengthRating = (1 - normalizedPathLength) * _pathLengthWeight;
            return playerCharacterPathLengthRating;
        }

        private float GetNormalizedAdditionalEffectsRating(BaseAction attackAction, Unit friendlyUnit)
        {
            var normalizedAdditionalEffectsRating = 0f;
            switch (attackAction)
            {
                case ParalyzeShotAction paralyzeShotAction:
                    if (friendlyUnit.UnitType != UnitType.HeavyWarrior && friendlyUnit.UnitType != UnitType.LightWarrior) break;
                    if (IsThereEnemyAroundFriendlyUnit(friendlyUnit)) break;
                    normalizedAdditionalEffectsRating = 1f;
                    break;
                case PushAction pushAction:
                    if (friendlyUnit.UnitType == UnitType.Archer) break;
                    normalizedAdditionalEffectsRating = GetNormalizedAdditionalEffectsRatingForPush(friendlyUnit);
                    break;
                case KnockDownAction knockDownAction:
                    switch (friendlyUnit.UnitType)
                    {
                        case UnitType.None:
                            break;
                        case UnitType.Archer:
                            normalizedAdditionalEffectsRating = 1f;
                            break;
                        case UnitType.HeavyWarrior:
                            normalizedAdditionalEffectsRating = 2f;
                            break;
                        case UnitType.LightWarrior:
                            normalizedAdditionalEffectsRating = 1.5f;
                            break;
                    }
                    break;
            }
            return normalizedAdditionalEffectsRating * _additionalEffectsWeight;
        }

        private float GetNormalizedAdditionalEffectsRatingForPush(Unit friendlyUnit)
        {
            float normalizedAdditionalEffectsRating = 0;
            var isEnemyArcherCloserValidFactor = false;
            var isPushActionValid = false;

            List<Tuple<int, int, CheckType>> coordinateTemplateForCheck = new List<Tuple<int, int, CheckType>>()
            {
                new(0, 1, CheckType.IsEnemyArcherOnGrid),
                new(0, -1, CheckType.IsEnemyArcherOnGrid),
                new(1, 1, CheckType.IsEnemyArcherOnGrid),
                new(1, -1, CheckType.IsEnemyArcherOnGrid),
                new(2, 0, CheckType.IsGridAvailableToMove),
                new(2, 1, CheckType.IsFriendlyWarriorUnitNotOnGrid),
                new(3, 1, CheckType.IsFriendlyWarriorUnitNotOnGrid),
                new(3, 0, CheckType.IsFriendlyWarriorUnitNotOnGrid),
                new(2, -1, CheckType.IsFriendlyWarriorUnitNotOnGrid),
                new(3, -1, CheckType.IsFriendlyWarriorUnitNotOnGrid)
            };

            var differencePositionVectorNormalized = (friendlyUnit.WorldPosition - _enemyUnit.WorldPosition).normalized;


            if (differencePositionVectorNormalized.x != 0)
            {
                foreach (var templateGridPositionOffset in coordinateTemplateForCheck)
                {
                    isPushActionValid = CheckAroundGridPositionWithTemplateIsValid(
                        templateGridPositionOffset.Item1 * (int) differencePositionVectorNormalized.x,
                        templateGridPositionOffset.Item2 * (int) differencePositionVectorNormalized.x,
                        templateGridPositionOffset.Item3);
                }
            }

            if (differencePositionVectorNormalized.z != 0)
            {
                foreach (var templateGridPositionOffset in coordinateTemplateForCheck)
                {
                    isPushActionValid = CheckAroundGridPositionWithTemplateIsValid(
                        templateGridPositionOffset.Item2 * (int) differencePositionVectorNormalized.z,
                        templateGridPositionOffset.Item1 * (int) differencePositionVectorNormalized.z,
                        templateGridPositionOffset.Item3);
                }
            }

            if (!isPushActionValid)
            {
                return normalizedAdditionalEffectsRating;
            }
            
            normalizedAdditionalEffectsRating = 1.1f;
            return normalizedAdditionalEffectsRating;

            bool CheckAroundGridPositionWithTemplateIsValid(int xOffset, int zOffset, CheckType checkType)
            {
                GridPosition testGridPosition = _enemyUnit.GetGridPosition() + new GridPosition(xOffset, zOffset);
                switch (checkType)
                {
                    case CheckType.IsEnemyArcherOnGrid:
                        var unitOnGrid = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                        if (unitOnGrid != null &&
                            LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition).IsUnitAnEnemy &&
                            unitOnGrid.UnitType == UnitType.Archer)
                        {
                            return isEnemyArcherCloserValidFactor = true;
                        }

                        return isEnemyArcherCloserValidFactor;
                    case CheckType.IsFriendlyWarriorUnitNotOnGrid:
                        if (!isEnemyArcherCloserValidFactor) return false;
                        unitOnGrid = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                        var isFriendlyWarriorUnitNotOnGrid = !(unitOnGrid != null &&
                                                               !LevelGrid.Instance
                                                                   .GetUnitAtGridPosition(testGridPosition)
                                                                   .IsUnitAnEnemy &&
                                                               unitOnGrid.UnitType != UnitType.Archer);
                        return isFriendlyWarriorUnitNotOnGrid && isPushActionValid;
                    case CheckType.IsGridAvailableToMove:
                        if (!isEnemyArcherCloserValidFactor) return false;
                        var isGridAvailableToMove = GridPositionValidator.IsGridPositionReachable(testGridPosition,
                            friendlyUnit.GetGridPosition(), GameGlobalConstants.ONE_GRID_MOVEMENT_COST);
                        return isGridAvailableToMove && isPushActionValid;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(checkType), checkType, null);
                }
            }
        }

        private float GetNormalizedCooldownRating(BaseAction attackAction)
        {
            float normalizedCooldownRating = 1;
            if (attackAction.HasCoolDown)
            {
                normalizedCooldownRating /= attackAction.FullCoolDownValue * _coolDownWeight;
            }

            return normalizedCooldownRating;
        }

        private float GetNormalizedActionCostRating(BaseAction attackAction)
        {
            float normalizedActionCostRating =
                (1 - attackAction.ActionPointCost / _enemyUnit.ActionPoints) * _actionPointCostWeight;
            return normalizedActionCostRating;
        }

        private float GetNormalizedDamageToHealthRating(BaseAction attackAction, Unit friendlyUnit)
        {
            float normalizedDamageToHealthRating = 0;
            if (attackAction.Damage != 0)
            {
                var rawNormalizedDamageToHealthRating =
                    (1 - Mathf.Abs(friendlyUnit.HealthPointsLeft - attackAction.Damage) / attackAction.Damage);
                normalizedDamageToHealthRating = Mathf.Clamp(rawNormalizedDamageToHealthRating, 0, 1) * _normalizedDamageWeight;
            }

            return normalizedDamageToHealthRating;
        }

        private float GetPlayerUnitHealthLeftRating(Unit friendlyUnit)
        {
            return (1 - friendlyUnit.HealthNormalised) * _healthLeftWeight;
        }

        private bool IsThereEnemyAroundFriendlyUnit(Unit friendlyUnit)
        {
            var friendlyUnitGridPosition = friendlyUnit.GetGridPosition();
            for (int i = -1; i < 2; i+=2)
            {
                for (int j = -1; j < 2; j+=2)
                {
                    var testedPosition = friendlyUnitGridPosition + new GridPosition(i, j);
                    if (LevelGrid.Instance.IsValidGridPosition(testedPosition))
                    {
                        if (GridPositionValidator.IsGridPositionWithEnemy(testedPosition, friendlyUnit))
                        {
                            return true;
                        };
                    }
                }
            }

            return false;
        }
    }
}