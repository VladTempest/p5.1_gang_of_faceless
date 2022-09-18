using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler<OnDeadEventArgs> OnDead;
    public event EventHandler OnDamaged;
    public static event EventHandler OnAnyUnitDamaged;
    
    [SerializeField] private int _health = 100;
    private int _healthMax = 100;
    private Vector3 _lastDamageSourcePosition;
    private float _lastDamagedHealthAmountNormilised;


    public class OnDeadEventArgs : EventArgs
    {
        public readonly Vector3 sourceOfDeathPosition;
        public readonly float damageAmountNormalized;

        public OnDeadEventArgs(Vector3 sourceOfDeathPosition, float damageAmountNormalized)
        {
            this.sourceOfDeathPosition = sourceOfDeathPosition;
            this.damageAmountNormalized = damageAmountNormalized;

        }
    }
    
    private void Awake()
    {
        _healthMax = _health;
    }

    public void Damage(float damageAmount, Vector3 damageSourcePosition)
    {
        _health -= (int) damageAmount;
        _lastDamageSourcePosition = damageSourcePosition;
        _lastDamagedHealthAmountNormilised = (float) damageAmount / _healthMax;
        if (_health < 0)
        {
            _health = 0;
        }
        
        OnDamaged?.Invoke(this, EventArgs.Empty);
        OnAnyUnitDamaged?.Invoke(this, EventArgs.Empty);

        if (_health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        OnDead?.Invoke(this, new OnDeadEventArgs(_lastDamageSourcePosition, _lastDamagedHealthAmountNormilised));
    }

    public float GetNormalisedValueOfHealth()
    {
        return (float)_health / _healthMax;
    }
}
