using System;
using System.Collections.Generic;
using DefaultNamespace;
using Editor.Scripts;
using Editor.Scripts.Animation;
using GridSystems;
using Scripts.Unit;
using UnityEngine;

namespace Actions
{
    public class BackStabAction: BaseAction
    {
        public event EventHandler OnStartTeleporting;
        
        [SerializeField] private LightWarriorAnimationEvents _lightWarriorAnimationEvents;
        [SerializeField] private Transform _swordDamageSource;
        [SerializeField] private TrailRenderer _trailRenderer;
        private Dictionary<GridPosition, GridPosition> _firstValidTeleportGridPositionsAndEnemyGridPositions = new Dictionary<GridPosition, GridPosition>();
        private GridPosition _taragetPosition;
        private Unit _targetUnit;
        private float _timeForEnemyToRotate = 0.3f;
        private float _heightOfFog = 0.1f;

        private BackStabActionState CurrentState { get; set; } = BackStabActionState.Idle;

        
        private void Start()
        {
            base.Start();
            if (!enabled) return;
            _lightWarriorAnimationEvents.ActionTeleportCallback += ActionTeleportCallback;
            _lightWarriorAnimationEvents.ActionEffectCallback += ActionEffectCallback;
            _lightWarriorAnimationEvents.ActionFinishCallback += ActionFinishCallback;
            _lightWarriorAnimationEvents.BackStepCutWasMadeCallback += CutWasMadeCallback; 
        }

        private void CutWasMadeCallback()
        {
            _targetUnit.Damage(0, _swordDamageSource.position);
        }

        private void ActionFinishCallback()
        {
            TryToChangeState(BackStabActionState.Idle);
            ActionComplete();
        }

        private void ActionEffectCallback()
        {
            TryToChangeState(BackStabActionState.Stabbing);
        }

        private void ActionTeleportCallback()
        {
            TryToChangeState(BackStabActionState.Teleporting);
        }

        public override string GetActionName()
        {
            return "Back Stab";
        }

        public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
        {
            _taragetPosition = gridPosition;
            _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(_taragetPosition);
            CurrentState = BackStabActionState.Idle;
            ActionStart(onActionComplete);
            OnStartTeleporting?.Invoke(this, EventArgs.Empty);
            FXSpawner.Instance.InstantiateFog(transform.position);
            
        }

        private void Attack(GridPosition gridPosition)
        {
            _targetUnit.Damage(_damage, _swordDamageSource.position);
        }

        private void TeleportWithRotate(GridPosition gridPosition)
        {
            gameObject.transform.position = LevelGrid.Instance.GetWorldPosition(_firstValidTeleportGridPositionsAndEnemyGridPositions[gridPosition]);
            gameObject.transform.LookAt(LevelGrid.Instance.GetWorldPosition(gridPosition));
            _firstValidTeleportGridPositionsAndEnemyGridPositions.Clear();
        }

        protected override bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
        {
            if (_unit.EffectSystem.IsParalyzed(out var durationLeft))
            {
                return false;
            }
            
            if (!base.IsGridPositionValid(testGridPosition, unitGridPosition))
            {
                return false;
            }
            if (!GridPositionValidator.IsPositionInsideBoundaries(testGridPosition))
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

            if (!GridPositionValidator.HasTestGridPositionAvailableNeighbours(testGridPosition,
                   _firstValidTeleportGridPositionsAndEnemyGridPositions))
            {
                return false;
            }

            return true;
        }

        protected override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
        {
            return new EnemyAIAction(){actionValue = 0, gridPosition = new GridPosition()};
        }
        
        protected void TryToChangeState(BackStabActionState state)
        {
        
            switch (state)
            {
                case BackStabActionState.Idle:
                    if (CurrentState == BackStabActionState.Stabbing) CurrentState = state;
                    break;
                case BackStabActionState.Teleporting:
                    if (CurrentState == BackStabActionState.Idle) CurrentState = state;
                    FXSpawner.Instance.InstantiateFog(transform.position + new Vector3(0, _heightOfFog,0));
                    _trailRenderer.emitting = true;
                    TeleportWithRotate(_taragetPosition);
                    FXSpawner.Instance.InstantiateFog(transform.position + new Vector3(0, _heightOfFog,0));
                    break;
                case BackStabActionState.Stabbing:
                    if (CurrentState == BackStabActionState.Teleporting) CurrentState = state;
                    _trailRenderer.emitting = false;
                    Attack(_taragetPosition);
                    StartCoroutine(UnitRotator.RotateUnitToDirection(_targetUnit, _unit.WorldPosition, _timeForEnemyToRotate));
                    break;
            }
        }
    }
}