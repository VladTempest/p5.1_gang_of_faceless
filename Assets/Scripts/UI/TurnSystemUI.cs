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

    private void Start()
    {
        _endTurnButton.onClick.AddListener(() => TurnSystem.Instance.NextTurn());

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
    }

    private void UpdateTurnText()
    {
        _turnNumberText.text = $"TURN {TurnSystem.Instance.GetCurrentTurnNumber()}";
    }

    private void UpdateEnemyTurnVisual()
    {
        _enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }

    private void UpdateEndTurnButtonVisibility()
    {
        _endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
