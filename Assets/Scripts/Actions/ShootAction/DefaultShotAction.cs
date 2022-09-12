using System;
using DefaultNamespace;
using GridSystems;
using Scripts.Unit;

namespace Actions
{
    public class DefaultShotAction : BaseShootAction
    {
        public event EventHandler<OnShootEventArgs> OnDefaultShot;

        public override string GetActionName()
        {
            return "Default Shot";
        }

        private void Start()
        {
            _archerAnimationEvents.DefaultShotCallback = () => TryToChangeState(State.Shooting);
            //_archerAnimationEvents.EndDefaultShotCallback = () => TryToChangeState(State.Idle);
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

            if (!GridPositionValidator.IsPositionInsideActionCircleRange(ActionRange, testGridPosition, unitGridPosition, MinActionRange))
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

            if (!GridPositionValidator.IsGridPositionOnLineOfSight(testGridPosition, unitGridPosition,
                    _obstaclesLayerMask))
            {
                return false;
            }

            return true;
        }

        protected override void Shoot()
        {
            OnDefaultShot?.Invoke(this, new OnShootEventArgs
            {
                TargetUnit = _targetUnit,
                HitCallback = () =>
                {
                    Hit();
                }
            });
            base.Shoot();
        }
    }
}