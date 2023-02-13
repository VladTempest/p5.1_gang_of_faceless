using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button[] _unitSwitchButtons;
    [SerializeField] private TextMeshProUGUI _turnNumberText;
    [SerializeField] private GameObject _enemyTurnVisualGameObject;
    [SerializeField] private GameObject _unitActionSystemUI;

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        UnitActionSystem.Instance.OnBusyChanged +=  UnitActionSystem_OnBusyChanged;
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnAndUnitSwitchButtonsVisibility();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusyActive)
    {
        UpdateEndTurnAndUnitSwitchButtonsVisibility(isBusyActive);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnAndUnitSwitchButtonsVisibility();
    }

    private void UpdateTurnText()
    {
        _turnNumberText.text = $"TURN {TurnSystem.Instance.CurrentTurnNumber}";
    }

    private void UpdateEnemyTurnVisual()
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn);
        _unitActionSystemUI.SetActive(TurnSystem.Instance.IsPlayerTurn);
    }

    private void UpdateEndTurnAndUnitSwitchButtonsVisibility()
    {
        foreach (var unitSwitchButton in _unitSwitchButtons)
        {
            unitSwitchButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn);   
        }
    }

    private void UpdateEndTurnAndUnitSwitchButtonsVisibility(bool isBusyActive)
    {
        foreach (var unitSwitchButton in _unitSwitchButtons)
        {
            unitSwitchButton.gameObject.SetActive(!isBusyActive);   
        }
    }
}
