using System;
using GridSystems;

namespace Editor.Scripts.AI
{
    public class AIAttackActionData
    {
        public BaseAction AttackAction;
        public GridPosition TargetPosition;
        public Action OnActionComplete;
        public float ActionRating;

        public AIAttackActionData(BaseAction attackAction, GridPosition targetPosition, Action onActionComplete,
            float actionRating)
        {
            AttackAction = attackAction;
            TargetPosition = targetPosition;
            OnActionComplete = onActionComplete;
            ActionRating = actionRating;
        }

        public override string ToString()
        {
            return $"{AttackAction.GetActionName()} on position {TargetPosition} with rating {ActionRating}";
        }
    }
}