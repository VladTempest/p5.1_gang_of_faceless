using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    private Action _onArrowHit;
    private Vector3 _targetPosition;
    private float _moveSpeed = 50f;

    private void Update()
    {
        var position = transform.position;
        Vector3 moveDirection = (_targetPosition - position).normalized;

        float distanceBeforeMoving = Vector3.Distance(position, _targetPosition);
        position += moveDirection * _moveSpeed * Time.deltaTime;
        transform.position = position;
        float distanceAfterMoving = Vector3.Distance(position, _targetPosition);
        if (distanceAfterMoving > distanceBeforeMoving)
        {
            transform.position = _targetPosition;
            Destroy(gameObject);
        }

    }

    public void Setup(Vector3 targetPosition, Action ArrowHitCallback)
    {
        _onArrowHit = ArrowHitCallback;
        _targetPosition = targetPosition;
    }

    private void OnDestroy()
    {
        _onArrowHit?.Invoke();
        _trailRenderer.transform.parent = null;
    }
}
