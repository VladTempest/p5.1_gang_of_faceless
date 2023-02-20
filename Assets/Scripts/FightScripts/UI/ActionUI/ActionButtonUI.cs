using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private TextMeshProUGUI _textMeshPro;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedVisual;
    
    [SerializeField] private LocalizeStringEvent _chargesLeft;
    private LocalizedString _chargesLeftStringrReference;
    private TextMeshProUGUI _chargesLeftText;
    
    [FormerlySerializedAs("_apActionDescription")] [FormerlySerializedAs("_apDamageRange")] [SerializeField] private TextMeshProUGUI _actionDescription;
    [SerializeField] private Image _coolDownMask;
    [SerializeField] private GameObject _actionDescriptionUI;

    private BaseAction _baseAction;
    
    private float _pointerDownTime;
    private Coroutine _pointerDownCoroutine;
    private float _pointerDownDuration;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void BaseAction_OnActionStatusUpdate(object sender, EventArgs e)
    {
        if (_baseAction.IsChargeable) UpdateChargesVisuals();
        if (_baseAction.HasCoolDown) UpdateCoolDownVisuals();
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
            InitialSetUpChargesString();
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
        _chargesLeftStringrReference.Arguments[0] = _baseAction.ChargesLeft;
        ConvenientLogger.Log(nameof(ActionButtonUI), GlobalLogConstant.IsActionLogEnabled, "Charges left: " + _chargesLeftStringrReference.Arguments[0].ToString());
        _chargesLeftStringrReference.RefreshString();
    }

    private bool InitialSetUpChargesString()
    {
        _chargesLeftStringrReference = _chargesLeft.StringReference;
        _chargesLeftText = _chargesLeft.GetComponent<TextMeshProUGUI>();


        if (_chargesLeftText == null) return true;
        _chargesLeftText.text = _baseAction.ChargesLeft.ToString();

        if (_chargesLeftStringrReference.Arguments == null)
        {
            _chargesLeftStringrReference.Arguments = new object[] {_baseAction.ChargesLeft};
            _chargesLeftStringrReference.StringChanged += OnChargesLeftStringrReferenceOnStringChanged;
        }

        return false;
    }

    private void OnChargesLeftStringrReferenceOnStringChanged(string value)
    {
        _chargesLeftText.text = value;
    }

    private void OnDestroy()
    {
        if (_baseAction!=null) _baseAction.OnActionStatusUpdate -= BaseAction_OnActionStatusUpdate;
        UnitActionSystem.Instance.OnSelectedActionChanged -= BaseAction_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= BaseAction_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged -= BaseAction_OnActionStarted;
        if (_chargesLeftStringrReference != null) _chargesLeftStringrReference.StringChanged -= OnChargesLeftStringrReferenceOnStringChanged;
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
        HandleEventDurationWithCoroutine(ShowActionDescriptionUI);
    }

    private void HandleEventDurationWithCoroutine(Action actionToPerform)
    {
        if (_pointerDownCoroutine == null)
        {
            _pointerDownTime = Time.time;
            _pointerDownCoroutine = StartCoroutine(GetPointerDownDuration(actionToPerform));
        }
    }

    private void TryStopEventCoroutineAndResetTime()
    {
        if (_pointerDownCoroutine != null)
        {
            StopCoroutine(_pointerDownCoroutine);
            _pointerDownCoroutine = null;
            _pointerDownDuration = 0;
            _pointerDownTime = 0;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (Application.platform == RuntimePlatform.Android) return;
        TryStopEventCoroutineAndResetTime();
        HideActionDescriptionUI();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        HandleEventDurationWithCoroutine(ShowActionDescriptionUI);
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (Application.platform != RuntimePlatform.Android) return;
        TryStopEventCoroutineAndResetTime();
        HideActionDescriptionUI();
    }
    
    IEnumerator GetPointerDownDuration(Action actionToPerform)
    {
        while (_pointerDownDuration < GameGlobalConstants.POINTER_DOWN_DURATION_TO_SHOW_ACTION_DESCRIPTION)
        {
            _pointerDownDuration = Time.time - _pointerDownTime;
            yield return null;
        }
        
        actionToPerform?.Invoke();
        TryStopEventCoroutineAndResetTime();
    }
}
