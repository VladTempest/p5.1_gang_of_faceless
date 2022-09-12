using System;
using DefaultNamespace;
using GridSystems;

namespace Actions
{
    public class ParalyzeShotAction : BaseShootAction
    {
        public event EventHandler<OnShootEventArgs> OnParalyzeShot;

        public override string GetActionName()
        {
            return "Paralyze";
        }

        private void Start()
        {
            _archerAnimationEvents.ParalyzeShotCallback = () => TryToChangeState(State.Shooting);
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
            OnParalyzeShot?.Invoke(this, new OnShootEventArgs
            {
                TargetUnit = _targetUnit,
                HitCallback = () =>
                {
                    Hit();
                    _targetUnit.EffectSystem.ParalyzeUnit();
                }
            });
            base.Shoot();
        }
    }
}