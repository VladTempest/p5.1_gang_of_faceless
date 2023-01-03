using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSwitcherUI : MonoBehaviour
{
    [SerializeField] private Button _previousUnitButton;
    [SerializeField] private Button _nextUnitButton;

    private void Start()
    {
        _nextUnitButton.onClick.AddListener(ChooseNextUnit);
        _previousUnitButton.onClick.AddListener(ChoosePreviousUnit);
    }

    private void OnDestroy()
    {
        _nextUnitButton.onClick.RemoveListener(ChooseNextUnit);
        _previousUnitButton.onClick.RemoveListener(ChoosePreviousUnit);
    }

    private void ChooseNextUnit()
    {
        UnitActionSystem.Instance.SelectNextUnit();
    }

    private void ChoosePreviousUnit()
    {
        UnitActionSystem.Instance.SelectPreviousUnit();
    }
}
