using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.Actions.BaseAction;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    public event EventHandler OnActionStart;

    public event EventHandler OnActionStatusUpdate;

    public int ActionRange = 0;


    public bool IsAvailable => _currentStatus == ActionStatus.ReadyToUse;
    public bool HasCoolDown => (_cooldownValue != 0);
    public float CoolDownLeftNormalized
    {
        get
        {
            if (_cooldownValue != 0)
            {
                return (float) _coolDownTurnsLeft / FullCoolDownValue;
            }
            
            return FullCoolDownValue;
        }
    }

    [SerializeField] private int _cooldownValue = 2;
    public int FullCoolDownValue => _cooldownValue * 2;
    private ActionStatus _currentStatus;
    private int _coolDownTurnsLeft = 0;

    public bool IsChargeable => (_maxCharges != 0);
    [SerializeField] private int _maxCharges = 0;
    public int ChargesLeft => _chargesLeft;
    private int _chargesLeft = 0;

    protected Unit _unit;


    protected bool IsActive
    {
        get => _currentStatus == ActionStatus.InProgress;
    }

    protected Action _onActionComplete;

    protected virtual void Awake()
    {
        _chargesLeft = _maxCharges;
        
        if (_coolDownTurnsLeft == 0)
        {
            _currentStatus = ActionStatus.ReadyToUse;
        }
        else
        {
            _currentStatus = ActionStatus.OnCoolDown;
        }
        _unit = GetComponent<Unit>();
    }

    protected void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    
    protected void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        DecreaseCoolDownValueLeft();
        ChangeActionStatusFrom(_currentStatus);
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);
    
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        var validGridPositions = GetValidGridPositions();
        return validGridPositions.Contains(gridPosition);
    }

    protected virtual bool IsGridPositionValid(GridPosition testGridPosition, GridPosition unitGridPosition)
    {
        if (_unit.EffectSystem.IsKnockedDown(out int durationLeft))
        {
            return false;
        }

        return true;
    }

    public virtual int GetActionPointCost()
    {
        return 1;
    }

    public List<GridPosition> GetValidGridPositions()
    {
        if (IsActive) return new List<GridPosition>();
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        
        for (int x = -ActionRange; x <= ActionRange; x++)
        {
            for (int z = -ActionRange; z <= ActionRange; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (IsGridPositionValid(testGridPosition, unitGridPosition))
                {
                    validGridPositionList.Add(testGridPosition);
                }
            }
        }

        return validGridPositionList;
    }

    protected void ActionStart(Action onActionComplete)
    {
        
        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
        InvokeOnActionStart(this, EventArgs.Empty);
        _onActionComplete = onActionComplete;
    }

    protected void ActionComplete()
    {
        ChangeActionStatusFrom(_currentStatus);
        _onActionComplete?.Invoke();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidGridPositions();

        foreach (var gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }
        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort(((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue));
            return enemyAIActionList[0];
        }
        else
        {
            Debug.LogError("No EnemyActions");
            return null;
        }

    }

    protected abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    private void InvokeOnActionStart(object sender, EventArgs eventArgs)
    {
        if (_maxCharges != 0) _chargesLeft--;
        if (HasCoolDown) _coolDownTurnsLeft = FullCoolDownValue;
        ChangeActionStatusFrom(_currentStatus);
        OnActionStart?.Invoke(sender, eventArgs);
    }

    protected void ChangeActionStatusFrom(ActionStatus status)
    {
        switch (status)
        {
            case ActionStatus.ReadyToUse:
                _currentStatus = ActionStatus.InProgress;
                break;
            case ActionStatus.Discharged:
                break;
            case ActionStatus.OnCoolDown:
                _currentStatus = ActionStatus.ReadyToUse;
                OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                break;
            case ActionStatus.InProgress:
                if (_maxCharges != 0 && _chargesLeft <= 0)
                {
                    _currentStatus = ActionStatus.Discharged;
                    OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                    break;
                }
                if (_cooldownValue != 0 && _coolDownTurnsLeft != 0)
                {
                    _currentStatus = ActionStatus.OnCoolDown;
                    OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                    break;
                }
                _currentStatus = ActionStatus.ReadyToUse;
                OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(status), status, null);
        }
    }

    private void DecreaseCoolDownValueLeft()
    {
        if (_coolDownTurnsLeft > 0)
        {
            --_coolDownTurnsLeft;
        }
    }
}
