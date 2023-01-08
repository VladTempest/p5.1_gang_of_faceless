using System;
using UnityEngine;

namespace Editor.Scripts.Utils
{
    public static class Utils
    {
        public static void CallWithDelay(float delay, Action callback)
        {
            var gameObjectWithDelay = new GameObject();
            DelayUtil gameObjectComponent = gameObjectWithDelay.AddComponent<DelayUtil>();
            gameObjectComponent.StartCoroutineForDelayAndCallback(delay, callback);
        }
        
        public static void CallWithDelay(float delay, Action<Action> callback, Action actionParam)
        {
            var gameObjectWithDelay = new GameObject();
            DelayUtil gameObjectComponent = gameObjectWithDelay.AddComponent<DelayUtil>();
            gameObjectComponent.StartCoroutineForDelayAndCallback(delay, callback, actionParam);
        }
    }
}