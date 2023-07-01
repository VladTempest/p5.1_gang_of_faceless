using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using Editor.Scripts.Actions;
using Editor.Scripts.GlobalUtils;
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

    [SerializeField] private TypeOfGridVisual _typeOfGridVisual;

    [SerializeField] private GridVisualizerFactory _gridVisualizerFactory;

    [SerializeField] private GameObject _gridPreview;

    private IGridVisualizer _gridVisualizer;


    private void Awake()
    {
        if (Instance != null)
        {
            ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    private void Start()
    {
        _gridVisualizer = _gridVisualizerFactory.GetGridVisualizer(_typeOfGridVisual);
        _gridPreview.gameObject.SetActive(false);

        UnitActionSystem.Instance.OnBusyChanged += UnitActionSystem_OnBusyChanged;
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
        UnitActionSystem.Instance.OnBusyChanged -= UnitActionSystem_OnBusyChanged;
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

        _gridVisualizer.UpdateGridVisuals(CreateGridPositions(selectedUnit));
    }

    public List<GridPosition> GetGridPositionRangeCircle(GridPosition gridPosition, int maxRange, int minRange = 0)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -maxRange; x <= maxRange; x++)
        {
            for (int z = -maxRange; z <= maxRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > maxRange || testDistance < minRange) continue;

                gridPositionList.Add(testGridPosition);
            }
        }

        return gridPositionList;
    }

    public List<GridPosition> GetGridPositionRangeSquare(GridPosition gridPosition, int range)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        return gridPositionList;
    }

    private Dictionary<GridVisualType, List<GridPosition>> CreateGridPositions(Unit selectedUnit)
    {
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        Dictionary<GridVisualType, List<GridPosition>> gridPositionDictionary =
            new Dictionary<GridVisualType, List<GridPosition>>();

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
            case InteractAction interactAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case GrenadeAction grenadeAction:
                gridVisualType = GridVisualType.Red;
                break;
            case BackStabAction backstab:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeCircle(selectedUnit.GetGridPosition(), backstab.MaxActionRange));
                break;
            case GreatSwordAction swordAction:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeSquare(selectedUnit.GetGridPosition(), swordAction.MaxActionRange));
                break;
            case DualSwordsAction dualSwordAction:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeCircle(selectedUnit.GetGridPosition(), dualSwordAction.MaxActionRange));
                break;
            case KnockDownAction knockDownAction:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeCircle(selectedUnit.GetGridPosition(), knockDownAction.MaxActionRange));
                break;
            case BaseShootAction shootAction:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeCircle(selectedUnit.GetGridPosition(), shootAction.MaxActionRange));
                break;
            case PushAction pushAction:
                gridVisualType = GridVisualType.Red;
                gridPositionDictionary.Add(GridVisualType.RedSoft,
                    GetGridPositionRangeCircle(selectedUnit.GetGridPosition(), pushAction.MaxActionRange));
                break;
        }

        /*List<GridPosition> validGridPositions = selectedAction.GetValidGridPositions();
        
        //Delete valid grid positions from every dictionary key value pair in gridPositionDictionary
        foreach (KeyValuePair<GridVisualType, List<GridPosition>> gridPositionPair in gridPositionDictionary)
        {
            foreach (GridPosition validGridPosition in validGridPositions)
            {
                gridPositionPair.Value.Remove(validGridPosition);
            }
        }*/
        
        gridPositionDictionary.Add(gridVisualType, selectedAction.GetValidGridPositions());
        return gridPositionDictionary;
    }
}
