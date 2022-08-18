using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;
using UnityEngine.Serialization;

public class Pathfinding : MonoBehaviour
{
    public static Pathfinding Instance { get; private set; }
    
    [SerializeField] private Transform _gridDebugObjectrPrefab;
    [SerializeField] private LayerMask _obstaclesLayerMask;

    private int _width;
    private int _height;
    private float _cellSize;
    private GridSystem<PathNode> _gridSystem;

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;


    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There are many singletonss");
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    public void SetUp(int width, int height, float cellSize)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        
        _gridSystem = new GridSystem<PathNode>(_width, _height, _cellSize,
            (GridSystem<PathNode> g, GridPosition gridPosition) => new PathNode(gridPosition));
        
        //if (_gridDebugObjectrPrefab) _gridSystem.CreateDebugObjects(_gridDebugObjectrPrefab);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Vector3 worldPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
                if (Physics.Raycast(worldPosition+Vector3.down, Vector3.up, 10,  _obstaclesLayerMask))
                {
                    if (TryGetNode(x,z, out var node)) node.IsWalkable = false;
                }
            }
        }
    }

    public List<GridPosition> FindPath(GridPosition startGridPosition, GridPosition endGridPosition, out int pathLength)
    {
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();

        PathNode startNode = _gridSystem.GetGridObject(startGridPosition);
        PathNode endNode = _gridSystem.GetGridObject(endGridPosition);
        openList.Add(startNode);

        for (int x = 0; x < _gridSystem.Width; x++)
        {
            for (int z = 0; z < _gridSystem.Height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                PathNode pathNode = _gridSystem.GetGridObject(gridPosition);
                
                pathNode.GCost= int.MaxValue;
                pathNode.HCost = 0;
                pathNode.CalculateFCost();
                pathNode.ResetCameFromPathNode();
            }   
        }
        
        startNode.GCost = 0;
        startNode.HCost = CalculateDistance(startGridPosition, endGridPosition);
        startNode.CalculateFCost();

        while (openList.Count > 0)
        {
            PathNode currentNode = GetLowestFCostPathNode(openList);

            if (currentNode == endNode)
            {
                pathLength = endNode.FCost;
                return CalculatePath(endNode);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            foreach (var neighbourNode in GetNeighborList(currentNode))
            {
                if (closedList.Contains(neighbourNode))
                {
                    continue;
                }

                if (!neighbourNode.IsWalkable)
                {
                    closedList.Add(neighbourNode);
                    continue;
                }
                
                int tentativeGCost = currentNode.GCost +
                                     CalculateDistance(currentNode.GridPosition, neighbourNode.GridPosition);

                if (tentativeGCost < neighbourNode.GCost)
                {
                    neighbourNode.CameFromPathNode = currentNode;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistance(neighbourNode.GridPosition, endGridPosition);
                    neighbourNode.CalculateFCost();

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }

        pathLength = 0;
        return null;

    }

    public bool IsWalkableGridPosition(GridPosition gridPosition)
    {
        return _gridSystem.GetGridObject(gridPosition).IsWalkable;
    }
    
    public void SetIsWalkableGridPosition(GridPosition gridPosition, bool IsWalkable)
    {
        _gridSystem.GetGridObject(gridPosition).IsWalkable = IsWalkable;
    }
    
    public bool HasPath(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        
        if (FindPath(startGridPosition, endGridPosition, out var pathLength) == null) return false;
        return true;
    }

    public int GetPathLength(GridPosition startGridPosition, GridPosition endGridPosition)
    {
        FindPath(startGridPosition, endGridPosition, out var pathLength);
        return pathLength;
    }
    
    private int CalculateDistance(GridPosition gridPositionA, GridPosition gridPositionB)
    {
        GridPosition gridPositionDistance = gridPositionA - gridPositionB;
        int xDistance = Mathf.Abs(gridPositionDistance.x);
        int zDistance = Mathf.Abs(gridPositionDistance.z);
        int remaining = Mathf.Abs(xDistance - zDistance);
        return MOVE_DIAGONAL_COST * Mathf.Min(xDistance, zDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private List<GridPosition> CalculatePath(PathNode endNode)
    {
        List<PathNode> pathNodeList = new List<PathNode>();
        pathNodeList.Add(endNode);
        PathNode currentNode = endNode;
        while (currentNode.CameFromPathNode != null)
        {
            pathNodeList.Add(currentNode.CameFromPathNode);
            currentNode = currentNode.CameFromPathNode;
        }
        pathNodeList.Reverse();

        List<GridPosition> gridPositionList = new List<GridPosition>();
        foreach (var pathNode in pathNodeList)
        {
            gridPositionList.Add(pathNode.GridPosition);
        }
        
        return gridPositionList;
    }

    private PathNode GetLowestFCostPathNode(List<PathNode> pathNodeList)
    {
        PathNode lowestFCostPathNode = pathNodeList[0];
        for (int i = 0; i < pathNodeList.Count; i++)
        {
            if (pathNodeList[i].FCost < lowestFCostPathNode.FCost)
            {
                lowestFCostPathNode = pathNodeList[i];
            }
        }

        return lowestFCostPathNode;
    }

    private List<PathNode> GetNeighborList(PathNode currenNode)
    {
        List<PathNode> neighbors = new List<PathNode>();

        GridPosition gridPosition = currenNode.GridPosition;
        
        TryAddToNeighboursList(neighbors, gridPosition.x-1, gridPosition.z);
        TryAddToNeighboursList(neighbors, gridPosition.x-1, gridPosition.z-1);
        TryAddToNeighboursList(neighbors, gridPosition.x-1, gridPosition.z+1);
        TryAddToNeighboursList(neighbors, gridPosition.x+1, gridPosition.z);
        TryAddToNeighboursList(neighbors, gridPosition.x+1, gridPosition.z-1);
        TryAddToNeighboursList(neighbors, gridPosition.x+1, gridPosition.z+1);
        TryAddToNeighboursList(neighbors, gridPosition.x, gridPosition.z-1);
        TryAddToNeighboursList(neighbors, gridPosition.x, gridPosition.z+1);
        return neighbors;
    }

    private void TryAddToNeighboursList(List<PathNode> neighbors, int gridPositionX, int gridPositionZ)
    {
        if (!TryGetNode(gridPositionX, gridPositionZ, out var node)) return;
        neighbors.Add(node);
        
    }

    private bool TryGetNode(int gridPositionX, int gridPositionZ, out PathNode node)
    {
        if (IsPositionValid(gridPositionX, gridPositionZ))
        {
            node = _gridSystem.GetGridObject(new GridPosition(gridPositionX, gridPositionZ));
            return true;
        }

        node = null;
        return false;
    }

    private bool IsPositionValid(int gridPositionX, int gridPositionZ)
    {
        return gridPositionX >= 0 && gridPositionX < _gridSystem.Width && gridPositionZ >= 0 && gridPositionZ < _gridSystem.Height;
    }
}
