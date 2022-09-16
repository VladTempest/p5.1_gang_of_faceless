using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State
    {
      WaitingForEnemyTurn,
      TakingTurn,
      Busy
    }

    private State _state;
    private float _timer;

    private void Awake()
    {
        _state = State.WaitingForEnemyTurn;
    }

    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn)
        {
            _state = State.TakingTurn;
            _timer = 2f;
        }
    }

    void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn)
        {
            return;
        }

        switch (_state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.Busy:
                break;
            case State.TakingTurn:
                _timer -= Time.deltaTime;
                if (_timer <= 0f)
                {
                    if (TryTakeEnemyActionAI(SetStateTakingTurn))
                    {
                        _state = State.Busy;
                    }
                    else
                    {
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
        }

        
    }

    private bool TryTakeEnemyActionAI(Action onActionComplete)
    {
        var enemyUnitList = UnitManager.Instance.EnemyUnitList;
        foreach (var enemyUnit in enemyUnitList)
        {
            if (TryTakeEnemyActionAI(enemyUnit, onActionComplete)) return true;
        }

        return false;
    }

    private bool TryTakeEnemyActionAI(Unit enemyUnit, Action onActionComplete)
    {
        EnemyAIAction bestEnemyAIAction = null;
        BaseAction bestBaseAction = null;
        foreach (var baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!baseAction.enabled) continue;
            if (!enemyUnit.CanSpendActionPointToTakeAction(baseAction))
            {
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }

        if (bestEnemyAIAction != null && enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction))
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onActionComplete);
            return true;
        }
        return false;
    }

    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _state = State.TakingTurn;
    }
}
