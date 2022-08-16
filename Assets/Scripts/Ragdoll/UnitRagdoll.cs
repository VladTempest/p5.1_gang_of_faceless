using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform _unitRootBone;
    [SerializeField] private float _explosionBaseValue;
    [SerializeField] private float _explosionBaseRange;
    
    public void Setup(Transform originalRootBone, Vector3 positionOfInfluencerBody, float damageAmountNormalized)
    {
        MatchAllChildTransform(originalRootBone, _unitRootBone);
        
        var influenceDirection = (transform.position - positionOfInfluencerBody).normalized;
        var explosionPosition = transform.position - influenceDirection;
        
        ApplyExplosionToRagdoll(_unitRootBone, _explosionBaseValue * damageAmountNormalized, explosionPosition, _explosionBaseRange);
    }

    private void MatchAllChildTransform(Transform root, Transform clone)
    {
        foreach (Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if (cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                
                MatchAllChildTransform(child,cloneChild);
            }
        }
    }

    private void ApplyExplosionToRagdoll(Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent<Rigidbody>(out Rigidbody childRigidBody))
            {
                childRigidBody.AddExplosionForce(explosionForce, explosionPosition,explosionRange);
            }
            ApplyExplosionToRagdoll(child, explosionForce, explosionPosition,explosionRange);
        }
    }
}
