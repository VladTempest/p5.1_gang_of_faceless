using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GridSystems
{
    public class GridObject
    {
        public IInteractable Interactable { get; set; }
        private GridSystem<GridObject> _gridSystem;
        private GridPosition _gridPosition;
        private List<Unit> _unitList = new List<Unit>();

        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
            _gridSystem = gridSystem;
            _gridPosition = gridPosition;
        }
        
        public override string ToString()
        {
            string unitString = "";
            foreach (Unit unit in _unitList)
            {
                unitString += unit + " ";
            }
            return _gridPosition  + "\n" + unitString;
        }

        public void AddUnit(Unit unit)
        {
            
            if (!_unitList.Contains(unit)) _unitList.Add(unit);
        }

        public void RemoveUnit(Unit unit)
        {
            _unitList.Remove(unit);
        }

        public List<Unit> GetUnitList()
        {
            return _unitList;
        }

        public bool HasAnyUnit()
        {
            return _unitList.Count > 0;
        }

        public Unit GetUnit()
        {
            if (HasAnyUnit()) return _unitList[0];
            return null;
        }
        
        
        
    }
}