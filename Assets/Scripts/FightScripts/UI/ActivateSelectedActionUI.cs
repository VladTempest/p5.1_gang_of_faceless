using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using SoundSystemScripts;
using UnityEngine;
using UnityEngine.UI;

public class ActivateSelectedActionUI : MonoBehaviour
{
    [SerializeField] private Button _activateButton;
    void Start()
    {
        UnitActionSystem.Instance.OnSelectedPositionChanged += UnitActionSystem_OnSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        _activateButton.onClick.AddListener(ActivateSelectedAction);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn);
    }

    private void ActivateSelectedAction()
    {
        UnitActionSystem.Instance.ActivateSelectedActionOnSelectedPosition();
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, OnSelectedPositionChangedArgs e)
    {
        if (e.NewGridPosition == new GridPosition(0, 0))
        {
            if (_activateButton.interactable)
            {
                PlayUISound();
            }
            else return;
            
            _activateButton.interactable = false;
        }
        else
        {
            _activateButton.interactable = true;
            PlayUISound();
        }
        
    }

    private static void PlayUISound()
    {
        SoundtrackPlayerWrapper.PlayUISoundtrack(TypeOfSFXByItsNature.UI_Click_long);
    }


    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedPositionChanged -= UnitActionSystem_OnSelectedUnitChanged;
        _activateButton.onClick.RemoveListener(ActivateSelectedAction);
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }
}
