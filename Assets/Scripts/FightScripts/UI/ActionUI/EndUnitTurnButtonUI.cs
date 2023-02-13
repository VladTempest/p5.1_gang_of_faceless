using System;
using Scripts.Unit;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Editor.Scripts.FightScripts.UI.ActionUI
{
    public class EndUnitTurnButtonUI : MonoBehaviour
    {
        [SerializeField] private Button endTurnButton;

        private void Start()
        {
            endTurnButton.onClick.AddListener(EndSelectedUnitTurn);
        }

        private void EndSelectedUnitTurn()
        {
            var selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            selectedUnit.ChangeUnitState(UnitAvailabilityForActState.EndedTurn);
            UnitActionSystem.Instance.SelectNextUnit();
        }

        private void OnDestroy()
        {
            endTurnButton.onClick.RemoveListener(EndSelectedUnitTurn);
        }
    }
}