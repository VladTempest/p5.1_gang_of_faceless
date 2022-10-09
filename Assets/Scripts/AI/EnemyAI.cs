using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.AI;
using GridSystems;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private int _enemiesCount;
    private int _currentEnemyInAction;
    private List<Unit> _enemyUnitList;
    
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn)
        {
            MakeAITurn();
        }
    }

    private void MakeAITurn()
    {
        _enemyUnitList = UnitManager.Instance.EnemyUnitList;
        _enemiesCount = _enemyUnitList.Count;
        _currentEnemyInAction = 0;
        MakeTurnOfEnemyWithIndex(_currentEnemyInAction);
    }

    private void MakeTurnOfEnemyWithIndex(int currentEnemyInAction)
    {
        Debug.Log($"{Time.time} [ENEMY AI] Trying make action for {currentEnemyInAction} enemy");
        if (currentEnemyInAction <= _enemiesCount - 1)
        {
            var enemyAiUnit = _enemyUnitList[currentEnemyInAction].gameObject.GetComponent<EnemyAIUnit>();
            _currentEnemyInAction++;
            Debug.Log($"{Time.time} [ENEMY AI] Start make action for {currentEnemyInAction} enemy");
            enemyAiUnit.MakeAIAction(() =>
            {
                Debug.Log($"{Time.time} [ENEMY AI] action finished on" + enemyAiUnit.gameObject.name);
                MakeTurnOfEnemyWithIndex(_currentEnemyInAction);
                
            });
            return;
        }
        
        TurnSystem.Instance.NextTurn();
        
    }

    /*IEnumerator StartEnemiesAction()
    {
        var enemyUnitList = UnitManager.Instance.EnemyUnitList;
        var enemyAiUnit = enemyUnitList[0].gameObject.GetComponent<EnemyAIUnit>();
        
    }*/
    
    /*private bool TryTakeEnemyActionAI(Action onActionComplete)
    {
        var enemyUnitList = UnitManager.Instance.EnemyUnitList;
        bool isActionHappen = false;
        StartCoroutine(MakeAction(onActionComplete));
        return false;
    }*/
    

    /*IEnumerator MakeAction(Action onActionComplete)
    {
        var enemyUnitList = UnitManager.Instance.EnemyUnitList;
        foreach (var enemyUnit in enemyUnitList)
        {
            var enemyAiUnit = enemyUnit.gameObject.GetComponent<EnemyAIUnit>();
            Debug.Log("Checking unit" + enemyUnit.gameObject.name);
            while (isEnemyShouldTryMakeAction)
            {
                isEnemyShouldTryMakeAction = enemyAiUnit.TryMakeAIAction(onActionComplete);
                yield return null;
            }
        }
    }*/
}
