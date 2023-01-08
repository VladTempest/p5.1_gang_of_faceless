using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using GridSystems;
using Sirenix.Utilities;
using SoundSystemScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    public event EventHandler OnSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    
    public event EventHandler OnActionStarted;

    [SerializeField] private LayerMask _unitLayerMask;

    private Unit _selectedUnit;
    private BaseAction _selectedAction;

    public bool IsBusy => _isBusy;
    private bool _isBusy;

    public Unit GetSelectedUnit()
    {
        if (_selectedUnit == null)
        {
            if (UnitManager.Instance.FriendlyUnitList.IsNullOrEmpty())
            {
                Debug.LogError("There is no friendly units");
                return null;
            }
            
            _selectedUnit = UnitManager.Instance.FriendlyUnitList[0];
            OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
            //ToDo: Удалить стопяцот инвоков ивента
        }
        return _selectedUnit;
    }

    public BaseAction GetMoveAction()
    {
        if (_selectedUnit != null && _selectedUnit.UnitMoveAction != null)
        {
            return _selectedUnit.UnitMoveAction;
        }
        
        Debug.LogError("There is no Actions on Unit");
        var dummyAction = gameObject.AddComponent<MoveAction>();
        return dummyAction;
    }

    public BaseAction GetSelectedAction()
    {
        if (_selectedAction != null)
        {
            return _selectedAction;
        }

        return GetMoveAction();
    }

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
        SetUpSelectedUnit();
        TurnSystem.Instance.OnTurnChanged += ChangeSelectedPlayer;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        OnBusyChanged += ChangeSelectedActionToMoveAction;
    }

    private void Unit_OnAnyUnitDead(object sender, Unit.OnAnyUnitDiedEventArgs e)
    {
        SetUpSelectedUnit();
    }

    private void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= ChangeSelectedPlayer;
        Unit.OnAnyUnitDead -= Unit_OnAnyUnitDead;
        OnBusyChanged -= ChangeSelectedActionToMoveAction;
    }

    private void ChangeSelectedPlayer(object sender, EventArgs e)
    {
        SetUpSelectedUnit();
    }

    private void ChangeSelectedActionToMoveAction(object sender, bool isBusy)
    {
        if (!isBusy) SetSelectedAction(GetMoveAction());
    }

    private void SetUpSelectedUnit()
    {
        if (_selectedUnit != null)
        {
            SetSelectedAction(GetMoveAction());
            return;
        }
        _selectedUnit = GetSelectedUnit();
        if (_selectedUnit == null) return;
        SetSelectedAction(GetMoveAction());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (_isBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn) return;
        
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
        if (TryHandleUnitSelection()) return;
        
        HandleSelectedAction();
    }


    private void HandleSelectedAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPointerInWorldPosition());

            if (!_selectedAction.IsGridPositionValid(mouseGridPosition, _selectedUnit.GetGridPosition())) return;
            if (!_selectedUnit.TrySpendActionPointsToTakeAction(_selectedAction)) return;
            if (InputManager.Instance.IsMouseButtonDownThisFrame()) SoundtrackPlayerWrapper.PlayUITargetChooseSound();
            SetBusy();
            _selectedAction.TakeAction(mouseGridPosition, ClearBusy);
            OnActionStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool TryHandleUnitSelection()
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame())
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, float.MaxValue, _unitLayerMask))
            {
                if (raycastHit.transform.TryGetComponent<Unit>(out Unit unit))
                {
                    if (unit == _selectedUnit) return false;
                    if (unit.IsUnitAnEnemy) return false;
                    SetSelectedUnit(unit);
                    return true;
                }
            }
        }

        return false;
    }

    public void SelectNextUnit()
    {
        var indexOfSelectedUnit = UnitManager.Instance.FriendlyUnitList.FindIndex(0,item => item == _selectedUnit);
        indexOfSelectedUnit++;
        if (indexOfSelectedUnit >= UnitManager.Instance.FriendlyUnitList.Count) indexOfSelectedUnit = 0;
        SetSelectedUnit(UnitManager.Instance.FriendlyUnitList[indexOfSelectedUnit]);
    }
    
    public void SelectPreviousUnit()
    {
        var indexOfSelectedUnit = UnitManager.Instance.FriendlyUnitList.FindIndex(0,item => item == _selectedUnit);
        indexOfSelectedUnit--;
        if (indexOfSelectedUnit < 0 ) indexOfSelectedUnit =  UnitManager.Instance.FriendlyUnitList.Count - 1;
        SetSelectedUnit(UnitManager.Instance.FriendlyUnitList[indexOfSelectedUnit]);
    }

    private void SetSelectedUnit(Unit unit)
    {
        if (InputManager.Instance.IsMouseButtonDownThisFrame()) SoundtrackPlayerWrapper.PlayUIUnitChooseSound();
        _selectedUnit = unit;
        SetSelectedAction(GetMoveAction());
        
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction)
    {
        _selectedAction = baseAction;
        
        OnSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }
    private void SetBusy()
    {
        _isBusy = true;
        OnBusyChanged?.Invoke(this, _isBusy);
    }

    private void ClearBusy()
    {
        _isBusy = false;
        OnBusyChanged?.Invoke(this, _isBusy);
    }
}
