using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.SceneLoopScripts;
using Editor.Scripts.Utils;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance { get; private set; }

    public List<Unit> UnitList => _unitList;
    public List<Unit> FriendlyUnitList => _friendlyUnitList;
    public List<Unit> EnemyUnitList => _enemyUnitList;
   

    private List<Unit> _unitList;
    private List<Unit> _friendlyUnitList;
    private List<Unit> _enemyUnitList;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are many singletonss");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        _unitList = new List<Unit>();
        _friendlyUnitList = new List<Unit>();
        _enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        _unitList.Add(unit);
        
        if (unit.IsUnitAnEnemy)
        {
            _enemyUnitList.Add(unit);
            _enemyUnitList.Sort(((a, b) => a.ActionPointsMax - b.ActionPointsMax));
        }
        else
        {
            _friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, Unit.OnAnyUnitDiedEventArgs onAnyUnitDiedEventArgs)
    {
        Unit unit = sender as Unit;
        _unitList.Remove(unit);
        
        if (unit.IsUnitAnEnemy)
        {
            _enemyUnitList.Remove(unit);
        }
        else
        {
            _friendlyUnitList.Remove(unit);
        }
        if (_enemyUnitList.Count == 0 || _friendlyUnitList.Count == 0) Utils.CallWithDelay(2f,() => ScenesController.Instance.LoadScene(ScenesEnum.MainMenu)); // ToDo : Сделать окно поражения или победы через общий класс
    }
}
