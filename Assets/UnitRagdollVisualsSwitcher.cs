using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitRagdollVisualsSwitcher : MonoBehaviour
{
    [SerializeField] private Transform _archerVisuals;
    [SerializeField] private Transform _heavyWarriorVisuals;
    [SerializeField] private Transform _lightWarriorVisuals;
    [SerializeField] private Transform _defaultVisuals;

    
    public void SetActiveArcherVisuals()
    {
        DeactivateDefaultVisuals();
        _archerVisuals.gameObject.SetActive(true);
    }
    
    public void SetActiveHeavyWarriorVisuals()
    {
        DeactivateDefaultVisuals();
        _heavyWarriorVisuals.gameObject.SetActive(true);
    }

    public void SetActiveLightWarriorVisuals()
    {
        DeactivateDefaultVisuals();
        _lightWarriorVisuals.gameObject.SetActive(true);
    }

    private void DeactivateDefaultVisuals()
    {
        _defaultVisuals.gameObject.SetActive(false);
    }
}
