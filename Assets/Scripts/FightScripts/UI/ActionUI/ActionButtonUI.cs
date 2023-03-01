using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using Editor.Scripts.Utils;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [FormerlySerializedAs("_textMeshPro")] [SerializeField] private TextMeshProUGUI _actionButtonText;
    [SerializeField] private LocalizeStringEvent _actionNameStringEvent;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _selectedVisual;

    [Header("String References")]
    [SerializeField] private LocalizeStringEvent _chargesLeft;
    private LocalizedString _chargesLeftStringrReference;
    private TextMeshProUGUI _chargesLeftText;
    
    [SerializeField] private LocalizeStringEvent _damageString;
    private LocalizedString _damageStringReference;
    private TextMeshProUGUI _damageStringText;
    
    [SerializeField] private LocalizeStringEvent _rangeString;
    private LocalizedString _rangeStringReference;
    private TextMeshProUGUI _rangeStringText;

    [SerializeField] private LocalizeStringEvent _coolDownString;
    private LocalizedString _coolDownStringReference;
    private TextMeshProUGUI _coolDownStringText;
    
    [SerializeField] private LocalizeStringEvent _actionCostString;
    private LocalizedString _actionCostStringReference;
    private TextMeshProUGUI _actionCostStringText;
    
    [SerializeField] private LocalizeStringEvent _chargesString;
    private LocalizedString _chargesStringReference;
    private TextMeshProUGUI _chargesStringText;
    
    [SerializeField] private LocalizeStringEvent _nameString;
    private LocalizedString _nameStringReference;
    private TextMeshProUGUI _nameStringText;

    [SerializeField] private LocalizeStringEvent _descriptionString;
    private LocalizedString _descriptionStringReference;
    private TextMeshProUGUI _descriptionStringText;
    
    private Dictionary<LocalizeStringEvent, Tuple<LocalizedString, TextMeshProUGUI>> _localizeStringEventsDictionary;
    
    [Header("Action Description UI")]
    [FormerlySerializedAs("_apActionDescription")] [FormerlySerializedAs("_apDamageRange")] [SerializeField] private TextMeshProUGUI _actionDescription;
    [SerializeField] private GameObject _actionDescriptionUI;
    
    [SerializeField] private Image _coolDownMask;

    private BaseAction _baseAction;
    
    private float _pointerDownTime;
    private Coroutine _pointerDownCoroutine;
    private float _pointerDownDuration;

    private void Awake()
    {
        _button = GetComponent<Button>();
        
        _localizeStringEventsDictionary = new Dictionary<LocalizeStringEvent, Tuple<LocalizedString, TextMeshProUGUI>>
        {
            {_chargesLeft, new Tuple<LocalizedString, TextMeshProUGUI>(_chargesLeftStringrReference = _chargesLeft.StringReference, _chargesLeftText = _chargesLeft.GetComponent<TextMeshProUGUI>())},
            {_damageString, new Tuple<LocalizedString, TextMeshProUGUI>(_damageStringReference = _damageString.StringReference, _damageStringText = _damageString.GetComponent<TextMeshProUGUI>())},
            {_rangeString, new Tuple<LocalizedString, TextMeshProUGUI>(_rangeStringReference = _rangeString.StringReference, _rangeStringText = _rangeString.GetComponent<TextMeshProUGUI>())},
            {_coolDownString, new Tuple<LocalizedString, TextMeshProUGUI>(_coolDownStringReference = _coolDownString.StringReference, _coolDownStringText = _coolDownString.GetComponent<TextMeshProUGUI>())},
            {_actionCostString, new Tuple<LocalizedString, TextMeshProUGUI>(_actionCostStringReference = _actionCostString.StringReference, _actionCostStringText = _actionCostString.GetComponent<TextMeshProUGUI>())},
            {_chargesString, new Tuple<LocalizedString, TextMeshProUGUI>(_chargesStringReference = _chargesString.StringReference, _chargesStringText = _chargesString.GetComponent<TextMeshProUGUI>())},
            {_nameString, new Tuple<LocalizedString, TextMeshProUGUI>(_nameStringReference = _nameString.StringReference, _nameStringText = _nameString.GetComponent<TextMeshProUGUI>())},
            {_descriptionString, new Tuple<LocalizedString, TextMeshProUGUI>(_descriptionStringReference = _descriptionString.StringReference, _descriptionStringText = _descriptionString.GetComponent<TextMeshProUGUI>())},
        };
    }

    private void BaseAction_OnActionStatusUpdate(object sender, EventArgs e)
    {
        if (_baseAction.IsChargeable) UpdateChargesVisuals();
        if (_baseAction.HasCoolDown) UpdateCoolDownVisuals();
        UpdateButtonInteractivity();
    }

    
    public void SetBaseAction(BaseAction baseAction)
    {
        _baseAction = baseAction;

        SetUpLocalizesStringAccordinglyToType(_actionNameStringEvent, _baseAction.GetActionName());
        SetUpLocalizesStringAccordinglyToType(_nameString, _baseAction.GetActionName());
        SetUpLocalizesStringAccordinglyToType(_descriptionString, _baseAction.Description);

        
            InitialSetUpLocalizedString(_damageString, _baseAction.Damage);
            if (_baseAction is not MoveAction) InitialSetUpLocalizedString(_actionCostString, _baseAction.GetActionPointCost());
            InitialSetUpLocalizedString(_rangeString, new object[] {_baseAction.MinActionRange, baseAction.MaxActionRange});
            InitialSetUpLocalizedString(_coolDownString, _baseAction.FullCoolDownValue);
            InitialSetUpLocalizedString(_chargesString, _baseAction.MaxCharges);
        

        CreateDescription();

        if (LocalizationSettings.Instance != null) LocalizationSettings.Instance.OnSelectedLocaleChanged += OnSelectedLocaleChanged;
        _button.onClick.AddListener(() => UnitActionSystem.Instance.SetSelectedAction(_baseAction));
        SetUpActionVisuals();
        _baseAction.OnActionStatusUpdate += BaseAction_OnActionStatusUpdate;
        UnitActionSystem.Instance.OnSelectedActionChanged += BaseAction_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedUnitChanged += BaseAction_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged += BaseAction_OnActionStarted;

    }

    private void CreateDescription()
    {
        if (_baseAction is not MoveAction)
        {
            var damageString = _baseAction.Damage == 0
                ? ""
                : $"{_damageString.StringReference.GetLocalizedString()}" + "\n";
            var actionCostString = _baseAction.GetActionPointCost() == 0
                ? ""
                : $"{_actionCostString.StringReference.GetLocalizedString()}" + "\n";
            var rangeString = _baseAction.MaxActionRange <= 1
                ? ""
                : $"{_rangeString.StringReference.GetLocalizedString()}" +
                  "\n";
            var cooldownString = !_baseAction.HasCoolDown
                ? ""
                : $"{_coolDownString.StringReference.GetLocalizedString()}" + "\n";
            var chargesString = !_baseAction.IsChargeable
                ? ""
                : $"{_chargesString.StringReference.GetLocalizedString()}" + "\n";

            _actionDescription.text = $"<b>{_nameString.StringReference.GetLocalizedString()}</b>" + "\n" +
                                      $"{_descriptionString.StringReference.GetLocalizedString()}" + "\n" + "\n" +
                                      actionCostString +
                                      damageString +
                                      rangeString +
                                      cooldownString +
                                      chargesString;
        }
        else
        {
            _actionDescription.text = $"<b>{_nameString.StringReference.GetLocalizedString()}</b>" + "\n" +
                                      $"{_descriptionString.StringReference.GetLocalizedString()}";
        }
    }

    private void OnSelectedLocaleChanged(Locale obj)
    {
        CreateDescription();
    }

    private void SetUpLocalizesStringAccordinglyToType(LocalizeStringEvent actionStringEvent, TableEntryReference tableEntryReference)
    {
        var stringRef = actionStringEvent.StringReference;
        stringRef.TableReference = "ActionUI";
        stringRef.TableEntryReference = tableEntryReference;
        stringRef.GetLocalizedString();
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
            InitialSetUpLocalizedString(_chargesLeft, OnChargesLeftStringrReferenceOnStringChanged, _baseAction.ChargesLeft);
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

    private void InitialSetUpLocalizedString(LocalizeStringEvent stringEvent, LocalizedString.ChangeHandler stringChangedCallback, params object[] dynamicStringArguments)
    {
        if (_localizeStringEventsDictionary[stringEvent].Item2 == null) return;
        
        InitialSetUpLocalizedString(stringEvent, dynamicStringArguments);
        
        _localizeStringEventsDictionary[stringEvent].Item1.StringChanged += stringChangedCallback;
    }
    
    private void InitialSetUpLocalizedString(LocalizeStringEvent stringEvent, params object[] dynamicStringArguments)
    {
        if (_localizeStringEventsDictionary[stringEvent].Item1.Arguments == null)
        {
            _localizeStringEventsDictionary[stringEvent].Item1.Arguments =  dynamicStringArguments;
        }

        for (int i = 0; i < _localizeStringEventsDictionary[stringEvent].Item1.Arguments.Count; i++)
        {
            _localizeStringEventsDictionary[stringEvent].Item1.Arguments[i] = dynamicStringArguments[i];
            ConvenientLogger.Log(nameof(ActionButtonUI), GlobalLogConstant.IsActionLogEnabled, $"{_baseAction.GetActionName()} {stringEvent.StringReference.GetLocalizedString()} : " + _localizeStringEventsDictionary[stringEvent].Item1.Arguments[i].ToString());
        }

        stringEvent.RefreshString();
    }

    private void OnChargesLeftStringrReferenceOnStringChanged(string value)
    {
        _localizeStringEventsDictionary[_chargesLeft].Item2.text = value;
    }

    private void OnDestroy()
    {
        if (_baseAction!=null) _baseAction.OnActionStatusUpdate -= BaseAction_OnActionStatusUpdate;
        UnitActionSystem.Instance.OnSelectedActionChanged -= BaseAction_OnActionStarted;
        UnitActionSystem.Instance.OnSelectedUnitChanged -= BaseAction_OnActionStarted;
        TurnSystem.Instance.OnTurnChanged -= BaseAction_OnActionStarted;
        if (_chargesLeftStringrReference != null) _chargesLeftStringrReference.StringChanged -= OnChargesLeftStringrReferenceOnStringChanged;
        if (LocalizationSettings.Instance != null) LocalizationSettings.Instance.OnSelectedLocaleChanged -= OnSelectedLocaleChanged;
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
