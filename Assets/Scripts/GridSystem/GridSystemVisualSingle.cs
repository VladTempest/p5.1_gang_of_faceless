using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    
    public void Show(Material material)
    {
        _meshRenderer.material = material;
        _meshRenderer.enabled = true;
    }
    
    public void Hide()
    {
        _meshRenderer.enabled = false;
    }
}
