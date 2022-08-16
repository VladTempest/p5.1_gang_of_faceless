using System;
using System.Collections;
using System.Collections.Generic;
using GridSystems;
using UnityEngine;

public class InteractSphere : MonoBehaviour, IInteractable
{
    [SerializeField] private Material _greenMaterial;
    [SerializeField] private Material _redMaterial;
    [SerializeField] private MeshRenderer _meshRenderer;
    
    private Action _onInteractComplete;
    private float _timer = 0.5f;
    private bool _isActive;
    private bool _isGreen;
    private GridPosition _gridPosition;

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.SetInteractableAtGridPosition(_gridPosition, this);
        
        SetColorRed();
    }
    
    private void Update()
    {
        if (!_isActive)
        {
            return;
        }
        _timer -= Time.deltaTime;
        if (_timer <= 0)
        {
            _isActive = false;
            _onInteractComplete();
        }
    }

    private void SetColorGreen()
    {
        _meshRenderer.material = _greenMaterial;
        _isGreen = true;
    }

    private void SetColorRed()
    {
        _meshRenderer.material = _redMaterial;
        _isGreen = false;
    }

    public void Interact(Action onActionComplete)
    {
        _onInteractComplete = onActionComplete;
        _isActive = true;
        _timer = 0.5f;
        
        if (_isGreen)
        {
            SetColorRed();
        }
        else
        {
            SetColorGreen();
        }
    }
}
