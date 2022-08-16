using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private Transform _ragdollPrefab;
    [SerializeField] private Transform _originalRootBone;
    private HealthSystem _healthSystem;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();

        _healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, HealthSystem.OnDeadEventArgs e)
    {
        Transform ragdoll = Instantiate(_ragdollPrefab, transform.position, transform.rotation);
        UnitRagdoll unityRagdool = ragdoll.GetComponent<UnitRagdoll>();
        unityRagdool.Setup(_originalRootBone, e.sourceOfDeathPosition, e.damageAmountNormalized);
    }
}
