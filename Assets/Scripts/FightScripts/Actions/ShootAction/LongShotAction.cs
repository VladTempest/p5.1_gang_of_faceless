using System;
using DefaultNamespace;
using GridSystems;

namespace Actions
{
    public class LongShotAction : BaseShootAction
    {
        public event EventHandler<OnShootEventArgs> OnLongShot;
        
        private new void Start()
        {
            base.Start();
            if (!enabled) return;
            _archerAnimationEvents.LongShotCallback = () => TryToChangeState(State.Shooting);
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

            return true;
        }
        
        protected override void Shoot()
        {
            OnLongShot?.Invoke(this, new OnShootEventArgs
            {
                TargetUnit = _targetUnit,
                HitCallback = () => Hit()

            });
            base.Shoot();
        }
    }
}