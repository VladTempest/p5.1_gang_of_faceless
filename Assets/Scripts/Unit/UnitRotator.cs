using System.Collections;
using UnityEngine;

namespace Scripts.Unit
{
    public class UnitRotator
    {
        internal static IEnumerator RotateToDirection(Transform transform, Vector3 positionToLook, float timeToRotate)
        {
            var startRotation = transform.rotation;
            var direction = positionToLook - transform.position;
            var finalRotation = Quaternion.LookRotation(direction);
            var t = 0f;
            while (t <= 1f)
            {
                t += Time.deltaTime / timeToRotate;
                transform.rotation = Quaternion.Lerp(startRotation, finalRotation, t);
                yield return null;
            }

            transform.rotation = finalRotation;
        }
    }
}