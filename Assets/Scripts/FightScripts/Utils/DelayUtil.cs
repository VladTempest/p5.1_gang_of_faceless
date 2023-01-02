using System;
using System.Collections;
using UnityEngine;

namespace Editor.Scripts.Utils
{
    public class DelayUtil : MonoBehaviour
    {
        public IEnumerator DelayAndCallback(float delay, Action callback)
        {
            yield return new WaitForSeconds(delay);
            callback();
            Destroy(gameObject);
        }

        public void StartCoroutineForDelayAndCallback(float delay, Action callback)
        {
            StartCoroutine(DelayAndCallback(delay, callback));
        }
    }
}