using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    
    public void Show(Material material)
    {
        try
        {
            _meshRenderer.material = material;
            _meshRenderer.enabled = true;
        }
        catch (Exception e)
        {
            Debug.Log(transform.position + " " + gameObject);
        }
    }
    
    public void Hide()
    {
        _meshRenderer.enabled = false;
    }
}
