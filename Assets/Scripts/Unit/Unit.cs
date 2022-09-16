using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using GridSystems;
using Scripts.Unit;
using Systems.HealthStatus;
using UnityEngine;
using UnityEngine.Analytics;

public class Unit : MonoBehaviour
{
    public class OnAnyUnitDiedEventArgs
    {
        public GridPosition deadUnitGridPosition;
    }
    
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler<OnAnyUnitDiedEventArgs> OnAnyUnitDead;

    public UnitType UnitType = UnitType.None;
    public EffectSystem EffectSystem => _effectSystem;
    public int ActionPoints => _actionPoint;
    public bool IsUnitAnEnemy => _isEnemy;
    public Vector3 WorldPosition => transform.position;
    public float HealthNormalised => _healthSystem.GetNormalisedValueOfHealth();

    public MoveAction UnitMoveAction => _moveAction;

    [SerializeField] private int ACTION_POINT_MAX = 10;
    [SerializeField] private bool _isEnemy;
    [SerializeField] private int _actionPoint = 10;

    private MoveAction _moveAction;
    private HealthSystem _healthSystem;
    private EffectSystem _effectSystem;
    private GridPosition _currentGridPosition;
    private BaseAction[] _baseActionArray;


    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _effectSystem = GetComponent<EffectSystem>();
        _baseActionArray = GetComponents<BaseAction>();
        
        SetGameObjectName();
    }

    private void Start()
    {
        _actionPoint = ACTION_POINT_MAX;
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        _moveAction = GetComponent<MoveAction>();
        
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        _healthSystem.OnDead += HealthSystem_OnDead;
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != _currentGridPosition)
        {
            GridPosition oldGridPosition = _currentGridPosition;
            _currentGridPosition = newGridPosition;
            LevelGrid.Instance.UnitMovedGriPosition(this, oldGridPosition, newGridPosition);
        } 
    }

    public GridPosition GetGridPosition()
    {
        return _currentGridPosition;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return _baseActionArray;
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoints(baseAction.GetActionPointCost());
            return true;
        }

        return false;
    }

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        return _actionPoint >= baseAction.GetActionPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        _actionPoint -= amount;
        
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if ((_isEnemy && !TurnSystem.Instance.IsPlayerTurn) || (!_isEnemy && TurnSystem.Instance.IsPlayerTurn))
        {
            _actionPoint = ACTION_POINT_MAX;

            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Damage(int damageAmount, Vector3 damageSourcePosition)
    {
        _healthSystem.Damage(damageAmount, damageSourcePosition);
    }
    
    private void SetGameObjectName()
    {
        StringBuilder objectName = new StringBuilder();
        objectName.Append(UnitType.ToString());
        objectName.Append(IsUnitAnEnemy ? "Enemy" : "Player");
        gameObject.name = objectName.ToString();
    }
    
    private void HealthSystem_OnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_currentGridPosition, this);
        
        Destroy(gameObject);
        
        OnAnyUnitDead?.Invoke(this, new OnAnyUnitDiedEventArgs(){deadUnitGridPosition = _currentGridPosition});
    }


    public T GetAction<T>() where T : BaseAction
    {
        foreach (var baseAction in _baseActionArray)
        {
            if (baseAction is T) return (T) baseAction;
        }

        return null;
    }
}