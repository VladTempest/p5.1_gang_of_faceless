﻿using System;
using Editor.Scripts;
using Editor.Scripts.Utils;
using Scripts.Unit;
using UnityEngine;

namespace Scripts.Utils
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