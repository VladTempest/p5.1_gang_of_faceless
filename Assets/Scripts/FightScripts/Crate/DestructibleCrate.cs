using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using SoundSystemScripts;
using UnityEngine;

public class DestructibleCrate : MonoBehaviour, IInteractable
{
    public static event EventHandler OnAnyCrateDestroyed;

    [SerializeField] private Transform _destroyedCratePrefab;

    public GridPosition GridPosition { get; private set; }

    
    private void Start()
    {
        GridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(GridPosition, this);
    }



    public void Damage(Vector3 destructionSourcePosition, float damage = 200f)
    {
        Transform crateTransform = Instantiate(_destroyedCratePrefab, transform.position, Quaternion.identity);
        
        var destructionDirection = (transform.position - destructionSourcePosition).normalized;
        var explosionPosition = transform.position - destructionDirection;
        
        
        ApplyExplosionToChildren(crateTransform, damage, explosionPosition, 4f);
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

    public void Interact(Action onActionComplete)
    {
        Damage(transform.position, 100f);
        SoundtrackPlayerWrapper.PlayBombExplosionSound(transform);
        SoundtrackPlayerWrapper.PlaySwordHitSound(HeavyWarriorActionEnum.Knockdown, transform);
        onActionComplete.Invoke();
    }

    private void OnDestroy()
    {
        
    }
}
