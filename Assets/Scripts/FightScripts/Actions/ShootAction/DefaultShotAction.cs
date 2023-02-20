using System;
using DefaultNamespace;
using GridSystems;
using Scripts.Unit;

namespace Actions
{
    public class DefaultShotAction : BaseShootAction
    {
        public event EventHandler<OnShootEventArgs> OnDefaultShot;
        
        private new void Start()
        {
            base.Start();
            if (!enabled) return;
            _archerAnimationEvents.DefaultShotCallback = () => TryToChangeState(State.Shooting);
        }

        public override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
        {
            if (!base.IsGridPositionValid(testGridPosition, unitGridPosition))
            {
                return false;
            }
            if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
            {
                return false;
            }

            if (!GridPositionValidator.IsPositionInsideActionCircleRange(MaxActionRange, testGridPosition, unitGridPosition, _minActionRange))
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
                HitCallback = Hit
            });
            base.Shoot();
        }
    }
}