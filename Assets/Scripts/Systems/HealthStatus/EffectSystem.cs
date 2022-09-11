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
        public event EventHandler OnKnockDownStart;
        public event EventHandler OnKnockDownOver;
        
        public event EventHandler OnParalyzeStart;
        public event EventHandler OnParalyzeOver;
        
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
            OnKnockDownStart?.Invoke(this, EventArgs.Empty);
        }
        
        public void ParalyzeUnit()
        {
            _currentEffectsAndDurationLeftDict.TryAdd(EffectStatus.Paralyzed, _paralyzeDuration);
            OnParalyzeStart?.Invoke(this, EventArgs.Empty);
        }

        public bool IsKnockedDown(out int duration)
        {
            return _currentEffectsAndDurationLeftDict.TryGetValue(EffectStatus.KnockedDown, out duration);
        }
        public bool IsKnockedDown()
        {
            return _currentEffectsAndDurationLeftDict.TryGetValue(EffectStatus.KnockedDown, out var duration);
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
                    switch (effect)
                    {
                        case EffectStatus.None:
                            break;
                        case EffectStatus.NotHostile:
                            break;
                        case EffectStatus.KnockedDown:
                            OnKnockDownOver?.Invoke(this, EventArgs.Empty);
                            break;
                        case EffectStatus.Paralyzed:
                            OnParalyzeOver?.Invoke(this, EventArgs.Empty);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
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