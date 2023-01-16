using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedVisual;
    [SerializeField] private TextMeshProUGUI _chargesLeft;
    [FormerlySerializedAs("_apActionDescription")] [FormerlySerializedAs("_apDamageRange")] [SerializeField] private TextMeshProUGUI _actionDescription;
    [SerializeField] private Image _coolDownMask;
    [SerializeField] private GameObject _actionDescriptionUI;

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
        if (!(_baseAction is MoveAction))
        {
            var damageString = _baseAction.Damage == 0 ? "" : $"It deals  <b><i>{_baseAction.Damage}</i></b> damage."+"\n";
            var actionCostString =_baseAction.GetActionPointCost() ==0 ? "" : $"Action costs  <b><i>{_baseAction.GetActionPointCost()}</i></b> points" +"\n";
            var rangeString = _baseAction.MaxActionRange <= 1 ? "" : $"It has range from  <b><i>{_baseAction.MinActionRange}</i></b> to  <b><i>{_baseAction.MaxActionRange}</i></b> cells." +"\n";
            var cooldownString = !_baseAction.HasCoolDown ? "" : $"Time to reload is  <b><i>{_baseAction.FullCoolDownValue}</i></b> turns." +"\n";
            var chargesString = !_baseAction.IsChargeable ? "" : $"Action has  <b><i>{_baseAction.MaxCharges}</i></b> charges." +"\n";
            
            _actionDescription.text = $"<b>{_baseAction.GetActionName()}</b>" + "\n"+ $"{_baseAction.Description}"+"\n"+"\n"+
                                        actionCostString+
                                        damageString+
                                        rangeString+
                                        cooldownString+
                                        chargesString;
        }
        else
        {
            _actionDescription.text =  $"<b>{_baseAction.GetActionName()}</b>" + "\n"+ $"{_baseAction.Description}";
        }

        _button.onClick.AddListener(() => UnitActionSystem.Instance.SetSelectedAction(baseAction));
        SetUpActionVisuals();
        _baseAction.OnActionStatusUpdate += BaseAction_OnActionStatusUpdate;
        UnitActionSystem.Instance.OnSelectedActionChanged += BaseAction_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedUnitChanged += BaseAction_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += BaseAction_OnActionStarted;

    }

    

    private void BaseAction_OnActionStarted(object sender, EventArgs e)
    {
       if (!_baseAction.Unit.IsUnitAnEnemy && TurnSystem.Instance.IsPlayerTurn)
       {
            UpdateButtonInteractivity();
            UpdateCoolDownVisuals();
       }
       
       HideActionDescriptionUI();
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
        
        
        HideActionDescriptionUI();
        UpdateButtonInteractivity();

    }

    private void UpdateButtonInteractivity()
    {
        _button.interactable = _baseAction.IsAvailable;
    }

    private void UpdateCoolDownVisuals()
    {
        if (_coolDownMask == null) return;
        if (!_coolDownMask.gameObject.activeInHierarchy) return;
        _coolDownMask.fillAmount = _baseAction.CoolDownLeftNormalized;
    }

    private void UpdateChargesVisuals()
    {
        _chargesLeft.text = $"<b><i>{_baseAction.ChargesLeft}</b></i> charges left";
    }

    private void OnDestroy()
    {
        if (_baseAction!=null) _baseAction.OnActionStatusUpdate -= BaseAction_OnActionStatusUpdate;
        UnitActionSystem.Instance.OnSelectedActionChanged -= BaseAction_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= BaseAction_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged -= BaseAction_OnActionStarted;
    }

    private void ShowActionDescriptionUI()
    {
        _actionDescriptionUI.SetActive(true);
    }
    
    private void HideActionDescriptionUI()
    {
        _actionDescriptionUI.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        ShowActionDescriptionUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        HideActionDescriptionUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        ShowActionDescriptionUI();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        HideActionDescriptionUI();
    }
}
