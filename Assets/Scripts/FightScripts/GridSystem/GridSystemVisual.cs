using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using Editor.Scripts.Actions;
using FightScripts.GridSystem;
using GridSystems;
using Scripts.Unit;
using Unity.VisualScripting;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{ 
    public enum TypeOfGridVisual
    {
        SeparateGrids,
        OnlyPerimeter
    }

    private static GridSystemVisual Instance { get; set; }

    [SerializeField]
    private TypeOfGridVisual _typeOfGridVisual;
    
    [SerializeField]
    private GridVisualizerFactory _gridVisualizerFactory;
    
    private IGridVisualizer _gridVisualizer;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are many singletonss");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    

    private void Start()
    {
        _gridVisualizer = _gridVisualizerFactory.GetGridVisualizer(_typeOfGridVisual);
        
        UnitActionSystem.Instance.OnBusyChanged +=  UnitActionSystem_OnBusyChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        LevelGrid.Instance.OnAnyUnitChangedGridPosition += LevelGrid_OnAnyUnitChangedGridPosition;
        Unit.OnAnyUnitDead += Unit_OnUnitDied;
    }

    
    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn) HideGridVisuals();
    }

    private void UnitActionSystem_OnBusyChanged(object sender, bool isBusyActive)
    {
        if (isBusyActive) HideGridVisuals();
    }


    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnBusyChanged -=  UnitActionSystem_OnBusyChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged -= UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitChangedGridPosition -= LevelGrid_OnAnyUnitChangedGridPosition;
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        Unit.OnAnyUnitDead -= Unit_OnUnitDied;
    }

    private void LevelGrid_OnAnyUnitChangedGridPosition(object sender, OnAnyUnitChangedArgs onAnyUnitChangedArgs)
    {
        if (UnitActionSystem.Instance.GetSelectedUnit() != onAnyUnitChangedArgs.unit) return;
        UpdateGridVisual();
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    
    private void Unit_OnUnitDied(object sender, Unit.OnAnyUnitDiedEventArgs onAnyUnitDiedEventArgs)
    {
        UpdateGridVisual();
    }

    private void HideGridVisuals()
    {
        _gridVisualizer.HideGridVisuals();
    }
    
    private void UpdateGridVisual()
    {
        if (UnitActionSystem.Instance.IsBusy || !TurnSystem.Instance.IsPlayerTurn) return;
        HideGridVisuals();
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit == null) return;

        _gridVisualizer.UpdateGridVisuals(selectedUnit);
    }
}
