using System;
using System.Collections.Generic;
using Editor.Scripts;
using Editor.Scripts.Utils;
using UnityEngine;

namespace Scripts.Utils
{
    [CreateAssetMenu(fileName = "ActionsSO", menuName = "ScriptableObjects/ActionsSO", order = 1)]
    public class ActionsParamtersSO  : ScriptableObject
    {
        public SerializableDictionary<ActionsEnum, ActionsParameters> ActionsParametersDictionary;
    }
}