using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSwitcherUI : MonoBehaviour
{
    [SerializeField] private Button _nextUnitButton;

    private void Start()
    {
        if (_nextUnitButton != null)_nextUnitButton.onClick.AddListener(ChooseNextUnit);
    }

    private void OnDestroy()
    {
        if (_nextUnitButton != null) _nextUnitButton.onClick.RemoveListener(ChooseNextUnit);
    }

    private void ChooseNextUnit()
    {
        UnitActionSystem.Instance.SelectNextUnit();
    }
}
