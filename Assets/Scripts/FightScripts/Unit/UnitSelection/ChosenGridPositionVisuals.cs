using System;
using GridSystems;
using UnityEngine;

namespace Editor.Scripts.FightScripts.Unit.UnitSelection
{
    public class ChosenGridPositionVisuals : MonoBehaviour
    {
        
          private void EnableVisuals(){
            if (gameObject.activeSelf) return;
            gameObject.SetActive(true);
        }
        
        private void DisableVisuals(){
            if (!gameObject.activeSelf) return;
            gameObject.SetActive(false);
        }

        private void Start()
        {
            UnitActionSystem.Instance.OnSelectedPositionChanged += OnSelectedPositionChanged;
            TurnSystem.Instance.OnTurnChanged += OnTurnChanged;
            UnitActionSystem.Instance.OnBusyChanged += OnBusyChanged;
            DisableVisuals();   
        }

        private void OnBusyChanged(object sender, bool isBusy)
        {
            if (TurnSystem.Instance.IsPlayerTurn == false) return;
            if (isBusy)
            {
                DisableVisuals();
            }
            else
            {
                EnableVisuals();
            }
        }

        private void OnTurnChanged(object sender, EventArgs e)
        {
            if (TurnSystem.Instance.IsPlayerTurn)
            {
                EnableVisuals();
            }
            else
            {
                DisableVisuals();
            }
        }

        private void OnSelectedPositionChanged(object sender, OnSelectedPositionChangedArgs e)
        {
            if (TurnSystem.Instance.IsPlayerTurn == false) return;
            if (e.NewGridPosition == new GridPosition(0, 0))
            {
                DisableVisuals();
            }
            EnableVisuals();
            transform.position = new Vector3(LevelGrid.Instance.GetWorldPosition(e.NewGridPosition).x, transform.position.y,LevelGrid.Instance.GetWorldPosition(e.NewGridPosition).z );
        }
        
        private void OnDestroy()
        {
            UnitActionSystem.Instance.OnSelectedPositionChanged -= OnSelectedPositionChanged;
            TurnSystem.Instance.OnTurnChanged -= OnTurnChanged;
            UnitActionSystem.Instance.OnBusyChanged -= OnBusyChanged;
        }
    }
}