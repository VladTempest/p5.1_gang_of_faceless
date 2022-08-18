using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class PathNode
{
    public GridPosition GridPosition => _gridPosition;


    public PathNode CameFromPathNode
    {
        get => _cameFromPathNode;
        set => _cameFromPathNode = value;
    }

    public bool IsWalkable
    {
        get => _isWalkable;
        set => _isWalkable = value;
    }

    public int GCost
    {
        get => _gCost;
        set => _gCost = value;
    }


    public int FCost
    {
        get => _fCost;
        set => _fCost = value;
    }

    public int HCost
    {
        get => _hCost;
        set => _hCost = value;
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

    public void CalculateFCost()
    {
        _fCost = _gCost+_hCost;
    }

    public void ResetCameFromPathNode()
    {
        _cameFromPathNode = null;
    }
}
