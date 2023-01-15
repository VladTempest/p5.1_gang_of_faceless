using System;
using System.Text;
using GridSystems;
using Scripts.Unit;
using SoundSystemScripts;
using Systems.HealthStatus;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Unit : MonoBehaviour
{
    public class OnAnyUnitDiedEventArgs
    {
        public GridPosition deadUnitGridPosition;
    }
    
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler<OnAnyUnitDiedEventArgs> OnAnyUnitDead;
    
    public event EventHandler OnUnitEndedTurn;
    public event EventHandler OnUnitAvailableForAction;

    public UnitType UnitType = UnitType.None;

    public EffectSystem EffectSystem => _effectSystem;
    public int ActionPoints => _actionPoint;
    public int ActionPointsMax => _actionPointMax;
    public bool IsUnitAnEnemy => _isEnemy;
    public Vector3 WorldPosition => transform.position;
    public float HealthNormalised => _healthSystem.GetNormalisedValueOfHealth();
    public float HealthPointsLeft => _healthSystem.Health;
    public BaseAction[] BaseActions => _baseActionArray;

    public MoveAction UnitMoveAction => _moveAction;


    [SerializeField] private int _actionPointMax = 10;
    [SerializeField] private bool _isEnemy;
    [SerializeField] private int _actionPoint = 10;

    public bool IsUnitAvailableForAction => _unitState == UnitAvailabilityForActState.AvailableForAction;
    private UnitAvailabilityForActState _unitState = UnitAvailabilityForActState.AvailableForAction;

    private MoveAction _moveAction;
    private HealthSystem _healthSystem;
    private EffectSystem _effectSystem;
    private GridPosition _currentGridPosition;
    private BaseAction[] _baseActionArray;


    public void ChangeUnitState(UnitAvailabilityForActState newState)
    {
        switch (_unitState)
        {
            case UnitAvailabilityForActState.AvailableForAction:
                if (newState == UnitAvailabilityForActState.EndedTurn)
                {
                    _unitState = newState;
                    OnUnitEndedTurn?.Invoke(this, EventArgs.Empty);
                }
                break;
            case UnitAvailabilityForActState.EndedTurn:
                if (newState == UnitAvailabilityForActState.AvailableForAction)
                {
                    _unitState = newState;
                    OnUnitAvailableForAction?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
        {
            
        }
        
    }
    
    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _effectSystem = GetComponent<EffectSystem>();
        _baseActionArray = GetComponents<BaseAction>();
        
        SetGameObjectName();
    }

    private void Start()
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene") return; //TODO: использовать кастомный левел менеждер
        SetUpClassParameters();
        _actionPoint = _actionPointMax;
        GridPosition gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        _moveAction = GetComponent<MoveAction>();
        
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
        
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;

        _healthSystem.OnDead += HealthSystem_OnDead;
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        if (TurnSystem.Instance != null) TurnSystem.Instance.OnTurnChanged -= TurnSystem_OnTurnChanged;
        _healthSystem.OnDead -= HealthSystem_OnDead;
    }

    private void SetUpClassParameters()
    {
        if (ConstantsProvider.Instance.classesParametersSO.ClassesParametersDictionary.TryGetValue(UnitType,
                out var classesParameters))
        {
            _actionPointMax = classesParameters.ActionPoints;
        }
        else
        {
            Debug.LogError($"[Action] Can not find {UnitType} in Constants Provider dict", this);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().name == "MainMenuScene") return; //TODO: использовать кастомный левел менеждер
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
    
    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction, GridPosition targetGridPosition)
    {
        if (CanSpendActionPointToTakeAction(baseAction, targetGridPosition))
        {
            SpendActionPoints(baseAction.GetActionPointCost(targetGridPosition));
            return true;
        }

        return false;
    }

    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        return _actionPoint >= baseAction.GetActionPointCost();
    }
    
    public bool CanSpendActionPointToTakeAction(BaseAction baseAction, GridPosition targetGridPosition)
    {
        return _actionPoint >= baseAction.GetActionPointCost(targetGridPosition);
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
            _actionPoint = _actionPointMax;
            ChangeUnitState(UnitAvailabilityForActState.AvailableForAction);
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Damage(float damageAmount, Vector3 damageSourcePosition)
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
        SoundtrackPlayerWrapper.PlayDeathSounds(this, transform);
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