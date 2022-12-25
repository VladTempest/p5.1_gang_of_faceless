using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour
{
    public static event EventHandler OnAnyCrateDestroyed;

    [SerializeField] private Transform _destroyedCratePrefab;

    public GridPosition GridPosition { get; private set; }

    
    private void Start()
    {
        GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
    }
    
    

    public void Damage(Vector3 destructionSourcePosition)
    {
        Transform crateTransform = Instantiate(_destroyedCratePrefab, transform.position, Quaternion.identity);
        
        var destructionDirection = (transform.position - destructionSourcePosition).normalized;
        var explosionPosition = transform.position - destructionDirection;
        
        
        ApplyExplosionToChildren(crateTransform, 200f, explosionPosition, 4f);
        Destroy(gameObject);
        
        OnAnyCrateDestroyed?.Invoke(this, EventArgs.Empty);
    }
    
    private void ApplyExplosionToChildren(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody))
            {
                childRigidBody.AddExplosionForce(explosionForce, explosionPosition,explosionRange);
            }
            ApplyExplosionToChildren(child, explosionForce, explosionPosition,explosionRange);
        }
    }
}
