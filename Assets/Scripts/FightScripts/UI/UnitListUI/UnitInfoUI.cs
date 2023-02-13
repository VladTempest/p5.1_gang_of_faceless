using System;
using System.Globalization;
using Editor.Scripts.Utils;
using SoundSystemScripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Editor.Scripts.FightScripts.UI.CharacterListUI
{
    public class UnitInfoUI : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _unitLogo;
        [SerializeField] private Image _deadLogo;
        
        [SerializeField] private Slider _unitHPSlider;
        [SerializeField] private TMP_Text _unitHPText;
        
        [SerializeField] private Slider _unitAPSlider;
        [SerializeField] private TMP_Text _unitAPText;
        
        [SerializeField] private GameObject _unitActiveOutline;
        [SerializeField] private CanvasGroup _canvasGroup;

        [SerializeField] private TypeOfSFXByItsNature _soundtrack;
        private global::Unit _unit;

        private HealthSystem _healthSystem;

        public void SetUpUnitInfo(ClassesParameters classesParameters, global::Unit unit)
        {
            _unit = unit;
            
            _unitLogo.sprite = classesParameters.ClassLogo;
            _deadLogo.gameObject.SetActive(false);


            if (_unit.gameObject.TryGetComponent(out HealthSystem healthSystem))
            {
                _healthSystem = healthSystem;
                _healthSystem.OnDamaged += HealthSystem_OnDamaged;
            }
            UpdateHealthUI();

            global::Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
            UpdateAPUI();
            


            _unitActiveOutline.SetActive(UnitActionSystem.Instance.IfThisSelectedUnit(_unit));
            UnitActionSystem.Instance.OnSelectedUnitChanged += OnSelectedUnitChanged;
            TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            
            _unit.OnUnitEndedTurn += Unit_OnUnitEndedTurn;
            _unit.OnUnitAvailableForAction += Unit_OnUnitAvailableForAction;
        }

        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedUnitChanged -= OnSelectedUnitChanged;
            if (_healthSystem != null) _healthSystem.OnDamaged -= HealthSystem_OnDamaged;
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
            
            if (_unit == null) return;
            _unit.OnUnitEndedTurn -= Unit_OnUnitEndedTurn;
            _unit.OnUnitAvailableForAction -= Unit_OnUnitAvailableForAction;
        }

        private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
        {
            if ((global::Unit) sender == UnitActionSystem.Instance.GetSelectedUnit())
            {
                UpdateAPUI();
            }
        }

        private void OnTurnChanged(object sender, EventArgs e)
        {
            if (!TurnSystem.Instance.IsPlayerTurn)
            {
                _unitActiveOutline.SetActive(false);
            }
            else
            {
                _unitActiveOutline.SetActive(UnitActionSystem.Instance.IfThisSelectedUnit(_unit));
                UpdateAPUI();
            }
        }

        private void Unit_OnUnitAvailableForAction(object sender, EventArgs e)
        {
            _canvasGroup.alpha = 1;
            _canvasGroup.interactable = true;
        }

        private void Unit_OnUnitEndedTurn(object sender, EventArgs e)
        {
            _canvasGroup.alpha = 0.2f;
            _canvasGroup.interactable = false;
        }

        private void UpdateHealthUI()
        {
            _unitHPText.text = _unit.HealthPointsLeft.ToString(CultureInfo.InvariantCulture) + " HP";
            _unitHPSlider.value = _unit.HealthNormalised;
        }

        private void UpdateAPUI()
        {
            _unitAPText.text = _unit.ActionPoints.ToString(CultureInfo.InvariantCulture) + " AP";
            _unitAPSlider.value = (float) _unit.ActionPoints / _unit.ActionPointsMax;
        }

        private void HealthSystem_OnDamaged(object sender, EventArgs e)
        {
            if (_unit.HealthNormalised <= 0)
            {
                _canvasGroup.alpha = 0.2f;
                _canvasGroup.interactable = false;
                _deadLogo.gameObject.SetActive(true);
                
                UnitActionSystem.Instance.OnSelectedUnitChanged -= OnSelectedUnitChanged;
                if (_healthSystem != null) _healthSystem.OnDamaged -= HealthSystem_OnDamaged;
            }
            UpdateHealthUI();
        }

        private void OnSelectedUnitChanged(object sender, EventArgs e)
        {
            _unitActiveOutline.SetActive(TurnSystem.Instance.IsPlayerTurn && UnitActionSystem.Instance.IfThisSelectedUnit(_unit));
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundtrackPlayerWrapper.PlayUISoundtrack(_soundtrack);
            
            if (!_canvasGroup.interactable) return;
            if (!UnitActionSystem.Instance.IfThisSelectedUnit(_unit) || eventData.clickCount == 2)
            {
                UnitActionSystem.Instance.SetSelectedUnit(_unit);
            }
        }
    }
}