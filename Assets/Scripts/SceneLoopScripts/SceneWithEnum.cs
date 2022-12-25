using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Editor.Scripts.SceneLoopScripts
{
    [Serializable]
    public struct SceneWithEnum
    {
        public ScenesEnum sceneEnumName;
        [FormerlySerializedAs("scenePath")] public string sceneName;
    }
}
