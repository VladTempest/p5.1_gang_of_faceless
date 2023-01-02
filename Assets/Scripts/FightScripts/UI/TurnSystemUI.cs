using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    [SerializeField] private Button _endTurnButton;
    [SerializeField] private TextMeshProUGUI _turnNumberText;
    [SerializeField] private GameObject _enemyTurnVisualGameObject;
    [SerializeField] private GameObject _unitActionSystemUI;

    private void Start()
    {
        _endTurnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        UnitActionSystem.Instance.OnBusyChanged +=  UnitActionSystem_OnBusyChanged;
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusyActive)
    {
        ChangeStateOfEndTurnButton(isBusyActive);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
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

    private void UpdateEndTurnButtonVisibility()
    {
        _endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn);
    }

    private void ChangeStateOfEndTurnButton(bool isBusyActive)
    {
        _endTurnButton.interactable = !isBusyActive;
    }
}
