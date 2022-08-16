using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class PathNode
{
    public bool IsWalkable
    {
        get => _isWalkable;
        set => _isWalkable = value;
    }

    private GridPosition _gridPosition;
    private int _gCost;
    private int _hCost;
    private int _fCost;
    private PathNode _cameFromPathNode;
    private bool _isWalkable = true;
    public PathNode(GridPosition gridPosition)
    {
        _gridPosition = gridPosition;
    }
    
    public override string ToString()
    {
        return _gridPosition.ToString();
    }

    public int GetGCost()
    {
        return _gCost;
    }
    
    public int GetFCost()
    {
        return _fCost;
    }
    
    public int GetHCost()
    {
        return _hCost;
    }
    
    public void SetGCost(int newValue)
    {
        _gCost = newValue;
    }
    
    public void SetHCost(int newValue)
    {
        _hCost = newValue;
    }

    public void CalculateFCost()
    {
        _fCost = _gCost+_hCost;
    }

    public void ResetCameFromPathNode()
    {
        _cameFromPathNode = null;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }

    public void SetCameFromPathNode(PathNode newValue)
    {
        _cameFromPathNode = newValue;
    }
    
    public PathNode GetCameFromPathNode()
    {
        return _cameFromPathNode;
    }
}
