using System;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts;
using Sirenix.Serialization;
using UnityEngine;

namespace Systems.HealthStatus
{
    [Serializable]
    public class EffectsAndDurationLeftDictionary : UnitySerializedDictionary<EffectStatus, int>
    {
    }

    public class EffectSystem : MonoBehaviour
    {
        [SerializeField] private EffectsAndDurationLeftDictionary _currentEffectsAndDurationLeftDict;
        [SerializeField] private int _knockDownDuration = 2;
        [SerializeField] private int _paralyzeDuration = 2;

        private void Start()
        {
            TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        }

        private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
        {
            DecreaseDurationOfEffects();
        }

        public void KnockDownUnit()
        {
            _currentEffectsAndDurationLeftDict.TryAdd(EffectStatus.KnockedDown, _knockDownDuration);
        }
        
        public void ParalyzeUnit()
        {
            _currentEffectsAndDurationLeftDict.TryAdd(EffectStatus.Paralyzed, _paralyzeDuration);
        }

        public bool IsKnockedDown(out int duration)
        {
            return _currentEffectsAndDurationLeftDict.TryGetValue(EffectStatus.KnockedDown, out duration);
        }
        
        public bool IsParalyzed(out int duration)
        {
            return _currentEffectsAndDurationLeftDict.TryGetValue(EffectStatus.Paralyzed, out duration);
        }

        private void DecreaseDurationOfEffects()
        {
            List<EffectStatus> effectList = _currentEffectsAndDurationLeftDict.Keys.ToList();
            foreach (var effect in effectList)
            {
                if (_currentEffectsAndDurationLeftDict[effect] - 1 == 0)
                {
                    _currentEffectsAndDurationLeftDict.Remove(effect);
                    continue;
                }
                _currentEffectsAndDurationLeftDict[effect] -= 1;
            }
        }

        private void OnDestroy()
        {
            TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        }
    }
}