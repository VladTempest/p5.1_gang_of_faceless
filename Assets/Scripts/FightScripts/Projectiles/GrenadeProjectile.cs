using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform _explodeVfxPrefab;
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private AnimationCurve _arcYAnimationCurve;
    [SerializeField] private AnimationCurve _arcYAnimationDistanceCorrectionCurve;
    [SerializeField] private AnimationCurve _speedAnimationCurve;

    [SerializeField] private float _damageAmount;
    
    private Vector3 _targetPosition;
    private float _moveSpeed = 20f;
    private Action _OnGrenadeBehaviourComplete;
    private float _totalDistance;
    private Vector3 _positionXZ;
    private float _startHeight;

    private void Update()
    {
        var distanceNormalizedBefore = GetDistanceNormalized(_positionXZ, _targetPosition, _totalDistance);
        float currentMoveSpeed = _speedAnimationCurve.Evaluate(distanceNormalizedBefore) * _moveSpeed;
        
        Vector3 moveDirection = (_targetPosition - _positionXZ).normalized;
        _positionXZ += moveDirection * currentMoveSpeed * Time.deltaTime;

        var distanceNormalizedAfter = GetDistanceNormalized(_positionXZ, _targetPosition, _totalDistance);
        float maxHeight = _totalDistance / 3f;
        float positionY = _arcYAnimationCurve.Evaluate(distanceNormalizedAfter) * _startHeight + _arcYAnimationDistanceCorrectionCurve.Evaluate(distanceNormalizedAfter) * maxHeight;
        transform.position = new Vector3(_positionXZ.x, positionY, _positionXZ.z);
        
        float reachedTargetDistance = .2f;
        if (Vector3.Distance(transform.position, _targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, damageRadius);

            foreach (var collider in colliderArray)
            {
                
                if (collider.TryGetComponent(out Unit targetUnit))
                {
                    targetUnit.Damage(_damageAmount, _targetPosition);
                }
                if (collider.TryGetComponent(out DestructibleCrate crate))
                {
                    crate.Damage(_targetPosition);
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            _trailRenderer.transform.parent = null;
            Instantiate(_explodeVfxPrefab, _targetPosition + Vector3.up, Quaternion.identity);
            Destroy(gameObject);

            _OnGrenadeBehaviourComplete?.Invoke();
        }
    }

    private float GetDistanceNormalized(Vector3 currentPosition, Vector3 targetPosition, float totalDistance)
    {
        float distance = Vector3.Distance(currentPosition, targetPosition);
        float distanceNormalized = 1 - distance / totalDistance;
        return distanceNormalized;
    }

    public void Setup(Transform throwStartPoint, GridPosition targetGridPosition, Action OnGrenadeBehaviourComplete, float damage)
    {
        _damageAmount = damage;
        
        _startHeight = throwStartPoint.position.y;
        _OnGrenadeBehaviourComplete = OnGrenadeBehaviourComplete;
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        _positionXZ = throwStartPoint.position;
        _totalDistance = Vector3.Distance(_targetPosition, transform.position);
    }
}
