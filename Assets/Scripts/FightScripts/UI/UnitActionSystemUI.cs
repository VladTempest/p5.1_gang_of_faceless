using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitActionSystemUI : MonoBehaviour
{

   [SerializeField] private Transform _actionBittonPrefab;
   [SerializeField] private Transform _actionButtonContainerTransform;

   private List<ActionButtonUI> _actionButtonUIList;

   private void Awake()
   {
      _actionButtonUIList = new List<ActionButtonUI>();
   }

   private void Start()
   {
      UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
      UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
      UnitActionSystem.Instance.OnActionStarted += UnitActionSystem_OnActionStarted;
      TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
      Unit.OnAnyActionPointsChanged += Unit_OnAnyActionPointsChanged;
      CreateUnitActionButtons();
      UpdateSelectedVisual();
      UpdateActionPoint();
   }

   private void Unit_OnAnyActionPointsChanged(object sender, EventArgs e)
   {
      UpdateActionPoint();
   }

   private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
   {
      UpdateActionPoint();
   }

   private void UnitActionSystem_OnActionStarted(object sender, EventArgs e)
   {
      UpdateActionPoint();
   }

   private void UpdateSelectedVisual()
   {
      foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
      {
         actionButtonUI.UpdateSelectedVisual();
      }
   }

   private void CreateUnitActionButtons()
   {
      _actionButtonUIList.Clear();
      foreach (Transform buttonTransform in _actionButtonContainerTransform)
      {
         Destroy(buttonTransform.gameObject);
      }

      Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
      foreach (BaseAction baseAction in selectedUnit.GetBaseActionArray())
      {
         if (!baseAction.enabled) continue;
         Transform actionButtonTransform = Instantiate(_actionBittonPrefab, _actionButtonContainerTransform);
         var actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
         actionButtonUI.SetBaseAction(baseAction);
         _actionButtonUIList.Add(actionButtonUI);
      }
   }

   private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
   {
      CreateUnitActionButtons();
      UpdateSelectedVisual();
      UpdateActionPoint();
   }

   private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
   {
      UpdateSelectedVisual();
   }

   private void UpdateActionPoint()
   {
      Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
   }
}
