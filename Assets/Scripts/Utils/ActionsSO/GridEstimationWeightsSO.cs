using System;
using System.Collections.Generic;
using Scripts.Unit;
using UnityEngine;

namespace Editor.Scripts.Utils
{
    [CreateAssetMenu(fileName = "WeigthsSO", menuName = "ScriptableObjects/WeigthsSO", order = 1)]
    public class GridEstimationWeightsSO  : ScriptableObject
    {
        public GridEstimationWeightsDictionary GridEstimationWeightsDictionary;
    }
    
    [Serializable]
    public class GridEstimationWeightsDictionary : UnitySerializedDictionary<UnitType, GridEstimationWeights>
    {
    }

}