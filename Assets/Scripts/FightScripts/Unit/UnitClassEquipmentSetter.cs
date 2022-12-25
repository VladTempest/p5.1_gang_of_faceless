using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Scripts.Unit
{
    public class UnitClassEquipmentSetter : MonoBehaviour
    {
        [SerializeField] private global::Unit _unit;
        
        [SerializeField] private Transform _bow;
        [SerializeField]  private Transform _sword;
        [SerializeField]  private Transform _leftDualSword;
        [SerializeField]  private Transform _rightDualSword;
        
        [SerializeField]  private Transform _archerVisuals;
        [SerializeField]  private Transform _heavyWarriorVisuals;
        [SerializeField]  private Transform _lightWarriorVisuals;
        [SerializeField] private Transform _defaultVisuals;

        private void Start()
        {
            switch (_unit.UnitType)
            {
                case UnitType.None:
                    break;
                case UnitType.Archer:
                    SetActiveArcherVisuals();
                    EquipBow();
                    break;
                case UnitType.HeavyWarrior:
                    SetActiveHeavyWarriorVisuals();
                    EquipSword();
                    break;
                case UnitType.LightWarrior:
                    SetActiveLightWarriorVisuals();
                    EquipDualSwords();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        private void EquipSword()
        {
            _sword.gameObject.SetActive(true);
        }

        private void EquipBow()
        {
            _bow.gameObject.SetActive(true);
        }

        private void EquipDualSwords()
        {
            _leftDualSword.gameObject.SetActive(true);
            _rightDualSword.gameObject.SetActive(true);
        }
        
        public void SetActiveArcherVisuals()
        {
            DeactivateDefaultVisuals();
            _archerVisuals.gameObject.SetActive(true);
        }
    
        public void SetActiveHeavyWarriorVisuals()
        {
            DeactivateDefaultVisuals();
            _heavyWarriorVisuals.gameObject.SetActive(true);
        }

        public void SetActiveLightWarriorVisuals()
        {
            DeactivateDefaultVisuals();
            _lightWarriorVisuals.gameObject.SetActive(true);
        }

        private void DeactivateDefaultVisuals()
        {
            _defaultVisuals.gameObject.SetActive(false);
        }
    }
}