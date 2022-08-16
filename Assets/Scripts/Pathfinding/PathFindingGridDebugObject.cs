using System.Collections;
using System.Collections.Generic;
using GridSystems;
using TMPro;
using UnityEngine;

public class PathFindingGridDebugObject : GridDebugObject
{
    [SerializeField] private TextMeshPro _gCostText;
    [SerializeField] private TextMeshPro _hCostText;
    [SerializeField] private TextMeshPro _fCostText;
    [SerializeField] private SpriteRenderer isWalkableSpriteRenderer;

    private PathNode _pathNode;

    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        _pathNode = (PathNode) gridObject;
    }

    protected override void Update()
    {
        base.Update();
        _gCostText.text = _pathNode.GetGCost().ToString();
        _fCostText.text = _pathNode.GetFCost().ToString();
        _hCostText.text = _pathNode.GetHCost().ToString();
        isWalkableSpriteRenderer.color = _pathNode.IsWalkable ? Color.green : Color.red;
    }
}
