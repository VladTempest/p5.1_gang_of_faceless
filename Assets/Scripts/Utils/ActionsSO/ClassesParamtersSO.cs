using System;
using System.Collections.Generic;
using Scripts.Unit;
using UnityEngine;

namespace Editor.Scripts.Utils
{
    [CreateAssetMenu(fileName = "ClassesSO", menuName = "ScriptableObjects/ClassesSO", order = 1)]
    public class ClassesParamtersSO  : ScriptableObject
    {
        public ClassesParametersDictionary ClassesParametersDictionary;
    }
    
    [Serializable]
    public class ClassesParametersDictionary : UnitySerializedDictionary<UnitType, ClassesParameters>
    {
    }

}