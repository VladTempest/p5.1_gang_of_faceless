using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedVisual;

    private BaseAction _baseAction;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void SetBaseAction(BaseAction baseAction)
    {
        _textMeshPro.text = baseAction.GetActionName().ToUpper();
        _baseAction = baseAction;
        _button.onClick.AddListener(() => UnitActionSystem.Instance.SetSelectedAction(baseAction));
    }
    
    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        _selectedVisual.SetActive(selectedBaseAction == _baseAction);
    }
    
}
