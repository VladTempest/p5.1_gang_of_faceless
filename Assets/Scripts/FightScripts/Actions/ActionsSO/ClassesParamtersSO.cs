using System;
using System.Collections.Generic;
using Editor.Scripts;
using Editor.Scripts.Utils;
using Scripts.Unit;
using UnityEngine;

namespace Scripts.Utils
{
    [CreateAssetMenu(fileName = "ClassesSO", menuName = "ScriptableObjects/ClassesSO", order = 1)]
    public class ClassesParamtersSO  : ScriptableObject
    {
        public SerializableDictionary<UnitType,ClassesParameters> ClassesParametersDictionary;
    }
}