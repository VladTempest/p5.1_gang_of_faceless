using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.AI;
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
            Debug.Log("Checking unit" + enemyUnit.gameObject.name);
            bool isActionHappen = enemyUnit.gameObject.GetComponent<EnemyAIUnit>().TryMakeAIAction(onActionComplete);
            if (isActionHappen) return true;
        }

        return false;
    }

    private void SetStateTakingTurn()
    {
        _timer = 0.5f;
        _state = State.TakingTurn;
    }
}
