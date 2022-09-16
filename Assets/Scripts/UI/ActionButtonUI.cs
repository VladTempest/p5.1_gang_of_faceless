using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedVisual;
    [SerializeField] private TextMeshProUGUI _chargesLeft;
    [SerializeField] private Image _coolDownMask;

    private BaseAction _baseAction;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }
    
    private void BaseAction_OnActionStatusUpdate(object sender, EventArgs e)
    {
        UpdateChargesVisuals();
        UpdateCoolDownVisuals();
        UpdateButtonInteractivity();
    }

    public void SetBaseAction(BaseAction baseAction)
    {
        _textMeshPro.text = baseAction.GetActionName().ToUpper();
        _baseAction = baseAction;
        _button.onClick.AddListener(() => UnitActionSystem.Instance.SetSelectedAction(baseAction));
        SetUpActionVisuals();
        _baseAction.OnActionStatusUpdate += BaseAction_OnActionStatusUpdate;
    }
    
    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        _selectedVisual.SetActive(selectedBaseAction == _baseAction);
    }

    private void SetUpActionVisuals()
    {
        if (_baseAction.IsChargeable)
        {
            _chargesLeft.gameObject.SetActive(true);
            UpdateChargesVisuals();
        }

        if (_baseAction.HasCoolDown)
        {
            _coolDownMask.gameObject.SetActive(true);
            UpdateCoolDownVisuals();
        }

        UpdateButtonInteractivity();

    }

    private void UpdateButtonInteractivity()
    {
        _button.interactable = _baseAction.IsAvailable;
    }

    private void UpdateCoolDownVisuals()
    {
        if (!_coolDownMask.gameObject.activeInHierarchy) return;
        _coolDownMask.fillAmount = _baseAction.CoolDownLeftNormalized;
    }

    private void UpdateChargesVisuals()
    {
        _chargesLeft.text = $"{_baseAction.ChargesLeft} charges";
    }

    private void OnDestroy()
    {
        if (_baseAction!=null) _baseAction.OnActionStatusUpdate -= BaseAction_OnActionStatusUpdate;
    }
}
