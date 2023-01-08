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
        
        public IEnumerator DelayAndCallback(float delay, Action<Action> callback, Action actionParam)
        {
            yield return new WaitForSeconds(delay);
            callback(actionParam);
            Destroy(gameObject);
        }

        public void StartCoroutineForDelayAndCallback(float delay, Action callback)
        {
            StartCoroutine(DelayAndCallback(delay, callback));
        }
        
        public void StartCoroutineForDelayAndCallback(float delay, Action<Action> callback, Action actionParam)
        {
            StartCoroutine(DelayAndCallback(delay, callback, actionParam));
        }
    }
}