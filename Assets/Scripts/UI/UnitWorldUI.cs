using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _actionPointText;
    [SerializeField] private Unit _unit;
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private HealthSystem _healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
        _healthSystem.OnDamaged += Unit_OnDamaged;
        UpdateActionPointsText();
        UpdateHealthBar();
    }

    private void Unit_OnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }

    private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPointsText();
    }

    private void UpdateActionPointsText()
    {
        _actionPointText.text = _unit.ActionPoints.ToString();
    }

    private void UpdateHealthBar()
    {
        _healthBarImage.fillAmount = _healthSystem.GetNormalisedValueOfHealth();
    }
    
    
    
}
