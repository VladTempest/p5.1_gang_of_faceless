using System;
using System.Collections;
using System.Collections.Generic;
using Scripts.Unit;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private UnitRagdoll _ragdoll;
    [SerializeField] private Transform _originalRootBone;
    [SerializeField] private Unit _unit;
    private HealthSystem _healthSystem;
    private UnitRagdollVisualsSwitcher _visualsRagdollSwitcher;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _visualsRagdollSwitcher = _ragdoll.GetComponentInChildren<UnitRagdollVisualsSwitcher>();

        _healthSystem.OnDead += HealthSystem_OnDead;
    }

    private void HealthSystem_OnDead(object sender, HealthSystem.OnDeadEventArgs e)
    {
        _ragdoll.gameObject.SetActive(true);
        _ragdoll.transform.parent = null;
        _ragdoll.transform.position = transform.position;
        _ragdoll.transform.rotation = transform.rotation;
        _ragdoll.Setup(_originalRootBone, e.sourceOfDeathPosition, e.damageAmountNormalized);
    }

    private void Start()
    {
        if (_visualsRagdollSwitcher == null) return;
            SwitchRagdollVisuals();
    }

    private void SwitchRagdollVisuals()
    {
        switch (_unit.UnitType)
        {
            case UnitType.None:
                break;
            case UnitType.Archer:
                _visualsRagdollSwitcher.SetActiveArcherVisuals();
                break;
            case UnitType.HeavyWarrior:
                _visualsRagdollSwitcher.SetActiveHeavyWarriorVisuals();
                break;
            case UnitType.LightWarrior:
                _visualsRagdollSwitcher.SetActiveLightWarriorVisuals();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
