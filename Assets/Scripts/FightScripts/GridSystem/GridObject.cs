using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace GridSystems
{
    public class GridObject
    {
        public IInteractable Interactable { get; set; }
        public List<Unit> UnitList => _unitList;

        public bool HasAnyUnit => _unitList.Count > 0;
        

        public Unit Unit
        {
            get
            {
                if (HasAnyUnit) return _unitList[0];
                return null;
            }
        }

                private readonly GridPosition _gridPosition;

        private readonly List<Unit> _unitList = new List<Unit>();


        public GridObject(GridSystem<GridObject> gridSystem, GridPosition gridPosition)
        {
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
    }
}