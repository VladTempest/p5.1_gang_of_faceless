using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.AI;
using GridSystems;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
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
        if (currentEnemyInAction <= _enemiesCount - 1)
        {
            var enemyAiUnit = _enemyUnitList[currentEnemyInAction].gameObject.GetComponent<EnemyAIUnit>();
            _currentEnemyInAction++;
            _cameraController.StartMovingToTargetUnit(enemyAiUnit.transform.position);
            Debug.Log($"{Time.time} [ENEMY AI] Start make action for {_enemyUnitList[currentEnemyInAction]}({_enemyUnitList[currentEnemyInAction].GetGridPosition()}) enemy");
            enemyAiUnit.MakeAIAction(() =>
            {
                Debug.Log($"{Time.time} [ENEMY AI] action finished on" + enemyAiUnit.gameObject.name);
                MakeTurnOfEnemyWithIndex(_currentEnemyInAction);
                
            });
            return;
        }
        
        TurnSystem.Instance.NextTurn();
        
    }
}
