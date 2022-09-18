using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.Actions.BaseAction;
using Editor.Scripts.Utils;
using GridSystems;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    public static event EventHandler OnAnyActionCompleted;
    public event EventHandler OnActionStart;

    [SerializeField] private ActionsEnum _actionType;
    
    public event EventHandler OnActionStatusUpdate;

    public int MaxActionRange = 0;


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

    public int MinActionRange => _minActionRange;

    [SerializeField] private float _cooldownValue = 2;
    private float FullCoolDownValue => _cooldownValue;

    private ActionStatus _currentStatus;
    
    private float _coolDownTurnsLeft = 0;

    public bool IsChargeable => (_maxCharges != 0);
    [SerializeField] private int _maxCharges = 0;
    public int ChargesLeft => _chargesLeft;
    private int _chargesLeft = 0;

    protected Unit _unit;


    protected bool IsActive => _currentStatus == ActionStatus.InProgress;

    private Action _onActionComplete;
    protected float _damage;
    protected int _minActionRange;
    protected int _actionPointCost;

    protected virtual void Awake()
    {
        if (!enabled) return;
        
        var actionsParameters = ConstantsProvider.Instance.actionsParametersSO.ActionsParametersDictionary[_actionType];
        
        MaxActionRange = actionsParameters.MaxRange;
        _maxCharges = actionsParameters.Charges;
        _cooldownValue = actionsParameters.CoolDown;
        _damage = actionsParameters.Damage;
        _minActionRange = actionsParameters.MinRange;
        _actionPointCost = actionsParameters.ActionPoints;
        
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
        if (!enabled) return;
        
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    
    protected void OnDestroy()
    {
        TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        DecreaseCoolDownValueLeft();
        TryChangeStatusAfterCoolDown();
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
        if (_unit.EffectSystem.IsKnockedDown(out float durationLeft))
        {
            return false;
        }

        return true;
    }

    public virtual int GetActionPointCost()
    {
        return _actionPointCost;
    }

    public List<GridPosition> GetValidGridPositions()
    {
        //if (IsActive) return new List<GridPosition>();
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        GridPosition unitGridPosition = _unit.GetGridPosition();
        
        for (int x = -MaxActionRange; x <= MaxActionRange; x++)
        {
            for (int z = -MaxActionRange; z <= MaxActionRange; z++)
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
        UpdateActionStatus();
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
            Debug.Log($"[Enemy AI] There is no valid actions for {this}");
            return null;
        }

    }

    protected abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);

    private void InvokeOnActionStart(object sender, EventArgs eventArgs)
    {
        if (_maxCharges != 0) _chargesLeft--;
        if (HasCoolDown) _coolDownTurnsLeft = FullCoolDownValue;
        UpdateActionStatus();
        OnActionStart?.Invoke(sender, eventArgs);
    }

    private void TryChangeStatusAfterCoolDown()
    {
        switch (_currentStatus)
        {
            case ActionStatus.Discharged:
                break;
            case ActionStatus.OnCoolDown:
                if (_cooldownValue != 0 && _coolDownTurnsLeft > 0)
                {
                    OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                    break;
                }
                _currentStatus = ActionStatus.ReadyToUse;
                OnActionStatusUpdate?.Invoke(this, EventArgs.Empty);
                break;
        }
    }
    
    private void UpdateActionStatus()
    {
        switch (_currentStatus)
        {
            case ActionStatus.ReadyToUse:
                _currentStatus = ActionStatus.InProgress;
                break;
            case ActionStatus.Discharged:
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
        }
    }

    private void DecreaseCoolDownValueLeft()
    {
        if (_coolDownTurnsLeft > 0)
        {
            _coolDownTurnsLeft -= GameGlobalConstants.TURN_WEIGHT_VALUE;
        }
    }
}
