using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFindingUpdater : MonoBehaviour
{
    private void Start()
    {
        DestructibleCrate.OnAnyCrateDestroyed += DestructibleCrate_OnOnAnyCrateDestroyed;
    }

    private void DestructibleCrate_OnOnAnyCrateDestroyed(object sender, EventArgs e)
    {
        DestructibleCrate destructibleCrate = sender as DestructibleCrate;
        if (destructibleCrate != null)
            Pathfinding.Instance.SetIsWalkableGridPosition(destructibleCrate.GridPosition, true);
    }
}
