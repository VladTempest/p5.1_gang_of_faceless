using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    [SerializeField] private TrailRenderer _trailRenderer;
    [SerializeField] private Transform _bulletHitVfxPrefab;
    private Vector3 _targetPosition;
    private float _moveSpeed = 200f;

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - transform.position).normalized;

        float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition);
        transform.position += moveDirection * _moveSpeed * Time.deltaTime;
        float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition);
        if (distanceAfterMoving > distanceBeforeMoving)
        {
            transform.position = _targetPosition;
            Destroy(gameObject);

            Instantiate(_bulletHitVfxPrefab, _targetPosition, Quaternion.identity);
        }

    }

    public void Setup(Vector3 targetPosition)
    {
        _targetPosition = targetPosition;
    }

    private void OnDestroy()
    {
        _trailRenderer.transform.parent = null;
    }
}
