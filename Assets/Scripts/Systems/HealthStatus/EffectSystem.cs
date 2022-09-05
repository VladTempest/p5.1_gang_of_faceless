using System;
using System.Collections.Generic;
using Editor.Scripts;
using Sirenix.Serialization;
using UnityEngine;

namespace Systems.HealthStatus
{
    [Serializable]
    public class EffectsAndDurationLeftDictionary : UnitySerializedDictionary<EffectStatus[], int>
    {
    }

    public class EffectSystem : MonoBehaviour
    {
        [SerializeField] private EffectsAndDurationLeftDictionary _currentEffectsAndDurationLeftDict;
    }
}