using System;
using System.Collections.Generic;
using System.Linq;
using GridSystems;
using Sirenix.Utilities;
using UnityEngine;

namespace Editor.Scripts.AI
{
    public class AIAttackActionData
    {
        public BaseAction AttackAction;
        public GridPosition TargetPosition;
        public Action OnActionComplete;
        public float ActionRating;

        public AIAttackActionData(BaseAction attackAction, GridPosition targetPosition, Action onActionComplete, float actionRating)
        {
            AttackAction = attackAction;
            TargetPosition = targetPosition;
            OnActionComplete = onActionComplete;
            ActionRating = actionRating;
        }

        public override string ToString()
        {
            return $"{AttackAction.name} on position {TargetPosition} with rating {ActionRating}";
        }
    }
    
    public class GridRatingEstimator
    {
        public GridRatingEstimator(Unit enemyUnit)
        {
            _enemyUnit = enemyUnit;
        }

        private Unit _enemyUnit;


        private float healthLeftWeight = 1;
        private float playerCharacterStatusWeight = 1;
        private float playerCharacterPathLengthWeight = 1;
        private float playerCharacterClassWeight = 1;
        private float allyInNeighbourGridWeight = 1;
        private float normilizedActionPointCostWeight = 1;
        private float coolDownWeight = 1;
        private float normilizedDamageWeight = 1;
        
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
                var orderedList=listOfBestActions.OrderByDescending(aiActionData =>
                    aiActionData.ActionRating); //ToDo: выделить в метод!
                var bestActionAiData = orderedList.First();
                Debug.LogWarning($"[Enemy AI] Resulting {bestActionAiData} for {_enemyUnit.name}");
                return bestActionAiData;
            }
            
            Debug.LogWarning($"[Enemy AI] Resulting best action IS NULL for {_enemyUnit.name}");
            return null;
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
                var orderedList = listOfAiActionDataForUnit.OrderByDescending(aiActionData => aiActionData.ActionRating);
                var bestActionAiData =  orderedList.First();
                Debug.LogWarning($"[Enemy AI] Rated action for friendly unit: {friendlyUnit.name} - {bestActionAiData} for {_enemyUnit.name}");
                return bestActionAiData;
            }
            Debug.LogWarning($"[Enemy AI] Rated action for friendly unit: {friendlyUnit.name} -IS NULL for {_enemyUnit.name}");
            return null;
        }

        private AIAttackActionData GetAIAttackActionData(BaseAction attackAction, Unit friendlyUnit, Action onActionComplete)
        {
            float resultingRating = 0;

            if (_enemyUnit.CanSpendActionPointToTakeAction(attackAction) && attackAction.IsGridPositionValid(friendlyUnit.GetGridPosition(), _enemyUnit.GetGridPosition()))
            {
                float playerUnitHealthLeftRating = GetPlayerUnitHealthLeftRating(friendlyUnit);

                float normalizedDamageToHealthRating = GetNormalizedDamageToHealthRating(attackAction, friendlyUnit);
                
                var normalizedActionCostRating = GetNormalizedActionCostRating(attackAction);







                resultingRating = normalizedActionCostRating + normalizedDamageToHealthRating + playerUnitHealthLeftRating;
                return new AIAttackActionData(attackAction, friendlyUnit.GetGridPosition(), onActionComplete,
                    resultingRating);
            }

            return null;
        }

        private float GetNormalizedActionCostRating(BaseAction attackAction)
        {
            float normalizedActionCostRating =
                (1 - attackAction.ActionPointCost / _enemyUnit.ActionPoints) * normilizedActionPointCostWeight;
            return normalizedActionCostRating;
        }

        private float GetNormalizedDamageToHealthRating(BaseAction attackAction, Unit friendlyUnit)
        {
            float normalizedDamageToHealthRating = 0;
            if (attackAction.Damage != 0)
            {
                var rawNormalizedDamageToHealthRating =
                    (1 - Mathf.Abs(friendlyUnit.HealthPointsLeft - attackAction.Damage) / attackAction.Damage);
                normalizedDamageToHealthRating = Mathf.Clamp(rawNormalizedDamageToHealthRating, 0, 1) * normilizedDamageWeight;
            }

            return normalizedDamageToHealthRating;
        }

        private float GetPlayerUnitHealthLeftRating(Unit friendlyUnit)
        {
            return (1 - friendlyUnit.HealthNormalised) * healthLeftWeight;
        }
    }
}