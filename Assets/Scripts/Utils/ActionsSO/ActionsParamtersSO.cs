using System;
using System.Collections.Generic;
using UnityEngine;

namespace Editor.Scripts.Utils
{
    [CreateAssetMenu(fileName = "ActionsSO", menuName = "ScriptableObjects/ActionsSO", order = 1)]
    public class ActionsParamtersSO  : ScriptableObject
    {
        public ActionsParametersDictionary ActionsParametersDictionary;
    }
    
    [Serializable]
    public class ActionsParametersDictionary : UnitySerializedDictionary<ActionsEnum, ActionsParameters>
    {
    }

}