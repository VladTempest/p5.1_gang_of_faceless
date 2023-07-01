using System;
using System.Collections;
using System.Collections.Generic;
using Editor.Scripts.GlobalUtils;
using UnityEngine;

namespace GridSystems
{
    

    public class LevelGrid : MonoBehaviour
    {
        public static LevelGrid Instance { get; private set; }
        public event EventHandler<OnAnyUnitChangedArgs> OnAnyUnitChangedGridPosition;
        
        private GridSystem<GridObject> _gridSystem;
        
        [SerializeField]
        private int _width = 10;
        [SerializeField]
        private int _height = 10;
        [SerializeField]
        private float _cellSize =2;

        private void Awake()
        {
            if (Instance != null)
            {
                ConvenientLogger.Log(name, GlobalLogConstant.IsSingltonsLogEnabled, $"There are many singletonss");
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            _gridSystem = new GridSystem<GridObject>(_width, _height, _cellSize, (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));
            //if (_gridDebugObjectrPrefab) _gridSystem.CreateDebugObjects(_gridDebugObjectrPrefab);
        }

        private void Start()
        {
            Pathfinding.Instance.SetUp(_width,_height, _cellSize);
        }

        public void AddUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }

        public List<Unit> GetUnitListAtGriPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.UnitList;
        }

        public void RemoveUnitAtGridPosition(GridPosition gridPosition, Unit unit)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.RemoveUnit(unit);
        }

        public void UnitMovedGriPosition(Unit unit, GridPosition fromGridPosition, GridPosition toGridPosition)
        {
            RemoveUnitAtGridPosition(fromGridPosition, unit);
            
            AddUnitAtGridPosition(toGridPosition, unit);
            
            OnAnyUnitChangedGridPosition?.Invoke(this, new OnAnyUnitChangedArgs(fromGridPosition, toGridPosition, unit));
        }

        public Vector3 GetWorldPosition(GridPosition gridPosition) => _gridSystem.GetWorldPosition(gridPosition);
        
        public GridPosition GetGridPosition(Vector3 worldPosition) => _gridSystem.GetGridPosition(worldPosition);

        public bool IsValidGridPosition(GridPosition gridPosition) => _gridSystem.IsValidGridPosition(gridPosition);

        public int GetWidth() => _gridSystem.Width;
        public int GetHeight() => _gridSystem.Height;

        public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.HasAnyUnit;
        }
        
        public Unit GetUnitAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.Unit;
        }

        public IInteractable GetInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            return gridObject.Interactable;
        }

        public void SetInteractableAtGridPosition(GridPosition gridPosition, IInteractable Interactable)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.Interactable = Interactable;
        }

        public void ClearInteractableAtGridPosition(GridPosition gridPosition)
        {
            GridObject gridObject = _gridSystem.GetGridObject(gridPosition);
            gridObject.Interactable = null;
        }


    }
}
