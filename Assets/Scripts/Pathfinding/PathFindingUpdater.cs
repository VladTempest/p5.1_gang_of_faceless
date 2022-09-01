using System;
using System.Collections;
using System.Collections.Generic;
using Actions;
using UnityEngine;

public class PathFindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyCrateDestroyed += DestructibleCrate_OnOnAnyCrateDestroyed;
        PushAction.OnAnyUnitPushed += PushAction_OnAnyUnitPushed;
    }

    private void PushAction_OnAnyUnitPushed(object sender, OnPushActionEventArgs e)
    {
        PushAction pushAction = sender as PushAction;
        if (pushAction != null) 
            Pathfinding.Instance.SetIsWalkableGridPosition(e.pushedFromGridPosition, true);
    }

    private void DestructibleCrate_OnOnAnyCrateDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        if (destructibleCrate != null)
            Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GridPosition, true);
    }
}
