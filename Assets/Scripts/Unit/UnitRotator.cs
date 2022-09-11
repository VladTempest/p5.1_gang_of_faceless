using System.Collections;
using UnityEngine;

namespace Scripts.Unit
{
    public class UnitRotator
    {
        internal static IEnumerator RotateToDirection(Transform transform, Vector3 positionToLook, float timeToRotate)
        {
            if (transform == null) yield break;
            
            var startRotation = transform.rotation;
            var direction = positionToLook - transform.position;
            if (direction == Vector3.zero) yield break;
            
            var finalRotation = Quaternion.LookRotation(direction);
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeToRotate;
                if (transform == null) yield break;
                transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
                yield return null;
            }

            transform.rotation = finalRotation;
        }
        
        internal static IEnumerator RotateUnitToDirection(global::Unit unit, Vector3 positionToLook, float timeToRotate)
        {
            if (unit.EffectSystem.IsKnockedDown()) yield break;
            yield return RotateToDirection(unit.transform, positionToLook, timeToRotate);
        }
    }
}